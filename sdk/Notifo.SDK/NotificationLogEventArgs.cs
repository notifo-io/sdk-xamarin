// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;

namespace Notifo.SDK;

/// <summary>
/// Event arguments containing the error and the source.
/// </summary>
public sealed class NotificationLogEventArgs : EventArgs
{
    /// <summary>
    /// Gets the type.
    /// </summary>
    public NotificationLogType Type { get; }

    /// <summary>
    /// Gets the message.
    /// </summary>
    public string Message { get; private set; }

    /// <summary>
    /// Gets the source.
    /// </summary>
    public object? Source { get; private set; }

    /// <summary>
    /// Gets the additional arguments.
    /// </summary>
    public object[]? MessageArgs { get; private set; }

    /// <summary>
    /// Gets the exception.
    /// </summary>
    public Exception? Exception { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationLogEventArgs"/> class with the message and source.
    /// </summary>>
    /// <param name="type">The type.</param>
    /// <param name="source">The source.</param>
    /// <param name="message">The message.</param>
    /// <param name="messageArgs">The message arguments.</param>
    /// <param name="exception">The exception.</param>
    public NotificationLogEventArgs(NotificationLogType type, object? source, string message, object[]? messageArgs, Exception? exception)
    {
        Type = type;
        Source = source;
        Message = message;
        MessageArgs = messageArgs;
        Exception = exception;
    }
}

/// <summary>
/// The type of log events.
/// </summary>
public enum NotificationLogType
{
    /// <summary>
    /// The log entry is an error.
    /// </summary>
    Error,

    /// <summary>
    /// The log entry is a debug message.
    /// </summary>
    Debug,
}
