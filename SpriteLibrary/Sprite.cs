using NLog;
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
    public class Sprite
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

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
        type (2 bytes)
        reserved (6 bytes)
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

        public ushort SpriteType { get; protected set; }
        protected const int spriteTypeOffset = paletteDataLengthOffset + paletteDataLengthLength;
        protected const int spriteTypeLength = 2;

        public byte[] Reserved { get; protected set; } = new byte[reservedLength];
        protected const int reservedOffset = spriteTypeOffset + spriteTypeLength;
        protected const int reservedLength = 6;

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
        public void Set4bppPixelData(byte[] pixels)
        {
            this.pixelData = pixels;

            RecalculatePixelAndPaletteOffset();
            BuildTileArray();
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
        public void SetPalette(Color[] palette)
        {
            this.Palette = palette;

            RebuildPaletteData();
        }

        public Sprite()
        {
            Version = 1;
            CheckSum = 0xFFFF0000;
            PixelDataLength = 0x7000;
            PaletteDataLength = 0x78;
            SpriteType = 0;
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
                SpriteType = 1;
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

            if (PaletteDataLength % 2 != 0)
            {
                throw new Exception("Invalid sprite file. Palette size must be even.");
            }

            SpriteType = bytesToUShort(rawData, spriteTypeOffset);

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

        public void DrawTiles(Graphics g, Point pos, Point origin, int width, int height, Color[] pal, params int [] tileIndexes)
        {
            int x = 0;
            int y = 0;
            foreach(var i in tileIndexes)
            {
                this.Tiles[i].Draw(g, pal, origin.X + pos.X + x * 8, origin.Y + pos.Y + y * 8);
                x++;
                if(x > width - 1)
                {
                    x = 0;
                    y++;
                }
            }
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
            this.RebuildPaletteData();

            if (false == skipChecksum)
            {
                UpdateChecksum();
            }

            Version = currentVersion; // update the version

            byte[] ret = new byte[headerLength + versionLength + checksumLength + pixelDataOffsetLength + pixelDataLengthLength + paletteDataOffsetLength + paletteDataLengthLength + spriteTypeLength + reservedLength + displayBytesLength + authorBytesLength + authorRomDisplayBytesLength + PixelDataLength + PaletteDataLength];

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

            byte[] spriteType = BitConverter.GetBytes(SpriteType);
            ret[i++] = spriteType[0];
            ret[i++] = spriteType[1];

            ret[i++] = Reserved[0];
            ret[i++] = Reserved[1];
            ret[i++] = Reserved[2];
            ret[i++] = Reserved[3];
            ret[i++] = Reserved[4];
            ret[i++] = Reserved[5];

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

        protected void RebuildPaletteData()
        {
            if(this.Palette.Length != 60)
            {
                paletteData = new byte[this.Palette.Length * 2];
            }
            else
            {
                paletteData = new byte[this.Palette.Length * 2 + 4];
                // F652 7603
                paletteData[120] = 0xF6;
                paletteData[121] = 0x52;
                paletteData[122] = 0x76;
                paletteData[123] = 0x03;
            }
            this.PaletteDataLength = (ushort)paletteData.Length;

            for(int i=0; i<this.Palette.Length; i++)
            {
                var bytes = Utilities.GetBytesFromColor(this.Palette[i]);
                paletteData[i * 2] = bytes[0];
                paletteData[i * 2 + 1] = bytes[1];
            }

            if (this.Palette.Length == 62 && this.Palette[60] == Color.Black)
            {
                paletteData[120] = 0xF6;
                paletteData[121] = 0x52;
                paletteData[122] = 0x76;
                paletteData[123] = 0x03;
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

        public void DrawPNGto4BPPPalette(Graphics g, int posX, int posY)
        {
            Bitmap bitmap = new Bitmap(8, 8, PixelFormat.Format32bppArgb);

            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;

            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);

            var totalBytes = bitmapData.Stride * bitmapData.Height;
            var bpp = Image.GetPixelFormatSize(bitmap.PixelFormat) / 8;
            var pixels = new byte[totalBytes];

            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    int pixelPosition = y * bitmapData.Stride + (x * 4);

                    Color drawColor = Color.FromArgb(0, 0, 0);
                    if (x == 0 && (y == 0 || y == 6))
                    {
                    }
                    else if (x == 0 && y == 2)
                    {
                        if (this.Palette.Length == 60)
                        {
                            drawColor = Utilities.GetColorFromBytes(0xF6, 0x52);

                        }
                        else if (this.Palette.Length == 62)
                        {
                            drawColor = this.Palette[60];
                        }
                    }
                    else if (x == 0 && y == 4)
                    {
                        if (this.Palette.Length == 60)
                        {
                            drawColor = Utilities.GetColorFromBytes(0x76, 0x03);

                        }
                        else if (this.Palette.Length == 62)
                        {
                            drawColor = this.Palette[61];
                        }
                    }
                    else if (y >= 0 && y < 2)
                    {
                        drawColor = this.Palette[x + y * 8 - 1];
                    }
                    else if (y >= 2 && y < 4)
                    {
                        drawColor = this.Palette[x + y * 8 - 2];
                    }
                    else if (y >= 4 && y < 6)
                    {
                        drawColor = this.Palette[x + y * 8 - 3];
                    }
                    else if (y >= 6 && y < 8)
                    {
                        drawColor = this.Palette[x + y * 8 - 4];
                    }

                    DrawPixel(pixels, pixelPosition, drawColor);
                }
            }

            IntPtr ptrFirstPixel = bitmapData.Scan0;
            Marshal.Copy(pixels, 0, ptrFirstPixel, pixels.Length);
            bitmap.UnlockBits(bitmapData);

            g.DrawImage(bitmap, new Rectangle(posX, posY, 8, 8), 0, 0, 8, 8, GraphicsUnit.Pixel);
        }

        void DrawPixel(byte[] pixelArray, int position, Color color)
        {
            pixelArray[position + 0] = color.B;
            pixelArray[position + 1] = color.G;
            pixelArray[position + 2] = color.R;
            pixelArray[position + 3] = color.A;
        }
    }

    public enum TileDrawType
    {
        FULL,
        TOP_HALF,
        BOTTOM_HALF,
        RIGHT_HALF,
        LEFT_HALF,
        TOP_RIGHT,
        TOP_LEFT,
        BOTTOM_RIGHT,
        BOTTOM_LEFT,
        TALL_8X24,
        WIDE_24X8,
        LARGE_16X24,
        LARGE_32X24,
        EMPTY
    }

    public enum TileFlipType
    {
        NO_FLIP,
        X_FLIP,
        Y_FLIP,
        XY_FLIP,
    }

    public enum RowType
    {
        A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z, AA, AB, 
        SWORD, FSWORD, MSWORD, TSWORD, BSWORD,
        SHIELD, FSHIELD, RSHIELD, MSHIELD,
        SHADOW,
        ITEMSHADOW,
        BOOK,
        PENDANT,
        CRYSTAL,
        BUSH,
        CANE,
        ROD,
        HAMMER,
        HOOKSHOT,
        BOOMERANG,
        NET,
        BOW,
        SHOVEL,
        DUCK,
        BED,
        GRASS
    }
}
