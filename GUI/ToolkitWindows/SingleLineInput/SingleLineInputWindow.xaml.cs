using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using TerminologyLauncher.GUI.Annotations;

namespace TerminologyLauncher.GUI.ToolkitWindows.SingleLineInput
{
    /// <summary>
    /// Interaction logic for SingleLineInputWindow.xaml
    /// </summary>
    public sealed partial class SingleLineInputWindow : Window, INotifyPropertyChanged
    {
        private string InputContentValue;
        private string FieldNameValue;
        private Boolean IsCanceled { get; set; }

        public SingleLineInputWindow(String title, String inputFieldName)
        {
            InitializeComponent();
            this.Title = title;
            this.OnPropertyChanged();
            this.FieldName = inputFieldName;
        }

        public new Boolean? ShowDialog()
        {
            throw new InvalidOperationException();
        }

        public WindowResult ReceiveUserInput()
        {
            base.ShowDialog();
            var result = new WindowResult()
            {
                Type = this.IsCanceled ? WindowResultType.Canceled : WindowResultType.CommonFinished
            };
            if (result.Type == WindowResultType.CommonFinished)
            {
                result.InputLine = this.InputContent;
            }
            return result;
        }


        public String InputContent
        {
            get { return this.InputContentValue; }
            set
            {
                this.InputContentValue = value;
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
            this.DragMove();
        }
    }
}
