namespace PhotoFun.Effects
{
    /// <summary>
    /// This class implements Echo effect - i.e no processing at all
    /// </summary>
    public class EchoEffect : EffectBase, IEffect
    {
        public override int[] ProcessImage(int[] source)
        {
            return source;
        }
    }
}
