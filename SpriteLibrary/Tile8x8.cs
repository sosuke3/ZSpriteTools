using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SpriteLibrary
{
    public class Tile8x8
    {
        public byte[] RawTile { get; private set; }
        public byte[] Pixels { get; private set; }

        public Tile8x8(byte[] bytes)
        {
            if(bytes.Length != 32)
            {
                throw new Exception("Invalid tile data. Length should be 32 bytes.");
            }

            RawTile = bytes;

            RebuildPixels();
        }

        void RebuildPixels()
        {
            Pixels = new byte[8 * 8];

            for(int i=0; i<32; i++)
            {
                int plane = (i % 2) + ((i / 16) * 2);
                int row = i % 16 / 2;
                byte tileByte = RawTile[i];

                for(int p=0; p<8; p++)
                {
                    byte bit = (byte)((tileByte >> (7 - p)) & 0x1);
                    byte planeValue = (byte)(bit << plane);
                    Pixels[row * 8 + p] |= planeValue;
                }
            }
        }

        public void Draw(Graphics g, Color[] palette, int posX, int posY)
        {
            Bitmap bitmap = new Bitmap(8, 8, PixelFormat.Format32bppArgb);

            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;

            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);

            var totalBytes = bitmapData.Stride * bitmapData.Height;
            var bpp = Bitmap.GetPixelFormatSize(bitmap.PixelFormat) / 8;
            var pixels = new byte[totalBytes];

            for(int y = 0; y < 8; y++)
            {
                for(int x = 0; x < 8; x++)
                {
                    int pixelPosition = y * bitmapData.Stride + (x * 4);
                    if (Pixels[x + y * 8] == 0)
                    {
                        pixels[pixelPosition + 0] = 0;
                        pixels[pixelPosition + 1] = 0;
                        pixels[pixelPosition + 2] = 0;
                        pixels[pixelPosition + 3] = 0;
                    }
                    else
                    {
                        Color c = palette[Pixels[x + y * 8] - 1];
                        pixels[pixelPosition + 0] = c.B;
                        pixels[pixelPosition + 1] = c.G;
                        pixels[pixelPosition + 2] = c.R;
                        pixels[pixelPosition + 3] = 255;
                    }
                }
            }

            IntPtr ptrFirstPixel = bitmapData.Scan0;
            Marshal.Copy(pixels, 0, ptrFirstPixel, pixels.Length);
            bitmap.UnlockBits(bitmapData);

            g.DrawImage(bitmap, new Rectangle(posX, posY, 8, 8), 0, 0, 8, 8, GraphicsUnit.Pixel);
        }
    }
}
