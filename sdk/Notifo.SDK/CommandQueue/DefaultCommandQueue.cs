// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections.Concurrent;
using System.Diagnostics;
using Notifo.SDK.Extensions;
using Notifo.SDK.Resources;

namespace Notifo.SDK.CommandQueue;

internal sealed class DefaultCommandQueue : ICommandQueue
{
    private readonly ICommandStore commandStore;
    private readonly ICommandTrigger[] commandTriggers;
    private readonly int maxRetries;
    private readonly TimeSpan timeout;
    private readonly Task task;
    private readonly BlockingCollection<QueuedCommand> queue = new BlockingCollection<QueuedCommand>();
    private readonly Queue<QueuedCommand> retryQueue = new Queue<QueuedCommand>();

    public event EventHandler<NotificationLogEventArgs> OnLog;

    public DefaultCommandQueue(
        ICommandStore commandStore,
        ICommandTrigger[] commandTriggers,
        int maxRetries,
        TimeSpan timeout)
    {
        this.commandStore = commandStore;
        this.commandTriggers = commandTriggers;
        this.maxRetries = maxRetries;
        this.timeout = timeout;

        task = Task.Run(RunAsync);
    }

    public async Task CompleteAsync(
        CancellationToken ct)
    {
        queue.CompleteAdding();

        Trigger();

        foreach (var trigger in commandTriggers.OfType<IDisposable>())
        {
            trigger.Dispose();
        }

        await task.WithCancellation(ct);
    }

    public void Dispose()
    {
        queue.CompleteAdding();

        foreach (var trigger in commandTriggers.OfType<IDisposable>())
        {
            trigger.Dispose();
        }

        task.Wait();
    }

    public void Run(ICommand command)
    {
        var queudCommand = new QueuedCommand
        {
            Command = command,
            CommandId = Guid.NewGuid()
        };

        lock (retryQueue)
        {
            foreach (var existing in retryQueue)
            {
                if (existing.Command.Merge(command))
                {
                    return;
                }
            }

            retryQueue.Enqueue(queudCommand);
        }

        Trigger();

        _ = StoreAsync(queudCommand);
    }

    private async Task StoreAsync(QueuedCommand queudCommand)
    {
        try
        {
            await commandStore.StoreAsync(queudCommand).AsTask();
        }
        catch (Exception ex)
        {
            OnLog?.Invoke(this, new NotificationLogEventArgs(NotificationLogType.Error, this, Strings.CommandError, null, ex));
        }
    }

    public void Trigger()
    {
        lock (retryQueue)
        {
            if (retryQueue.TryDequeue(out var dequeued))
            {
                queue.Add(dequeued);
            }
        }
    }

    private async Task RunAsync()
    {
        try
        {
            var pendingCommands = await commandStore.GetCommandsAsync();

            foreach (var command in pendingCommands)
            {
                retryQueue.Enqueue(command);
            }
        }
        catch (Exception ex)
        {
            OnLog?.Invoke(this, new NotificationLogEventArgs(NotificationLogType.Error, this, Strings.CommandError, null, ex));
        }

        foreach (var trigger in commandTriggers)
        {
            trigger.Start(this);
        }

        try
        {
            foreach (var enqueued in queue.GetConsumingEnumerable())
            {
                try
                {
                    if (!Debugger.IsAttached)
                    {
                        using (var cts = new CancellationTokenSource(timeout))
                        {
                            await enqueued.Command.ExecuteAsync(cts.Token);
                        }
                    }
                    else
                    {
                        await enqueued.Command.ExecuteAsync(default);
                    }

                    // We have completed the command successfully, so we can remove it here.
                    try
                    {
                        await commandStore.RemoveAsync(enqueued.CommandId);
                    }
                    catch (Exception ex)
                    {
                        OnLog?.Invoke(this, new NotificationLogEventArgs(NotificationLogType.Error, this, Strings.CommandError, null, ex));
                    }

                    // We have just completed a command, so it is very likely that the next one will be successful as well.
                    Trigger();
                }
                catch
                {
                    if (enqueued.Retries < maxRetries)
                    {
                        enqueued.Retries++;

                        lock (retryQueue)
                        {
                            retryQueue.Enqueue(enqueued);
                        }
                    }
                }
            }
        }
        catch
        {
            try
            {
                queue.CompleteAdding();
            }
            catch
            {
                return;
            }
        }
    }
}
