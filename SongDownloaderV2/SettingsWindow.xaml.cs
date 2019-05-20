using MahApps.Metro;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace SongDownloaderV2
{
    /// <summary>
    /// Logika interakcji dla klasy ThemeEditorWindow.xaml
    /// </summary>
    public partial class SettingsWindow : MetroWindow
    {
        public static readonly List<string> AccentColors = new List<string>()
        {
            "Red",
            "Green",
            "Blue",
            "Purple",
            "Orange",
            "Lime",
            "Emerald",
            "Teal",
            "Cyan",
            "Cobalt",
            "Indigo",
            "Violet",
            "Pink",
            "Magenta",
            "Crimson",
            "Amber",
            "Yellow",
            "Brown",
            "Olive",
            "Steel",
            "Mauve",
            "Taupe",
            "Sienna"
        };
        private Tuple<AppTheme, Accent> _prevTheme;

        public SettingsWindow()
        {
            InitializeComponent();

            Combo_AccentColor.ItemsSource = AccentColors;
            _prevTheme = ThemeManager.DetectAppStyle(Application.Current);
        }

        private void Combo_AccentColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var theme = ThemeManager.DetectAppStyle(Application.Current);
            ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent((sender as ComboBox).SelectedValue.ToString()), theme.Item1);
        }

        private void Button_BGColor_Click(object sender, RoutedEventArgs e)
        {
            var theme = ThemeManager.DetectAppStyle(this);
            ThemeManager.ChangeAppStyle(Application.Current, theme.Item2, ThemeManager.GetAppTheme((sender as Button).Tag.ToString()));
        }

        private void Button_SaveTheme_Click(object sender, RoutedEventArgs e)
        {
            var theme = ThemeManager.DetectAppStyle(this);
            ThemeManager.ChangeAppStyle(Application.Current, theme.Item2, theme.Item1);
            this.Close();
        }

        private void Button_DiscardAndClose_Click(object sender, RoutedEventArgs e)
        {
            ThemeManager.ChangeAppStyle(Application.Current, _prevTheme.Item2, _prevTheme.Item1);
            this.Close();
        }

        private void Button_RestoreDefaults_Click(object sender, RoutedEventArgs e)
        {
            ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent("Blue"), ThemeManager.GetAppTheme("BaseLight"));
            Combo_AccentColor.SelectedIndex = Combo_AccentColor.Items.IndexOf("Blue");
        }
    }
}
