using Microsoft.Win32;
using Win10NightLightThemeSync.Helper;
using Win10NightLightThemeSync.Models;

namespace Win10NightLightThemeSync.Service
{
    public class ThemeService
    {
        private readonly SettingService _settingService;

        private SettingModel Setting => _settingService.CurrentSetting;

        private const string ThemeKeyPath =
            @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize";


        public ThemeService(SettingService settingService)
        {
            _settingService = settingService;

            NightLightWatcher.NightLightStatusChanged += ApplyTheme;
            ThemeWatcher.AppThemeChanged += ThemeWatcher_AppThemeChanged;
            ThemeWatcher.SystemThemeChanged += ThemeWatcher_SystemThemeChanged;

            Setting.Day.PropertyChanged += Day_PropertyChanged;
            Setting.Night.PropertyChanged += Night_PropertyChanged;

            ApplyTheme(NightLightWatcher.NightLightStatus);
        }

        private void ThemeWatcher_SystemThemeChanged(Theme newTheme)
        {
            if (NightLightWatcher.NightLightStatus == NightLightStatus.Enable)
            {
                Setting.Night.System = newTheme;
            }
            else
            {
                Setting.Day.System = newTheme;
            }
        }

        private void ThemeWatcher_AppThemeChanged(Theme newTheme)
        {
            if (NightLightWatcher.NightLightStatus == NightLightStatus.Enable)
            {
                Setting.Night.App = newTheme;
            }
            else
            {
                Setting.Day.App = newTheme;
            }
        }

        private enum ChangedType { Day , Night}

        private void Day_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
            => ApplyTheme(ChangedType.Day);

        private void Night_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        => ApplyTheme(ChangedType.Night);

        //public void SetPreferredThemeInNightLight(bool? SystemUsesLightTheme,bool? AppsUseLightTheme)
        //{
        //    if(SystemUsesLightTheme.HasValue) Setting.Night.System = SystemUsesLightTheme.Value ? Theme.Light : Theme.Dark;
        //    if(AppsUseLightTheme.HasValue) Setting.Night.App = AppsUseLightTheme.Value ? Theme.Light : Theme.Dark;
        //    ApplyTheme(true);
        //}

        //public void SetPreferredThemeOutNightLight(bool? SystemUsesLightTheme, bool? AppsUseLightTheme)
        //{
        //    if (SystemUsesLightTheme.HasValue) Setting.Day.System = SystemUsesLightTheme.Value ? Theme.Light : Theme.Dark;
        //    if (AppsUseLightTheme.HasValue) Setting.Day.App = AppsUseLightTheme.Value ? Theme.Light : Theme.Dark;
        //    ApplyTheme(false);
        //}

        private void ApplyTheme(ChangedType type)
        {
            if (type == ChangedType.Night && NightLightWatcher.NightLightStatus == NightLightStatus.Enable)
            {
                Registry.SetValue(ThemeKeyPath, "SystemUsesLightTheme", (int)Setting.Night.System, RegistryValueKind.DWord);
                Registry.SetValue(ThemeKeyPath, "AppsUseLightTheme", (int)Setting.Night.App, RegistryValueKind.DWord);
            }
            else if (type == ChangedType.Day && NightLightWatcher.NightLightStatus == NightLightStatus.Disable)
            {
                Registry.SetValue(ThemeKeyPath, "SystemUsesLightTheme", (int)Setting.Day.System, RegistryValueKind.DWord);
                Registry.SetValue(ThemeKeyPath, "AppsUseLightTheme", (int)Setting.Day.App, RegistryValueKind.DWord);
            }
        }

        private void ApplyTheme(NightLightStatus status)
        {
            if (status == NightLightStatus.Enable)
            {
                Registry.SetValue(ThemeKeyPath, "SystemUsesLightTheme", (int)Setting.Night.System, RegistryValueKind.DWord);
                Registry.SetValue(ThemeKeyPath, "AppsUseLightTheme", (int)Setting.Night.App, RegistryValueKind.DWord);
            }
            else
            {
                Registry.SetValue(ThemeKeyPath, "SystemUsesLightTheme", (int)Setting.Day.System, RegistryValueKind.DWord);
                Registry.SetValue(ThemeKeyPath, "AppsUseLightTheme", (int)Setting.Day.App, RegistryValueKind.DWord);
            }
        }
    }
}