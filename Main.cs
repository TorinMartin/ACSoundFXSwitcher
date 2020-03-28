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
    public class Main
    {
        String SFXPath = null;

        String dft_GUIDs = "dft\\GUIDs.txt";
        String dft_common_bank = "dft\\common.bank";
        String dft_common_strings = "dft\\common.strings.bank";

        String mod_GUIDs = "mod\\GUIDs.txt";
        String mod_common_bank = "mod\\common.bank";
        String mod_common_strings = "mod\\common.strings.bank";

        String currentVersion;
        String updateVersion;

        bool usingDefault = true;
        bool cannotLocate = false;

        Form1 form = null;

        public Main(Form1 form)
        {
            this.form = form;
        }

        public void VersionCheckComplete()
        {
            updateVersion = File.ReadAllText("files\\temp\\soundver.dat");

            if (updateVersion == currentVersion)
            {
                MessageBox.Show("Sound files are up to date!", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                form.ProgressForm.Close();
            }
            else
            {
                DialogResult dialogResult = MessageBox.Show("A sound file update is available. Would you like to install it?", "Update Available", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    form.ProgressForm.Close();
                    DownloadUpdate();
                }
                else if (dialogResult == DialogResult.No)
                {
                    form.ProgressForm.Close();
                }
            }
        }

        public void InstallApp()
        {
            string path = @SFXPath;
            string newPath = Path.GetFullPath(Path.Combine(path, @"..\..\apps\python\sfxSwitcher\sfxSwitcher.py"));

            if (File.Exists(newPath))
            {
                MessageBox.Show("In-game app is already installed!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                DialogResult dialogResult = MessageBox.Show("It appears the in-game app is not installed. Would you like to install it?", "App Available", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    DownloadApp();
                }
                else if (dialogResult == DialogResult.No)
                {
                    //no
                }
            }
        }

        public void DownloadApp()
        {
            form.ProgressForm = new Progress();
            form.ProgressForm.Text = "Installing App...";
            form.ProgressForm.Show();

            using (WebClient wc = new WebClient())
            {

                wc.DownloadProgressChanged += form.wc_DownloadProgressChanged;
                wc.DownloadFileCompleted += new AsyncCompletedEventHandler(form.wc_DownloadCompleteApp);
                wc.DownloadFileAsync(
                    // Param1 = Link of file
                    new System.Uri("https://www.dropbox.com/s/j3v7sh9kbivgw48/sfxSwitcherAPP.zip?dl=1"),
                    // Param2 = Path to save
                    "sfxSwitcher.zip"
                );
            }
        }

        public void ExtractApp()
        {
            try
            {
                string zipPath = @".\sfxSwitcher.zip";
                string path = @SFXPath;
                string extractPath = Path.GetFullPath(Path.Combine(path, @"..\..\apps\python"));
                ZipFile.ExtractToDirectory(zipPath, extractPath);
            }
            catch (System.IO.InvalidDataException)
            {
                MessageBox.Show("Unable to connect to server! Exiting...", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                form.ForceExit();
            }

            if (File.Exists("sfxSwitcher.zip"))
            {
                File.Delete("sfxSwitcher.zip");
            }

            MessageBox.Show("In-game app is now installed!", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            MessageBox.Show("Please enable in Assetto Corsa > Options > General!", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            form.ProgressForm.Close();
        }

        public void CheckForUpdate()
        {
            if (!File.Exists("files\\soundver.dat"))
            {
                MessageBox.Show("Soundver.dat file is missing! Unable to update", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                this.currentVersion = File.ReadAllText("files\\soundver.dat");

                form.ProgressForm = new Progress();
                form.ProgressForm.Text = "Checking Version...";
                form.ProgressForm.Show();

                using (WebClient wc = new WebClient())
                {
                    wc.DownloadProgressChanged += form.wc_DownloadProgressChanged;
                    wc.DownloadFileCompleted += new AsyncCompletedEventHandler(form.wc_DownloadCompleteVersion);
                    wc.DownloadFileAsync(
                        // Param1 = Link of file
                        new System.Uri("https://www.dropbox.com/s/d57zyjgunh8f9fd/soundver.dat?dl=1"),
                        // Param2 = Path to save
                        "files\\temp\\soundver.dat"
                    );
                }
            }
        }

        public void DownloadUpdate()
        {
            form.ProgressForm = new Progress();
            form.ProgressForm.Text = "Updating Sounds...";
            form.ProgressForm.Show();

            using (WebClient wc = new WebClient())
            {

                wc.DownloadProgressChanged += form.wc_DownloadProgressChanged;
                wc.DownloadFileCompleted += new AsyncCompletedEventHandler(form.wc_DownloadCompleteSounds);
                wc.DownloadFileAsync(
                    // Param1 = Link of file
                    new System.Uri("https://www.dropbox.com/s/ggc34isozgecm1h/soundfiles.zip?dl=1"),
                    // Param2 = Path to save
                    "soundfiles.zip"
                );
            }
        }

        public void InstallUpdate()
        {
            try
            {
                string zipPath = @".\soundfiles.zip";
                string extractPath = @".\files\temp\";
                ZipFile.ExtractToDirectory(zipPath, extractPath);
            }
            catch (System.IO.InvalidDataException)
            {
                MessageBox.Show("Unable to connect to server! Exiting...", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                form.ForceExit();
            }

            File.Copy("files\\temp\\dft\\GUIDs.txt", "dft\\GUIDs.txt", true);
            File.Copy("files\\temp\\dft\\common.bank", "dft\\common.bank", true);
            File.Copy("files\\temp\\dft\\common.strings.bank", "dft\\common.strings.bank", true);

            File.Copy("files\\temp\\mod\\GUIDs.txt", "mod\\GUIDs.txt", true);
            File.Copy("files\\temp\\mod\\common.bank", "mod\\common.bank", true);
            File.Copy("files\\temp\\mod\\common.strings.bank", "mod\\common.strings.bank", true);

            if (Directory.Exists("files\\temp\\dft"))
            {
                Directory.Delete("files\\temp\\dft", true);
            }
            if (Directory.Exists("files\\temp\\mod"))
            {
                Directory.Delete("files\\temp\\mod", true);
            }
            if (File.Exists("soundfiles.zip"))
            {
                File.Delete("soundfiles.zip");
            }
            File.Copy("files\\temp\\soundver.dat", "files\\soundver.dat", true);
            MessageBox.Show("Sound update completed!", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            form.ProgressForm.Close();
        }

        public void SwitchSounds()
        {
            if (usingDefault)
            {
                try
                {
                    File.Copy(mod_GUIDs, SFXPath + "\\GUIDs.txt", true);
                    File.Copy(mod_common_bank, SFXPath + "\\common.bank", true);
                    File.Copy(mod_common_strings, SFXPath + "\\common.strings.bank", true);
                    MessageBox.Show("Switched to MODDED Sounds", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (UnauthorizedAccessException)
                {
                    MessageBox.Show("Permission Denied", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (IOException)
                {
                    MessageBox.Show("An uknown error has occoured", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                try
                {
                    File.Copy(dft_GUIDs, SFXPath + "\\GUIDs.txt", true);
                    File.Copy(dft_common_bank, SFXPath + "\\common.bank", true);
                    File.Copy(dft_common_strings, SFXPath + "\\common.strings.bank", true);
                    MessageBox.Show("Switched to DEFAULT Sounds", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (UnauthorizedAccessException)
                {
                    MessageBox.Show("Permission Denied", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (IOException)
                {
                    MessageBox.Show("An uknown error has occoured", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public void SaveSettings(String SoundPath)
        {
            string path = @"files\\settings.ini";
            if (!File.Exists(path))
            {
                File.Create(path).Close();
            }
            TextWriter tw = new StreamWriter(path);
            tw.Write(SoundPath);
            tw.Close();
        }

        public Boolean IsCorrectFolder(String Path)
        {
            if (File.Exists(Path + "\\GUIDs.txt"))
            {
                if (File.Exists(Path + "\\common.bank"))
                {
                    if (File.Exists(Path + "\\common.strings.bank"))
                    {
                        SetPath(Path);
                        return true;
                    }
                }
            }
            return false;
        }

        public void CheckForFiles()
        {

            if (!File.Exists(dft_GUIDs))
            {
                MessageBox.Show("GUIDs.txt not found!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                form.ForceExit();
            }
            if (!File.Exists(dft_common_bank))
            {
                MessageBox.Show("common.bank not found!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                form.ForceExit();
            }
            if (!File.Exists(dft_common_strings))
            {
                MessageBox.Show("common.strings.bank not found!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                form.ForceExit();
            }
            if (!File.Exists(mod_GUIDs))
            {
                MessageBox.Show("Modded GUIDs.txt not found!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                form.ForceExit();
            }
            if (!File.Exists(mod_common_bank))
            {
                MessageBox.Show("Modded common.bank not found!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                form.ForceExit();
            }
            if (!File.Exists(mod_common_strings))
            {
                MessageBox.Show("Modded common.strings.bank not found!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                form.ForceExit();
            }

        }

        public void UsingDefaultSounds()
        {
            var areEquals = false;
            try
            {
                areEquals = System.IO.File.ReadLines(SFXPath + "\\GUIDs.txt").SequenceEqual(
                System.IO.File.ReadLines(dft_GUIDs));
            }
            catch (System.IO.FileNotFoundException)
            {
                MessageBox.Show("Incorrect Folder Selected!");
            }
            catch (System.IO.DirectoryNotFoundException)
            {
                MessageBox.Show("Folder Not Found!");
            }


            if (areEquals)
            {
                form.SetLabelSounds(true);
                usingDefault = true;
            }
            else
            {
                form.SetLabelSounds(false);
                usingDefault = false;
            }
        }

        public void CheckForFolder()
        {
            string steam = null;

            if (!File.Exists("files\\settings.ini"))
            {
                steam = Environment.ExpandEnvironmentVariables("%ProgramFiles(x86)%\\Steam\\SteamApps\\common\\assettocorsa\\content\\sfx");
                IsCorrectFolder(steam);
            }
            else
            {
                steam = File.ReadAllText("files\\settings.ini");
                IsCorrectFolder(steam);
            }

            if (!DoesPathExist(steam))
            {
                MessageBox.Show("Could not locate Assetto Corsa content/sfx directory! Please locate it", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cannotLocate = true;
                FolderSelect();
            }
        }

        public void FolderSelect()
        {
            string steam = null;

            while (!IsCorrectFolder(steam))
            {
                if (form.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                {
                    steam = form.folderBrowserDialog1.SelectedPath;
                    if (IsCorrectFolder(steam))
                    {
                        SaveSettings(steam);
                        UsingDefaultSounds();
                        cannotLocate = false;
                    }
                }
                else
                {
                    if (cannotLocate)
                        form.ForceExit();
                    else
                        break;
                }
            }
        }

        public void SetPath(String Path)
        {
            this.SFXPath = Path;
            form.SetLabelPath(Path);
        }

        public Boolean DoesPathExist(String Path)
        {
            if (Directory.Exists(Path))
            {
                SetPath(Path);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
