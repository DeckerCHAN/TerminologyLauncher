using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using TerminologyLauncher.GUI.Annotations;
using TerminologyLauncher.GUI.ToolkitWindows.ConfigWindow.ConfigObjects;

namespace TerminologyLauncher.GUI.ToolkitWindows.ConfigWindow
{
    /// <summary>
    /// Interaction logic for ConfigWindow.xaml
    /// </summary>
    public sealed partial class ConfigWindow : INotifyPropertyChanged
    {
        private ObservableCollection<TextInputConfigObject> TextInputConfigObjectsValue;
        private bool IsCanceled { get; set; }

        public ObservableCollection<TextInputConfigObject> TextInputConfigObjects
        {
            get { return this.TextInputConfigObjectsValue; }
            set
            {
                this.TextInputConfigObjectsValue = value; 
                this.OnPropertyChanged();
            }
        }

        public WindowResult ReceiveUserConfigs()
        {
            base.ShowDialog();
            var result = new WindowResult()
            {
                Type = this.IsCanceled ? WindowResultType.Canceled : WindowResultType.CommonFinished
            };
            if (result.Type == WindowResultType.CommonFinished)
            {
                result.Result = new List<ConfigObject>(this.TextInputConfigObjects);
            }
            return result;
        }

        public ConfigWindow(IEnumerable<TextInputConfigObject> textInputConfigs)
        {

            this.InitializeComponent();
            this.TextInputConfigObjects = new ObservableCollection<TextInputConfigObject>(textInputConfigs);
            this.OnPropertyChanged();
        }

        public void CrossThreadClose()
        {
            this.Dispatcher.Invoke(this.Close);
        }

        public new Boolean? ShowDialog()
        {
            throw new InvalidOperationException();
        }

        public new void Show()
        {
            throw new InvalidOperationException();
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
            this.DragMove();
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.IsCanceled = true;
            this.CrossThreadClose();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            this.IsCanceled = false;
            this.CrossThreadClose();
        }
    }
}
