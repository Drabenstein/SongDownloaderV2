using MahApps.Metro.Controls;
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WinForms = System.Windows.Forms;

namespace SongDownloaderV2
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private string _targetPath;

        public MainWindow()
        {
            InitializeComponent();

            ((INotifyCollectionChanged)ListBox_Log.Items).CollectionChanged += ListBox_Log_OnCollectionChanged;
        }

        private void BTN_StartStop_Click(object sender, RoutedEventArgs e)
        {
            BTN_StartStop.IsEnabled = false;
            PB_Progress.Visibility = Visibility.Visible;
            int qual = Convert.ToInt32((Combo_Quality.SelectedValue as ComboBoxItem).Content.ToString());
            DataStore data1 = new DataStore(TB_API.Text, TB_Target.Text, qual);
            Task.Run(() => PerformTasks(data1));

            async Task PerformTasks(DataStore data)
            {
                await Task.Delay(50);
                string tempPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SongDownloaderV2");
                FileInfo dlAndConv = new FileInfo(
                    System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DlAndConv.exe"));

                if (!File.Exists(dlAndConv.FullName))
                {
                    MessageBox.Show($"Nie znaleziono pliku: {dlAndConv.Name}", "Nie znaleziono plików!",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Close();
                }

                ProcessStartInfo dlAndConvInfo = new ProcessStartInfo();
                dlAndConvInfo.UseShellExecute = false;
                dlAndConvInfo.CreateNoWindow = true;
                dlAndConvInfo.WindowStyle = ProcessWindowStyle.Hidden;
                dlAndConvInfo.RedirectStandardOutput = true;
                dlAndConvInfo.FileName = dlAndConv.FullName;
                dlAndConvInfo.Arguments = $"{data.ApiKey} {tempPath} {data.TargetPath} {data.Quality}";
                var proc1 = Process.Start(dlAndConvInfo);

                while (!proc1.StandardOutput.EndOfStream)
                {
                    ListBox_Log.Items.Add(proc1.StandardOutput.ReadLine());
                }

                proc1.WaitForExit();

                FileInfo convertSongNames = new FileInfo(
                    System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ConvertSongNames.exe"));

                if (!File.Exists(convertSongNames.FullName))
                {
                    MessageBox.Show($"Nie znaleziono pliku: {convertSongNames.Name}", "Nie znaleziono plików!",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Close();
                }

                ProcessStartInfo convertSongNamesInfo = new ProcessStartInfo();
                convertSongNamesInfo.UseShellExecute = false;
                convertSongNamesInfo.CreateNoWindow = false;
                convertSongNamesInfo.WindowStyle = ProcessWindowStyle.Hidden;
                convertSongNamesInfo.RedirectStandardOutput = true;
                convertSongNamesInfo.FileName = convertSongNames.FullName;
                convertSongNamesInfo.Arguments = data.TargetPath;
                var proc2 = Process.Start(convertSongNamesInfo);

                while (!proc2.StandardOutput.EndOfStream)
                {
                    ListBox_Log.Items.Add(proc2.StandardOutput.ReadLine());
                }

                proc2.WaitForExit();

                FileInfo MakeID3Tags = new FileInfo(
                    System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MakeID3Tags.exe"));

                if (!File.Exists(MakeID3Tags.FullName))
                {
                    MessageBox.Show($"Nie znaleziono pliku: {MakeID3Tags.Name}", "Nie znaleziono plików!",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Close();
                }

                ProcessStartInfo makeId3TagsInfo = new ProcessStartInfo();
                makeId3TagsInfo.UseShellExecute = false;
                makeId3TagsInfo.CreateNoWindow = false;
                makeId3TagsInfo.WindowStyle = ProcessWindowStyle.Hidden;
                makeId3TagsInfo.RedirectStandardOutput = true;
                makeId3TagsInfo.FileName = MakeID3Tags.FullName;
                makeId3TagsInfo.Arguments = data.TargetPath;
                var proc3 = Process.Start(makeId3TagsInfo);

                while (!proc3.StandardOutput.EndOfStream)
                {
                    ListBox_Log.Items.Add(proc3.StandardOutput.ReadLine());
                }

                proc3.WaitForExit();

                PB_Progress.Visibility = Visibility.Hidden;
                BTN_StartStop.IsEnabled = true;

                ListBox_Log.Items.Add("Zakończono pobieranie, konwersję i dodawanie tagów ID3");
                ListBox_Log.Items.Add("Miłego korzystania z audio :)");
            }
        }

        private void BTN_Browse_Click(object sender, RoutedEventArgs e)
        {
            WinForms.FolderBrowserDialog dialog = new WinForms.FolderBrowserDialog();
            if (dialog.ShowDialog() == WinForms.DialogResult.OK && dialog.SelectedPath.Length > 0)
            {
                _targetPath = dialog.SelectedPath;
                TB_Target.Tag = "OK";
                TB_Target.Text = dialog.SelectedPath;
            }
            dialog.Dispose();
        }

        private void ListBox_Log_OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Autoscroll
            ListBox_Log.ScrollIntoView(ListBox_Log.Items[ListBox_Log.Items.Count - 1]);
        }

        private void Button_Settings_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settings = new SettingsWindow();
            settings.ShowDialog();
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_AuthorInformation_Click(object sender, RoutedEventArgs e)
        {
            AuthorInfoWindow author = new AuthorInfoWindow();
            author.ShowDialog();
        }

        private void Button_Minimize_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
           
            foreach (var window in Application.Current.Windows)
            {
                (window as Window).WindowState = WindowState.Minimized;
            }
        }

        private void Button_Maximize_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow.WindowState != WindowState.Maximized)
            {
                Application.Current.MainWindow.WindowState = WindowState.Maximized;
            }
            else
            {
                Application.Current.MainWindow.WindowState = WindowState.Normal;
            }

        }
    }
}
