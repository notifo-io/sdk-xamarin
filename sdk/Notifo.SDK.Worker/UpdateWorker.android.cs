// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Android.Runtime;
using AndroidX.Work;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

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
