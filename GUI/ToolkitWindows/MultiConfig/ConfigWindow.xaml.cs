using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using TerminologyLauncher.GUI.Annotations;

namespace TerminologyLauncher.GUI.ToolkitWindows.MultiConfig
{
    /// <summary>
    /// Interaction logic for ConfigWindow.xaml
    /// </summary>
    public sealed partial class ConfigWindow : INotifyPropertyChanged
    {
        private bool IsCanceled { get; set; }

        public Dictionary<String, String> Configs
        {
            get
            {
                return this.ConfigObservableCollection.ToDictionary(frontSideKvPire => frontSideKvPire.Key, frontSideKvPire => frontSideKvPire.Value);
            }
            set
            {
                this.ConfigObservableCollection = new ObservableCollection<FrontSideKvPire>();
                foreach (var config in value)
                {
                    this.ConfigObservableCollection.Add(new FrontSideKvPire { Key = config.Key, Value = config.Value });
                }
            }
        }
        private ObservableCollection<FrontSideKvPire> ConfigsConfigObservableCollectionValue;

        public ObservableCollection<FrontSideKvPire> ConfigObservableCollection
        {
            get { return this.ConfigsConfigObservableCollectionValue ?? (this.ConfigsConfigObservableCollectionValue = new ObservableCollection<FrontSideKvPire>()); }
            set
            {
                this.ConfigsConfigObservableCollectionValue = value;
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
                result.Result = this.Configs;
            }
            return result;
        }

        public ConfigWindow(String title, Dictionary<String, String> configs)
        {

            InitializeComponent();
            this.Title = title;
            this.Configs = configs;
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

    public class FrontSideKvPire : INotifyPropertyChanged
    {
        private string KeyV;
        private string ValueV;

        public String Key
        {
            get { return this.KeyV; }
            set
            {
                this.KeyV = value;
                this.OnPropertyChanged();
            }
        }

        public String Value
        {
            get { return this.ValueV; }
            set
            {
                this.ValueV = value;
                this.OnPropertyChanged();
            }
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
