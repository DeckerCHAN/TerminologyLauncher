using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using TerminologyLauncher.GUI.Annotations;
using TerminologyLauncher.GUI.Toolkits;

namespace TerminologyLauncher.GUI.ToolkitWindows.SingleLineInput
{
    /// <summary>
    /// Interaction logic for SingleLineInputWindow.xaml
    /// </summary>
    public sealed partial class SingleLineInputWindow : INotifyPropertyChanged
    {
        private FieldReference<String> InputContentValue;
        private string FieldNameValue;
        private Boolean IsCanceled { get; set; }

        public SingleLineInputWindow(Window owner, String title, String inputFieldName, FieldReference<String> content)
        {
            this.InputContentValue = content;
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
            this.OnPropertyChanged();
            this.FieldName = inputFieldName;
        }

        public new Boolean? ShowDialog()
        {
            base.ShowDialog();
            return !this.IsCanceled;
        }


        public String InputContent
        {
            get { return this.InputContentValue.Value; }
            set
            {
                this.InputContentValue.Value = value;
                this.OnPropertyChanged();
            }
        }

        public String FieldName
        {
            get { return this.FieldNameValue; }
            set
            {
                this.FieldNameValue = value;
                this.OnPropertyChanged();
            }
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


        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.IsCanceled = true;
            this.Close();
        }

        private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) this.DragMove();
        }
    }
}
