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
checksum (4 byte)
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

        public string Header { get; private set; } = "ZSPR";
        const int headerOffset = 0;
        const int headerLength = 4;

        public byte Version { get; private set; } = 1;
        const int versionOffset = headerOffset + headerLength;
        const int versionLength = 1;
        const int currentVersion = 1;

        public uint CheckSum { get; private set; }
        const int checksumOffset = versionOffset + versionLength;
        const int checksumLength = 4;

        public uint SpriteDataOffset { get; private set; }
        const int spriteDataOffsetOffset = checksumOffset + checksumLength;
        const int spriteDataOffsetLength = 4;

        public ushort SpriteDataLength { get; private set; }
        const int spriteDataLengthOffset = spriteDataOffsetOffset + spriteDataOffsetLength;
        const int spriteDataLengthLength = 2;

        public uint PaletteDataOffset { get; private set; }
        const int paletteDataOffsetOffset = spriteDataLengthOffset + spriteDataLengthLength;
        const int paletteDataOffsetLength = 4;

        public ushort PaletteDataLength { get; private set; }
        const int paletteDataLengthOffset = paletteDataOffsetOffset + paletteDataOffsetLength;
        const int paletteDataLengthLength = 2;

        public byte[] Reserved { get; private set; } = new byte[reservedLength];
        const int reservedOffset = paletteDataLengthOffset + paletteDataLengthLength;
        const int reservedLength = 8;

        string displayText;
        byte[] displayBytes;
        public string DisplayText
        {
            get { return displayText; }
            set
            {
                displayText = value;
                displayBytes = Encoding.Unicode.GetBytes(displayText + '\0');
                displayBytesLength = (uint)displayBytes.Length;

                SpriteDataOffset = displayTextOffset + displayBytesLength + authorBytesLength;
                PaletteDataOffset = SpriteDataOffset + SpriteDataLength;
            }
        }
        const uint displayTextOffset = reservedOffset + reservedLength;
        uint displayBytesLength = 0;

        string author;
        byte[] authorBytes;
        public string Author
        {
            get { return author; }
            set
            {
                author = value;
                authorBytes = Encoding.Unicode.GetBytes(author + '\0');
                authorBytesLength = (uint)authorBytes.Length;

                SpriteDataOffset = displayTextOffset + displayBytesLength + authorBytesLength;
                PaletteDataOffset = SpriteDataOffset + SpriteDataLength;
            }
        }
        uint authorBytesLength = 0;

        public byte[] PixelData { get; set; }

        public byte[] PaletteData { get; set; }

        public Sprite()
        {
            Version = 1;
            CheckSum = 0xFFFF0000;
            SpriteDataLength = 0x7000;
            PaletteDataLength = 0x78;
            DisplayText = "Unknown";
            Author = "Unknown";
            PixelData = new byte[SpriteDataLength];
            PaletteData = new byte[PaletteDataLength];
        }

        public Sprite(byte[] rawData)
        {
            if (rawData.Length == 0x7078)
            {
                // old headerless sprite file
                Version = 0;
                CheckSum = 0;
                SpriteDataLength = 0x7000;
                SpriteDataOffset = 0;
                PaletteDataLength = 0x78;
                PaletteDataOffset = 0x7000;
                DisplayText = "";
                Author = "";
                PixelData = new byte[SpriteDataLength];
                Array.Copy(rawData, PixelData, SpriteDataLength);
                PaletteData = new byte[PaletteDataLength];
                Array.Copy(rawData, PaletteDataOffset, PaletteData, 0, PaletteDataLength);
                return;
            }

            if(rawData.Length < headerLength + versionLength + checksumLength + spriteDataOffsetLength + spriteDataLengthLength + paletteDataOffsetLength + paletteDataLengthLength)
            {
                throw new Exception("Invalid sprite file. Too short.");
            }
            if (false == IsZSprite(rawData))
            {
                throw new Exception("Invalid sprite file. Wrong header.");
            }

            Version = rawData[versionOffset];

            CheckSum = bytesToUInt(rawData, checksumOffset);

            SpriteDataOffset = bytesToUInt(rawData, spriteDataOffsetOffset);
            SpriteDataLength = bytesToUShort(rawData, spriteDataLengthOffset);

            PaletteDataOffset = bytesToUInt(rawData, paletteDataOffsetOffset);
            PaletteDataLength = bytesToUShort(rawData, paletteDataLengthOffset);

            Array.Copy(rawData, reservedOffset, Reserved, 0, reservedLength);

            uint endOfDisplay = GetNullTerminatorLocation(rawData, displayTextOffset);
            uint displayLength = endOfDisplay - displayTextOffset;
            if (displayLength > 0)
            {
                byte[] displayTextBytes = new byte[displayLength];
                Array.Copy(rawData, displayTextOffset, displayTextBytes, 0, displayLength);
                DisplayText = Encoding.Unicode.GetString(displayTextBytes);
            }
            else
            { 
                DisplayText = "";
            }

            uint authorTextOffset = endOfDisplay + 2;
            uint endOfAuthor = GetNullTerminatorLocation(rawData, authorTextOffset);
            uint authorLength = endOfAuthor - authorTextOffset;
            if(authorLength > 0)
            {
                byte[] authorTextBytes = new byte[authorLength];
                Array.Copy(rawData, authorTextOffset, authorTextBytes, 0, authorLength);
                Author = Encoding.Unicode.GetString(authorTextBytes);
            }
            else
            {
                Author = "";
            }

            PixelData = new byte[SpriteDataLength];
            Array.Copy(rawData, SpriteDataOffset, PixelData, 0, SpriteDataLength);
            PaletteData = new byte[PaletteDataLength];
            Array.Copy(rawData, PaletteDataOffset, PaletteData, 0, PaletteDataLength);
        }

        public uint GetNullTerminatorLocation(byte[] rawData, uint offset)
        {
            for(uint i = offset; i < rawData.Length; i+=2)
            {
                if(rawData[i] == 0 && i+1 < rawData.Length && rawData[i+1] == 0)
                {
                    return i;
                }
            }

            return offset;
        }

        public bool IsZSprite(byte[] rawData)
        {
            if (rawData.Length < 4 || rawData[0] != 'Z' || rawData[1] != 'S' || rawData[2] != 'P' || rawData[3] != 'R')
            {
                return false;
            }

            return true;
        }

        public byte[] ToByteArray()
        {
            Version = currentVersion; // update the version

            byte[] ret = new byte[headerLength + versionLength + checksumLength + spriteDataOffsetLength + spriteDataLengthLength + paletteDataOffsetLength + paletteDataLengthLength + reservedLength + displayBytesLength + authorBytesLength + SpriteDataLength + PaletteDataLength];

            int i = 0;
            ret[i++] = (byte)Header[0];
            ret[i++] = (byte)Header[1];
            ret[i++] = (byte)Header[2];
            ret[i++] = (byte)Header[3];

            ret[i++] = Version;

            // check sum
            byte[] checksum = BitConverter.GetBytes(CheckSum);
            ret[i++] = checksum[0];
            ret[i++] = checksum[1];
            ret[i++] = checksum[2];
            ret[i++] = checksum[3];

            byte[] spriteDataOffset = BitConverter.GetBytes(SpriteDataOffset);
            ret[i++] = spriteDataOffset[0];
            ret[i++] = spriteDataOffset[1];
            ret[i++] = spriteDataOffset[2];
            ret[i++] = spriteDataOffset[3];

            byte[] spriteDataLength = BitConverter.GetBytes(SpriteDataLength);
            ret[i++] = spriteDataLength[0];
            ret[i++] = spriteDataLength[1];

            byte[] paletteDataOffset = BitConverter.GetBytes(PaletteDataOffset);
            ret[i++] = paletteDataOffset[0];
            ret[i++] = paletteDataOffset[1];
            ret[i++] = paletteDataOffset[2];
            ret[i++] = paletteDataOffset[3];

            byte[] paletteDataLength = BitConverter.GetBytes(PaletteDataLength);
            ret[i++] = paletteDataLength[0];
            ret[i++] = paletteDataLength[1];

            ret[i++] = Reserved[0];
            ret[i++] = Reserved[1];
            ret[i++] = Reserved[2];
            ret[i++] = Reserved[3];
            ret[i++] = Reserved[4];
            ret[i++] = Reserved[5];
            ret[i++] = Reserved[6];
            ret[i++] = Reserved[7];

            for (int x = 0; x < displayBytes.Length; x++)
            {
                ret[i++] = displayBytes[x];
            }

            for (int x = 0; x < authorBytes.Length; x++)
            {
                ret[i++] = authorBytes[x];
            }

            for (int x=0; x < PixelData.Length; x++)
            {
                ret[i++] = PixelData[x];
            }

            for (int x = 0; x < PaletteData.Length; x++)
            {
                ret[i++] = PaletteData[x];
            }

            return ret;
        }

        uint bytesToUInt(byte[] bytes, int offset)
        {
            return BitConverter.ToUInt32(bytes, offset);
        }

        ushort bytesToUShort(byte[] bytes, int offset)
        {
            return BitConverter.ToUInt16(bytes, offset);
        }
    }
}
