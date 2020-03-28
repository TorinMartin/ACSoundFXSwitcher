using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Net;
using System.IO.Compression;

namespace SFXSwitcher
{
    public partial class Form1 : Form
    {

        public Progress ProgressForm;

        private static Form1 form = null;
        Main Main = null;


        public Form1()
        {
            form = this;
            Main = new Main(form);

            InitializeComponent();

            this.MaximizeBox = false;

            this.Menu = new MainMenu();
            MenuItem item = new MenuItem("Settings");
            this.Menu.MenuItems.Add(item);
            item.MenuItems.Add("Change AC SFX Directory...", new EventHandler(Change_Click));
            item.MenuItems.Add("Check For Sound File Update...", new EventHandler(Update_Click));

            // Make text transparent
            var pos = this.PointToScreen(label_logo.Location);
            pos = pictureBox1.PointToClient(pos);
            label_logo.Parent = pictureBox1;
            label_logo.Location = pos;
            label_logo.BackColor = Color.Transparent;

            Main.CheckForFolder();
            Main.CheckForFiles();

            Main.UsingDefaultSounds();

        }

        private void Update_Click(object sender, EventArgs e)
        {
            Main.CheckForUpdate();
        }

        private void ingame_btn_Click(object sender, EventArgs e)
        {
            Main.InstallApp();
        }

        private void Change_Click(object sender, EventArgs e)
        {
            Main.FolderSelect();
        }

        private void btn_switch_Click(object sender, EventArgs e)
        {
            Main.SwitchSounds();
            Main.UsingDefaultSounds();
        }

        public void wc_DownloadCompleteSounds(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            MessageBox.Show("Download Complete! Installing...", "Success!", MessageBoxButtons.OK, MessageBoxIcon.None);
            Main.InstallUpdate();
        }

        public void wc_DownloadCompleteApp(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            MessageBox.Show("Download Complete! Installing...", "Success!", MessageBoxButtons.OK, MessageBoxIcon.None);
            Main.ExtractApp();
        }

        public void wc_DownloadCompleteVersion(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            Main.VersionCheckComplete();
        }
        public void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            ProgressForm.progressBar1.Value = e.ProgressPercentage;
        }

        public void SetLabelPath(String Path)
        {
            label_path.Text = Path;
        }

        public void SetLabelSounds(Boolean UsingDefault)
        {
            switch (UsingDefault)
            {
                case true:
                    label_sounds.Text = "Default Sounds";
                    label_sounds.ForeColor = System.Drawing.Color.Green;
                    break;
                case false:
                    label_sounds.Text = "Modded Sounds";
                    label_sounds.ForeColor = System.Drawing.Color.Red;
                    break;
            }
        }

        public void ForceExit()
        {
            Environment.Exit(0);
        }
    }
}
