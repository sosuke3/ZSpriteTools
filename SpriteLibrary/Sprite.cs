using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpriteLibrary
{
    public class Sprite
    {
        /*
header flag (4 bytes) = ZSPR
version (1 byte)
checksum (2 byte)
sprite data offset (4 bytes)
sprite data length (2 bytes)
palette data offset (4 bytes)
palette data length (2 bytes)
reserved (8 bytes)
display text (x bytes) (null terminated)
author (x bytes) (null terminated)
sprite data (0x7000 bytes)
palette data (0x78 + ? bytes) (remember to add extra bytes for gloves)
         */
        // 0-3
        public string Header { get; private set; } = "ZSPR";
        const int headerOffset = 0;
        const int headerLength = 4;
        // 4
        public byte Version { get; private set; } = 1;
        const int versionOffset = headerOffset + headerLength;
        const int versionLength = 1;
        // 5
        public byte CheckSum { get; private set; }
        const int checksumOffset = versionOffset + versionLength;
        const int checksumLength = 1;
        // 6-9
        public int SpriteDataOffset { get; private set; }
        const int spriteDataOffsetOffset = checksumOffset + checksumLength;
        const int spriteDataOffsetLength = 4;
        // 10-11
        public int SpriteDataLength { get; private set; }
        const int spriteDataLengthOffset = spriteDataOffsetOffset + spriteDataOffsetLength;
        const int spriteDataLengthLength = 2;
        // 12-15
        public int PaletteDataOffset { get; private set; }
        const int paletteDataOffsetOffset = spriteDataLengthOffset + spriteDataLengthLength;
        const int paletteDataOffsetLength = 4;
        // 16-17
        public int PaletteDataLength { get; private set; }
        const int paletteDataLengthOffset = paletteDataOffsetOffset + paletteDataOffsetLength;
        const int paletteDataLengthLength = 2;
        public byte[] Reserved { get; private set; } = new byte[8];
        const int reservedOffset = paletteDataLengthOffset + paletteDataLengthLength;
        const int reservedLength = 8;
        public string DisplayText { get; set; }
        const int displayTextOffset = reservedOffset + reservedLength;
        public string Author { get; set; }
        public byte[] PixelData { get; set; }
        public byte[] PaletteData { get; set; }

        public Sprite(byte[] rawData)
        {
            if(false == IsZSprite(rawData))
            {
                throw new Exception("Invalid sprite file");
            }

            Version = rawData[4];
        }

        public bool IsZSprite(byte[] rawData)
        {
            if (rawData.Length < 4 || rawData[0] != 'Z' || rawData[1] != 'S' || rawData[2] != 'P' || rawData[3] != 'R')
            {
                return false;
            }

            return true;
        }
    }
}
