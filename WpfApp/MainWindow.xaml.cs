using System;
using System.ComponentModel;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Animation;
using WpfApp.Misc;

namespace WpfApp {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        public MainWindow() {
            InitializeComponent();
            this.DataContext = puzzle.DataContext;
            generatePuzzleFromAppConfig();
        }

        private void generatePuzzleFromAppConfig() {
            int rows = int.Parse(ConfigurationManager.AppSettings["Rows"]);
            int cols = int.Parse(ConfigurationManager.AppSettings["Cols"]);
            string level = ConfigurationManager.AppSettings["Level"];

            PuzzleArgs args = new PuzzleArgs(rows, cols, LevelClass.getLevelFromString(level));
            puzzle.createGrid(args);
        }

        private void Solve_Button_Click(object sender, RoutedEventArgs e) {
            puzzle.solve();
        }

        private void Settings_Button_Click(object sender, RoutedEventArgs e) {
            Settings settingsWindow = new Settings();
            settingsWindow.ShowDialog();
        }

        private void Generate_Button_Click(object sender, RoutedEventArgs e) {
            generatePuzzleFromAppConfig();
        }

    }
}
