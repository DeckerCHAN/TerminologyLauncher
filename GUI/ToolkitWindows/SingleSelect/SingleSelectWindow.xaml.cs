using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using TerminologyLauncher.GUI.Annotations;
using TerminologyLauncher.GUI.Toolkits;

namespace TerminologyLauncher.GUI.ToolkitWindows.SingleSelect
{
    /// <summary>
    /// Interaction logic for SingleSelectWindow.xaml
    /// </summary>
    public partial class SingleSelectWindow : INotifyPropertyChanged
    {
        private String FieldNameValue;
        private ObservableDictionary<string, object> SelectItemsValue;
        private Boolean IsCanceled { get; set; }
        public String FieldName
        {
            get { return this.FieldNameValue; }
            set
            {
                this.FieldNameValue = value;
                this.OnPropertyChanged();
            }
        }

        public ObservableDictionary<String, Object> SelectItems
        {
            get { return this.SelectItemsValue; }
            set
            {
                this.SelectItemsValue = value;
                this.OnPropertyChanged();
            }
        }

        public SingleSelectWindow()
        {
            this.SelectItems = new ObservableDictionary<string, object>();
            this.InitializeComponent();
            this.SelectItems.Add("Hello", "Hi");
            this.SelectItems.Add("PPP", "asdasdHi");
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
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
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
