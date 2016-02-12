using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using TerminologyLauncher.GUI.Properties;
using TerminologyLauncher.I18n;

namespace TerminologyLauncher.GUI.ToolkitWindows.NotifyWindow
{
    /// <summary>
    /// Interaction logic for NotifyWindow.xaml
    /// </summary>
    public sealed partial class NotifyWindow : INotifyPropertyChanged
    {
        private string ContentStringValue;
  
        public String ContentString
        {
            get { return this.ContentStringValue; }
            set
            {
                this.ContentStringValue = value;
                this.OnPropertyChanged();
            }
        }

        public object ConfirmButtonTranslation
        {
            get { return TranslationManager.GetManager.Localize("ConfirmButton", "Confirm"); }
        }

        public void CrossThreadClose()
        {
            this.Dispatcher.Invoke(this.Close);
        }

        internal NotifyWindow(Window owner, String title, String content)
        {
            this.Owner = owner;
            this.ContentString = content;
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
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = this.PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) this.DragMove();
        }

        private void ConfirmButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
