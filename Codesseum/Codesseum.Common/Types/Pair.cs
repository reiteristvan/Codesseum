using System.ComponentModel;

namespace Codesseum.Common.Types
{
    public class Pair : INotifyPropertyChanged
    {
        private string key = "Key";
        public string Key
        {
            get { return key; }
            set
            {
                if (key != value)
                {
                    key = value;
                    NotifyPropertyChanged("Key");
                }
            }
        }


        private double value;
        public double Value
        {
            get { return value; }
            set
            {
                if (this.value != value)
                {
                    this.value = value;
                    NotifyPropertyChanged("Value");
                }
            }
        }

        public Pair() { }
        public Pair(string key, double value)
            : this()
        {
            Key = key;
            Value = value;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
