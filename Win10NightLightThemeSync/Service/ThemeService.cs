using Microsoft.Win32;
using Win10NightLightThemeSync.Models;

namespace Win10NightLightThemeSync.Service
{
    public class ThemeService
    {
        private readonly NightLightMonitor _nightLightMonitor;
        private readonly SettingService _settingService;

        private SettingModel Setting => _settingService.CurrentSetting;

        private const string ThemeKeyPath =
            @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize";

        public ThemeService(NightLightMonitor nightLightMonitor, SettingService settingService)
        {
            _nightLightMonitor = nightLightMonitor;
            _settingService = settingService;

            _nightLightMonitor.NightLightStatusChanged += _nightLightMonitor_NightLightStatusChanged;
        }

        public void SetPreferredThemeInNightLight(bool? SystemUsesLightTheme,bool? AppsUseLightTheme)
        {
            if(SystemUsesLightTheme.HasValue) Setting.Night.System = SystemUsesLightTheme.Value ? Theme.Light : Theme.Dark;
            if(AppsUseLightTheme.HasValue) Setting.Night.App = AppsUseLightTheme.Value ? Theme.Light : Theme.Dark;
            ApplyTheme(true);
        }

        public void SetPreferredThemeOutNightLight(bool? SystemUsesLightTheme, bool? AppsUseLightTheme)
        {
            if (SystemUsesLightTheme.HasValue) Setting.Day.System = SystemUsesLightTheme.Value ? Theme.Light : Theme.Dark;
            if (AppsUseLightTheme.HasValue) Setting.Day.App = AppsUseLightTheme.Value ? Theme.Light : Theme.Dark;
            ApplyTheme(false);
        }

        private void ApplyTheme(bool nightLight)
        {
            if (nightLight && _nightLightMonitor.NightLightStatus == NightLightStatus.Enable)
            {
                Registry.SetValue(ThemeKeyPath, "SystemUsesLightTheme", (int)Setting.Night.System, RegistryValueKind.DWord);
                Registry.SetValue(ThemeKeyPath, "AppsUseLightTheme", (int)Setting.Night.App, RegistryValueKind.DWord);
            }
            else if (!nightLight && _nightLightMonitor.NightLightStatus == NightLightStatus.Disable)
            {
                Registry.SetValue(ThemeKeyPath, "SystemUsesLightTheme", (int)Setting.Day.System, RegistryValueKind.DWord);
                Registry.SetValue(ThemeKeyPath, "AppsUseLightTheme", (int)Setting.Day.App, RegistryValueKind.DWord);
            }
        }

        private void _nightLightMonitor_NightLightStatusChanged(NightLightStatus status)
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