using System;
using System.ComponentModel;
using System.Windows.Media;

namespace WpfApp.Model {
    public class Tile : INotifyPropertyChanged, IComparable<Tile> {

        #region Members
        private string value;
        private SolidColorBrush backgroundColor;
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        public Tile(string v) {
            this.Value = v;
            if (v.Equals(" "))
                this.BackgroundColor = Brushes.Gray;
            else
                this.BackgroundColor = Brushes.White;
        }

        public Tile(Tile tile) {
            this.value = tile.value;
            this.backgroundColor = tile.backgroundColor;
        }

        #region Properties
        public string Value {
            get {
                return value;
            }
            set {
                if (value != this.value) {
                    this.value = value;
                    NotifyPropertyChanged("Value");
                }
            }
        }

        public SolidColorBrush BackgroundColor {
            get {
                return this.backgroundColor;
            }

            set {
                if (value != this.backgroundColor) {
                    this.backgroundColor = value;
                    NotifyPropertyChanged("BackgroundColor");
                }
            }
        }
        #endregion

        public void copyValues(Tile tile) {
            this.Value = tile.Value;
            this.BackgroundColor = tile.BackgroundColor;
        }

        public bool isSpace() {
            return this.Value.Equals(" ");
        }

        private void NotifyPropertyChanged(string v) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(v));
        }

        public int CompareTo(Tile other) {
            return this.value.CompareTo(other.value);
        }
    }
}
