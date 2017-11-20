using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            output.AppendLine($"{r} {g} {b}\t{name}");
        }

        public static Color[] BuildSpritePaletteColorsFromStringArray(string[] gimpFile)
        {
            int currentLine = 0;
            if(gimpFile[currentLine++] != "GIMP Palette")
            {
                throw new Exception("File is not a GIMP palette.");
            }

            if(gimpFile[currentLine++].StartsWith("Name: ", StringComparison.Ordinal))
            {
                if(gimpFile[currentLine++].StartsWith("Column: ", StringComparison.Ordinal))
                {

                }
            }

            List<Color> palette = new List<Color>();

            while(currentLine < gimpFile.Length)
            {
                var line = gimpFile[currentLine++];
                if(!line.StartsWith("#", StringComparison.Ordinal) && !String.IsNullOrEmpty(line))
                {
                    var colors = Regex.Replace(line.Trim(), @"\s+", " ").Split(' ', '\t');
                    int r, g, b;
                    if(!Int32.TryParse(colors[0], out r))
                    {
                        throw new Exception($"Invalid red color value in palette [line {currentLine-1}]: {colors[0]}");
                    }
                    if (!Int32.TryParse(colors[1], out g))
                    {
                        throw new Exception($"Invalid green color value in palette [line {currentLine - 1}]: {colors[1]}");
                    }
                    if (!Int32.TryParse(colors[2], out b))
                    {
                        throw new Exception($"Invalid blue color value in palette [line {currentLine - 1}]: {colors[2]}");
                    }

                    if(r < 0 || r > 255 || g < 0 || g > 255 || b < 0 || b > 255)
                    {
                        throw new Exception($"Invalid color value in palette [line {currentLine - 1}]");
                    }

                    palette.Add(Color.FromArgb(r, g, b));
                }
            }

            // move gloves to end
            palette.Add(palette[16]);
            palette.Add(palette[32]);
            // remove transparent
            palette.RemoveAt(48);
            palette.RemoveAt(32);
            palette.RemoveAt(16);
            palette.RemoveAt(0);


            return palette.ToArray();
        }

        static string[] IndexNames =
        {
            "Transparent",
            "Eyes / White",
            "Belt / Yellow",
            "Dark Face",
            "Light Face",
            "Outline",
            "Hat Trim / Orange",
            "Mouth / Red",
            "Hair",
            "Dark Tunic",
            "Light Tunic",
            "Dark Hat",
            "Light Hat",
            "Hands",
            "Sleeves",
            "Water Ripples",
            "Glove color",
            "Eyes / White",
            "Belt / Yellow",
            "Dark Face",
            "Light Face",
            "Outline",
            "Hat Trim / Orange",
            "Mouth / Red",
            "Hair",
            "Dark Tunic",
            "Light Tunic",
            "Dark Hat",
            "Light Hat",
            "Hands",
            "Sleeves",
            "Water Ripples",
            "Mitts color",
            "Eyes / White",
            "Belt / Yellow",
            "Dark Face",
            "Light Face",
            "Outline",
            "Hat Trim / Orange",
            "Mouth / Red",
            "Hair",
            "Dark Tunic",
            "Light Tunic",
            "Dark Hat",
            "Light Hat",
            "Hands",
            "Sleeves",
            "Water Ripples",
            "Transparent",
            "Eyes / White",
            "Belt / Yellow",
            "Dark Face",
            "Light Face",
            "Outline",
            "Hat Trim / Orange",
            "Mouth / Red",
            "Hair",
            "Dark Tunic",
            "Light Tunic",
            "Dark Hat",
            "Light Hat",
            "Hands",
            "Sleeves",
            "Water Ripples",
        };
    }
}
