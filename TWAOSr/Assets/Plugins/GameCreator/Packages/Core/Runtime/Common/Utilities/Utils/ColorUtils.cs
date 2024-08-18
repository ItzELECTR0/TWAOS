using System.Globalization;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    public static class ColorUtils
    {
        private static readonly CultureInfo CULTURE = CultureInfo.InvariantCulture;
        private const NumberStyles HEX = NumberStyles.HexNumber;

        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public static Color Parse(string input)
        {
            if (input.Length > 9) return default;

            if (input[0] == '#') input = input[1..];
            int inputLength = input.Length;
            
            if (inputLength != 6 && inputLength != 8) return default;
            int r = int.Parse($"{input[0]}{input[1]}", HEX, CULTURE);
            int g = int.Parse($"{input[2]}{input[3]}", HEX, CULTURE);
            int b = int.Parse($"{input[4]}{input[5]}", HEX, CULTURE);
            
            int a = inputLength == 8
                ? int.Parse($"{input[6]}{input[7]}", HEX, CULTURE)
                : 255;
            
            return new Color(
                r / 255f,
                g / 255f,
                b / 255f,
                a / 255f
            );
        }
        
        public static Color SetRed(Color color, float red) => new Color(
            red, 
            color.g,
            color.b, 
            color.a
        );
        
        public static Color SetGreen(Color color, float green) => new Color(
            color.r, 
            green,
            color.b, 
            color.a
        );
        
        public static Color SetBlue(Color color, float blue) => new Color(
            color.r,
            color.g,
            blue, 
            color.a
        );
        
        public static Color SetAlpha(Color color, float alpha) => new Color(
            color.r, 
            color.g,
            color.b, 
            alpha
        );
    }
}