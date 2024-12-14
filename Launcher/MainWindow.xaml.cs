using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
//using System.IO.Compression;
using System.Net;
using System.Windows;
using Ionic.Zip;
using Ookii.Dialogs.Wpf;

namespace Launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string serverUri;
        private string versionFileName;
        private string releaseNotesFileName;
        private string releaseNotesContents;
        private string zipFileName;
        private string exeFileName;

        private string rootPath;
        private string gameExe;
        private string versionFile;
        private string gameZip;

        private string versionFileLink = "";
        private string relNotesFileLink = "";
        private string gameZipLink = "";

        private bool canClickPlay;
        private LauncherStatus _status;
        internal LauncherStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                var enabled = true;
                switch (_status)
                {
                    case LauncherStatus.notready:
                        enabled = false;
                        break;
                    case LauncherStatus.ready:
                        PlayButton.Content = "Play";
                        break;
                    case LauncherStatus.failed:
                        PlayButton.Content = "Update Failed - Retry";
                        break;
                    case LauncherStatus.downloadingGame:
                        PlayButton.Content = "Downloading Game";
                        enabled = false;
                        break;
                    case LauncherStatus.downloadingUpdate:
                        PlayButton.Content = "Downloading Update";
                        enabled = false;
                        break;
                    default:
                        break;
                }
                canClickPlay = enabled;
            }
        }

        internal enum LauncherStatus
        {
            notready, ready, failed, downloadingGame, downloadingUpdate
        }

        public delegate void ConfigsUpdatedHandler();
        public event ConfigsUpdatedHandler ConfigsUpdated;
        public void OnConfigsUpdated()
        {
            ConfigsUpdated?.Invoke();
        }

        public MainWindow()
        {
            InitializeComponent();
            ConfigsUpdated += updateAfterConfigChange;
        }

        public void InitConfigs()
        {
            serverUri = ConfigurationManager.AppSettings["ServerUri"];
            versionFileName = ConfigurationManager.AppSettings["VersionFileName"];
            releaseNotesFileName = ConfigurationManager.AppSettings["releaseNotesFileName"];
            exeFileName = ConfigurationManager.AppSettings["ExeFileName"];
            zipFileName = ConfigurationManager.AppSettings["ZipFileName"];

            versionFileLink = Path.Combine(serverUri, versionFileName);
            gameZipLink = Path.Combine(serverUri, zipFileName);
            relNotesFileLink = Path.Combine(serverUri, releaseNotesFileName);

            rootPath = Properties.Settings.Default.RootDir;
            //rootPath = @"F:\games\HackerWars";
            versionFile = Path.Combine(rootPath, versionFileName);
            gameZip = Path.Combine(rootPath, zipFileName);
            gameExe = Path.Combine(rootPath, exeFileName);


            txtRootDir.Text = rootPath;
            if (CheckConfigs())
            {
                txtRootDir.Visibility = Visibility.Visible;
                btnChooseRootDir.Visibility = Visibility.Hidden;
                Status = LauncherStatus.ready;
            }
            else
            {
                txtRootDir.Visibility = Visibility.Hidden;
                btnChooseRootDir.Visibility = Visibility.Visible;
            }
            OnConfigsUpdated();
        }

        public bool CheckConfigs()
        {
            var ok = true;
            if (string.IsNullOrEmpty(rootPath))
            {
                PlayButton.Content = "Please set install directory.";
                Status = LauncherStatus.notready;
                ok = false;
            }
            return ok;
        }

        private void btnChooseRootDir_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new VistaFolderBrowserDialog();
            if (dlg.ShowDialog() == true)
            {
                MessageBox.Show(dlg.SelectedPath);
                rootPath = dlg.SelectedPath;
                Properties.Settings.Default.RootDir = rootPath;
                Properties.Settings.Default.Save();
                InitConfigs();
            }
        }

        private void CheckForUpdates()
        {
            if (File.Exists(versionFile))
            {
                var localVersion = new Version(File.ReadAllText(versionFile));
                VersionText.Text = localVersion.ToString();

                try
                {
                    var wc = getWebClient();
                    var onlineVersion = new Version(wc.DownloadString(versionFileLink));

                    if (onlineVersion.IsDifferent(localVersion))
                    {
                        InstallGameFiles(true, onlineVersion);
                    }
                    else
                    {
                        Status = LauncherStatus.ready;
                    }
                }
                catch (Exception e)
                {
                    Status = LauncherStatus.failed;
                    MessageBox.Show($"Error checking for game updates: {e}");
                }
            }
            else
            {
                InstallGameFiles(false, Version.zero);
            }
        }

        private WebClient getWebClient()
        {
            var wc = new WebClient();
            wc.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["GetUser"], ConfigurationManager.AppSettings["GetPass"]);
            return wc;
        }

        private void InstallGameFiles(bool isUpdate, Version onlineVersion)
        {
            try
            {
                var wc = getWebClient();
                if (isUpdate)
                {
                    Status = LauncherStatus.downloadingUpdate;
                }
                else
                {
                    Status = LauncherStatus.downloadingGame;
                    onlineVersion = new Version(wc.DownloadString(versionFileLink));
                }

                wc.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadGameCompletedCallback);
                wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadGameProgressChanged);
                wc.DownloadFileAsync(new Uri(gameZipLink), gameZip, onlineVersion);
            }
            catch (Exception e)
            {
                Status = LauncherStatus.failed;
                MessageBox.Show($"Error installing game files: {e}");
            }
        }

        private void DownloadGameProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            // Displays the operation identifier, and the transfer progress.
            var content = string.Format("Downloading Game Files\n{0} % complete...",
                e.ProgressPercentage);
            PlayButton.Content = content;
        }

        private void CleanDirectory(string path)
        {

        }

        private void DownloadGameCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            try
            {
                var onlineVersion = ((Version)e.UserState).ToString();
                PlayButton.Content = "Extracting zip file..";
                //ZipFile.ExtractToDirectory(gameZip, tmpPath);
                using (var zip1 = ZipFile.Read(gameZip))
                {
                    foreach(var zipEntry in zip1)
                    {
                        zipEntry.Extract(rootPath, ExtractExistingFileAction.OverwriteSilently);
                    }
                }

                PlayButton.Content = "Cleaning up temp files ..";

                File.Delete(gameZip);
                File.WriteAllText(versionFile, onlineVersion);
                VersionText.Text = onlineVersion;
                Status = LauncherStatus.ready;
            }
            catch (Exception ex)
            {
                Status = LauncherStatus.failed;
                MessageBox.Show($"Error finishing download: {ex}");
            }

        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            InitConfigs();

        }

        private void updateAfterConfigChange()
        {
            if (Status != LauncherStatus.notready)
            {
                CheckForUpdates();
            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (!canClickPlay) { return; }

            if (File.Exists(gameExe) && Status == LauncherStatus.ready)
            {
                ProcessStartInfo startInfo = new ProcessStartInfo(gameExe);
                startInfo.WorkingDirectory = Path.Combine(rootPath, "");
                Process.Start(startInfo);

                Close();
            }
            else
            {
                CheckForUpdates();
            }
        }
        
        struct Version
        {
            internal static Version zero = new Version(0, 0, 0);

            private short major, minor, subMinor;

            internal Version (short m, short mm, short sm)
            {
                major = m; minor = mm; subMinor = sm;
            }
            internal Version (string versionString)
            {
                var versionStrings = versionString.Split('.');
                if (versionStrings.Length != 3)
                {
                    major = 0; minor = 0; subMinor = 0;
                    return;
                }

                major = short.Parse(versionStrings[0]);
                minor = short.Parse(versionStrings[1]);
                subMinor = short.Parse(versionStrings[2]);
            }

            internal bool IsDifferent(Version other)
            {
                if (major != other.major) { return true; }
                if (minor != other.minor) { return true; }
                if (subMinor != other.subMinor) { return true; }
                return false;
            }

            public override string ToString()
            {
                return string.Format("{0}.{1}.{2}", major, minor, subMinor);
            }

        }
    }
}
