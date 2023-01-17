// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Notifo.SDK.Extensions;

internal static class StringExtensions
{
    public static string ToBase64(this string value)
    {
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
    }

    public static string Sha256(this string value)
    {
        using (var hash = SHA256.Create())
        {
            var bytesString = Encoding.UTF8.GetBytes(value);
            var bytesHash = hash.ComputeHash(bytesString);

            var builder = new StringBuilder();

            foreach (byte b in bytesHash)
            {
                builder.Append(b.ToString("x2"));
            }

            return builder.ToString();
        }
    }

    public static string AppendQueries(this string url, string? key1, object? value1)
    {
        return AppendQueries(url, key1, value1, null!, null!);
    }

    public static string AppendQueries(this string url, string? key1, object? value1, string? key2, object? value2)
    {
        return AppendQueries(url, key1, value1, key2, value2, null!, null!);
    }

    public static string AppendQueries(this string url, string? key1, object? value1, string? key2, object? value2, string? key3, object? value3)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return url;
        }

        var builder = new StringBuilder(url);

        var hasQuery = url.Contains('?', StringComparison.OrdinalIgnoreCase);

        void Append(string? key, object? value)
        {
            if (string.IsNullOrWhiteSpace(key) || value == null)
            {
                return;
            }

            var valueString = string.Format(CultureInfo.InvariantCulture, "{0}", value);

            if (hasQuery)
            {
                builder.Append('&');
            }
            else
            {
                builder.Append('?');
            }

            builder.Append(key);
            builder.Append('=');
            builder.Append(Uri.EscapeDataString(valueString));

            hasQuery = true;
        }

        Append(key1, value1);
        Append(key2, value2);
        Append(key3, value3);

        return builder.ToString();
    }
}