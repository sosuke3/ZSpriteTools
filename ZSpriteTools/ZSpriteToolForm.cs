using Microsoft.Win32;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Squirrel;

namespace ZSpriteTools
{
    public partial class ZSpriteToolForm : Form
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        static string LogPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ZSpriteTools");
        static string OopsMessage = $"Something went wrong. Please send your log file to Sosuke3. Go to \"Help->Open Log Folder\" to view log folder.";

        SpriteLibrary.PlayerSprite currentSprite;
        SpriteLibrary.AnimationType currentAnimation;
        Timer frameTimer;

        public ZSpriteToolForm()
        {
            if(false == Directory.Exists(LogPath))
            {
                Directory.CreateDirectory(LogPath);
            }

            this.Icon = ZSpriteTools.Properties.Resources.main;

            InitializeComponent();

            this.Text = this.Text + " - " + ProductVersion;

            authorRomDisplayTextBox.MaxLength = SpriteLibrary.Sprite.AuthorRomDisplayMaxLength;

            LoadAnimationComboBox();

            this.paletteComboBox.SelectedIndex = 0;
            this.animationComboBox.SelectedIndex = 0;
            this.animationPictureBox.BackColor = Color.Black;

            frameTimer = new Timer();
            frameTimer.Interval = 17;
            frameTimer.Tick += FrameTimer_Tick;
            frameTimer.Start();
        }

