using System.IO;
using Microsoft.Devices;

namespace PhotoFun.Effects
{
    /// <summary>
    /// This class is a base class for all effect classes
    /// It contains basic functionality that all effect
    /// classes uses
    /// </summary>
    public abstract class EffectBase : IEffect
    {
        /// <summary>
        /// Extracts the Alpha, Red, Green, Blue from the source color 
        /// </summary>
        /// <param name="color">Source color</param>
        /// <param name="a">Alpha component value</param>
        /// <param name="r">Red component value</param>
        /// <param name="g">Green component value</param>
        /// <param name="b">Blue component value</param>
        protected void GetARGB(int color, out int a, out int r, out int g, out int b)
        {
            a = color >> 24;
            r = (color & 0x00ff0000) >> 16;
            g = (color & 0x0000ff00) >> 8;
            b = (color & 0x000000ff);
        }

        /// <summary>
        /// Assemble the ARGB values to one color value 
        /// </summary>
        /// <param name="a">Alpha component value</param>
        /// <param name="r">Red component value</param>
        /// <param name="g">Green component value</param>
        /// <param name="b">Blue component value</param>
        /// <returns>One color value from the given ARGB</returns>
        protected int GetColorFromArgb(int a, int r, int g, int b)
        {
            int result = ((a & 0xFF) << 24) | ((r & 0xFF) << 16) | ((g & 0xFF) << 8) | (b & 0xFF);
            return result;
        }

        /// <summary>
        /// When implemented in a derived class,
        /// takes the raw image as array of pixels and 
        /// returns a processed one
        /// </summary>
        /// <param name="source">Raw image content as array of pixels</param>
        /// <returns>Processed image as array of pixels</returns>
        public abstract int[] ProcessImage(int[] source);
    }
}
