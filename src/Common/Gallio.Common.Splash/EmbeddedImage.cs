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
using System.Drawing;
using System.Windows.Forms;

namespace Gallio.Common.Splash
{
    /// <summary>
    /// An embedded image wraps an <see cref="Image"/> so that it can be embedded in
    /// a Splash document.
    /// </summary>
    public class EmbeddedImage : EmbeddedObject
    {
        private readonly Image image;

        /// <summary>
        /// Creates an embedded image.
        /// </summary>
        /// <param name="image">The image to draw into the client area.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="image"/> is null.</exception>
        public EmbeddedImage(Image image)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            this.image = image;
        }

        /// <summary>
        /// Gets the image.
        /// </summary>
        public Image Image
        {
            get { return image; }
        }

        /// <summary>
        /// Gets or sets the margin around the image.
        /// </summary>
        public Padding Margin { get; set; }

        /// <summary>
        /// Gets or sets the baseline of the image.
        /// </summary>
        /// <remarks>
        /// The default value is 0 which positions the bottom of the image in line with the
        /// baseline of surrounding text.  Use a positive value to raise the image above the
        /// text baseline or a negative value to lower it below the text baseline.
        /// </remarks>
        public int Baseline { get; set; }

        /// <inheritdoc />
        public override IEmbeddedObjectClient CreateClient(IEmbeddedObjectSite site)
        {
            return new Client(this);
        }

        private sealed class Client : IEmbeddedObjectClient
        {
            private readonly EmbeddedImage embeddedImage;

            public Client(EmbeddedImage embeddedImage)
            {
                this.embeddedImage = embeddedImage;
            }

            public void Dispose()
            {
            }

            public bool RequiresPaint
            {
                get { return true; }
            }

            public EmbeddedObjectMeasurements Measure()
            {
                return new EmbeddedObjectMeasurements(embeddedImage.image.Size)
                {
                    Margin = embeddedImage.Margin,
                    Descent = embeddedImage.Baseline
                };
            }

            public void Show(Rectangle bounds, bool rightToLeft)
            {
            }

            public void Hide()
            {
            }

            public void Paint(Graphics g, PaintOptions paintOptions, Rectangle bounds, bool rightToLeft)
            {
                g.DrawImage(embeddedImage.Image, bounds);
            }
        }
    }
}
