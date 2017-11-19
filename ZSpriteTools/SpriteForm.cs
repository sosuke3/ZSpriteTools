using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZSpriteTools
{
    public partial class SpriteForm : Form
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public SpriteLibrary.Sprite loadedSprite;
        public string Filename;

        public SpriteForm()
        {
            InitializeComponent();

            loadedSprite = new SpriteLibrary.Sprite();
            this.Text = loadedSprite.DisplayText;
            this.Icon = ZSpriteTools.Properties.Resources.main;

            UpdateForm();
        }

        public SpriteForm(string Filename, SpriteLibrary.Sprite sprite)
        {
            InitializeComponent();

            loadedSprite = sprite;
            this.Icon = ZSpriteTools.Properties.Resources.main;
            this.Filename = Filename;

            this.Text = Filename;

            UpdateForm();
        }

        public void UpdateForm()
        {
            BuildPalette();
            BuildSprite();
        }

        private void BuildPalette()
        {
            palettePictureBox.BackColor = Color.Black;
            palettePictureBox.SizeMode = PictureBoxSizeMode.StretchImage;

            int rows = loadedSprite.Palette.Length / 32;
            if (loadedSprite.Palette.Length % 32 != 0)
            {
                rows++; // partial row
            }
            if (rows < 4)
            {
                rows = 4;
            }
            palettePictureBox.Image = new Bitmap(512, 16 * rows);

            Graphics g = Graphics.FromImage(palettePictureBox.Image);

            int x = 0;
            int y = 0;

            for (int i = 0; i < loadedSprite.Palette.Length; i++)
            {
                x = i * 16 % 512;
                y = i * 16 / 512 * 16;

                g.FillRectangle(new SolidBrush(loadedSprite.Palette[i]), x, y, 16, 16);
            }

        }

        private void BuildSprite()
        {
            int rows = loadedSprite.Tiles.Length / 16;
            if (loadedSprite.Tiles.Length % 16 != 0)
            {
                rows++;
            }

            var tempBitmap = new Bitmap(128, rows * 8);
            var graphics = Graphics.FromImage(tempBitmap);
            for (int i = 0; i < loadedSprite.Tiles.Length; i++)
            {
                var x = i * 8 % 128;
                var y = i * 8 / 128 * 8;

                loadedSprite.Tiles[i].Draw(graphics, loadedSprite.Palette, x, y);
            }

            spritePictureBox.BackColor = Color.LightGray;
            spritePictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            spritePictureBox.Image = ResizeBitmap(tempBitmap, 512, 1792);
        }

        private Bitmap ResizeBitmap(Bitmap input, int width, int height)
        {
            var ret = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(ret))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.DrawImage(input, 0, 0, width, height);
            }
            return ret;
        }

        public void ImportRawPixels(byte[] pixels)
        {
            loadedSprite.PixelData = pixels;
            UpdateForm();
        }

        public void ImportRawPalette(byte[] palette)
        {
            loadedSprite.PaletteData = palette;
            UpdateForm();
        }

        public void UpdateFilename(string filename)
        {
            this.Filename = filename;
            this.Text = filename;
        }
    }
}
