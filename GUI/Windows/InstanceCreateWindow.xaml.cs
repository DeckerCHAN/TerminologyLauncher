﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TerminologyLauncher.Entities.InstanceManagement.FileSystem;
using TerminologyLauncher.GUI.Annotations;
using TerminologyLauncher.I18n;

namespace TerminologyLauncher.GUI.Windows
{
    /// <summary>
    /// Interaction logic for InstanceCreateWindow.xaml
    /// </summary>
    public partial class InstanceCreateWindow : Window
    {
        private ObservableCollection<FileBaseEntity> FileListValue;

        public InstanceCreateWindow()
        {
            InitializeComponent();
        }

        public ObservableCollection<FileBaseEntity> FileList

        {
            get { return this.FileListValue; }
            set
            {
                this.FileListValue = value;
                this.OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public object InstanceCreateWindowTitleTranslation => TranslationManager.GetManager.Localize("InstanceCreateWindowTitle", "Create My Instance");
        private void HeadBarPanel_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
           
            this.CrossThreadClose();
        }

        public void CrossThreadClose()
        {
            this.Dispatcher.Invoke(this.Close);
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}