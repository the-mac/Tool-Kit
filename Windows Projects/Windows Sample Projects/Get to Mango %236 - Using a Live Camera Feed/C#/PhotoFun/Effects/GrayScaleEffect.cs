namespace PhotoFun.Effects
{
    /// <summary>
    /// This class implements gray scale effect 
    /// </summary>
    public class GrayScaleEffect : EffectBase, IEffect
    {
        /// <summary>
        /// Gets the source color value and returns it's gray-scale value
        /// </summary>
        /// <param name="color">Source color value</param>
        /// <returns>Gray scale value</returns>
        private int ColorToGray(int color)
        {
            int gray = 0;

            int a, r, g, b;
            GetARGB(color, out a, out r, out g, out b);
            if ((r == g) && (g == b))
            {
                gray = color;
            }
            else
            {
                int i = (7 * r + 38 * g + 19 * b + 32) >> 6;

                gray = ((a & 0xFF) << 24) | ((i & 0xFF) << 16) | ((i & 0xFF) << 8) | (i & 0xFF);
            }

            return gray;
        }

        /// <summary>
        /// Convert the source array to it's gray-scale values
        /// </summary>
        /// <param name="source">Picture source in array of pixels</param>
        /// <returns>Gray-scale array of pixels</returns>
        public override int[] ProcessImage(int[] source)
        {
            int[] target = new int[source.Length];
            for (int i = 0; i < source.Length; i++)
            {
                target[i] = ColorToGray(source[i]);
            }

            return target;
        }
    }
}
