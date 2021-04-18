using System;

namespace SimpleMeshGraphics
{
    public class MathHelper
    {
        public static float DegreesToRadians(float degrees)
        {
            return MathF.PI / 180f * degrees;
        }

        public static float DegreesToRadians(double degrees)
        {
            return (float) (Math.PI / 180.0 * degrees);
        }

        public static float RadiansToDegrees(float radians)
        {
            return (180f / MathF.PI) * radians;
        }

        public static float RadiansToDegrees(double radians)
        {
            return (float)((180.0 / Math.PI) * radians);
        }

        public static void Clamp(ref float val, float max, float min)
        {
            if (val > max)
            {
                val = max;
            } else if (val < min)
            {
                val = min;
            }
        }
    }
}