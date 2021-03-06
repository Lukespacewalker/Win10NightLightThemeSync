﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Unity;
using Win10NightLightThemeSync.Helper;
using Win10NightLightThemeSync.Models;
using Win10NightLightThemeSync.Service;
using Win10NightLightThemeSync.ViewModel;
using Application = System.Windows.Application;

namespace Win10NightLightThemeSync
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow(MainWindowViewModel viewModel)
        {
            DataContext = viewModel;
            // Init
            InitializeComponent();
            // Set Icon
            SetAppIcon(ThemeWatcher.AppTheme);
            SetTaskBarIcon(ThemeWatcher.SystemTheme);
            // Event
            WeakEventManager<ThemeWatcher, ThemeChangedEventArg>.AddHandler(null, "SystemThemeChanged", ThemeWatcherOnSystemThemeChanged);
            WeakEventManager<ThemeWatcher, ThemeChangedEventArg>.AddHandler(null, "AppThemeChanged", ThemeWatcherOnAppThemeChanged);
            this.StateChanged += MainWindow_StateChanged;
        }

        private void ThemeWatcherOnAppThemeChanged(object sender, ThemeChangedEventArg arg) => SetAppIcon(arg.NewTheme);
        private void ThemeWatcherOnSystemThemeChanged(object sender, ThemeChangedEventArg arg) => SetTaskBarIcon(arg.NewTheme);

        private void SetAppIcon(Theme newTheme)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
            {
                this.AppIcon.Source = (BitmapImage)(newTheme == Theme.Light ? FindResource("LightIcon") : FindResource("DarkIcon"));
            }));
           
        }

        private void SetTaskBarIcon(Theme newTheme)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                {
                    this.Icon = (BitmapImage)(newTheme == Theme.Light ? FindResource("LightIcon") : FindResource("DarkIcon"));
                }));
        }

        protected override void OnActivated(EventArgs e)
        {
            //base.OnActivated(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!NightLightWatcher.IsWatching)
            {
                // If not monitoring, Shutdown app instead
                Application.Current.Shutdown();
            }
            else
            {
                (App.Current as App).Tray.ShowBalloonTip(5000,"The app is still running." , "It has been minimized to system tray.", ToolTipIcon.Info);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            // Dispose Context
            // (DataContext as MainWindowViewModel)?.Dispose();
            DataContext = null;
            GC.Collect(2, GCCollectionMode.Forced,true);
            GC.WaitForPendingFinalizers();
            GC.WaitForFullGCComplete();
            GC.Collect();
        }

        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized && NightLightWatcher.IsWatching)
            {
                this.Close();
            }
        }

    }
}
