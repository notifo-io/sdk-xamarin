// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections.Generic;
using System.Linq;
using Foundation;

namespace Notifo.SDK.Extensions;

internal static class NSDictionaryExtensions
{
    public static Dictionary<string, object> ToDictionary(this NSDictionary nsDictionary)
    {
        return nsDictionary.ToDictionary<KeyValuePair<NSObject, NSObject>, string, object>(
            item => item.Key as NSString,
            item => item.Value
        );
    }

    public static NSDictionary ToNSDictionary(this Dictionary<string, string> dictionary)
    {
        return NSDictionary.FromObjectsAndKeys(
            dictionary.Values.ToArray(),
            dictionary.Keys.ToArray()
        );
    }
}
