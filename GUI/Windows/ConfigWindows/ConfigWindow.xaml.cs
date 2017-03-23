using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using TerminologyLauncher.GUI.Properties;
using TerminologyLauncher.GUI.Windows.ConfigWindows.ConfigObjects;
using TerminologyLauncher.I18n;

namespace TerminologyLauncher.GUI.Windows.ConfigWindows
{
    /// <summary>
    /// Interaction logic for ConfigWindow.xaml
    /// </summary>
    public sealed partial class ConfigWindow : INotifyPropertyChanged
    {
        private ObservableCollection<TextInputConfigObject> TextInputConfigObjectsValue;
        private ObservableCollection<ItemSelectConfigObject> ItemSelectConfigObjectsValue;
        private ObservableCollection<RangeRestrictedSelectConfigObject> RangeRestrictedSelectObjectsValue;

        public ObservableCollection<RangeRestrictedSelectConfigObject> RangeRestrictedSelectObjects
        {
            get { return this.RangeRestrictedSelectObjectsValue; }
            set
            {
                this.RangeRestrictedSelectObjectsValue = value;
                this.OnPropertyChanged();
            }
        }

        private bool IsCanceled { get; set; }

        public string TitileTranslation => TranslationManager.GetManager.Localize("Titile", "Configs");
        public string ConfirmButtionTranslation => TranslationManager.GetManager.Localize("ConfirmButton", "Confirm");


        public ObservableCollection<ItemSelectConfigObject> ItemSelectConfigObjects
        {
            get { return this.ItemSelectConfigObjectsValue; }
            set
            {
                this.ItemSelectConfigObjectsValue = value;
                this.OnPropertyChanged();
            }
        }

        public ObservableCollection<TextInputConfigObject> TextInputConfigObjects
        {
            get { return this.TextInputConfigObjectsValue; }
            set
            {
                this.TextInputConfigObjectsValue = value;
                this.OnPropertyChanged();
            }
        }


        internal ConfigWindow(IEnumerable<TextInputConfigObject> textInputConfigs,
            IEnumerable<ItemSelectConfigObject> itemSelectConfigs,
            IEnumerable<RangeRestrictedSelectConfigObject> rangeRestrictedSelectConfigs)
        {
            this.InitializeComponent();
            this.TextInputConfigObjects =
                new ObservableCollection<TextInputConfigObject>(textInputConfigs ?? new List<TextInputConfigObject>());
            this.RangeRestrictedSelectObjects =
                new ObservableCollection<RangeRestrictedSelectConfigObject>(rangeRestrictedSelectConfigs ??
                                                                            new List<RangeRestrictedSelectConfigObject>());
            this.ItemSelectConfigObjects =
                new ObservableCollection<ItemSelectConfigObject>(itemSelectConfigs ?? new List<ItemSelectConfigObject>());
            this.OnPropertyChanged();
        }

        public void CrossThreadClose()
        {
            this.Dispatcher.Invoke(this.Close);
        }

        public new bool? ShowDialog()
        {
            base.ShowDialog();
            return !this.IsCanceled;
        }

        public new void Show()
        {
            throw new InvalidOperationException();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void HeadBarPanel_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) this.DragMove();
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