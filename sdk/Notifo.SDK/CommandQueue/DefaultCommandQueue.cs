﻿// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Notifo.SDK.CommandQueue
{
    internal sealed class DefaultCommandQueue : ICommandQueue
    {
        private readonly ICommandStore commandStore;
        private readonly ICommandTrigger[] commandTriggers;
        private readonly int maxRetries;
        private readonly Task task;
        private readonly BlockingCollection<QueuedCommand> queue = new BlockingCollection<QueuedCommand>();
        private readonly Queue<QueuedCommand> retryQueue = new Queue<QueuedCommand>();

        public DefaultCommandQueue(
            ICommandStore commandStore,
            ICommandTrigger[] commandTriggers,
            int maxRetries)
        {
            this.commandStore = commandStore;
            this.commandTriggers = commandTriggers;
            this.maxRetries = maxRetries;

            foreach (var trigger in commandTriggers)
            {
                trigger.Start(this);
            }

            task = Task.Run(RunAsync);
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

        public Task ExecuteAsync(ICommand command)
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
                        return Task.CompletedTask;
                    }
                }

                retryQueue.Enqueue(queudCommand);
            }

            Trigger();

            return commandStore.StoreAsync(queudCommand).AsTask();
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
            var pendingCommands = await commandStore.GetCommandsAsync();

            foreach (var command in pendingCommands)
            {
                retryQueue.Enqueue(command);
            }

            Trigger();

            try
            {
                foreach (var enqueued in queue.GetConsumingEnumerable())
                {
                    try
                    {
                        await enqueued.Command.ExecuteAsync();

                        // We have completed the command successfully, so we can remove it here.
                        await commandStore.RemoveAsync(enqueued.CommandId);

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
}
