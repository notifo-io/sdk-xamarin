// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;

namespace Notifo.SDK
{
    /// <summary>
    /// Event arguments containing the error and the source.
    /// </summary>
    public class NotificationErrorEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the notification.
        /// </summary>
        public string Error { get; private set; }

        /// <summary>
        /// Gets the exception.
        /// </summary>
        public Exception? Exception { get; private set; }

        /// <summary>
        /// Gets the source.
        /// </summary>
        public object? Source { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationErrorEventArgs"/> class with the message and source.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="source">The source.</param>
        public NotificationErrorEventArgs(string error, Exception? exception, object? source)
        {
            Error = error;
            Exception = exception;
            Source = source;
        }
    }
}
