using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SpriteLibrary
{
    public class Palette : INotifyPropertyChanged
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static readonly string[] PaletteColorNames =
        {
			"Transparent",
			"White",
			"Belt / Yellow",
			"Skin Shade",
			"Skin",
			"Outline",
			"Hat Trim / Orange",
			"Mouth / Red",
			"Hair",
			"Tunic Shade",
			"Tunic",
			"Hat Shade",
			"Hat",
			"Hands",
			"Sleeves",
			"Water"
        };

        public Palette(int size = 16)
        {
            this.palette = new Color[size];

            for(int i=0; i<this.palette.Length; i++)
            {
                this.palette[i] = Color.FromArgb(i * 15, i * 15, i * 15);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        Color[] palette { get; set; }
        byte[] rawPalette { get; set; }

        public Color this[int i]
        {
            get
            {
                if (this.palette.Length < i)
                {
                    throw new IndexOutOfRangeException("Invalid palette index");
                }

                return this.palette[i];
            }
            set
            {
                if(this.palette.Length < i)
                {
                    throw new IndexOutOfRangeException("Invalid palette index");
                }

                if (value != this.palette[i])
                {
                    this.palette[i] = value;

                    this.UpdateRawFromPalette();

                    NotifyPropertyChanged();
                }
            }
        }

        public int Length
        {
            get { return this.palette.Length; }
        }

        public byte[] GetRawPalette()
        {
            return this.rawPalette;
        }

        public void SetRawPalette(byte[] rawpalette)
        {
            this.rawPalette = new byte[rawpalette.Length];
            Array.Copy(rawpalette, this.rawPalette, rawpalette.Length);

            UpdatePaletteFromRaw();
        }

        void UpdatePaletteFromRaw()
        {
            int startIndex = 0;
            int length = rawPalette.Length == 30 ? 16 : rawPalette.Length / 2;
            this.palette = new Color[length];

            if(rawPalette.Length == 30)
            {
                this.palette[0] = Color.FromArgb(0, 0, 0);
                startIndex = 1;
            }

            for (int i = startIndex; i < this.palette.Length; i++)
            {
                this.palette[i] = Utilities.GetColorFromBytes(this.rawPalette[(i - startIndex) * 2], this.rawPalette[(i - startIndex) * 2 + 1]);
            }
        }

        void UpdateRawFromPalette()
        {
            this.rawPalette = new byte[this.palette.Length * 2];

            for(int i = 0; i < this.palette.Length; i++)
            {
                var rawBytes = Utilities.GetBytesFromColor(this.palette[i]);

                this.rawPalette[i * 2] = rawBytes[0];
                this.rawPalette[i * 2 + 1] = rawBytes[1];
            }
        }
    }
}
