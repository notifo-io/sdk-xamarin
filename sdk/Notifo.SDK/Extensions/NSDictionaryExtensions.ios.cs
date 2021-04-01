// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections.Generic;
using System.Linq;
using Foundation;

namespace Notifo.SDK.Extensions
{
    public static class NSDictionaryExtensions
    {
        public static Dictionary<string, string> ToDictionary(this NSDictionary nsDictionary)
        {
            return nsDictionary.ToDictionary<KeyValuePair<NSObject, NSObject>, string, string>(
                item => item.Key as NSString, item => item.Value.ToString()
            );
        }
    }
}
