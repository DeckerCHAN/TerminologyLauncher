using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using TerminologyLauncher.GUI.Properties;
using TerminologyLauncher.GUI.Toolkits;
using TerminologyLauncher.I18n;
using TerminologyLauncher.I18n.TranslationObjects.GUITranslations;

namespace TerminologyLauncher.GUI.ToolkitWindows.SingleSelect
{
    /// <summary>
    /// Interaction logic for SingleSelectWindow.xaml
    /// </summary>
    public sealed partial class SingleSelectWindow : INotifyPropertyChanged
    {
        private String FieldNameValue;
        private ObservableCollection<String> SelectItemsValue;
        private FieldReference<String> SelectItemValue;
        private Boolean IsCanceled { get; set; }
        public SingleSelectWindowTranslation Translation { get; set; }
        public String FieldName
        {
            get { return this.FieldNameValue; }
            set
            {
                this.FieldNameValue = value;
                this.OnPropertyChanged();
            }
        }

        public String SelectItem
        {
            get { return this.SelectItemValue.Value; }
            set { this.SelectItemValue.Value = value; }
        }



        public ObservableCollection<String> SelectItems
        {
            get { return this.SelectItemsValue; }
            set
            {
                this.SelectItemsValue = value;
                this.OnPropertyChanged();
            }
        }

        internal SingleSelectWindow(Window owner, String title, String fieldName, IEnumerable<String> options, FieldReference<String> selection)
        {
            this.SelectItemValue = selection;
            this.SelectItems = new ObservableCollection<String>();
            this.Translation =
               TranslationProvider.TranslationProviderInstance.TranslationObject.GuiTranslation
                    .SingleSelectWindowTranslation;
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
            this.Title = title;
            this.FieldName = fieldName;
            this.SelectItems = new ObservableCollection<string>(options);
            this.OnPropertyChanged();
        }

        new public Boolean? ShowDialog()
        {
            base.ShowDialog();
            return !this.IsCanceled;
        }

        new public void Show()
        {
            throw new NotSupportedException();
        }

        private void HeadBarPanel_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) this.DragMove();
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.IsCanceled = true;
            this.Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = this.PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
