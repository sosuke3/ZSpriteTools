namespace ZSpriteTools
{
    partial class ZSpriteToolForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.exportRawPixelDataButton = new System.Windows.Forms.Button();
            this.importRawPixelDataButton = new System.Windows.Forms.Button();
            this.saveSpriteButton = new System.Windows.Forms.Button();
            this.loadSpriteButton = new System.Windows.Forms.Button();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.authorTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.displayTextTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importRawPaletteDataButton = new System.Windows.Forms.Button();
            this.exportRawPaletteDataButton = new System.Windows.Forms.Button();
            this.palettePictureBox = new System.Windows.Forms.PictureBox();
            this.spritePictureBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.palettePictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spritePictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.exportRawPaletteDataButton);
            this.splitContainer1.Panel1.Controls.Add(this.importRawPaletteDataButton);
            this.splitContainer1.Panel1.Controls.Add(this.exportRawPixelDataButton);
            this.splitContainer1.Panel1.Controls.Add(this.importRawPixelDataButton);
            this.splitContainer1.Panel1.Controls.Add(this.saveSpriteButton);
            this.splitContainer1.Panel1.Controls.Add(this.loadSpriteButton);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(474, 557);
            this.splitContainer1.SplitterDistance = 137;
            this.splitContainer1.SplitterWidth = 2;
            this.splitContainer1.TabIndex = 0;
            // 
            // exportRawPixelDataButton
            // 
            this.exportRawPixelDataButton.Location = new System.Drawing.Point(3, 101);
            this.exportRawPixelDataButton.Name = "exportRawPixelDataButton";
            this.exportRawPixelDataButton.Size = new System.Drawing.Size(130, 23);
            this.exportRawPixelDataButton.TabIndex = 3;
            this.exportRawPixelDataButton.Text = "Export Raw Pixel Data";
            this.exportRawPixelDataButton.UseVisualStyleBackColor = true;
            this.exportRawPixelDataButton.Click += new System.EventHandler(this.exportRawPixelDataButton_Click);
            // 
            // importRawPixelDataButton
            // 
            this.importRawPixelDataButton.Location = new System.Drawing.Point(3, 72);
            this.importRawPixelDataButton.Name = "importRawPixelDataButton";
            this.importRawPixelDataButton.Size = new System.Drawing.Size(130, 23);
            this.importRawPixelDataButton.TabIndex = 2;
            this.importRawPixelDataButton.Text = "Import Raw Pixel Data";
            this.importRawPixelDataButton.UseVisualStyleBackColor = true;
            this.importRawPixelDataButton.Click += new System.EventHandler(this.importRawPixelDataButton_Click);
            // 
            // saveSpriteButton
            // 
            this.saveSpriteButton.Location = new System.Drawing.Point(3, 32);
            this.saveSpriteButton.Name = "saveSpriteButton";
            this.saveSpriteButton.Size = new System.Drawing.Size(130, 23);
            this.saveSpriteButton.TabIndex = 1;
            this.saveSpriteButton.Text = "Save Sprite";
            this.saveSpriteButton.UseVisualStyleBackColor = true;
            this.saveSpriteButton.Click += new System.EventHandler(this.saveSpriteButton_Click);
            // 
            // loadSpriteButton
            // 
            this.loadSpriteButton.Location = new System.Drawing.Point(3, 3);
            this.loadSpriteButton.Name = "loadSpriteButton";
            this.loadSpriteButton.Size = new System.Drawing.Size(130, 23);
            this.loadSpriteButton.TabIndex = 0;
            this.loadSpriteButton.Text = "Load Sprite";
            this.loadSpriteButton.UseVisualStyleBackColor = true;
            this.loadSpriteButton.Click += new System.EventHandler(this.loadSpriteButton_Click);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer2.IsSplitterFixed = true;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.authorTextBox);
            this.splitContainer2.Panel1.Controls.Add(this.label2);
            this.splitContainer2.Panel1.Controls.Add(this.displayTextTextBox);
            this.splitContainer2.Panel1.Controls.Add(this.label1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.spritePictureBox);
            this.splitContainer2.Panel2.Controls.Add(this.palettePictureBox);
            this.splitContainer2.Size = new System.Drawing.Size(335, 557);
            this.splitContainer2.SplitterDistance = 96;
            this.splitContainer2.TabIndex = 0;
            // 
            // authorTextBox
            // 
            this.authorTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.authorTextBox.Location = new System.Drawing.Point(6, 58);
            this.authorTextBox.Name = "authorTextBox";
            this.authorTextBox.Size = new System.Drawing.Size(317, 20);
            this.authorTextBox.TabIndex = 3;
            this.authorTextBox.TextChanged += new System.EventHandler(this.authorTextBox_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Author:";
            // 
            // displayTextTextBox
            // 
            this.displayTextTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.displayTextTextBox.Location = new System.Drawing.Point(6, 19);
            this.displayTextTextBox.Name = "displayTextTextBox";
            this.displayTextTextBox.Size = new System.Drawing.Size(317, 20);
            this.displayTextTextBox.TabIndex = 1;
            this.displayTextTextBox.TextChanged += new System.EventHandler(this.displayTextTextBox_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Display Text:";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(474, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(92, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            // 
            // importRawPaletteDataButton
            // 
            this.importRawPaletteDataButton.Location = new System.Drawing.Point(3, 144);
            this.importRawPaletteDataButton.Name = "importRawPaletteDataButton";
            this.importRawPaletteDataButton.Size = new System.Drawing.Size(130, 23);
            this.importRawPaletteDataButton.TabIndex = 4;
            this.importRawPaletteDataButton.Text = "Import Raw Palette Data";
            this.importRawPaletteDataButton.UseVisualStyleBackColor = true;
            this.importRawPaletteDataButton.Click += new System.EventHandler(this.importRawPaletteDataButton_Click);
            // 
            // exportRawPaletteDataButton
            // 
            this.exportRawPaletteDataButton.Location = new System.Drawing.Point(3, 173);
            this.exportRawPaletteDataButton.Name = "exportRawPaletteDataButton";
            this.exportRawPaletteDataButton.Size = new System.Drawing.Size(130, 23);
            this.exportRawPaletteDataButton.TabIndex = 5;
            this.exportRawPaletteDataButton.Text = "Export Raw Palette Data";
            this.exportRawPaletteDataButton.UseVisualStyleBackColor = true;
            this.exportRawPaletteDataButton.Click += new System.EventHandler(this.exportRawPaletteDataButton_Click);
            // 
            // palettePictureBox
            // 
            this.palettePictureBox.Location = new System.Drawing.Point(6, 3);
            this.palettePictureBox.Name = "palettePictureBox";
            this.palettePictureBox.Size = new System.Drawing.Size(317, 47);
            this.palettePictureBox.TabIndex = 0;
            this.palettePictureBox.TabStop = false;
            // 
            // spritePictureBox
            // 
            this.spritePictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.spritePictureBox.Location = new System.Drawing.Point(6, 56);
            this.spritePictureBox.Name = "spritePictureBox";
            this.spritePictureBox.Size = new System.Drawing.Size(317, 389);
            this.spritePictureBox.TabIndex = 1;
            this.spritePictureBox.TabStop = false;
            // 
            // ZSpriteToolForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(474, 581);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(490, 620);
            this.Name = "ZSpriteToolForm";
            this.Text = "ZSpriteTool";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.palettePictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spritePictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button saveSpriteButton;
        private System.Windows.Forms.Button loadSpriteButton;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.TextBox displayTextTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.TextBox authorTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button exportRawPixelDataButton;
        private System.Windows.Forms.Button importRawPixelDataButton;
        private System.Windows.Forms.Button exportRawPaletteDataButton;
        private System.Windows.Forms.Button importRawPaletteDataButton;
        private System.Windows.Forms.PictureBox spritePictureBox;
        private System.Windows.Forms.PictureBox palettePictureBox;
    }
}

