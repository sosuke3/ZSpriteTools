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
            this.Icon = ZSpriteTools.Properties.Resources.main;

            InitializeComponent();

            this.Text = this.Text + " - " + ProductVersion;

            authorRomDisplayTextBox.MaxLength = SpriteLibrary.Sprite.AuthorRomDisplayMaxLength;

            DisableSave();
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
                var spriteFile = File.ReadAllBytes(ofd.FileName);
                loadedSprite = new SpriteLibrary.Sprite(spriteFile);
                if(loadedSprite.Version == 0)
                {
                    loadedSprite.DisplayText = Path.GetFileNameWithoutExtension(ofd.FileName);
                }

                SpriteForm newMDI = new SpriteForm(ofd.FileName, loadedSprite);
                newMDI.MdiParent = this;
                newMDI.Show();
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
                    File.WriteAllBytes(filename, spriteData);
                }
                catch
                {
                    MessageBox.Show("Something went wrong. Haha");
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
                        loadedSprite.PixelData = rawFile;

                        activeChild.ImportRawPixels(rawFile);
                    }
                }
                catch
                {
                    MessageBox.Show("Something went wrong. Haha");
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
                        loadedSprite.PaletteData = rawFile;

                        activeChild.ImportRawPalette(rawFile);
                    }
                }
                catch
                {
                    MessageBox.Show("Something went wrong. Haha");
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
                catch
                {
                    MessageBox.Show("Something went wrong. Haha");
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
                catch
                {
                    MessageBox.Show("Something went wrong. Haha");
                }
            }
        }

        private void ZSpriteToolForm_MdiChildActivate(object sender, EventArgs e)
        {
            if(this.ActiveMdiChild == null)
            {
                DisableSave();
            }
            else
            {
                SpriteForm activeChild = (SpriteForm)this.ActiveMdiChild;

                this.displayTextTextBox.Text = activeChild.loadedSprite.DisplayText;
                this.authorTextBox.Text = activeChild.loadedSprite.Author;
                this.authorRomDisplayTextBox.Text = activeChild.loadedSprite.AuthorRomDisplay;

                EnableSave();
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
    }
}
