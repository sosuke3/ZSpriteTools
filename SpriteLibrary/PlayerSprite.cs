using NLog;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpriteLibrary
{
    public class PlayerSprite : Sprite
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public Palette GreenMailPalette { get; set; } = new SpriteLibrary.Palette();
        public Palette BlueMailPalette { get; set; } = new SpriteLibrary.Palette();
        public Palette RedMailPalette { get; set; } = new SpriteLibrary.Palette();
        public Palette BunnyPalette { get; set; } = new SpriteLibrary.Palette();
        public Palette GlovePalette { get; set; } = new SpriteLibrary.Palette(2);

        //public Color[] GlovePalette
        //{
        //    get
        //    {
        //        if (IsValidPlayerSprite == false)
        //        {
        //            throw new Exception("Invalid sprite file.");
        //        }

        //        if(Palette.Length == 60)
        //        {
        //            // F652 7603
        //            return new Color[] { Utilities.GetColorFromBytes(0xF6, 0x52), Utilities.GetColorFromBytes(0x76, 0x03) };
        //        }

        //        return Palette.Skip(60).Take(2).ToArray();
        //    }
        //}

        public bool IsValidPlayerSprite
        {
            get
            {
                return Palette.Length >= 60 && PixelDataLength == 0x7000;
            }
        }

        public PlayerSprite()
        {
            SetPlayerPaletteColors();

            SetupPropertyChangedHandlers();

            SpriteType = 1;
        }

        public PlayerSprite(byte[] rawData) : base(rawData)
        {
            SetPlayerPaletteColors();

            SetupPropertyChangedHandlers();

            SpriteType = 1;
        }

        void SetPlayerPaletteColors()
        {
            CopyMergedPaletteToMailPalette(GreenMailPalette, 0);
            CopyMergedPaletteToMailPalette(BlueMailPalette, 15);
            CopyMergedPaletteToMailPalette(RedMailPalette, 30);
            CopyMergedPaletteToMailPalette(BunnyPalette, 45);

            if (Palette.Length == 60)
            {
                // Vanilla Glove Colors = F652 7603
                GlovePalette[0] = Utilities.GetColorFromBytes(0xF6, 0x52);
                GlovePalette[1] = Utilities.GetColorFromBytes(0x76, 0x03);

                var newPal = new Color[62];
                Array.Copy(this.Palette, newPal, this.Palette.Length);
                this.Palette = newPal;
                this.Palette[60] = GlovePalette[0];
                this.Palette[61] = GlovePalette[1];

                this.RebuildPaletteData();
            }
            else if(Palette.Length == 62)
            {
                GlovePalette[0] = this.Palette[60];
                GlovePalette[1] = this.Palette[61];
            }
        }

        void CopyMergedPaletteToMailPalette(Palette pal, int offset)
        {
            for (int i = 1; i < pal.Length; i++)
            {
                pal[i] = this.Palette[offset + i - 1];
            }
        }

        void CopyMailPaletteToMergedPalette(Palette pal, int offset)
        {
            if(this.Palette.Length < offset + pal.Length)
            {
                var thisPalette = this.Palette;
                Array.Resize<Color>(ref thisPalette, offset + pal.Length);
                this.Palette = thisPalette;
            }

            for (int i = 1; i < pal.Length; i++)
            {
                this.Palette[offset + i - 1] = pal[i];
            }
        }

        void CopyGlovePaletteToMergedPalette(Palette pal, int offset)
        {
            if (this.Palette.Length < offset + pal.Length)
            {
                var thisPalette = this.Palette;
                Array.Resize<Color>(ref thisPalette, offset + pal.Length);
                this.Palette = thisPalette;
            }

            for (int i = 0; i < pal.Length; i++)
            {
                this.Palette[offset + i] = pal[i];
            }
        }

        void SetupPropertyChangedHandlers()
        {
            GreenMailPalette.PropertyChanged += GreenMailPalette_PropertyChanged;
            BlueMailPalette.PropertyChanged += BlueMailPalette_PropertyChanged;
            RedMailPalette.PropertyChanged += RedMailPalette_PropertyChanged;
            BunnyPalette.PropertyChanged += BunnyPalette_PropertyChanged;
            GlovePalette.PropertyChanged += GlovePalette_PropertyChanged;
        }

        void GreenMailPalette_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            CopyMailPaletteToMergedPalette(GreenMailPalette, 0);
        }

        void BlueMailPalette_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            CopyMailPaletteToMergedPalette(BlueMailPalette, 15);
        }

        void RedMailPalette_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            CopyMailPaletteToMergedPalette(RedMailPalette, 30);
        }

        void BunnyPalette_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            CopyMailPaletteToMergedPalette(BunnyPalette, 45);
        }

        void GlovePalette_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            CopyGlovePaletteToMergedPalette(GlovePalette, 60);
        }

        int currentFrame = 0;
        AnimationType currentAnimation = null;
        int currentPalette = 0;
        Point origin = new Point(xoffset, yoffset);
        const int xoffset = 4;
        const int yoffset = 4;

        public void SetAnimation(AnimationType pose)
        {
            currentAnimation = pose;
            currentFrame = 0;

            if (pose == null)
            {
                origin = new Point(xoffset, yoffset);
            }
            else
            {
                int x = 0;
                int y = 0;
                if (pose.Steps != null)
                {
                    foreach (var step in pose.Steps)
                    {
                        if (step.Sprites != null)
                        {
                            foreach (var sprite in step.Sprites)
                            {
                                x = Math.Min(x, sprite.Position.X);
                                y = Math.Min(y, sprite.Position.Y);
                            }
                        }
                    }
                }

                origin = new Point(Math.Abs(x) + xoffset, Math.Abs(y) + yoffset);
            }
        }

        private static Color[] ZAPPALETTE = {
            //Color.FromArgb(0, 0, 0),
            Color.FromArgb(0, 0, 0),
            Color.FromArgb(208, 184, 24),
            Color.FromArgb(136, 112, 248),
            Color.FromArgb(0, 0, 0),
            Color.FromArgb(208, 192, 248),
            Color.FromArgb(0, 0, 0),
            Color.FromArgb(208, 192, 248),
            Color.FromArgb(112, 88, 224),
            Color.FromArgb(136, 112, 248),
            Color.FromArgb(56, 40, 128),
            Color.FromArgb(136, 112, 248),
            Color.FromArgb(56, 40, 128),
            Color.FromArgb(72, 56, 144),
            Color.FromArgb(120, 48, 160),
            Color.FromArgb(248, 248, 248),
        };

        public void DrawAnimation(Graphics g)
        {
            if (currentAnimation == null)
            {
                return;
            }

            try
            {
                Step step = null;
                int totalLength = 0;
                currentFrame += 1;

                foreach (var s in currentAnimation.Steps)
                {
                    if (totalLength + s.Length > currentFrame)
                    {
                        step = s;
                        break;
                    }

                    totalLength += s.Length;
                }
                if (step == null)
                {
                    currentFrame = 0;
                    step = currentAnimation.Steps.FirstOrDefault();
                }

                foreach (var s in step.Sprites)
                {
                    DrawTile(g, s.Row, s.Col, s.Position, origin, s.Size, s.Flip);
                }
            }
            catch
            {

            }
        }

        public void DrawTile(Graphics g, string Row, int Col, Point pos, Point origin, TileDrawType drawType = TileDrawType.FULL, TileFlipType flipType = TileFlipType.NO_FLIP)
        {
            if (String.IsNullOrEmpty(Row))
            {
                throw new ArgumentException("Row is empty", nameof(Row));
            }
            var isValidRowType = Enum.GetNames(typeof(RowType)).Contains(Row);
            if (!isValidRowType)
            {
                throw new ArgumentException("Row contains an invalid value", nameof(Row));
            }

            var rowType = Enum.Parse(typeof(RowType), Row);
            if ((int)rowType > (int)RowType.AB)
            {
                // shield, etc
                // TODO: implement these later
                return;
            }

            int rowValue;

            if (Row == "AA" || Row == "AB")
            {
                rowValue = 'Z' - 'A';
                if (Row == "AA")
                {
                    rowValue += 1;
                }
                else
                {
                    rowValue += 2;
                }
            }
            else
            {
                rowValue = Row[0] - 'A';
            }

            int y = rowValue * 2;
            int x = Col * 2;

            List<int> tileIndices = new List<int>();

            int width = 2;
            int height = 2;
            switch (drawType)
            {
                case TileDrawType.FULL:
                    tileIndices.Add(x + y * 16);
                    tileIndices.Add(x + 1 + y * 16);
                    tileIndices.Add(x + (y + 1) * 16);
                    tileIndices.Add(x + 1 + (y + 1) * 16);
                    width = 2;
                    height = 2;
                    break;
                case TileDrawType.TOP_HALF:
                    tileIndices.Add(x + y * 16);
                    tileIndices.Add(x + 1 + y * 16);
                    width = 2;
                    height = 1;
                    break;
                case TileDrawType.BOTTOM_HALF:
                    tileIndices.Add(x + (y + 1) * 16);
                    tileIndices.Add(x + 1 + (y + 1) * 16);
                    width = 2;
                    height = 1;
                    break;
                case TileDrawType.RIGHT_HALF:
                    tileIndices.Add(x + 1 + y * 16);
                    tileIndices.Add(x + 1 + (y + 1) * 16);
                    width = 1;
                    height = 2;
                    break;
                case TileDrawType.LEFT_HALF:
                    tileIndices.Add(x + y * 16);
                    tileIndices.Add(x + (y + 1) * 16);
                    width = 1;
                    height = 2;
                    break;
                case TileDrawType.TOP_RIGHT:
                    tileIndices.Add(x + 1 + y * 16);
                    width = 1;
                    height = 1;
                    break;
                case TileDrawType.TOP_LEFT:
                    tileIndices.Add(x + y * 16);
                    width = 1;
                    height = 1;
                    break;
                case TileDrawType.BOTTOM_RIGHT:
                    tileIndices.Add(x + 1 + (y + 1) * 16);
                    width = 1;
                    height = 1;
                    break;
                case TileDrawType.BOTTOM_LEFT:
                    tileIndices.Add(x + (y + 1) * 16);
                    width = 1;
                    height = 1;
                    break;
                case TileDrawType.TALL_8X24:
                    tileIndices.Add(x + y * 16);
                    tileIndices.Add(x + (y + 1) * 16);
                    tileIndices.Add(x + (y + 2) * 16);
                    width = 1;
                    height = 3;
                    break;
                case TileDrawType.WIDE_24X8:
                    tileIndices.Add(x + y * 16);
                    tileIndices.Add(x + 1 + y * 16);
                    tileIndices.Add(x + 2 + y * 16);
                    width = 3;
                    height = 1;
                    break;
                case TileDrawType.LARGE_16X24:
                    tileIndices.Add(x + y * 16);
                    tileIndices.Add(x + 1 + y * 16);
                    tileIndices.Add(x + (y + 1) * 16);
                    tileIndices.Add(x + 1 + (y + 1) * 16);
                    tileIndices.Add(x + (y + 2) * 16);
                    tileIndices.Add(x + 1 + (y + 2) * 16);
                    width = 2;
                    height = 3;
                    break;
                case TileDrawType.LARGE_32X24:
                    tileIndices.Add(x + y * 16);
                    tileIndices.Add(x + 1 + y * 16);
                    tileIndices.Add(x + 2 + y * 16);
                    tileIndices.Add(x + 3 + y * 16);
                    tileIndices.Add(x + (y + 1) * 16);
                    tileIndices.Add(x + 1 + (y + 1) * 16);
                    tileIndices.Add(x + 2 + (y + 1) * 16);
                    tileIndices.Add(x + 3 + (y + 1) * 16);
                    tileIndices.Add(x + (y + 2) * 16);
                    tileIndices.Add(x + 1 + (y + 2) * 16);
                    tileIndices.Add(x + 2 + (y + 2) * 16);
                    tileIndices.Add(x + 3 + (y + 2) * 16);
                    width = 4;
                    height = 3;
                    break;
                case TileDrawType.EMPTY:
                default:
                    return;
            }

            var pal = new Color[15];
            switch(currentPalette)
            {
                case 0:
                    Array.Copy(this.GreenMailPalette.PaletteColors, 1, pal, 0, 15);
                    break;
                case 1:
                    Array.Copy(this.BlueMailPalette.PaletteColors, 1, pal, 0, 15);
                    break;
                case 2:
                    Array.Copy(this.RedMailPalette.PaletteColors, 1, pal, 0, 15);
                    break;
                case 3:
                    Array.Copy(this.BunnyPalette.PaletteColors, 1, pal, 0, 15);
                    break;
                default:
                    Array.Copy(this.Palette, pal, 15);
                    break;
            }

            if(currentAnimation.Name == "Zap")
            {
                pal = ZAPPALETTE;
            }

            //DrawTiles(g, pos, origin, width, height, pal, tileIndices.ToArray());

            Bitmap tempImage = new Bitmap(width * 8, height * 8, PixelFormat.Format32bppArgb);
            using (Graphics tempG = Graphics.FromImage(tempImage))
            {
                tempG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                tempG.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                DrawTiles(tempG, new Point(0, 0), new Point(0, 0), width, height, pal, tileIndices.ToArray());
            }

            var shiftedPosition = new Point(pos.X + origin.X, pos.Y + origin.Y);
            switch(flipType)
            {
                case TileFlipType.NO_FLIP:
                    //tempImage.RotateFlip(RotateFlipType.RotateNoneFlipNone);
                    break;
                case TileFlipType.X_FLIP: // flip about X access not on X access
                    tempImage.RotateFlip(RotateFlipType.RotateNoneFlipY);
                    break;
                case TileFlipType.Y_FLIP: // flip about Y access not on Y access
                    tempImage.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    break;
                case TileFlipType.XY_FLIP:
                    tempImage.RotateFlip(RotateFlipType.RotateNoneFlipXY);
                    break;
            }
            g.DrawImage(tempImage, shiftedPosition);
        }

        public void SetAnimationPalette(int selectedIndex)
        {
            this.currentPalette = selectedIndex;
        }
    }
}
