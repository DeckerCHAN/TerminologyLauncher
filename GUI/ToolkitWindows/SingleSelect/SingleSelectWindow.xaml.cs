using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using TerminologyLauncher.GUI.Annotations;
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
        private string SelectItemValue;
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
            get { return this.SelectItemValue; }
            set { this.SelectItemValue = value; }
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

        public SingleSelectWindow(String title, String selectFieldName, IEnumerable<string> items)
        {
            this.SelectItems = new ObservableCollection<String>();
            this.Translation =
               TranslationProvider.TranslationProviderInstance.TranslationObject.GuiTranslation
                    .SingleSelectWindowTranslation;
            this.InitializeComponent();
            this.Title = title;
            this.FieldName = selectFieldName;
            this.SelectItems = new ObservableCollection<string>(items);
            this.OnPropertyChanged();
        }

        internal WindowResult ReceiveUserSelect()
        {
            base.ShowDialog();
            var result = new WindowResult()
            {
                Type = this.IsCanceled ? WindowResultType.Canceled : WindowResultType.CommonFinished
            };
            if (result.Type == WindowResultType.CommonFinished)
            {
                result.Result = this.SelectItem;
            }
            return result;
        }

        new public Boolean? ShowDialog()
        {
            throw new InvalidOperationException("Do not directly show dialog by your self. Try use toolkit functions in UiControl!");
        }

       new  public void Show()
        {
            throw new InvalidOperationException("Do not directly show by your self. Try use toolkit functions in UiControl!");
        }

        private void HeadBarPanel_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
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