        private void LoadAnimationComboBox()
        {
            animationComboBox.Items.Clear();
            foreach(var a in SpriteLibrary.Animations.Instance.AnimationData)
            {
                animationComboBox.Items.Add(new ComboBoxItem() { Text = a.Value.Name, Value = a.Key });
            }
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
            try
            {
                var spriteFile = File.ReadAllBytes(fileName);
                var loadedSprite = new SpriteLibrary.PlayerSprite(spriteFile);
                if (loadedSprite.Version == 0)
                {
                    loadedSprite.DisplayText = Path.GetFileNameWithoutExtension(fileName);
                }

                SpriteForm newMDI = new SpriteForm(fileName, loadedSprite);
                newMDI.MdiParent = this;
                newMDI.Show();

                UpdateCurrentSprite(newMDI.loadedSprite);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                MessageBox.Show($"Failed to load file {fileName}. Make sure it's a sprite file.", "Error");
            }
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
                    FileUtilities.WriteAllBytes(filename, spriteData);
                    activeChild.UpdateFilename(filename);
                }
                catch(Exception ex)
                {
                    logger.Error(ex);
                    MessageBox.Show(OopsMessage, "Error");
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
                    MessageBox.Show(OopsMessage, "Error");
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
                    MessageBox.Show(OopsMessage, "Error");
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
                        FileUtilities.WriteAllBytes(sfd.FileName, activeChild.loadedSprite.PixelData);

                        MessageBox.Show($"Created {sfd.FileName}", "File Saved");
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    MessageBox.Show(OopsMessage, "Error");
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
                        FileUtilities.WriteAllBytes(sfd.FileName, activeChild.loadedSprite.PaletteData);
                        MessageBox.Show($"Created {sfd.FileName}", "File Saved");
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    MessageBox.Show(OopsMessage, "Error");
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
                DisableImport();

                UpdateCurrentSprite(null);
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
                EnableImport();

                UpdateCurrentSprite(activeChild.loadedSprite);
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

        void DisableImport()
        {
            this.importGIMPPaletteToolStripMenuItem.Enabled = false;
            this.importYYCharPaletteToolStripMenuItem.Enabled = false;
            this.importGraphicsGalePaletteToolStripMenuItem.Enabled = false;
        }

        void EnableImport()
        {
            this.importGIMPPaletteToolStripMenuItem.Enabled = true;
            this.importYYCharPaletteToolStripMenuItem.Enabled = true;
            this.importGraphicsGalePaletteToolStripMenuItem.Enabled = true;
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
            MessageBox.Show($"ZSpriteTools - {ProductVersion}\r\nSpecial thanks to fatmanspanda for providing animation frame data", "About");
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
            DisableImport();
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
                        MessageBox.Show($"Created {sfd.FileName}", "File Saved");
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    MessageBox.Show(OopsMessage, "Error");
                }
            }
        }

        void ExportPng(SpriteLibrary.PlayerSprite sprite, string filename)
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
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "PNG File (*.png)|*.png|All Files (*.*)|*.*";
                ofd.Title = "Select a PNG File";

                var invalidPixelsFound = false;

                var result = ofd.ShowDialog();
                if (result == DialogResult.OK)
                {
                    Bitmap tempBitmap = (Bitmap)Image.FromFile(ofd.FileName);
                    if (tempBitmap.Width != 128 || tempBitmap.Height != 448)
                    {
                        MessageBox.Show("Invalid PNG image. Must be 128 x 448 pixels", "Error");
                        return;
                    }
                    var sprite = new SpriteLibrary.PlayerSprite();
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

                    for (int y = 0; y < 8; y++)
                    {
                        for (int x = 0; x < 8; x++)
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
                    for (int i = 0; i < 15; i++)
                    {
                        palette[i] = colors[i + 1];
                    }
                    for (int i = 15; i < 30; i++)
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
                    for (int i = 0; i < 16; i++)
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

                            if (y >= 440 && x >= 120)
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
                                if (pixelValue.Key > 0)
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

                            for (int y = 0; y < 8; y++)
                            {
                                for (int x = 0; x < 8; x++)
                                {
                                    currentTile[x + y * 8] = pixelBytes[(xTile * 8 + x) + (yTile * 8 + y) * 128];
                                }
                            }

                            tiles.Add(currentTile);
                        }
                    }

                    byte[] pixels4bpp = new byte[0x7000];

                    int offset = 0;
                    foreach (var tile in tiles)
                    {
                        var four = SpriteLibrary.Utilities.Tile8x8To4Bpp(tile);
                        Array.Copy(four, 0, pixels4bpp, offset, 32);
                        offset += 32;
                    }

                    sprite.Set4bppPixelData(pixels4bpp);

                    if (invalidPixelsFound)
                    {
                        MessageBox.Show("PNG contains pixels that do not use the first 16 colors of the embedded palette. Pixels have been made transparent. Please verify your source file.", "Error");
                    }
                    SpriteForm newMDI = new SpriteForm(ofd.FileName, sprite);
                    newMDI.MdiParent = this;
                    newMDI.Show();
                }
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
                    FileUtilities.WriteAllBytes(filename, spriteData);
                    activeChild.UpdateFilename(filename);
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    MessageBox.Show(OopsMessage, "Error");
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
                    FileUtilities.WriteAllText(filename, pal);

                    MessageBox.Show($"Created {filename}", "File Saved");
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    MessageBox.Show(OopsMessage, "Error");
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
                        MessageBox.Show("Palette is not long enough. Character Sprites require at least 60 entries.", "Error");
                        return;
                    }

                    activeChild.loadedSprite.SetPalette(pal);
                    activeChild.UpdateForm();
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    MessageBox.Show(OopsMessage, "Error");
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
                    FileUtilities.WriteAllBytes(filename, pal);

                    MessageBox.Show($"Created {filename}", "File Saved");
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    MessageBox.Show(OopsMessage, "Error");
                }
            }
        }

        private void importYYCharPaletteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SpriteForm activeChild = (SpriteForm)this.ActiveMdiChild;
            if (activeChild != null)
            {
                try
                {
                    string filename = activeChild.Filename;

                    // new file, or old format file need to show save box
                    OpenFileDialog ofd = new OpenFileDialog();
                    ofd.Filter = "YY-Char Palette File (*.pal)|*.pal|All Files (*.*)|*.*";
                    ofd.Title = "Select a YY-Char Palette File";

                    var result = ofd.ShowDialog();
                    if (result != DialogResult.OK)
                    {
                        return;
                    }

                    filename = ofd.FileName;

                    var pal = SpriteLibrary.YYCharPalette.BuildSpritePaletteColorsFromByteArray(File.ReadAllBytes(filename));
                    if (pal.Length < 60)
                    {
                        MessageBox.Show("Palette is not long enough. Character Sprites require at least 60 entries.", "Error");
                        return;
                    }

                    activeChild.loadedSprite.SetPalette(pal);
                    activeChild.UpdateForm();
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    MessageBox.Show(OopsMessage, "Error");
                }
            }
        }

        private void exportGraphicsGalePaletteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SpriteForm activeChild = (SpriteForm)this.ActiveMdiChild;
            if (activeChild != null)
            {
                try
                {
                    string filename = activeChild.Filename;

                    // new file, or old format file need to show save box
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.Filter = "Graphics Gale Palette File (*.pal)|*.pal|All Files (*.*)|*.*";
                    sfd.Title = "Select a Graphics Gale Palette File";
                    sfd.FileName = String.IsNullOrEmpty(filename)
                                        ? activeChild.loadedSprite.DisplayText
                                        : Path.GetFileNameWithoutExtension(filename);

                    var result = sfd.ShowDialog();
                    if (result != DialogResult.OK)
                    {
                        return;
                    }

                    filename = sfd.FileName;

                    var pal = SpriteLibrary.GraphicsGalePalette.BuildPaletteFromColorArray(activeChild.loadedSprite.Palette);
                    FileUtilities.WriteAllText(filename, pal);

                    MessageBox.Show($"Created {filename}", "File Saved");
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    MessageBox.Show(OopsMessage, "Error");
                }
            }
        }

        private void importGraphicsGalePaletteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SpriteForm activeChild = (SpriteForm)this.ActiveMdiChild;
            if (activeChild != null)
            {
                try
                {
                    string filename = activeChild.Filename;

                    // new file, or old format file need to show save box
                    OpenFileDialog ofd = new OpenFileDialog();
                    ofd.Filter = "Graphics Gale Palette File (*.pal)|*.pal|All Files (*.*)|*.*";
                    ofd.Title = "Select a Graphics Gale Palette File";

                    var result = ofd.ShowDialog();
                    if (result != DialogResult.OK)
                    {
                        return;
                    }

                    filename = ofd.FileName;

                    var pal = SpriteLibrary.GraphicsGalePalette.BuildSpritePaletteColorsFromStringArray(File.ReadAllLines(filename));
                    if (pal.Length < 60)
                    {
                        MessageBox.Show("Palette is not long enough. Character Sprites require at least 60 entries.", "Error");
                        return;
                    }

                    activeChild.loadedSprite.SetPalette(pal);
                    activeChild.UpdateForm();
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    MessageBox.Show(OopsMessage, "Error");
                }
            }
        }

        private void exportRomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SpriteForm activeChild = (SpriteForm)this.ActiveMdiChild;
            if (activeChild != null)
            {
                try
                {
                    string filename = activeChild.Filename;

                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.Filter = "Rom File (*.sfc)|*.sfc|All Files (*.*)|*.*";
                    sfd.Title = "Select a Rom File";

                    var result = sfd.ShowDialog();
                    if (result != DialogResult.OK)
                    {
                        return;
                    }

                    filename = sfd.FileName;

                    var rom = new SpriteLibrary.Rom(filename);
                    rom.InjectSprite(activeChild.loadedSprite);

                    FileUtilities.WriteAllBytes(filename, rom.RomData);

                    MessageBox.Show($"Modified sprite in {filename}", "File Saved");
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    MessageBox.Show(OopsMessage, "Error");
                }
            }
        }

        private void importRomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Rom File (*.sfc)|*.sfc|All Files (*.*)|*.*";
            ofd.Title = "Select a Rom File";

            var result = ofd.ShowDialog();
            if (result == DialogResult.OK)
            {
                var rom = new SpriteLibrary.Rom(ofd.FileName);
                var sprite = rom.GetSprite();

                SpriteForm newMDI = new SpriteForm(ofd.FileName, sprite);
                newMDI.MdiParent = this;
                newMDI.Show();
            }
        }

        private void exportCopyToNewROMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SpriteForm activeChild = (SpriteForm)this.ActiveMdiChild;
            if (activeChild != null)
            {
                try
                {
                    OpenFileDialog ofd = new OpenFileDialog();
                    ofd.Filter = "Rom File (*.sfc)|*.sfc|All Files (*.*)|*.*";
                    ofd.Title = "Select a Base Rom File";

                    var result = ofd.ShowDialog();
                    if (result != DialogResult.OK)
                    {
                        return;
                    }

                    string filename = activeChild.Filename;

                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.Filter = "Rom File (*.sfc)|*.sfc|All Files (*.*)|*.*";
                    sfd.Title = "Select a New Rom File";
                    sfd.FileName = String.IsNullOrEmpty(filename)
                                        ? activeChild.loadedSprite.DisplayText
                                        : Path.GetFileNameWithoutExtension(filename);
                    sfd.FileName += " - " + Path.GetFileNameWithoutExtension(ofd.FileName);

                    result = sfd.ShowDialog();
                    if (result != DialogResult.OK)
                    {
                        return;
                    }

                    filename = sfd.FileName;

                    var rom = new SpriteLibrary.Rom(ofd.FileName);
                    rom.InjectSprite(activeChild.loadedSprite);

                    FileUtilities.WriteAllBytes(filename, rom.RomData);

                    MessageBox.Show($"Created {filename}", "File Saved");
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    MessageBox.Show(OopsMessage, "Error");
                }
            }
        }

        private void openLogDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(LogPath);
        }

        private void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var update = new UpdateForm();
            update.StartPosition = FormStartPosition.CenterParent;
            update.ShowDialog(this);
        }

        private void settingsToolStripButton_Click(object sender, EventArgs e)
        {
            var preferences = new PreferencesForm();
            preferences.ShowDialog();
        }

        private void batchConvertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var sourceFolder = "";
            var destinationFolder = "";

            using (var fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Select Source Folder";
                //fbd.SelectedPath = @"D:\Projects\Zelda\Sprites";
                //if (!String.IsNullOrEmpty(config.DefaultFolder) && Directory.Exists(config.DefaultFolder))
                //{
                //    fbd.SelectedPath = config.DefaultFolder;
                //}
                //else
                //{
                //    fbd.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                //}

                var result = fbd.ShowDialog();
                if (result == DialogResult.OK)
                {
                    sourceFolder = fbd.SelectedPath;
                    //config.DefaultFolder = fbd.SelectedPath;

                    //var fileName = fbd.SelectedPath;

                    //MessageBox.Show($"{fileName} has been created!", "Enemizer Rom Created");
                }
            }

            using (var fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Select Destination Folder";
                fbd.SelectedPath = sourceFolder;
                //if (!String.IsNullOrEmpty(config.DefaultFolder) && Directory.Exists(config.DefaultFolder))
                //{
                //    fbd.SelectedPath = config.DefaultFolder;
                //}
                //else
                //{
                //    fbd.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                //}

                var result = fbd.ShowDialog();
                if (result == DialogResult.OK)
                {
                    destinationFolder = fbd.SelectedPath;
                    //config.DefaultFolder = fbd.SelectedPath;

                    //var fileName = fbd.SelectedPath;

                    //MessageBox.Show($"{fileName} has been created!", "Enemizer Rom Created");
                }
            }

            if(String.IsNullOrEmpty(sourceFolder) || String.IsNullOrEmpty(destinationFolder))
            {
                return;
            }

            var files = Directory.GetFiles(sourceFolder, "*.spr");

            foreach(var file in files)
            {
                var destFilename = Path.Combine(destinationFolder, Path.GetFileNameWithoutExtension(file) + ".zspr");

                var spriteFile = File.ReadAllBytes(file);
                var loadedSprite = new SpriteLibrary.PlayerSprite(spriteFile);
                if (loadedSprite.Version == 0)
                {
                    loadedSprite.DisplayText = Path.GetFileNameWithoutExtension(file);
                }

                var spriteData = loadedSprite.ToByteArray();
                FileUtilities.WriteAllBytes(destFilename, spriteData);
            }
        }

        private void animationComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string key = ((ComboBoxItem)animationComboBox.SelectedItem).Value;
            if(SpriteLibrary.Animations.Instance.AnimationData.TryGetValue(key, out currentAnimation))
            {
                if (currentSprite != null)
                {
                    currentSprite.SetAnimation(currentAnimation);
                }
            }
            else
            {
                currentAnimation = null;
            }
        }

        private void FrameTimer_Tick(object sender, EventArgs e)
        {
            if (currentSprite == null || currentAnimation == null)
            {
                return;
            }
            // do animation stuff

            Bitmap tempBitmap = new Bitmap(64, 64, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(tempBitmap);

            //var origin = new Point(24, 24);// new Point(24, 24);

            currentSprite.DrawAnimation(g);

            animationPictureBox.Image = SpriteLibrary.Utilities.ResizeBitmap(tempBitmap, 256, 256);
        }

        const int paletteSquareSize = 24;

        private void BuildPalette(int paletteType)
        {
            if(currentSprite == null)
            {
                return;
            }
            currentSprite.SetAnimationPalette(paletteComboBox.SelectedIndex);

            palettePictureBox.BackColor = Color.Black;
            //palettePictureBox.SizeMode = PictureBoxSizeMode.StretchImage;

            Color[] palette = null;
            switch(paletteType)
            {
                case 0:
                    palette = currentSprite.GreenMailPalette.PaletteColors;
                    break;
                case 1:
                    palette = currentSprite.BlueMailPalette.PaletteColors;
                    break;
                case 2:
                    palette = currentSprite.RedMailPalette.PaletteColors;
                    break;
                case 3:
                    palette = currentSprite.BunnyPalette.PaletteColors;
                    break;
                default:
                    return;
            }

            int width = paletteSquareSize * 8;
            int height = paletteSquareSize * 2;
            palettePictureBox.Width = width;
            palettePictureBox.Height = height;
            palettePictureBox.Image = new Bitmap(width, height);

            Graphics g = Graphics.FromImage(palettePictureBox.Image);

            int x = 0;
            int y = 0;

            for (int i = 0; i < palette.Length; i++)
            {
                x = i * paletteSquareSize % (width);
                y = i * paletteSquareSize / (width) * paletteSquareSize;

                g.FillRectangle(new SolidBrush(palette[i]), x, y, paletteSquareSize, paletteSquareSize);
            }

            glovesPalettePictureBox.BackColor = Color.Black;
            glovesPalettePictureBox.Width = paletteSquareSize * 2;
            glovesPalettePictureBox.Height = paletteSquareSize;
            glovesPalettePictureBox.Image = new Bitmap(paletteSquareSize * 2, paletteSquareSize);

            g = Graphics.FromImage(glovesPalettePictureBox.Image);
            g.FillRectangle(new SolidBrush(currentSprite.GlovePalette[0]), 0, 0, paletteSquareSize, paletteSquareSize);
            g.FillRectangle(new SolidBrush(currentSprite.GlovePalette[1]), paletteSquareSize, 0, paletteSquareSize, paletteSquareSize);
        }

        private void paletteComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            BuildPalette(paletteComboBox.SelectedIndex);
        }

        private void UpdateCurrentSprite(SpriteLibrary.PlayerSprite loadedSprite)
        {
            currentSprite = loadedSprite;
            if (loadedSprite != null)
            {
                currentSprite.SetAnimation(currentAnimation);
                currentSprite.SetAnimationPalette(paletteComboBox.SelectedIndex);
                BuildPalette(paletteComboBox.SelectedIndex);
            }
        }

        private void clearPNGPaletteTileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(currentSprite == null)
            {
                return;
            }

            var empty = new byte[32];
            var pixels = currentSprite.PixelData;
            Array.Copy(empty, 0, pixels, 0x7000 - 32, 32);
            currentSprite.PixelData = pixels;

            if (this.ActiveMdiChild != null)
            {
                SpriteForm activeChild = (SpriteForm)this.ActiveMdiChild;

                activeChild.UpdateForm();
            }
        }

        private void palettePictureBox_DoubleClick(object sender, EventArgs e)
        {
            var mouseE = e as MouseEventArgs;
            var x = mouseE.X / paletteSquareSize;
            var y = mouseE.Y / paletteSquareSize;

            if(x >= 8 || y >= 2)
            {
                // clicked outside palette area (control is too big)
                return;
            }

            SpriteLibrary.Palette palette = null;
            switch (paletteComboBox.SelectedIndex)
            {
                case 0:
                    palette = currentSprite.GreenMailPalette;
                    break;
                case 1:
                    palette = currentSprite.BlueMailPalette;
                    break;
                case 2:
                    palette = currentSprite.RedMailPalette;
                    break;
                case 3:
                    palette = currentSprite.BunnyPalette;
                    break;
                default:
                    return;
            }

            int index = x + y * 8;

            ColorDialog colorDialog = new ColorDialog();
            colorDialog.Color = palette[index];
            if(colorDialog.ShowDialog() == DialogResult.OK)
            {
                palette[index] = colorDialog.Color;
            }

            BuildPalette(paletteComboBox.SelectedIndex);
            if (this.ActiveMdiChild != null)
            {
                SpriteForm activeChild = (SpriteForm)this.ActiveMdiChild;

                activeChild.UpdateForm();
            }
        }

        private void glovesPalettePictureBox_DoubleClick(object sender, EventArgs e)
        {
            var mouseE = e as MouseEventArgs;
            var x = mouseE.X / paletteSquareSize;
            var y = mouseE.Y / paletteSquareSize;

            if (x >= 2 || y >= 1)
            {
                // clicked outside palette area (control is too big)
                return;
            }

            ColorDialog colorDialog = new ColorDialog();
            colorDialog.Color = currentSprite.GlovePalette[x];

            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                currentSprite.GlovePalette[x] = colorDialog.Color;
            }

            BuildPalette(paletteComboBox.SelectedIndex);
            if (this.ActiveMdiChild != null)
            {
                SpriteForm activeChild = (SpriteForm)this.ActiveMdiChild;

                activeChild.UpdateForm();
            }
        }

        private void animationPreviewSetBackgroundColorMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            colorDialog.Color = animationPictureBox.BackColor;
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                animationPictureBox.BackColor = colorDialog.Color;
            }
        }
    }

    public struct ComboBoxItem
    {
        public string Text { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }
}
