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

        static Color[] BuildPaletteColorsFromStringArray(string[] gimpFile)
        {
            if(gimpFile[0] != "GIMP Palette")
            {
                throw new Exception("File is not a GIMP palette.");
            }

            return null;
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
