using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using TerminologyLauncher.GUI.Annotations;

namespace TerminologyLauncher.GUI
{
    /// <summary>
    /// Interaction logic for PopupWindow.xaml
    /// </summary>
    public partial class PopupWindow : Window, INotifyPropertyChanged
    {
        public String content { get; set; }
        public PopupWindow(Window owner, String title, String content)
        {
            this.Owner = owner;
            this.Title = title;
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = this.PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
