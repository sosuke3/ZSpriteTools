using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpriteLibrary
{
    public class GIMPPalette
    {
        public static string BuildPaletteFromColorArray(Color[] colors)
        {
            Color gloves = Utilities.GetColorFromBytes(0xF6, 0x52);
            Color mitts = Utilities.GetColorFromBytes(0x76, 0x03);
            if(colors.Length == 62)
            {
                gloves = colors[60];
                mitts = colors[61];
            }

            StringBuilder output = new StringBuilder();
            output.AppendLine("GIMP Palette");
            output.AppendLine("Name: Link");
            output.AppendLine("Columns: 0");
            output.AppendLine("#");

            WriteColor(output, Color.FromArgb(0, 0, 0), IndexNames[0]);
            for(int i = 0; i < 15; i++)
            {
                WriteColor(output, colors[i], IndexNames[i + 1]);
            }
            WriteColor(output, gloves, IndexNames[16]);
            for (int i = 15; i < 30; i++)
            {
                WriteColor(output, colors[i], IndexNames[i + 2]);
            }
            WriteColor(output, mitts, IndexNames[32]);
            for (int i = 30; i < 45; i++)
            {
                WriteColor(output, colors[i], IndexNames[i + 3]);
            }
            WriteColor(output, Color.FromArgb(0, 0, 0), IndexNames[48]);
            for (int i = 45; i < 60; i++)
            {
                WriteColor(output, colors[i], IndexNames[i + 4]);
            }

            return output.ToString();
        }

        static void WriteColor(StringBuilder output, Color color, string name)
        {
            var r = String.Format("{0,3}", color.R);
            var g = String.Format("{0,3}", color.G);
            var b = String.Format("{0,3}", color.B);
            output.AppendLine($"{r} {g} {b} {name}");
        }
        /*
        GIMP Palette
        Name: Link
        Columns: 0
        #
          0   0   0	Transparent
        248 248 248	White
        240 216  64	Yellow
        184 104  32	Skin
        240 160 104	Lieutenant face
         40  40  40	Outline
        248 120   0	Orange
        192  24  32	Red
        232  96 176	Hair
         56 144 104	Tunic
         64 216 112	Light tunic
         80 144  16	Hat
        120 184  32	Light hat
        224 144  80	Gloves
        136  88  40	Sleeves
        192 128 240	Water
        184 184 168	Glove color
        248 248 248	Untitled
        240 216  64	Untitled
        184 104  32	Untitled
        240 160 104	Untitled
         40  40  40	Untitled
        248 120   0	Untitled
        192  24  32	Untitled
        232  96 176	Untitled
          0  96 208	Untitled
        136 160 232	Untitled
        192 160  72	Untitled
        248 216 128	Untitled
        224 144  80	Untitled
        200  96  32	Untitled
        192 128 240	Untitled
        176 216   0	Mitts color
        248 248 248	Untitled
        240 216  64	Untitled
        184 104  32	Untitled
        240 160 104	Untitled
         40  40  40	Untitled
        248 120   0	Untitled
        192  24  32	Untitled
        232  96 176	Untitled
        184  16  32	Untitled
        240  88 136	Untitled
        152 120 216	Untitled
        200 168 248	Untitled
        224 144  80	Untitled
         56 136  64	Untitled
        192 128 240	Untitled
          0   0   0	Transparent - Bunny holds no data here
        248 248 248	Untitled
        240 216  64	Untitled
        184 104  32	Untitled
        240 160 104	Untitled
         40  40  40	Untitled
        248 120   0	Untitled
        192  24  32	Untitled
        184  96 120	Untitled
         56 144 104	Untitled
         64 216 112	Untitled
         80 144  16	Untitled
        120 184  32	Untitled
        240 152 168	Untitled
        144  24  48	Untitled
        192 128 240 Untitled
        */

        static string[] IndexNames =
        {
            "Transparent",
            "White",
            "Yellow",
            "Skin",
            "Lieutenant face",
            "Outline",
            "Orange",
            "Red",
            "Hair",
            "Tunic",
            "Light tunic",
            "Hat",
            "Light hat",
            "Gloves",
            "Sleeves",
            "Water",
            "Glove color",
            "Untitled",
            "Untitled",
            "Untitled",
            "Untitled",
            "Untitled",
            "Untitled",
            "Untitled",
            "Untitled",
            "Untitled",
            "Untitled",
            "Untitled",
            "Untitled",
            "Untitled",
            "Untitled",
            "Untitled",
            "Mitts color",
            "Untitled",
            "Untitled",
            "Untitled",
            "Untitled",
            "Untitled",
            "Untitled",
            "Untitled",
            "Untitled",
            "Untitled",
            "Untitled",
            "Untitled",
            "Untitled",
            "Untitled",
            "Untitled",
            "Untitled",
            "Transparent - Bunny holds no data here",
            "Untitled",
            "Untitled",
            "Untitled",
            "Untitled",
            "Untitled",
            "Untitled",
            "Untitled",
            "Untitled",
            "Untitled",
            "Untitled",
            "Untitled",
            "Untitled",
            "Untitled",
            "Untitled",
            "Untitled",
        };
    }
}
