// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Plugin.Connectivity.Abstractions;

namespace Notifo.SDK.CommandQueue
{
    internal sealed class TriggerWhenConnected : ICommandTrigger
    {
        private readonly IConnectivity connectivity;

        public TriggerWhenConnected(IConnectivity connectivity)
        {
            this.connectivity = connectivity;
        }

        public void Start(ICommandQueue queue)
        {
            connectivity.ConnectivityChanged += (sender, e) =>
            {
                if (e.IsConnected)
                {
                    queue.Trigger();
                }
            };
        }
    }
}