using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpriteLibrary
{
    public class Sprite
    {
        public const string OpenFileDialogFilter = "Sprite File (*.spr;*.zspr)|*.spr;*.zspr|ZSprite File (*.zspr)|*.zspr|Legacy Sprite File (*.spr)|*.spr|All Files (*.*)|*.*";
        public const string SaveFileDialogFilter = "ZSprite File (*.zspr)|*.zspr|All Files (*.*)|*.*";

        /*
        header flag (4 bytes) = ZSPR
        version (1 byte)
        checksum (4 byte) = 2 bytes + complement 2 bytes
        pixel data offset (4 bytes)
        pixel data length (2 bytes)
        palette data offset (4 bytes)
        palette data length (2 bytes)
        reserved (8 bytes)
        display text (x bytes) (unicode, null terminated)
        author (x bytes) (unicode, null terminated)
        author rom display (x bytes) (ascii, null terminated)
        sprite data (0x7000 bytes for character sprites)
        palette data (0x78 + 4 bytes [gloves] for character sprites) (remember to add extra bytes for gloves)
        */

        public string Header { get; protected set; } = "ZSPR";
        protected const int headerOffset = 0;
        protected const int headerLength = 4;

        public byte Version { get; protected set; } = 1;
        protected const int versionOffset = headerOffset + headerLength;
        protected const int versionLength = 1;
        protected const int currentVersion = 1;

        public uint CheckSum { get; protected set; }
        protected const int checksumOffset = versionOffset + versionLength;
        protected const int checksumLength = 4;

        public bool HasValidChecksum { get; protected set; }

        public uint PixelDataOffset { get; protected set; }
        protected const int pixelDataOffsetOffset = checksumOffset + checksumLength;
        protected const int pixelDataOffsetLength = 4;

        public ushort PixelDataLength { get; protected set; }
        protected const int pixelDataLengthOffset = pixelDataOffsetOffset + pixelDataOffsetLength;
        protected const int pixelDataLengthLength = 2;

        public uint PaletteDataOffset { get; protected set; }
        protected const int paletteDataOffsetOffset = pixelDataLengthOffset + pixelDataLengthLength;
        protected const int paletteDataOffsetLength = 4;

        public ushort PaletteDataLength { get; protected set; }
        protected const int paletteDataLengthOffset = paletteDataOffsetOffset + paletteDataOffsetLength;
        protected const int paletteDataLengthLength = 2;

        public byte[] Reserved { get; protected set; } = new byte[reservedLength];
        protected const int reservedOffset = paletteDataLengthOffset + paletteDataLengthLength;
        protected const int reservedLength = 8;

        protected string displayText;
        protected byte[] displayBytes;
        public string DisplayText
        {
            get { return displayText; }
            set
            {
                displayText = value;
                displayBytes = Encoding.Unicode.GetBytes(displayText + '\0');
                displayBytesLength = (uint)displayBytes.Length;

                RecalculatePixelAndPaletteOffset();
            }
        }

        protected const uint displayTextOffset = reservedOffset + reservedLength;
        protected uint displayBytesLength = 0;

        protected string author;
        protected byte[] authorBytes;
        public string Author
        {
            get { return author; }
            set
            {
                author = value;
                authorBytes = Encoding.Unicode.GetBytes(author + '\0');
                authorBytesLength = (uint)authorBytes.Length;

                RecalculatePixelAndPaletteOffset();
            }
        }
        protected uint authorBytesLength = 0;

        protected string authorRomDisplay;
        protected byte[] authorRomDisplayBytes;
        public string AuthorRomDisplay
        {
            get { return authorRomDisplay; }
            set
            {
                if(value.Length > 20)
                {
                    value = value.Substring(0, 20);
                }
                authorRomDisplay = value;

                authorRomDisplayBytes = Encoding.ASCII.GetBytes(authorRomDisplay + '\0');
                authorRomDisplayBytesLength = (uint)authorRomDisplayBytes.Length;

                RecalculatePixelAndPaletteOffset();
            }
        }
        protected uint authorRomDisplayBytesLength;
        public const int AuthorRomDisplayMaxLength = 20;

        protected byte[] pixelData;
        public byte[] PixelData
        {
            get { return pixelData; }
            set
            {
                pixelData = value;

                RecalculatePixelAndPaletteOffset();
                BuildTileArray();
            }
        }

        public Tile8x8[] Tiles { get; protected set; }


        protected byte[] paletteData;
        public byte[] PaletteData
        {
            get { return paletteData; }
            set
            {
                paletteData = value;

                RecalculatePixelAndPaletteOffset();
                RebuildPalette();
                BuildTileArray();
            }
        }

        public Color[] Palette { get; protected set; }

        public Sprite()
        {
            Version = 1;
            CheckSum = 0xFFFF0000;
            PixelDataLength = 0x7000;
            PaletteDataLength = 0x78;
            DisplayText = "Unknown";
            Author = "Unknown";
            AuthorRomDisplay = "Unknown";
            PixelData = new byte[PixelDataLength];
            PaletteData = new byte[PaletteDataLength];

            RebuildPalette();
            BuildTileArray();
        }

        public Sprite(byte[] rawData)
        {
            if (rawData.Length == 0x7078)
            {
                // old headerless sprite file
                Version = 0;
                CheckSum = 0;
                DisplayText = "";
                Author = "";
                AuthorRomDisplay = "";
                PixelData = new byte[0x7000];
                Array.Copy(rawData, PixelData, 0x7000);
                PaletteData = new byte[0x78];
                Array.Copy(rawData, 0x7000, PaletteData, 0, 0x78);

                PixelDataLength = 0x7000;
                PaletteDataLength = 0x78;

                RebuildPalette();
                BuildTileArray();

                return;
            }

            if(rawData.Length < headerLength + versionLength + checksumLength + pixelDataOffsetLength + pixelDataLengthLength + paletteDataOffsetLength + paletteDataLengthLength)
            {
                throw new Exception("Invalid sprite file. Too short.");
            }
            if (false == IsZSprite(rawData))
            {
                throw new Exception("Invalid sprite file. Wrong header.");
            }

            Version = rawData[versionOffset];

            CheckSum = bytesToUInt(rawData, checksumOffset);

            PixelDataOffset = bytesToUInt(rawData, pixelDataOffsetOffset);
            PixelDataLength = bytesToUShort(rawData, pixelDataLengthOffset);

            PaletteDataOffset = bytesToUInt(rawData, paletteDataOffsetOffset);
            PaletteDataLength = bytesToUShort(rawData, paletteDataLengthOffset);

            Array.Copy(rawData, reservedOffset, Reserved, 0, reservedLength);

            uint endOfDisplay = GetNullTerminatorUnicodeLocation(rawData, displayTextOffset);
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
            uint endOfAuthor = GetNullTerminatorUnicodeLocation(rawData, authorTextOffset);
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

            uint authorRomDisplayTextOffset = endOfAuthor + 2;
            uint endOfAuthorRomDisplay = GetNullTerminatorAsciiLocation(rawData, authorRomDisplayTextOffset);
            uint authorRomDisplayLength = endOfAuthorRomDisplay - authorRomDisplayTextOffset;
            if (authorRomDisplayLength > 0)
            {
                byte[] authorRomDisplayTextBytes = new byte[authorRomDisplayLength];
                Array.Copy(rawData, authorRomDisplayTextOffset, authorRomDisplayTextBytes, 0, authorRomDisplayLength);
                AuthorRomDisplay = Encoding.ASCII.GetString(authorRomDisplayTextBytes);
            }
            else
            {
                AuthorRomDisplay = "";
            }

            PixelData = new byte[PixelDataLength];
            Array.Copy(rawData, PixelDataOffset, PixelData, 0, PixelDataLength);
            PaletteData = new byte[PaletteDataLength];
            Array.Copy(rawData, PaletteDataOffset, PaletteData, 0, PaletteDataLength);

            RebuildPalette();
            BuildTileArray();

            HasValidChecksum = IsCheckSumValid();
        }

        public uint GetNullTerminatorUnicodeLocation(byte[] rawData, uint offset)
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

        public uint GetNullTerminatorAsciiLocation(byte[] rawData, uint offset)
        {
            for (uint i = offset; i < rawData.Length; i++)
            {
                if (rawData[i] == 0)
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
            return ToByteArray(false);
        }

        protected byte[] ToByteArray(bool skipChecksum)
        {
            if (false == skipChecksum)
            {
                UpdateChecksum();
            }

            Version = currentVersion; // update the version

            byte[] ret = new byte[headerLength + versionLength + checksumLength + pixelDataOffsetLength + pixelDataLengthLength + paletteDataOffsetLength + paletteDataLengthLength + reservedLength + displayBytesLength + authorBytesLength + authorRomDisplayBytesLength + PixelDataLength + PaletteDataLength];

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

            byte[] pixelDataOffset = BitConverter.GetBytes(PixelDataOffset);
            ret[i++] = pixelDataOffset[0];
            ret[i++] = pixelDataOffset[1];
            ret[i++] = pixelDataOffset[2];
            ret[i++] = pixelDataOffset[3];

            byte[] pixelDataLength = BitConverter.GetBytes(PixelDataLength);
            ret[i++] = pixelDataLength[0];
            ret[i++] = pixelDataLength[1];

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

            for (int x = 0; x < authorRomDisplayBytes.Length; x++)
            {
                ret[i++] = authorRomDisplayBytes[x];
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

        protected uint bytesToUInt(byte[] bytes, int offset)
        {
            return BitConverter.ToUInt32(bytes, offset);
        }

        protected ushort bytesToUShort(byte[] bytes, int offset)
        {
            return BitConverter.ToUInt16(bytes, offset);
        }

        protected void RecalculatePixelAndPaletteOffset()
        {
            PixelDataOffset = displayTextOffset + displayBytesLength + authorBytesLength + authorRomDisplayBytesLength;
            PaletteDataOffset = PixelDataOffset + PixelDataLength;
        }

        protected void UpdateChecksum()
        {
            byte[] checksum = { 0x00, 0x00, 0xFF, 0xFF };
            CheckSum = BitConverter.ToUInt32(checksum, 0);

            byte[] bytes = this.ToByteArray(true);
            int sum = 0;
            for(int i=0; i<bytes.Length; i++)
            {
                sum += bytes[i];
            }

            checksum[0] = (byte)(sum & 0xFF);
            checksum[1] = (byte)((sum >> 8) & 0xFF);

            int complement = (sum & 0xFFFF) ^ 0xFFFF;
            checksum[2] = (byte)(complement & 0xFF);
            checksum[3] = (byte)((complement >> 8) & 0xFF);

            CheckSum = BitConverter.ToUInt32(checksum, 0);
        }

        public bool IsCheckSumValid()
        {
            byte[] storedChecksum = BitConverter.GetBytes(CheckSum);
            byte[] checksum = { 0x00, 0x00, 0xFF, 0xFF };

            byte[] bytes = this.ToByteArray(true);
            int sum = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                sum += bytes[i];
            }

            checksum[0] = (byte)(sum & 0xFF);
            checksum[1] = (byte)((sum >> 8) & 0xFF);

            int complement = (sum & 0xFFFF) ^ 0xFFFF;
            checksum[2] = (byte)(complement & 0xFF);
            checksum[3] = (byte)((complement >> 8) & 0xFF);

            return (storedChecksum[0] == checksum[0] 
                && storedChecksum[1] == checksum[1] 
                && storedChecksum[2] == checksum[2] 
                && storedChecksum[3] == checksum[3]);
        }

        protected void RebuildPalette()
        {
            int numberOfPalettes = PaletteData.Length / 2;
            Palette = new Color[numberOfPalettes];

            for(int i=0; i<numberOfPalettes; i++)
            {
                Palette[i] = Utilities.GetColorFromBytes(PaletteData[i * 2], PaletteData[i * 2 + 1]);
            }
        }

        protected void BuildTileArray()
        {
            if(PaletteData == null)
            {
                return;
            }

            if(PaletteData.Length < 32)
            {
                // clearly this is not a 4bpp file
                Tiles = new Tile8x8[0];
                return;
            }

            List<Tile8x8> tiles = new List<Tile8x8>();

            for(int i=0; i<PixelData.Length; i+=32)
            {
                var tileBytes = new byte[32];
                Array.Copy(PixelData, i, tileBytes, 0, 32);

                var t = new Tile8x8(tileBytes);
                tiles.Add(t);
            }

            Tiles = tiles.ToArray();
        }
    }
}
