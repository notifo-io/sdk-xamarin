// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Threading;
using Android.Runtime;
using AndroidX.Work;

namespace Notifo.SDK
{
    internal sealed class UpdateWorker : Worker
    {
        public UpdateWorker(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public override Result DoWork()
        {
            NotifoIO.Current.Register();

            try
            {
                using (var cts = new CancellationTokenSource(TimeSpan.FromMinutes(10)))
                {
                    NotifoIO.Current.WaitForBackgroundTasksAsync(cts.Token).Wait(cts.Token);
                }
            }
            catch (OperationCanceledException)
            {
                return new Result.Retry();
            }

            return new Result.Success();
        }
    }
}