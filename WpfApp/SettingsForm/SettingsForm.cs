using System;
using System.Drawing;
using System.Windows.Forms;

namespace WpfApp.SettingsForm {
    public partial class SettingsForm : Form {
        public SettingsForm() {
            InitializeComponent();
        }

        

        private void saveBtn_Click(object sender, EventArgs e) {

        }

        private void CancelBtn_Click(object sender, EventArgs e) {

        }

        private void rowTextBox_TextChanged(object sender, EventArgs e) {
            TextBox tb = sender as TextBox;
            if (tb != null) {
                int value;
                if (!int.TryParse(tb.Text, out value)) {
                    tb.BackColor = Color.Red;
                    throw new Exception("Value Must Be 2 at least");
                }

                if (value >= 2) {
                    tb.BackColor = Color.White;
                }
            }
        }

        private void bindingNavigator1_RefreshItems(object sender, EventArgs e) {

        }

        private void SettingsForm_Load(object sender, EventArgs e) {
            this.rowTextBox.Text = WpfApp.Properties.Settings.Default["Rows"].ToString();
            this.colTextBox.Text = WpfApp.Properties.Settings.Default["Cols"].ToString();
            this.levelTrackBar.Value = int.Parse(WpfApp.Properties.Settings.Default["Level"].ToString());

        }
    }
}
