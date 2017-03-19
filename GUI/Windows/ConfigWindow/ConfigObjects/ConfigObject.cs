using System.ComponentModel;
using System.Runtime.CompilerServices;
using TerminologyLauncher.GUI.Properties;

namespace TerminologyLauncher.GUI.Windows.ConfigWindow.ConfigObjects
{
    public abstract class ConfigObject : INotifyPropertyChanged
    {
        protected ConfigObject(string name, string key)
        {
            this.Name = name;
            this.Key = key;
        }

        private string RealKey;
        private string RealName;

        public string Key
        {
            get { return this.RealKey; }
            protected set
            {
                this.RealKey = value;
                this.OnPropertyChanged();
            }
        }

        public string Name
        {
            get { return this.RealName; }
            set
            {
                this.RealName = value;
                this.OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}