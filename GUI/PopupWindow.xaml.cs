using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using TerminologyLauncher.GUI.Annotations;

namespace TerminologyLauncher.GUI
{
    /// <summary>
    /// Interaction logic for PopupWindow.xaml
    /// </summary>
    public sealed partial class PopupWindow : INotifyPropertyChanged
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

        public PopupWindow(Window owner, String title, String content)
        {
            this.Owner = owner;
            this.ContentString = content;
            InitializeComponent();
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
            this.DragMove();
        }

        private void ConfirmButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
