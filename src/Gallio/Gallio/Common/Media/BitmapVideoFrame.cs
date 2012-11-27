// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;

namespace Gallio.Common.Media
{
    /// <summary>
    /// A video frame that consists of a bitmap.
    /// </summary>
    public class BitmapVideoFrame : VideoFrame
    {
        private readonly Bitmap bitmap;

        /// <summary>
        /// Creates a bitmap video frame.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="bitmap"/> is null.</exception>
        public BitmapVideoFrame(Bitmap bitmap)
        {
            if (bitmap == null)
                throw new ArgumentNullException("bitmap");

            this.bitmap = bitmap;
        }

        /// <summary>
        /// Gets the bitmap.
        /// </summary>
        public Bitmap Bitmap
        {
            get { return bitmap; }
        }

        /// <inheritdoc />
        public override int Width
        {
            get { return bitmap.Width; }
        }

        /// <inheritdoc />
        public override int Height
        {
            get { return bitmap.Height; }
        }

        /// <inheritdoc />
        protected override void CopyPixelsImpl(Rectangle sourceRect, int[] buffer, int startOffset, int stride)
        {
            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            try
            {
                BitmapData bitmapData = new BitmapData()
                {
                    Width = sourceRect.Width,
                    Height = sourceRect.Height,
                    PixelFormat = PixelFormat.Format32bppRgb,
                    Scan0 = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, startOffset),
                    Stride = stride * 4
                };

                bitmapData = bitmap.LockBits(sourceRect, ImageLockMode.UserInputBuffer | ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb, bitmapData);
                bitmap.UnlockBits(bitmapData);
            }
            finally
            {
                handle.Free();
            }
        }
    }
}
