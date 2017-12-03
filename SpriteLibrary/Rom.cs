using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpriteLibrary
{
    public class Rom
    {
        byte[] romData;
        public byte[] RomData { get { return romData; } }

        public Rom(string filename)
        {
            this.romData = File.ReadAllBytes(filename);
        }

        public Rom(byte[] romData)
        {
            this.romData = romData;
        }

        public PlayerSprite GetSprite()
        {
            var bytes = new byte[0x7078];
            var palette = new byte[0x7C];

            Array.Copy(this.romData, 0x80000, bytes, 0, 0x7000);
            Array.Copy(this.romData, 0xDD308, bytes, 0x7000, 0x78);
            Array.Copy(this.romData, 0xDD308, palette, 0, 0x78);
            Array.Copy(this.romData, 0xDEDF5, palette, 0x78, 4);
            var ret = new PlayerSprite(bytes);            
            ret.PaletteData = palette;

            return ret;
        }

        public void InjectSprite(Sprite sprite)
        {
            Array.Copy(sprite.PixelData, 0, this.romData, 0x80000, 0x7000);
            Array.Copy(sprite.PaletteData, 0, this.romData, 0xDD308, 0x78);

            if(sprite.PaletteData.Length < 0x7C)
            {
                Array.Copy(new byte[] { 0xF6, 0x52, 0x76, 0x03 }, 0, this.romData, 0xDEDF5, 4);
            }
            else
            {
                Array.Copy(sprite.PaletteData, 0x78, this.romData, 0xDEDF5, 4);
            }
        }
    }
}
