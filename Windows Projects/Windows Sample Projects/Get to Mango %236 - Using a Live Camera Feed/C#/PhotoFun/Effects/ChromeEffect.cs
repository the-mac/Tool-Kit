using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PhotoFun.Effects
{
    /// <summary>
    /// This class implements Chrome effect,
    /// which is adding frame to the image
    /// </summary>
   
    public class ChromeEffect : EffectBase, IEffect
    {
        /// <summary>
        /// Default chrome effect, with no other image processing
        /// </summary>
        /// <param name="frameSourceUri">Frame image source Uri</param>
        /// <remarks>The frame image needs to be with the same dimensions as the image itself</remarks>
        public ChromeEffect(Uri frameSourceUri)
        {
            FrameSourceUri = frameSourceUri;
        }

        private int[] PutFrameOnImage(int[] source)
        {
            if (FrameSourceUri != null)
            {
                var resource = App.GetResourceStream(FrameSourceUri);
                WriteableBitmap frameBitmap = new WriteableBitmap(640, 480);
                resource.Stream.Position = 0;
                frameBitmap.LoadJpeg(resource.Stream);
                WriteableBitmap target = Blit(source, frameBitmap, Colors.White, BlendMode.Mask);
                return target.Pixels;
            }
            else
                return source;
        }

        /// <summary>
        /// Returns the source picture, after processing with the underlying effect
        /// within a frame
        /// </summary>
        /// <param name="source">Image source as array of pixels</param>
        /// <returns>Processed image within  a frame as array of pixels</returns>
        public override int[] ProcessImage(int[] source)
        {
            return PutFrameOnImage(source);
        }

        /// <summary>
        /// Copies (blits) the pixels from the WriteableBitmap source to the destination WriteableBitmap (this).
        /// </summary>
        /// <param name="bmpSource">The destination WriteableBitmap.</param>
        /// <param name="destRect">The rectangle that defines the destination region.</param>
        /// <param name="frameBitmap">The source WriteableBitmap.</param>
        /// <param name="color">If not Colors.White, will tint the source image. A partially transparent color and the image will be drawn partially transparent. If the BlendMode is ColorKeying, this color will be used as color key to mask all pixels with this value out.</param>
        /// <param name="BlendMode">The blending mode <see cref="BlendMode"/>.</param>
        public WriteableBitmap Blit(int[] bmpSource, WriteableBitmap frameBitmap, Color color, BlendMode BlendMode)
        {
            int dw = 640;// source.PixelWidth;// (int)destRect.Width;
            int dh = 480;// source.PixelHeight;// (int)destRect.Height;
            int dpw = 640;
            int dph = 480;
            //Rect intersect = new Rect(0, 0, dpw, dph);
            //intersect.Intersect(destRect);
            //if (intersect.IsEmpty)
            //{
            //    return;
            //}
            int sourceWidth = frameBitmap.PixelWidth;

            int[] sourcePixels = frameBitmap.Pixels;
            int[] destPixels = bmpSource;
            int sourceLength = sourcePixels.Length;
            int destLength = destPixels.Length;
            int sourceIdx = -1;
            int x;
            int y;
            int idx;
            double ii;
            double jj;
            int sr = 0;
            int sg = 0;
            int sb = 0;

            int sourcePixel;
            int sa = 0;

            int ca = color.A;
            int cr = color.R;
            int cg = color.G;
            int cb = color.B;
            bool tinted = color != Colors.White;
            var sdx = 1;
            var sdy = 1;
            int lastii, lastjj;
            lastii = -1;
            lastjj = -1;
            jj = 0;
            y = 0;
            for (int j = 0; j < dh; j++)
            {
                if (y >= 0 && y < dph)
                {
                    ii = 0;
                    idx = 0 + y * dpw;
                    x = 0;// px;
                    sourcePixel = sourcePixels[0];

                    // Pixel by pixel copying
                    if (BlendMode != Effects.BlendMode.None || tinted)
                    {
                        for (int i = 0; i < dw; i++)
                        {
                            if (x >= 0 && x < dpw)
                            {
                                if ((int)ii != lastii || (int)jj != lastjj)
                                {
                                    sourceIdx = (int)ii + (int)jj * sourceWidth;
                                    if (sourceIdx >= 0 && sourceIdx < sourceLength)
                                    {
                                        sourcePixel = sourcePixels[sourceIdx];
                                        GetARGB(sourcePixel, out sa, out sr, out sg, out sb);
                                        if (tinted && sa != 0)
                                        {
                                            sa = (((sa * ca) * 0x8081) >> 23);
                                            sr = ((((((sr * cr) * 0x8081) >> 23) * ca) * 0x8081) >> 23);
                                            sg = ((((((sg * cg) * 0x8081) >> 23) * ca) * 0x8081) >> 23);
                                            sb = ((((((sb * cb) * 0x8081) >> 23) * ca) * 0x8081) >> 23);
                                            sourcePixel = (sa << 24) | (sr << 16) | (sg << 8) | sb;
                                        }
                                    }
                                    else
                                    {
                                        sa = 0;
                                    }
                                }

                                if (BlendMode == BlendMode.Mask)
                                {
                                    if (sa != 0)
                                        destPixels[idx] = sourcePixel;
                                }
                            }
                            x++;
                            idx++;
                            ii += sdx;
                        }
                    }
                }
                jj += sdy;
                y++;
            }
            WriteableBitmap result = new WriteableBitmap(frameBitmap.PixelWidth, frameBitmap.PixelHeight);
            destPixels.CopyTo(result.Pixels, 0);

            return result;
        }

        /// <summary>
        /// Frame image Uri
        /// </summary>
        public Uri FrameSourceUri { get; private set; }
    }
}
