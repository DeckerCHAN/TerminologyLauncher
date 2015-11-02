﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using TerminologyLauncher.GUI.Annotations;
using TerminologyLauncher.I18n.TranslationObjects.GUITranslations;

namespace TerminologyLauncher.GUI.ToolkitWindows.PopupWindow
{
    /// <summary>
    /// Interaction logic for NotifyWindow.xaml
    /// </summary>
    public sealed partial class NotifyWindow : INotifyPropertyChanged
    {
        private string ContentStringValue;
        public NotifyWindowTranslation Translation
        {
            get
            {
                return
                    I18n.TranslationProvider.TranslationProviderInstance.TranslationObject.GuiTranslation
                        .NotifyWindowTranslation;
            }
        }
        public String ContentString
        {
            get { return this.ContentStringValue; }
            set
            {
                this.ContentStringValue = value;
                this.OnPropertyChanged();
            }
        }

        public void CrossThreadClose()
        {
            this.Dispatcher.Invoke(this.Close);
        }

        public NotifyWindow(Window owner, String title, String content)
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