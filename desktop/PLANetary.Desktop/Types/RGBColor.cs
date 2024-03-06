using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLANetary.Types
{
    /// <summary>
    /// Represents a 24bit color value
    /// </summary>
    public class RGBColor
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }

        public RGBColor()
        {
            R = G = B = 0;
        }

        public RGBColor(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }       

        public string ToHex()
        {
            return R.ToString("X2") + G.ToString("X2") + B.ToString("X2");
        }

        public override bool Equals(object obj)
        {
            if (obj is RGBColor)
                return Equals((RGBColor)obj);
            else
                return base.Equals(obj);
        }

        public bool Equals(RGBColor compareTo)
        {
            return compareTo.R == R && compareTo.G == G && compareTo.B == B;
        }

        public override int GetHashCode()
        {
            return R ^ G ^ B;
        }

    }
}
