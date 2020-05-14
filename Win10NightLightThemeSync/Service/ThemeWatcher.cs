using System;
using System.Management;
using System.Security.Principal;
using Microsoft.Win32;
using Win10NightLightThemeSync.Models;

namespace Win10NightLightThemeSync.Helper
{
    public delegate void ThemeChangedHandler(Theme newTheme);

    public class ThemeWatcher
    {
        private readonly ManagementEventWatcher _systemWatcher;
        private readonly ManagementEventWatcher _appWatcher;
        protected static ThemeWatcher Instance { get; set; }

        private const string ThemeKeyForWMI =
            @"\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize";

        private const string ThemeKeyPath =
            @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize";

        private const string SystemValueName = "SystemUsesLightTheme";
        private const string AppValueName = "AppsUseLightTheme";

        public static Theme SystemTheme => GetTheme(ThemeType.System);
        public static Theme AppTheme => GetTheme(ThemeType.App);

        protected ThemeWatcher()
        {
            WindowsIdentity currentUser = WindowsIdentity.GetCurrent();
            WqlEventQuery systemQuery = new WqlEventQuery(
                "SELECT * FROM RegistryValueChangeEvent WHERE " +
                "Hive = 'HKEY_USERs'" +
                $"AND KeyPath = '{currentUser.User.Value}{ThemeKeyForWMI}' AND ValueName='{SystemValueName}'");
            _systemWatcher = new ManagementEventWatcher(systemQuery);
 
            WqlEventQuery appQuery = new WqlEventQuery(
                "SELECT * FROM RegistryValueChangeEvent WHERE " +
                "Hive = 'HKEY_USERs'" +
                $"AND KeyPath = '{currentUser.User.Value}{ThemeKeyForWMI}' AND ValueName='{AppValueName}'");
            _appWatcher = new ManagementEventWatcher(appQuery);

        }

        private void _appWatcher_EventArrived(object sender, EventArrivedEventArgs e)
            => AppThemeChanged?.Invoke(GetTheme(ThemeType.App));

        private void _systemWatcher_EventArrived(object sender, EventArrivedEventArgs e)
            => SystemThemeChanged?.Invoke(GetTheme(ThemeType.System));

        private static Theme GetTheme(ThemeType type)
        {
            var value = Registry.GetValue(ThemeKeyPath, type == ThemeType.System ? SystemValueName : AppValueName, 1) as int?;
            return value == 1 ? Theme.Light : Theme.Dark;
        }
        private enum ThemeType
        {
            System,
            App
        }

        static ThemeWatcher()
        {
            ThemeWatcher.Instance = new ThemeWatcher();
        }

        public static void Start()
        {
            ThemeWatcher.Instance.StartInternal();
        }

        public static void Stop()
        {
            ThemeWatcher.Instance.StopInternal();
        }

        public static event ThemeChangedHandler SystemThemeChanged;
        public static event ThemeChangedHandler AppThemeChanged;

        private void StartInternal()
        {
            _systemWatcher.EventArrived += _systemWatcher_EventArrived;
            _systemWatcher.Start();
            _appWatcher.EventArrived += _appWatcher_EventArrived;
            _appWatcher.Start();
        }

        private void StopInternal()
        {
            _systemWatcher.EventArrived -= _systemWatcher_EventArrived;
            _systemWatcher?.Stop();
            _appWatcher.EventArrived -= _appWatcher_EventArrived;
            _appWatcher?.Stop();
        }
    }
}