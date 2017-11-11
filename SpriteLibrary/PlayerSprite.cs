using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpriteLibrary
{
    public class PlayerSprite : Sprite
    {
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
        }

        public PlayerSprite(byte[] rawData) : base(rawData)
        {
            SetPlayerPaletteColors();

            SetupPropertyChangedHandlers();
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
            }
            else
            {
                GlovePalette[0] = this.Palette[61];
                GlovePalette[1] = this.Palette[62];
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
            for (int i = 1; i < pal.Length; i++)
            {
                this.Palette[offset + i - 1] = pal[i];
            }
        }

        void SetupPropertyChangedHandlers()
        {
            GreenMailPalette.PropertyChanged += GreenMailPalette_PropertyChanged;
            BlueMailPalette.PropertyChanged += BlueMailPalette_PropertyChanged;
            RedMailPalette.PropertyChanged += RedMailPalette_PropertyChanged;
            BunnyPalette.PropertyChanged += BunnyPalette_PropertyChanged;
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
    }
}
