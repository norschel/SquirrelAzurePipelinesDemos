using Squirrel;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;

namespace SquirrelApp
{
    public class Updater : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _updateUrl = SettingsApp.Default.StableUpdateUrl;
        public string UpdateUrl
        {
            get
            {
                return _updateUrl;
            }
            set
            {
                if (value != _updateUrl)
                {
                    _updateUrl = value;
                    NotifyPropertyChanged("UpdateUrl");
                }
            }
        }

        private string _channel = "Stable";
        public string Channel
        {
            get
            {
                return _channel;
            }
            set
            {
                if (value != _channel)
                {
                    _channel = value;
                    SetChannelUrl(_channel);
                    NotifyPropertyChanged("Channel");
                }
            }
        }

        private Visibility _restartButtonVisibility = Visibility.Hidden;
        public Visibility RestartButtonVisibility
        {
            get
            {
                return _restartButtonVisibility;
            }
            private set
            {
                if (value != _restartButtonVisibility)
                {
                    _restartButtonVisibility = value;
                    NotifyPropertyChanged("RestartButtonVisibility");
                }
            }
        }

        public Visibility UpdateButtonVisible
        {
            get
            {
                using (var mgr = new UpdateManager(UpdateUrl))
                {
                    //var result = mgr.CheckForUpdate();

                    if (mgr.CurrentlyInstalledVersion() != null)
                    {
                        mgr.Dispose();
                        return Visibility.Visible;
                    }
                    mgr.Dispose();
                }


                return Visibility.Hidden;
            }
        }

        private void SetChannelUrl(string channel)
        {
            switch (channel)
            {
                case "Unstable":
                    UpdateUrl = SettingsApp.Default.UnstableUpdateUrl;
                    break;
                case "Stable":
                    UpdateUrl = SettingsApp.Default.StableUpdateUrl;
                    break;
                default:
                    UpdateUrl = SettingsApp.Default.UnstableUpdateUrl;
                    break;
            }
        }

        public async void UpdateApp()
        {
            try
            {
                using (var mgr = new UpdateManager(UpdateUrl))
                {
                    var result = await mgr.CheckForUpdate();
                   
                    // Check if app is managed by Squirrel
                    if (mgr.CurrentlyInstalledVersion() == null)
                    {
                        mgr.Dispose();
                        MessageBox.Show("App not managed by Squirrel.");
                        return;
                    }
                    else
                    {
                        MessageBox.Show("App managed by Squirrel");
                    }

                    Trace.WriteLine("Before check. Update-Url:" + UpdateUrl);
                    if (result.ReleasesToApply.Any())
                    {
                        #region prepare release notes
                        Trace.WriteLine($"Update available. Update-Url: {UpdateUrl}");
                        await mgr.DownloadReleases(result.ReleasesToApply);
                        Trace.WriteLine("Download completed.");

                        var futureReleaseNotes = result.FutureReleaseEntry.GetReleaseNotes(result.PackageDirectory);
                        RegexOptions options = RegexOptions.None;
                        Regex regex = new Regex(@"\<\!\[CDATA\[(?<text>[^\]]*)\]\]\>", options);

                        // Check for match
                        bool isMatch = regex.IsMatch(futureReleaseNotes);
                        string releaseNotesText = string.Empty;
                        if (isMatch)
                        {
                            Match match = regex.Match(futureReleaseNotes);
                            releaseNotesText = match.Groups["text"].Value;
                            releaseNotesText = StripHTML(releaseNotesText);
                        }
                        #endregion

                        MessageBox.Show(
                            $"Current version {result.CurrentlyInstalledVersion.Version.Version}. New version available {result.FutureReleaseEntry.Version.Version}. Release Notes: {releaseNotesText}");

                        var release = mgr.UpdateApp(UpdateProgress);

                        mgr.Dispose();

                        RestartButtonVisibility = Visibility.Visible;
                    }
                    else
                    {
                        MessageBox.Show("No updates found.");
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"{ex.Message}");
                Trace.WriteLine($"{ex.StackTrace}");
            }
        }
        private void UpdateProgress(int i)
        {
            if (i == 100)
            {
                MessageBox.Show("Update finished. Restart your app.");
            }
        }

        public void RestartApp()
        {
            UpdateManager.RestartApp();
        }

        #region helper methods
        public static string StripHTML(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}