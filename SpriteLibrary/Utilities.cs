using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpriteLibrary
{
    public static class Utilities
    {
        public static Color GetColorFromBytes(ushort s)
        {
            int b = (int)Math.Ceiling((float)((s & 0x7C00) >> 10) * 0xFF / 0x1F);
            int g = (int)Math.Ceiling((float)((s & 0x03E0) >> 5) * 0xFF / 0x1F);
            int r = (int)Math.Ceiling((float)((s & 0x001F) >> 0) * 0xFF / 0x1F);

            return Color.FromArgb(r, g, b);
        }

        public static Color GetColorFromBytes(byte b1, byte b2)
        {
            ushort combined = (ushort)(((ushort)b1 | ((ushort)b2 << 8)));

            return GetColorFromBytes(combined);
        }

        public static ushort GetUShortFromColor(Color c)
        {
            byte b = (byte)((c.B >> 3) & 0x1F);
            byte g = (byte)((c.G >> 3) & 0x1F);
            byte r = (byte)((c.R >> 3) & 0x1F);
            ushort combined = (ushort)((b << 10) | (g << 5) | (r));
            return combined;
        }

        public static byte[] GetBytesFromColor(Color c)
        {
            byte[] ret = new byte[2];

            ushort combined = GetUShortFromColor(c);
            ret[0] = (byte)(combined & 0xFF);
            ret[1] = (byte)((combined >> 8) & 0xFF);
            return ret;
        }
    }
}
