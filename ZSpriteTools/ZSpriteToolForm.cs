using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZSpriteTools
{
    public partial class ZSpriteToolForm : Form
    {
        SpriteLibrary.Sprite loadedSprite = new SpriteLibrary.Sprite();

        public ZSpriteToolForm()
        {
            InitializeComponent();

            this.Text = this.Text + " - " + ProductVersion;

            authorRomDisplayTextBox.MaxLength = SpriteLibrary.Sprite.AuthorRomDisplayMaxLength;

            //imagePanel.VerticalScroll.Maximum = 576;
            //imagePanelScrollbar.Maximum = 576;
        }

        private void loadSpriteButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Sprite File (*.spr;*.zspr)|*.spr;*.zspr|ZSprite File (*.zspr)|*.zspr|Legacy Sprite File (*.spr)|*.spr|All Files (*.*)|*.*";
            ofd.Title = "Select a Sprite File";

            var result = ofd.ShowDialog();
            if(result == DialogResult.OK)
            {
                var spriteFile = File.ReadAllBytes(ofd.FileName);
                loadedSprite = new SpriteLibrary.Sprite(spriteFile);
                UpdateForm();
            }
        }

        private void saveSpriteButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "ZSprite File (*.zspr)|*.zspr|All Files (*.*)|*.*";
            sfd.Title = "Select a Sprite File";

            var result = sfd.ShowDialog();
            if(result == DialogResult.OK)
            {
                var spriteData = loadedSprite.ToByteArray();
                File.WriteAllBytes(sfd.FileName, spriteData);
            }
        }

        private void importRawPixelDataButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Raw Pixel File (*.4bpp)|*.4bpp|All Files (*.*)|*.*";
            ofd.Title = "Select a Raw Pixel File";

            var result = ofd.ShowDialog();
            if (result == DialogResult.OK)
            {
                var rawFile = File.ReadAllBytes(ofd.FileName);
                loadedSprite.PixelData = rawFile;
                UpdateForm();
            }
        }

        private void exportRawPixelDataButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Raw Pixel File (*.4bpp)|*.4bpp|All Files (*.*)|*.*";
            sfd.Title = "Select a Raw Pixel File";

            var result = sfd.ShowDialog();
            if (result == DialogResult.OK)
            {
                File.WriteAllBytes(sfd.FileName, loadedSprite.PixelData);
            }
        }

        private void importRawPaletteDataButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Raw Palette File (*.rawpalette)|*.rawpalette|All Files (*.*)|*.*";
            ofd.Title = "Select a Raw Palette File";

            var result = ofd.ShowDialog();
            if (result == DialogResult.OK)
            {
                var rawFile = File.ReadAllBytes(ofd.FileName);
                loadedSprite.PaletteData = rawFile;
                UpdateForm();
            }
        }

        private void exportRawPaletteDataButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Raw Palette File (*.rawpalette)|*.rawpalette|All Files (*.*)|*.*";
            sfd.Title = "Select a Raw Palette File";

            var result = sfd.ShowDialog();
            if (result == DialogResult.OK)
            {
                File.WriteAllBytes(sfd.FileName, loadedSprite.PaletteData);
            }
        }

        private void displayTextTextBox_TextChanged(object sender, EventArgs e)
        {
            loadedSprite.DisplayText = displayTextTextBox.Text;
        }

        private void authorTextBox_TextChanged(object sender, EventArgs e)
        {
            loadedSprite.Author = authorTextBox.Text;
        }

        private void authorRomDisplayTextBox_TextChanged(object sender, EventArgs e)
        {
            loadedSprite.AuthorRomDisplay = authorRomDisplayTextBox.Text;
        }

        private void UpdateForm()
        {
            displayTextTextBox.Text = loadedSprite.DisplayText;
            authorTextBox.Text = loadedSprite.Author;
            authorRomDisplayTextBox.Text = loadedSprite.AuthorRomDisplay;

            BuildPalette();
            BuildSprite();
        }

        private void BuildPalette()
        {
            palettePictureBox.BackColor = Color.Black;
            palettePictureBox.SizeMode = PictureBoxSizeMode.StretchImage;

            int rows = loadedSprite.Palette.Length / 32;
            if(loadedSprite.Palette.Length % 32 != 0)
            {
                rows++; // partial row
            }
            if(rows < 4)
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
            if(loadedSprite.Tiles.Length % 16 != 0)
            {
                rows++;
            }

            spritePictureBox.Image = new Bitmap(128, rows * 8);
            spritePictureBox.BackColor = Color.LightGray;
            spritePictureBox.SizeMode = PictureBoxSizeMode.StretchImage;

            for (int i=0; i<loadedSprite.Tiles.Length; i++)
            {
                var x = i * 8 % 128;
                var y = i * 8 / 128 * 8;

                loadedSprite.Tiles[i].Draw(Graphics.FromImage(spritePictureBox.Image), loadedSprite.Palette, x, y);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
