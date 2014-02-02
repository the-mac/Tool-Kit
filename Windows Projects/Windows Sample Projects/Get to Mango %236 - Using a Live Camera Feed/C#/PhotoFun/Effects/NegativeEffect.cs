namespace PhotoFun.Effects
{
    public class NegativeEffect : EffectBase, IEffect
    {

        /// <summary>
        /// Returns the negative color of the source color
        /// </summary>
        /// <param name="color">Source color</param>
        /// <returns>Negative color</returns>
        private int Negate(int color)
        {
            int a, r, g, b;
            GetARGB(color, out a, out r, out g, out b);

            r = 255 - r;
            g = 255 - g;
            b = 255 - b;

            int result = GetColorFromArgb(a, r, g, b);

            return result;
        }

        /// <summary>
        ///  Returns the negative pixel values of the source array of pixels
        /// </summary>
        /// <param name="source">Source image as array of pixels</param>
        /// <returns>The negative pixel values of the source array of pixels</returns>
        public override int[] ProcessImage(int[] source)
        {
            int[] target = new int[source.Length];

            for (int i = 0; i < source.Length; i++)
            {
                target[i] = Negate(source[i]);
            }

            return target;
        }
    }
}
