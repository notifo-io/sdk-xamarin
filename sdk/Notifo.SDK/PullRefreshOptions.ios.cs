// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;

namespace Notifo.SDK;

/// <summary>
/// Options for handling the pending notifications pull refresh request.
/// </summary>
public class PullRefreshOptions
{
    /// <summary>
    /// The pull refresh request should raise the received event. Default: true.
    /// </summary>
    public bool RaiseEvent { get; set; } = true;

    /// <summary>
    /// The pull refresh request should present a local notification. Default: true.
    /// </summary>
    public bool PresentNotification { get; set; } = true;

    /// <summary>
    /// The pull refresh request pending notifications period. Default: 3 days.
    /// </summary>
    public TimeSpan Period { get; set; } = TimeSpan.FromDays(3);

    /// <summary>
    /// The pull refresh request pending notifications limit. Default: 20.
    /// </summary>
    public int Take { get; set; } = 20;
}
