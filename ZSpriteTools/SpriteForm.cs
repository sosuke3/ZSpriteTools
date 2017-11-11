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
        SpriteLibrary.Sprite loadedSprite;

        public SpriteForm()
        {
            loadedSprite = new SpriteLibrary.Sprite();

            InitializeComponent();

            UpdateForm();
        }

        public SpriteForm(SpriteLibrary.Sprite sprite)
        {
            loadedSprite = sprite;

            InitializeComponent();

            UpdateForm();
        }

        private void UpdateForm()
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

            spritePictureBox.Image = new Bitmap(128, rows * 8);
            spritePictureBox.BackColor = Color.LightGray;
            spritePictureBox.SizeMode = PictureBoxSizeMode.StretchImage;

            for (int i = 0; i < loadedSprite.Tiles.Length; i++)
            {
                var x = i * 8 % 128;
                var y = i * 8 / 128 * 8;

                loadedSprite.Tiles[i].Draw(Graphics.FromImage(spritePictureBox.Image), loadedSprite.Palette, x, y);
            }
        }
    }
}
