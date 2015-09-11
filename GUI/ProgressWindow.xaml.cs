using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TerminologyLauncher.GUI.Annotations;
using TerminologyLauncher.Utils.ProgressService;

namespace TerminologyLauncher.GUI
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

        public ProgressWindow(Progress progress)
        {
            this.Progress = progress;
            this.Progress.ProgressChanged += s =>
            {
                this.OnPropertyChanged("Progress");
            };
            this.InitializeComponent();
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
