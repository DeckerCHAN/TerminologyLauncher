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
        private double ProgressValue;

        public Double Progress
        {
            get { return this.ProgressValue; }
            set
            {
                if (value >= 100D)
                {
                    this.Dispatcher.Invoke(this.Close);
                }
                this.ProgressValue = value;
                this.OnPropertyChanged();
            }
        }

        private Progress ProgressObj { get; set; }

        public ProgressWindow(Progress progress)
        {
            this.ProgressObj = progress;
            progress.ProgressChanged += this.progress_ProgressChanged;
            this.InitializeComponent();
        }

        public void CrossThreadClose()
        {
            this.Dispatcher.Invoke(this.Close);
        }

        void progress_ProgressChanged(object sender)
        {
            this.Progress = this.ProgressObj.Percent;
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
