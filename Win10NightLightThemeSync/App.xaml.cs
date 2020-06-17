using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Unity;
using Win10NightLightThemeSync.Helper;
using Win10NightLightThemeSync.Models;
using Win10NightLightThemeSync.Service;
using Win10NightLightThemeSync.ViewModel;
using Application = System.Windows.Application;

namespace Win10NightLightThemeSync
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static Icon DarkIco;
        public static Icon LightIco;

        private SettingService _settingService;
        public NotifyIcon Tray;

        public static IUnityContainer Container = new UnityContainer();

        public App()
        {
            Container.RegisterSingleton<SettingService>();
            Container.RegisterSingleton<ThemeService>();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Stop Registry Monitoring
            NightLightWatcher.Stop();
            ThemeWatcher.Stop();
            // Save Configuration File
            var settingService = Container.Resolve<SettingService>();
            Task.WaitAll(settingService.SaveConfigFile());
            // Disable Tray
            Tray.Visible = false;
            // Continue App Termination
            base.OnExit(e);
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            // Preparation
            PrepIcon();
            Tray = new NotifyIcon();
            Tray.Visible = true;
            Tray.Text = "Windows 10 Night Light Theme Synchronization";
            Tray.ContextMenuStrip = new ContextMenuStrip();
            Tray.ContextMenuStrip.Items.Add("&Setting", null, TrayClickSetting);
            Tray.ContextMenuStrip.Items.Add("E&xit", null, TrayClickExit);

            // Load Configuration File
            _settingService = Container.Resolve<SettingService>();
            await _settingService.OpenConfigFile();
            // Background Services
            // -> Monitoring Service
            ThemeWatcher.Start();
            NightLightWatcher.Start();
            NightLightWatcher.WatchingStatusChanged += NightLightWatcher_WatchingStatusChanged;
            // -> Theme Service : Just constructs it as it will sync theme automatically
            var themeService = Container.Resolve<ThemeService>();
            ThemeWatcher.SystemThemeChanged += ThemeWatcher_SystemThemeChanged;
            // Tray Event
            Tray.MouseClick += _tray_MouseClick;
            SetTrayIcon(ThemeWatcher.SystemTheme);

            // Activate Window If StartMinimized is not enabled
            if (!_settingService.CurrentSetting.StartMinimized)
            {
                StartMainWindow();
            }
            else
            {
                Tray.ShowBalloonTip(5000, "The app is started.", "It has been minimized to system tray.", ToolTipIcon.Info);
            }

            // Prevent default startup behavior
            //base.OnStartup(e);
        }

        private void NightLightWatcher_WatchingStatusChanged(bool isWatching)
        {
            Tray.Visible = isWatching;
        }

        private void ThemeWatcher_SystemThemeChanged(object sender, ThemeChangedEventArg arg) => SetTrayIcon(arg.NewTheme);

        private void _tray_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ActivateWindow();
            }
        }

        private void PrepIcon()
        {
            var darkURI = new Uri("pack://application:,,,/Assets/icon_dark.ico", UriKind.RelativeOrAbsolute);
            var lightURI = new Uri("pack://application:,,,/Assets/icon_light.ico", UriKind.RelativeOrAbsolute);
            DarkIco = GenIcon(darkURI);
            LightIco = GenIcon(lightURI);
        }

        private Icon GenIcon(Uri uri)
        {
            var sri = Application.GetResourceStream(uri);
            return new Icon(sri.Stream);
        }

        private void SetTrayIcon(Theme newTheme)
        {
            Tray.Icon = newTheme == Theme.Light ? LightIco : DarkIco;
        }

        private void TrayClickSetting(object? sender, EventArgs e)
        {
            ActivateWindow();
        }
        private void ActivateWindow()
        {
            // Bring Window to foreground
            if (MainWindow != null)
            {
                MainWindow.Activate();
            }
            else
            {
                // Activate Windows
                StartMainWindow();
            }
        }

        private void TrayClickExit(object? sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private static void StartMainWindow()
        {
            // Activate Windows
            var window = Container.Resolve<MainWindow>();
            window.Show();
        }

    }


}
