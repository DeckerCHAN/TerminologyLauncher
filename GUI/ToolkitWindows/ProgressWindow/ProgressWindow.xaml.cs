using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using TerminologyLauncher.GUI.Properties;
using TerminologyLauncher.Utils.ProgressService;

namespace TerminologyLauncher.GUI.ToolkitWindows.ProgressWindow
{
    /// <summary>
    /// Interaction logic for ProgressWindow.xaml
    /// </summary>
    public partial class ProgressWindow : INotifyPropertyChanged
    {
        private Progress ProgressValue;

        public Progress Progress
        {
            get { return this.ProgressValue; }
            set
            {
                this.ProgressValue = value;
                this.OnPropertyChanged();
            }
        }

        internal ProgressWindow(Window owner, Progress progress)
        {
            this.Progress = progress;
            this.Progress.ProgressChanged += s =>
            {
                this.OnPropertyChanged("Progress");
            };
            this.InitializeComponent();
            if (owner != null)
            {
                this.Owner = owner;
                this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
            else
            {
                this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
        }

        public void CrossThreadClose()
        {
            this.Dispatcher.Invoke(this.Close);
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = this.PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
