using Microsoft.Xna.Framework;
using System;

namespace CSG.Sharp.Utils
{
    public static class ColorUtils
    {
        private static readonly Random _random;

        static ColorUtils()
        {
            _random = new Random();
        }

        /// <summary>
        /// Brush colours only vary from given mix
        /// </summary>
        public static Color GenerateRandomColor(Color mix)
        {
            int red = _random.Next(256);
            int green = _random.Next(256);
            int blue = _random.Next(256);

            if (mix != null)
            {
                red = (red + mix.R) / 2;
                green = (green + mix.G) / 2;
                blue = (blue + mix.B) / 2;
            }

            Color color = new Color(red, green, blue);
            return color;
        }

        /// <summary>
        /// Brush colours only vary from shades of green and blue
        /// </summary>
        public static Color GenerateRandomBrushColour()
        {
            return new Color(0, _random.Next(128, 256), _random.Next(128, 256));
        }
    }
}