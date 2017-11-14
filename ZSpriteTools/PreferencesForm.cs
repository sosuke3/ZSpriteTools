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
    public partial class PreferencesForm : Form
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public PreferencesForm()
        {
            InitializeComponent();
        }

        private void associateExtensionsButton_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            FileAssociations.EnsureAssociationsSet();
            Cursor = Cursors.Default;
        }

        private void Save_Click(object sender, EventArgs e)
        {
            // TODO: save config
            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
