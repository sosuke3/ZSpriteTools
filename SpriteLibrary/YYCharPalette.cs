using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpriteLibrary
{
    public class YYCharPalette
    {
        public static byte[] BuildPaletteFromColorArray(Color[] colors)
        {
            Color gloves = Utilities.GetColorFromBytes(0xF6, 0x52);
            Color mitts = Utilities.GetColorFromBytes(0x76, 0x03);
            if (colors.Length == 62)
            {
                gloves = colors[60];
                mitts = colors[61];
            }

            byte[] outputPalette = new byte[3 * 16 * 4];
            int index = 0;

            SetColor(outputPalette, index, Color.FromArgb(0, 0, 0));
            index += 3;
            for(int i = 0; i < 15; i++)
            {
                SetColor(outputPalette, index, colors[i]);
                index += 3;
            }
            SetColor(outputPalette, index, Color.FromArgb(0, 0, 0));
            index += 3;
            for (int i = 15; i < 30; i++)
            {
                SetColor(outputPalette, index, colors[i]);
                index += 3;
            }
            SetColor(outputPalette, index, Color.FromArgb(0, 0, 0));
            index += 3;
            for (int i = 30; i < 45; i++)
            {
                SetColor(outputPalette, index, colors[i]);
                index += 3;
            }
            SetColor(outputPalette, index, Color.FromArgb(0, 0, 0));
            index += 3;
            for (int i = 45; i < 60; i++)
            {
                SetColor(outputPalette, index, colors[i]);
                index += 3;
            }
            SetColor(outputPalette, (16 + 13) * 3, gloves);
            SetColor(outputPalette, (32 + 13) * 3, mitts);

            return outputPalette;
        }

        static void SetColor(byte[] palette, int index, Color color)
        {
            palette[index] = color.R;
            palette[index + 1] = color.G;
            palette[index + 2] = color.B;
        }

        public static Color[] BuildSpritePaletteColorsFromByteArray(byte[] yyPalette)
        {
            if(yyPalette.Length < 64 * 3)
            {
                throw new Exception("Invalid YY-Char Palette. Needs more colors.");
            }

            List<Color> palette = new List<Color>();

            int yyIndex = 0;
            for(int i=0; i<64; i++)
            {
                int r = yyPalette[yyIndex];
                int g = yyPalette[yyIndex + 1];
                int b = yyPalette[yyIndex + 2];
                palette.Add(Color.FromArgb(r, g, b));

                yyIndex += 3;
            }

            // move gloves to end
            palette.Add(palette[16 + 13]);
            palette.Add(palette[32 + 13]);
            // remove transparent
            palette.RemoveAt(48);
            palette.RemoveAt(32);
            palette.RemoveAt(16);
            palette.RemoveAt(0);

            return palette.ToArray();
        }
    }
}
