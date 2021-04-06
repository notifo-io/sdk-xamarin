// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using Android.Graphics;
using Java.Net;
using Microsoft.Extensions.Caching.Memory;
using Notifo.SDK.Resources;
using Serilog;

namespace Notifo.SDK.NotifoMobilePush
{
    internal partial class NotifoMobilePushImplementation
    {
        private readonly IMemoryCache bitmapCache = new MemoryCache(new MemoryCacheOptions());

        public Bitmap? GetBitmap(string bitmapUrl, int requestWidth = -1, int requestHeight = -1)
        {
            try
            {
                bool shouldResize = requestWidth > 0 && requestHeight > 0;
                bool resizeHandled = false;

                if (shouldResize && bitmapUrl.StartsWith(clientProvider.ApiUrl, StringComparison.InvariantCultureIgnoreCase))
                {
                    bitmapUrl = $"{bitmapUrl}?width={requestWidth}&height={requestHeight}";
                    resizeHandled = true;
                }

                if (bitmapCache.TryGetValue(bitmapUrl, out Bitmap cachedBitmap))
                {
                    return cachedBitmap;
                }

                var bitmapStream = new URL(bitmapUrl)?.OpenConnection()?.InputStream;

                var bitmap = BitmapFactory.DecodeStream(bitmapStream);
                if (bitmap != null)
                {
                    bitmapCache.Set(bitmapUrl, bitmap);
                }

                if (!resizeHandled && shouldResize)
                {
                    bitmap = ResizeBitmap(bitmap, requestWidth, requestHeight);
                }

                return bitmap;
            }
            catch (Exception ex)
            {
                Log.Error(Strings.DownloadImageError, ex);
            }

            return null;
        }

        private Bitmap? ResizeBitmap(Bitmap? bitmap, int requestWidth, int requestHeight)
        {
            if (bitmap == null)
            {
                return null;
            }

            try
            {
                if (bitmap.Width > requestWidth || bitmap.Height > requestHeight)
                {
                    int newWidth = requestWidth;
                    int newHeight = requestHeight;

                    if (bitmap.Height > bitmap.Width)
                    {
                        float ratio = (float)bitmap.Width / bitmap.Height;
                        newWidth = (int)(newHeight * ratio);
                    }
                    else if (bitmap.Width > bitmap.Height)
                    {
                        float ratio = (float)bitmap.Height / bitmap.Width;
                        newHeight = (int)(newWidth * ratio);
                    }

                    return Bitmap.CreateScaledBitmap(bitmap, newWidth, newHeight, true);
                }
            }
            catch (Exception ex)
            {
                Log.Error(Strings.ResizeImageError, ex);
            }

            return bitmap;
        }
    }
}
