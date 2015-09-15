using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TerminologyLauncher.Entities.Account;
using TerminologyLauncher.Entities.InstanceManagement;
using TerminologyLauncher.GUI.Annotations;

namespace TerminologyLauncher.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public sealed partial class MainWindow : INotifyPropertyChanged
    {
        private ObservableCollection<InstanceEntity> InstanceListValue;
        private InstanceEntity SelectInstanceValue;
        private PlayerEntity PlayerValue;
        private string CoreVersionValue;


        public PlayerEntity Player
        {
            get { return this.PlayerValue; }
            set
            {
                this.PlayerValue = value;
                this.OnPropertyChanged();
            }
        }

        public String CoreVersion
        {
            get { return this.CoreVersionValue; }
            set
            {
                this.CoreVersionValue = value;
                this.OnPropertyChanged();
            }
        }

        public InstanceEntity SelectInstance
        {
            get { return this.SelectInstanceValue; }
            set
            {
                this.SelectInstanceValue = value;
                this.OnPropertyChanged();
            }
        }

        public ObservableCollection<InstanceEntity> InstanceList
        {
            get { return this.InstanceListValue; }
            set
            {
                this.InstanceListValue = value;
                this.OnPropertyChanged();
            }
        }

        public MainWindow()
        {
            this.InitializeComponent();
        }

        public void CrossThreadClose()
        {
            this.Dispatcher.Invoke(this.Close);
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


        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var instance = (InstanceEntity)e.AddedItems[0];
                //  this.BackgroundImageSource
            }
            catch (Exception)
            {
                //ignore
            }
        }

        private void InstanceAddButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void InstanceRemoveButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }

    public class ImageInMemoryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var imagePath = (String)value;
            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = new Uri(imagePath);
            image.EndInit();
            return image;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
