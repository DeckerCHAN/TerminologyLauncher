using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using TerminologyLauncher.GUI.Properties;
using TerminologyLauncher.GUI.Toolkits;
using TerminologyLauncher.I18n;

namespace TerminologyLauncher.GUI.ToolkitWindows.SingleLineInput
{
    /// <summary>
    /// Interaction logic for SingleLineInputWindow.xaml
    /// </summary>
    public sealed partial class SingleLineInputWindow : INotifyPropertyChanged
    {
        private FieldReference<string> InputContentValue;
        private string FieldNameValue;
        private bool IsCanceled { get; set; }

        internal SingleLineInputWindow(Window owner, string title, string inputFieldName, FieldReference<string> content)
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
            this.IsCanceled = false;
            this.OnPropertyChanged();
            this.FieldName = inputFieldName;
        }

        public object ConfirmButtonTranslation
        {
            get { return TranslationManager.GetManager.Localize("ConfirmButton", "Confirm"); }
        }
        public new bool? ShowDialog()
        {
            base.ShowDialog();
            return !this.IsCanceled;
        }


        public string InputContent
        {
            get { return this.InputContentValue.Value; }
            set
            {
                this.InputContentValue.Value = value;
                this.OnPropertyChanged();
            }
        }
        public void CrossThreadClose()
        {
            this.Dispatcher.Invoke(() =>
            {
                this.IsCanceled = true;
                this.Close();
            });
        }
        public string FieldName
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
            if (e.LeftButton == MouseButtonState.Pressed) this.DragMove();
        }
    }
}
