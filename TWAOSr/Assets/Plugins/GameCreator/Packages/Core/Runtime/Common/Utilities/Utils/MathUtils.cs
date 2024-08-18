using System;

namespace GameCreator.Runtime.Common
{
    public static class MathUtils
    {
        // INTERPOLATION: -------------------------------------------------------------------------
        
        public static double Lerp(double a, double b, double t)
        {
            return LerpUnclamped(a, b, Clamp01(t));
        }
        
        public static double LerpUnclamped(double a, double b, double t)
        {
            return a + (b - a) * t;
        }
        
        public static double Eerp(double a, double b, double t)
        {
            return EerpUnclamped(a, b, Clamp01(t));
        }

        public static double EerpUnclamped(double a, double b, double t)
        {
            return Math.Pow(a, 1 - t) * Math.Pow(b, t);
        }
        
        // MAX: -----------------------------------------------------------------------------------

        public static float Max(float a, float b, float c)
        {
            return Math.Max(Math.Max(a, b), c);
        }
        
        public static float Max(float a, float b, float c, float d)
        {
            return Math.Max(Math.Max(a, b), Math.Max(c, d));
        }
        
        // MIN: -----------------------------------------------------------------------------------

        public static float Min(float a, float b, float c)
        {
            return Math.Min(Math.Min(a, b), c);
        }
        
        public static float Min(float a, float b, float c, float d)
        {
            return Math.Min(Math.Min(a, b), Math.Min(c, d));
        }
        
        // CLAMP: ---------------------------------------------------------------------------------

        public static double Clamp01(double value)
        {
            return value switch
            {
                < 0 => 0,
                > 1 => 1,
                _ => value
            };
        }
    }
}