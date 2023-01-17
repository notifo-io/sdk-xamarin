// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Notifo.SDK.Extensions;

internal static class TaskExtensions
{
    private static readonly Action<Task> IgnoreTaskContinuation = t => { var ignored = t.Exception; };

    public static void Forget(this Task task)
    {
        if (task.IsCompleted)
        {
#pragma warning disable IDE0059 // Unnecessary assignment of a value
            var ignored = task.Exception;
#pragma warning restore IDE0059 // Unnecessary assignment of a value
        }
        else
        {
            task.ContinueWith(
                IgnoreTaskContinuation,
                CancellationToken.None,
                TaskContinuationOptions.OnlyOnFaulted |
                TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Default);
        }
    }

    public static async Task WithCancellation(this Task task, CancellationToken cancellationToken)
    {
        var tcs = new TaskCompletionSource<bool>();

        using (cancellationToken.Register(s => ((TaskCompletionSource<bool>)s).TrySetResult(true), tcs))
        {
            if (task != await Task.WhenAny(task, tcs.Task).ConfigureAwait(false))
            {
                throw new OperationCanceledException(cancellationToken);
            }

            await task; // Already completed; propagate any exception
        }
    }
}
