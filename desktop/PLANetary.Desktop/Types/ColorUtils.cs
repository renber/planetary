using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLANetary.Types
{
    public static class ColorUtils
    {

        /// <summary>
        /// Returns a slightly darker version of the given color
        /// </summary>        
        public static RGBColor Darken(this RGBColor color)
        {
            return new RGBColor((byte)(color.R * 0.8), (byte)(color.G * 0.8), (byte)(color.B * 0.8));
        }

        public static RGBColor Brighten(this RGBColor color)
        {
            return new RGBColor((byte)(color.R * 1.2), (byte)(color.G * 1.2), (byte)(color.B * 1.2));
        }

        /// <summary>
        /// Converts a RGBColor to a System.Windows.Media.Color used by Wpf
        /// </summary>        
        public static System.Windows.Media.Color ToWpfColor(this RGBColor color)
        {
            return System.Windows.Media.Color.FromRgb(color.R, color.G, color.B);
        }

        /// <summary>
        /// Converts a RGBColor to a System.Windows.Media.Color used by Wpf
        /// </summary>        
        /// <param name="alpha">The alpha value of the new color</param>
        public static System.Windows.Media.Color ToWpfColor(this RGBColor color, byte alpha)
        {
            return System.Windows.Media.Color.FromArgb(alpha, color.R, color.G, color.B);
        }

    }
}
