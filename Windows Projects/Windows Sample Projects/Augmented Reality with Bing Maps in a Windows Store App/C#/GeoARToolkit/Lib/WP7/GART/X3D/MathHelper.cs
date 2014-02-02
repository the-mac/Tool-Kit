
namespace GART.X3D
{
    public class MathHelper
    {
        public static readonly float PiOver2 = 1.570796f;

        public static float ToDegrees(float radians)
        {
            return (radians * 57.29578f);
        }

        public static float ToRadians(float degrees)
        {
            return (degrees * 0.01745329f);
        }

        public static float Lerp(float lower, float upper, float amount)
        {
            return (lower + (upper - lower) * amount);
        }

        public static double Lerp(double value1, double value2, double amount)
        {
            return (value1 + ((value2 - value1) * amount));
        }

    }
}
