using System.Windows;
using System.Configuration;
using System;

namespace WpfApp {
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window {
        public Settings() {
            InitializeComponent();
            ResizeMode = ResizeMode.NoResize;

            setValuesFromAppConfig();
        }

        private void setValuesFromAppConfig() {
            rowTextBox.Text = ConfigurationManager.AppSettings["Rows"];
            colTextBox.Text = ConfigurationManager.AppSettings["Cols"];
            difficulty.Text = ConfigurationManager.AppSettings["Level"];
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e) {
            string rows = rowTextBox.Text.ToString();
            string cols = colTextBox.Text.ToString();
            string level = difficulty.Text.ToString();

            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings.Remove("Rows");
            config.AppSettings.Settings.Add("Rows", rows);

            config.AppSettings.Settings.Remove("Cols");
            config.AppSettings.Settings.Add("Cols", cols);

            config.AppSettings.Settings.Remove("Level");
            config.AppSettings.Settings.Add("Level", level);

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");

            this.Close();
        }
    }
}
