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

namespace TerminologyLauncher.GUI.ToolkitWindows.SingleSelect
{
    /// <summary>
    /// Interaction logic for SingleSelectWindow.xaml
    /// </summary>
    public sealed partial class SingleSelectWindow : INotifyPropertyChanged
    {
        private string FieldNameValue;
        private ObservableCollection<string> SelectItemsValue;
        private FieldReference<string> SelectItemValue;
        private bool IsCanceled { get; set; }

        public string FieldName
        {
            get { return this.FieldNameValue; }
            set
            {
                this.FieldNameValue = value;
                this.OnPropertyChanged();
            }
        }

        public string SelectItem
        {
            get { return this.SelectItemValue.Value; }
            set { this.SelectItemValue.Value = value; }
        }


        public ObservableCollection<string> SelectItems
        {
            get { return this.SelectItemsValue; }
            set
            {
                this.SelectItemsValue = value;
                this.OnPropertyChanged();
            }
        }

        public string ConfirmButtonTranslation => TranslationManager.GetManager.Localize("ConfirmButton", "Confirm");

        internal SingleSelectWindow(Window owner, string title, string fieldName, IEnumerable<string> options,
            FieldReference<string> selection)
        {
            this.SelectItemValue = selection;
            this.SelectItems = new ObservableCollection<string>();
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

        new public bool? ShowDialog()
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
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}