using Microsoft.Win32;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZSpriteTools
{
    public partial class ZSpriteToolForm : Form
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        const string OopsMessage = "Something went wrong. Please send your log file to Sosuke3.";

        public ZSpriteToolForm()
        {
            this.Icon = ZSpriteTools.Properties.Resources.main;

            InitializeComponent();

            this.Text = this.Text + " - " + ProductVersion;

            authorRomDisplayTextBox.MaxLength = SpriteLibrary.Sprite.AuthorRomDisplayMaxLength;
        }

        private void displayTextTextBox_TextChanged(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null)
            { 
                SpriteForm activeChild = (SpriteForm)this.ActiveMdiChild;

                activeChild.loadedSprite.DisplayText = displayTextTextBox.Text;
            }
        }

        private void authorTextBox_TextChanged(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null)
            {
                SpriteForm activeChild = (SpriteForm)this.ActiveMdiChild;

                activeChild.loadedSprite.Author = authorTextBox.Text;
            }
        }

        private void authorRomDisplayTextBox_TextChanged(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null)
            {
                SpriteForm activeChild = (SpriteForm)this.ActiveMdiChild;

                activeChild.loadedSprite.AuthorRomDisplay = authorRomDisplayTextBox.Text;
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SpriteForm newMDI = new SpriteForm();
            newMDI.MdiParent = this;
            newMDI.Show();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Sprite File (*.spr;*.zspr)|*.spr;*.zspr|ZSprite File (*.zspr)|*.zspr|Legacy Sprite File (*.spr)|*.spr|All Files (*.*)|*.*";
            ofd.Title = "Select a Sprite File";

            var result = ofd.ShowDialog();
            if (result == DialogResult.OK)
            {
                LoadFile(ofd.FileName);
            }
        }

        public void LoadFile(string fileName)
        {
            var spriteFile = File.ReadAllBytes(fileName);
            var loadedSprite = new SpriteLibrary.Sprite(spriteFile);
            if (loadedSprite.Version == 0)
            {
                loadedSprite.DisplayText = Path.GetFileNameWithoutExtension(fileName);
            }

            SpriteForm newMDI = new SpriteForm(fileName, loadedSprite);
            newMDI.MdiParent = this;
            newMDI.Show();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SpriteForm activeChild = (SpriteForm)this.ActiveMdiChild;
            if (activeChild != null)
            {
                try
                {
                    string filename = activeChild.Filename;

                    if(String.IsNullOrEmpty(filename) || Path.GetExtension(filename) != ".zspr")
                    {
                        // new file, or old format file need to show save box
                        SaveFileDialog sfd = new SaveFileDialog();
                        sfd.Filter = "ZSprite File (*.zspr)|*.zspr|All Files (*.*)|*.*";
                        sfd.Title = "Select a Sprite File";
                        sfd.FileName = String.IsNullOrEmpty(filename) 
                                            ? activeChild.loadedSprite.DisplayText 
                                            : Path.GetFileNameWithoutExtension(filename);

                        var result = sfd.ShowDialog();
                        if (result != DialogResult.OK)
                        {
                            return;
                        }

                        filename = sfd.FileName;
                    }

                    var spriteData = activeChild.loadedSprite.ToByteArray();
                    File.WriteAllBytes(filename, spriteData);
                    activeChild.UpdateFilename(filename);
                }
                catch(Exception ex)
                {
                    logger.Error(ex);
                    MessageBox.Show(OopsMessage);
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void importRawPixelDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SpriteForm activeChild = (SpriteForm)this.ActiveMdiChild;
            if(activeChild != null)
            {
                try
                {
                    OpenFileDialog ofd = new OpenFileDialog();
                    ofd.Filter = "Raw Pixel File (*.4bpp)|*.4bpp|All Files (*.*)|*.*";
                    ofd.Title = "Select a Raw Pixel File";

                    var result = ofd.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        var rawFile = File.ReadAllBytes(ofd.FileName);

                        activeChild.ImportRawPixels(rawFile);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    MessageBox.Show(OopsMessage);
                }
            }
        }

        private void importRawPaletteDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SpriteForm activeChild = (SpriteForm)this.ActiveMdiChild;
            if (activeChild != null)
            {
                try
                {
                    OpenFileDialog ofd = new OpenFileDialog();
                    ofd.Filter = "Raw Palette File (*.rawpalette)|*.rawpalette|All Files (*.*)|*.*";
                    ofd.Title = "Select a Raw Palette File";

                    var result = ofd.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        var rawFile = File.ReadAllBytes(ofd.FileName);

                        activeChild.ImportRawPalette(rawFile);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    MessageBox.Show(OopsMessage);
                }
            }
        }

        private void exportRawPixelDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SpriteForm activeChild = (SpriteForm)this.ActiveMdiChild;
            if (activeChild != null)
            {
                try
                {
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.Filter = "Raw Pixel File (*.4bpp)|*.4bpp|All Files (*.*)|*.*";
                    sfd.Title = "Select a Raw Pixel File";
                    sfd.FileName = Path.GetFileNameWithoutExtension(activeChild.Filename);

                    var result = sfd.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        File.WriteAllBytes(sfd.FileName, activeChild.loadedSprite.PixelData);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    MessageBox.Show(OopsMessage);
                }
            }
        }

        private void exportRawPaletteDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SpriteForm activeChild = (SpriteForm)this.ActiveMdiChild;
            if (activeChild != null)
            {
                try
                {
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.Filter = "Raw Palette File (*.rawpalette)|*.rawpalette|All Files (*.*)|*.*";
                    sfd.Title = "Select a Raw Palette File";
                    sfd.FileName = Path.GetFileNameWithoutExtension(activeChild.Filename);

                    var result = sfd.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        File.WriteAllBytes(sfd.FileName, activeChild.loadedSprite.PaletteData);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    MessageBox.Show(OopsMessage);
                }
            }
        }

        private void ZSpriteToolForm_MdiChildActivate(object sender, EventArgs e)
        {
            if(this.ActiveMdiChild == null)
            {
                DisableSave();
                DisableSaveAs();
                DisableEditExportImport();
                DisableExport();
            }
            else
            {
                SpriteForm activeChild = (SpriteForm)this.ActiveMdiChild;

                this.displayTextTextBox.Text = activeChild.loadedSprite.DisplayText;
                this.authorTextBox.Text = activeChild.loadedSprite.Author;
                this.authorRomDisplayTextBox.Text = activeChild.loadedSprite.AuthorRomDisplay;

                EnableSave();
                EnableSaveAs();
                EnableEditExportImport();
                EnableExport();
            }
        }

        void DisableSave()
        {
            this.saveToolStripButton.Enabled = false;
            this.saveToolStripMenuItem.Enabled = false;
        }

        void EnableSave()
        {
            this.saveToolStripButton.Enabled = true;
            this.saveToolStripMenuItem.Enabled = true;
        }

        void DisableSaveAs()
        {
            this.saveAsToolStripMenuItem.Enabled = false;
        }

        void EnableSaveAs()
        {
            this.saveAsToolStripMenuItem.Enabled = true;
        }

        void DisableExport()
        {
            this.exportToolStripMenuItem.Enabled = false;
        }

        void EnableExport()
        {
            this.exportToolStripMenuItem.Enabled = true;
        }

        void DisableEditExportImport()
        {
            this.importRawToolStripMenuItem.Enabled = false;
            this.exportRawToolStripMenuItem.Enabled = false;
            this.importRawPixelsToolStripButton.Enabled = false;
            this.importRawPaletteToolStripButton.Enabled = false;
            this.exportRawPixelsToolStripButton.Enabled = false;
            this.exportRawPaletteToolStripButton.Enabled = false;
        }

        void EnableEditExportImport()
        {
            this.importRawToolStripMenuItem.Enabled = true;
            this.exportRawToolStripMenuItem.Enabled = true;
            this.importRawPixelsToolStripButton.Enabled = true;
            this.importRawPaletteToolStripButton.Enabled = true;
            this.exportRawPixelsToolStripButton.Enabled = true;
            this.exportRawPaletteToolStripButton.Enabled = true;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show($"ZSpriteTools - {ProductVersion}", "Just for Mike");
        }

        private void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var preferences = new PreferencesForm();
            preferences.ShowDialog();

        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var commandLine = Environment.GetCommandLineArgs();

            if (commandLine.Length > 1)
            {
                var filename = commandLine[1];
                if (File.Exists(filename))
                {
                    LoadFile(filename);
                    return;
                }
            }

            DisableSave();
            DisableSaveAs();
            DisableEditExportImport();
            DisableExport();
        }

        void SetFileHandler()
        {
            // TODO: enable this later
            FileAssociations.EnsureAssociationsSet();
        }

        private void exportPNGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SpriteForm activeChild = (SpriteForm)this.ActiveMdiChild;
            if (activeChild != null)
            {
                try
                {
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.Filter = "PNG File (*.png)|*.png|All Files (*.*)|*.*";
                    sfd.Title = "Select a PNG File";
                    sfd.FileName = Path.GetFileNameWithoutExtension(activeChild.Filename);

                    var result = sfd.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        ExportPng(activeChild.loadedSprite, sfd.FileName);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    MessageBox.Show(OopsMessage);
                }
            }
        }

        void ExportPng(SpriteLibrary.Sprite sprite, string filename)
        {
            int rows = sprite.Tiles.Length / 16;
            if (sprite.Tiles.Length % 16 != 0)
            {
                rows++;
            }

            Bitmap tempBitmap = new Bitmap(128, 448);
            Graphics gg = Graphics.FromImage(tempBitmap);

            for (int i = 0; i < sprite.Tiles.Length; i++)
            {
                var x = i * 8 % 128;
                var y = i * 8 / 128 * 8;

                sprite.Tiles[i].Draw(gg, sprite.Palette, x, y);
            }

            sprite.DrawPNGto4BPPPalette(gg, 120, 440);

            tempBitmap.Save(filename, System.Drawing.Imaging.ImageFormat.Png);
        }

        private void importPNGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "PNG File (*.png)|*.png|All Files (*.*)|*.*";
            ofd.Title = "Select a PNG File";

            var invalidPixelsFound = false;

            var result = ofd.ShowDialog();
            if (result == DialogResult.OK)
            {
                Bitmap tempBitmap = (Bitmap)Image.FromFile(ofd.FileName);
                if(tempBitmap.Width != 128 || tempBitmap.Height != 448)
                {
                    MessageBox.Show("Invalid PNG image. Must be 128 x 448 pixels");
                    return;
                }
                var sprite = new SpriteLibrary.Sprite();
                sprite.DisplayText = Path.GetFileNameWithoutExtension(ofd.FileName);

                var bitmapData = tempBitmap.LockBits(new Rectangle(0, 0, tempBitmap.Width, tempBitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, tempBitmap.PixelFormat);
                var totalBytes = bitmapData.Stride * bitmapData.Height;
                var pixels = new byte[totalBytes];
                Marshal.Copy(bitmapData.Scan0, pixels, 0, totalBytes);
                tempBitmap.UnlockBits(bitmapData);

                // load palette from png
                int xoffset = 120;
                int yoffset = 440;

                var colors = new Color[8 * 8];

                for(int y = 0; y < 8; y++)
                {
                    for(int x = 0; x < 8; x++)
                    {
                        int pixelPosition = (y + yoffset) * bitmapData.Stride + ((x + xoffset) * 4);

                        int b = pixels[pixelPosition + 0];
                        int g = pixels[pixelPosition + 1];
                        int r = pixels[pixelPosition + 2];
                        int a = pixels[pixelPosition + 3];

                        colors[x + y * 8] = Color.FromArgb(a, r, g, b);
                    }
                }

                var palette = new Color[62];
                for(int i = 0; i < 15; i++)
                {
                    palette[i] = colors[i + 1];
                }
                for(int i = 15; i < 30; i++)
                {
                    palette[i] = colors[i + 2];
                }
                for (int i = 30; i < 45; i++)
                {
                    palette[i] = colors[i + 3];
                }
                for (int i = 45; i < 60; i++)
                {
                    palette[i] = colors[i + 4];
                }
                palette[60] = colors[16];
                palette[61] = colors[32];

                sprite.SetPalette(palette);

                // load pixel data
                var pixelBytes = new byte[128 * 448];
                var greenPalette = new Dictionary<int, Color>();
                for(int i = 0; i < 16; i++)
                {
                    greenPalette[i] = colors[i];
                }

                for (int y = 0; y < 448; y++)
                {
                    for (int x = 0; x < 128; x++)
                    {
                        int pixelPosition = y * bitmapData.Stride + (x * 4);

                        int b = pixels[pixelPosition + 0];
                        int g = pixels[pixelPosition + 1];
                        int r = pixels[pixelPosition + 2];
                        int a = pixels[pixelPosition + 3];

                        var pixel = Color.FromArgb(a, r, g, b);

                        if(y >= 440 && x >= 120)
                        {
                            // palette data, skip it
                            pixelBytes[x + y * 128] = 0;
                            continue;
                        }

                        if (a == 0 || greenPalette[0] == pixel)
                        {
                            pixelBytes[x + y * 128] = 0;
                        }
                        else
                        {
                            var pixelValue = greenPalette.FirstOrDefault(c => c.Value == pixel);
                            if(pixelValue.Key > 0)
                            {
                                pixelBytes[x + y * 128] = (byte)pixelValue.Key;
                            }
                            else
                            {
                                invalidPixelsFound = true;
                                //logger.Debug($"Import PNG: Bad pixel value found. File: {ofd.FileName}, Pixel: [{x + y * 128}] {pixel.ToString()}");
                            }
                        }
                    }
                }

                List<byte[]> tiles = new List<byte[]>();

                for (int yTile = 0; yTile < 56; yTile++)
                {
                    for (int xTile = 0; xTile < 16; xTile++)
                    {
                        var currentTile = new byte[8 * 8];

                        for(int y = 0; y < 8; y++)
                        {
                            for(int x = 0; x < 8; x++)
                            {
                                currentTile[x + y * 8] = pixelBytes[(xTile * 8 + x) + (yTile * 8 + y) * 128];
                            }
                        }

                        tiles.Add(currentTile);
                    }
                }

                byte[] pixels4bpp = new byte[0x7000];

                int offset = 0;
                foreach(var tile in tiles)
                {
                    var four = SpriteLibrary.Utilities.Tile8x8To4Bpp(tile);
                    Array.Copy(four, 0, pixels4bpp, offset, 32);
                    offset += 32;
                }

                sprite.Set4bppPixelData(pixels4bpp);

                if(invalidPixelsFound)
                {
                    MessageBox.Show("PNG contains pixels that do not use the first 16 colors of the embedded palette. Pixels have been made transparent. Please verify your source file.");
                }
                SpriteForm newMDI = new SpriteForm(ofd.FileName, sprite);
                newMDI.MdiParent = this;
                newMDI.Show();
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SpriteForm activeChild = (SpriteForm)this.ActiveMdiChild;
            if (activeChild != null)
            {
                try
                {
                    string filename = activeChild.Filename;

                    // new file, or old format file need to show save box
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.Filter = "ZSprite File (*.zspr)|*.zspr|All Files (*.*)|*.*";
                    sfd.Title = "Select a Sprite File";
                    sfd.FileName = String.IsNullOrEmpty(filename)
                                        ? activeChild.loadedSprite.DisplayText
                                        : Path.GetFileNameWithoutExtension(filename);

                    var result = sfd.ShowDialog();
                    if (result != DialogResult.OK)
                    {
                        return;
                    }

                    filename = sfd.FileName;

                    var spriteData = activeChild.loadedSprite.ToByteArray();
                    File.WriteAllBytes(filename, spriteData);
                    activeChild.UpdateFilename(filename);
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    MessageBox.Show(OopsMessage);
                }
            }
        }

        private void exportGIMPPaletteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SpriteForm activeChild = (SpriteForm)this.ActiveMdiChild;
            if (activeChild != null)
            {
                try
                {
                    string filename = activeChild.Filename;

                    // new file, or old format file need to show save box
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.Filter = "GIMP Palette File (*.gpl)|*.gpl|All Files (*.*)|*.*";
                    sfd.Title = "Select a GIMP Palette File";
                    sfd.FileName = String.IsNullOrEmpty(filename)
                                        ? activeChild.loadedSprite.DisplayText
                                        : Path.GetFileNameWithoutExtension(filename);

                    var result = sfd.ShowDialog();
                    if (result != DialogResult.OK)
                    {
                        return;
                    }

                    filename = sfd.FileName;

                    var pal = SpriteLibrary.GIMPPalette.BuildPaletteFromColorArray(activeChild.loadedSprite.Palette);
                    File.WriteAllText(filename, pal);
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    MessageBox.Show(OopsMessage);
                }
            }
        }

        private void importGIMPPaletteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SpriteForm activeChild = (SpriteForm)this.ActiveMdiChild;
            if (activeChild != null)
            {
                try
                {
                    string filename = activeChild.Filename;

                    // new file, or old format file need to show save box
                    OpenFileDialog ofd = new OpenFileDialog();
                    ofd.Filter = "GIMP Palette File (*.gpl)|*.gpl|All Files (*.*)|*.*";
                    ofd.Title = "Select a GIMP Palette File";

                    var result = ofd.ShowDialog();
                    if (result != DialogResult.OK)
                    {
                        return;
                    }

                    filename = ofd.FileName;

                    var pal = SpriteLibrary.GIMPPalette.BuildSpritePaletteColorsFromStringArray(File.ReadAllLines(filename));
                    if(pal.Length < 60)
                    {
                        MessageBox.Show("Palette is not long enough. Character Sprites require at least 60 entries.");
                        return;
                    }

                    activeChild.loadedSprite.SetPalette(pal);
                    activeChild.UpdateForm();
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    MessageBox.Show(OopsMessage);
                }
            }
        }

        private void exportYYCharPaletteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SpriteForm activeChild = (SpriteForm)this.ActiveMdiChild;
            if (activeChild != null)
            {
                try
                {
                    string filename = activeChild.Filename;

                    // new file, or old format file need to show save box
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.Filter = "YY-Char Palette File (*.pal)|*.pal|All Files (*.*)|*.*";
                    sfd.Title = "Select a YY-Char Palette File";
                    sfd.FileName = String.IsNullOrEmpty(filename)
                                        ? activeChild.loadedSprite.DisplayText
                                        : Path.GetFileNameWithoutExtension(filename);

                    var result = sfd.ShowDialog();
                    if (result != DialogResult.OK)
                    {
                        return;
                    }

                    filename = sfd.FileName;

                    var pal = SpriteLibrary.YYCharPalette.BuildPaletteFromColorArray(activeChild.loadedSprite.Palette);
                    File.WriteAllBytes(filename, pal);
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    MessageBox.Show(OopsMessage);
                }
            }
        }
    }
}
