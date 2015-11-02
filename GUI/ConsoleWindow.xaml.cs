using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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
using TerminologyLauncher.Configs;
using TerminologyLauncher.GUI.Annotations;

namespace TerminologyLauncher.GUI
{
    /// <summary>
    /// Interaction logic for ConsoleWindow.xaml
    /// </summary>
    public sealed partial class ConsoleWindow : INotifyPropertyChanged
    {
        private Process ProcessValue;
        public Process Process
        {
            get { return this.ProcessValue; }
            set
            {
                this.Dispatcher.Invoke(this.ProcessLogs.Clear);
                this.ProcessValue = value;
                this.ProcessValue.BeginOutputReadLine();
                this.ProcessValue.OutputDataReceived += (s, ea) =>
                {
                    if (String.IsNullOrEmpty(ea.Data)) return;
                    this.Dispatcher.InvokeAsync(() =>
                    {
                        if (this.ProcessLogs.Count >= 50) this.ProcessLogs.RemoveAt(this.ProcessLogs.Count - 1);
                        this.ProcessLogs.Insert(0, ea.Data);
                    });
                };
                this.OnPropertyChanged();
            }
        }

        private ObservableCollection<String> ProcessLogsValue;
        public ObservableCollection<String> ProcessLogs
        {
            get { return this.ProcessLogsValue; }
            private set
            {
                this.ProcessLogsValue = value;
                this.OnPropertyChanged();
            }
        }

        public String ProcessLogContent
        {
            get { return this.ProcessLogs.ToString(); }
        }

        public Config Config { get; set; }

        public ConsoleWindow(Config config)
        {
            this.Config = config;
            this.ProcessLogs = new ObservableCollection<string>();
            this.InitializeComponent();
            this.OnPropertyChanged();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = this.PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void HeadBarPanel_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) this.DragMove();
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
    }
}
