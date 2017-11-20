using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NLog;
using Squirrel;
using System.Reflection;
using System.IO;

namespace ZSpriteTools
{
    public partial class UpdateForm : Form
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        enum UpdateState { NoUpdates, Checking, HasUpdates, UpdateComplete, UpdateFailed };
        UpdateState state;

        public UpdateForm()
        {
            InitializeComponent();

            this.state = UpdateState.Checking;
            this.updateButton.Enabled = false;
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.Cursor = Cursors.AppStarting;

            if(await CheckForUpdates())
            {
                state = UpdateState.HasUpdates;
                this.updateButton.Enabled = true;
                this.updatesLabel.Text = "Updates are available.";
            }
            else
            {
                state = UpdateState.NoUpdates;
                this.updateButton.Enabled = false;
                this.updatesLabel.Text = "No updates are available.";
            }

            this.Cursor = Cursors.Default;
        }

        private async void updateButton_Click(object sender, EventArgs e)
        {
            try
            {
                await UpdateApp();
            }
            catch(Exception ex)
            {
                logger.Error(ex);
                MessageBox.Show("Something went wrong trying to update. If this problem persists please submit a copy of your log file, or try manually downloading the latest Setup and running it.", "Error");
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Squirrel stuff
        private const ShortcutLocation DefaultLocations = ShortcutLocation.StartMenu | ShortcutLocation.Desktop;
        private const string githubURL = "https://github.com/sosuke3/ZSpriteTools";

        public static async Task<bool> CheckForUpdates()
        {
            using (var mgr = await UpdateManager.GitHubUpdateManager(githubURL))
            {
                var updates = await mgr.CheckForUpdate();
                if (updates.ReleasesToApply.Any())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public async Task UpdateApp()
        {
            using (var mgr = await UpdateManager.GitHubUpdateManager(githubURL))
            {
                var updates = await mgr.CheckForUpdate();
                if (updates.ReleasesToApply.Any())
                {
                    var lastVersion = updates.ReleasesToApply.OrderBy(x => x.Version).Last();
                    await mgr.DownloadReleases(new[] { lastVersion });
                    await mgr.ApplyReleases(updates);
                    await mgr.UpdateApp();

                    state = UpdateState.UpdateComplete;
                    this.updateButton.Enabled = false;
                    updatesLabel.Text = "ZSpriteTools has been updated. Please close and restart.";
                }
                else
                {
                    state = UpdateState.NoUpdates;
                    this.updateButton.Enabled = false;
                    updatesLabel.Text = "No updates are available.";
                }
            }
        }

        public static void OnAppUpdate(Version version)
        {
            // Could use this to do stuff here too.
        }

        public static void OnInitialInstall(Version version)
        {
            var exePath = Assembly.GetEntryAssembly().Location;
            string appName = Path.GetFileName(exePath);

            using (var mgr = UpdateManager.GitHubUpdateManager(githubURL))
            {
                // Create Desktop and Start Menu shortcuts
                mgr.Result.CreateShortcutsForExecutable(appName, DefaultLocations, false);
            }
        }

        public static void OnAppUninstall(Version version)
        {
            var exePath = Assembly.GetEntryAssembly().Location;
            string appName = Path.GetFileName(exePath);

            using (var mgr = UpdateManager.GitHubUpdateManager(githubURL))
            {
                // Remove Desktop and Start Menu shortcuts
                mgr.Result.RemoveShortcutsForExecutable(appName, DefaultLocations);
            }
        }

    }
}
