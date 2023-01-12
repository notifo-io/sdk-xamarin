// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Threading.Tasks;
using AndroidX.Core.App;

namespace Notifo.SDK;

/// <summary>
/// Notification handler interface.
/// </summary>
public partial interface INotifoMobilePush
{
    /// <summary>
    /// Sets the notification handler.
    /// </summary>
    /// <param name="notificationHandler">The <see cref="INotificationHandler"/> implementation.</param>
    /// <returns>The current instance.</returns>
    public INotifoMobilePush SetNotificationHandler(INotificationHandler? notificationHandler);

    /// <summary>
    /// Sets the capacity of the image cache.
    /// </summary>
    /// <param name="capacity">The capacity in bytes.</param>
    /// <returns>The current instance.</returns>
    INotifoMobilePush SetImageCacheCapacity(int capacity);
}

/// <summary>
/// Provides methods that should not be triggered from user code.
/// </summary>
#pragma warning disable IDE1006 // Naming Styles
public interface InternalAndroidPushAdapter
#pragma warning restore IDE1006 // Naming Styles
{
    /// <summary>
    /// Called when a notification has been received.
    /// </summary>
    /// <param name="notificationBuilder">The android notification.</param>
    /// <param name="notification">The notifo notification.</param>
    /// <returns>The tasks.</returns>
    Task OnBuildNotificationAsync(NotificationCompat.Builder notificationBuilder, UserNotificationDto notification);
}