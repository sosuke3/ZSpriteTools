namespace ZSpriteTools
{
    partial class SpriteForm
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
            this.spritePictureBox = new System.Windows.Forms.PictureBox();
            this.palettePictureBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.spritePictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.palettePictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // spritePictureBox
            // 
            this.spritePictureBox.Location = new System.Drawing.Point(5, 76);
            this.spritePictureBox.Name = "spritePictureBox";
            this.spritePictureBox.Size = new System.Drawing.Size(512, 1792);
            this.spritePictureBox.TabIndex = 4;
            this.spritePictureBox.TabStop = false;
            // 
            // palettePictureBox
            // 
            this.palettePictureBox.Location = new System.Drawing.Point(5, 6);
            this.palettePictureBox.Name = "palettePictureBox";
            this.palettePictureBox.Size = new System.Drawing.Size(512, 64);
            this.palettePictureBox.TabIndex = 3;
            this.palettePictureBox.TabStop = false;
            // 
            // SpriteForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(543, 367);
            this.Controls.Add(this.spritePictureBox);
            this.Controls.Add(this.palettePictureBox);
            this.Name = "SpriteForm";
            this.Text = "Sprite";
            ((System.ComponentModel.ISupportInitialize)(this.spritePictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.palettePictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox spritePictureBox;
        private System.Windows.Forms.PictureBox palettePictureBox;
    }
}