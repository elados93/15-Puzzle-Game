using System.Configuration;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using WpfApp.Misc;
using WpfApp.SettingsForm;

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

        private async void Solve_Button_Click(object sender, RoutedEventArgs e) {
            BlurEffect blurEffect = new BlurEffect();
           
            this.Effect = blurEffect;
            using (SolvingWindow solvingWindow = new SolvingWindow(puzzle.solve)) {
                solvingWindow.Owner = this;
                solvingWindow.ShowDialog();
            }

            this.Effect = null;
            Task solveTask = new Task(() => puzzle.solveBySolution());
            solveTask.Start();
            await solveTask;
        }

        private void Settings_Button_Click(object sender, RoutedEventArgs e) {
            Settings settingsWindow = new Settings();
            settingsWindow.Owner = this;
            settingsWindow.ShowDialog();
        }

        private void Generate_Button_Click(object sender, RoutedEventArgs e) {
            generatePuzzleFromAppConfig();
        }

    }
}
