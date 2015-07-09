using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TerminologyLauncher.GUI.Annotations;

namespace TerminologyLauncher.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public sealed partial class MainWindow : INotifyPropertyChanged
    {
        private ImageSource BackgroundImageSourceValue;
        private ImageSource PlayerAvatarImageSourceValue;

        public ImageSource BackgroundImageSource
        {
            get { return this.BackgroundImageSourceValue; }
            set
            {
                this.BackgroundImageSourceValue = value;
                this.OnPropertyChanged();
            }
        }

        public ImageSource PlayerAvatarImageSource
        {
            get { return this.PlayerAvatarImageSourceValue; }
            set
            {
                this.PlayerAvatarImageSourceValue = value;
                this.OnPropertyChanged();
            }
        }

        public MainWindow()
        {
            this.InitializeComponent();
            this.PlayerAvatarImageSource = new BitmapImage(new Uri(@"pack://application:,,,/TerminologyLauncher.GUI;component/Resources/default_avatar.png"));
            this.BackgroundImageSource = new BitmapImage(new Uri(@"pack://application:,,,/TerminologyLauncher.GUI;component/Resources/login_bg.jpg"));
        }

        private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = this.PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
