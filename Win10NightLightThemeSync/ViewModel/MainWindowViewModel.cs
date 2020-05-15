using System;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Win10NightLightThemeSync.Helper;
using Win10NightLightThemeSync.Models;
using Win10NightLightThemeSync.Service;

namespace Win10NightLightThemeSync.ViewModel
{
    public class MainWindowViewModel : NotificableObject
    {
        private readonly ThemeService _themeService;
        private readonly SettingService _settingService;

        #region ThemeRadioButton

        private bool _nightUseAppLightTheme = false;
        public bool NightUseAppLightTheme {
            get => _nightUseAppLightTheme;
            set
            {
                _settingService.CurrentSetting.Night.App = value ? Theme.Light : Theme.Dark;
                SetAndRaiseIfChanged(ref _nightUseAppLightTheme, value);
            }
        }

        private bool _nightUseSystemLightTheme = false;
        public bool NightUseSystemLightTheme {
            get => _nightUseSystemLightTheme;
            set
            {
                _settingService.CurrentSetting.Night.System = value ? Theme.Light : Theme.Dark;
                SetAndRaiseIfChanged(ref _nightUseSystemLightTheme, value);
            }
        }

        private bool _dayUseAppLightTheme = true;
        public bool DayUseAppLightTheme {
            get => _dayUseAppLightTheme;
            set
            {
                _settingService.CurrentSetting.Day.App = value ? Theme.Light : Theme.Dark;
                SetAndRaiseIfChanged(ref _dayUseAppLightTheme, value);
            }
        }

        private bool _dayUseSystemLightTheme = true;
        public bool DayUseSystemLightTheme {
            get => _dayUseSystemLightTheme;
            set
            {
                _settingService.CurrentSetting.Day.System = value ? Theme.Light : Theme.Dark;
                SetAndRaiseIfChanged(ref _dayUseSystemLightTheme, value);
            }
        }

        private string _statusText = "Monitoring";
        public string StatusText {
            get => _statusText;
            private set
            {
                SetAndRaiseIfChanged(ref _statusText, value);
            }
        }

        private bool _isWatching = true;

        public bool IsWatching {
            get => _isWatching;
            private set
            {
                SetAndRaiseIfChanged(ref _isWatching, value);
            }
        }

        #endregion

        public bool StartMinimized
        {
            get => _settingService.CurrentSetting.StartMinimized;
            set
            {
                if (value != _settingService.CurrentSetting.StartMinimized)
                {
                    _settingService.CurrentSetting.StartMinimized = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool Autorun {
            get => _settingService.CurrentSetting.Autorun;
            set
            {
                if (value != _settingService.CurrentSetting.Autorun)
                {
                    _settingService.CurrentSetting.Autorun = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand StartMonitoring { get; private set; }
        public ICommand StopMonitoring { get; private set; }

        public MainWindowViewModel(ThemeService themeService, SettingService settingService)
        {
            _themeService = themeService;
            _settingService = settingService;

            DayUseSystemLightTheme = settingService.CurrentSetting.Day.System == Theme.Light;
            DayUseAppLightTheme = settingService.CurrentSetting.Day.App == Theme.Light;
            NightUseSystemLightTheme = settingService.CurrentSetting.Night.System == Theme.Light;
            NightUseAppLightTheme = settingService.CurrentSetting.Night.App == Theme.Light;

            WeakEventManager<ThemeSetting, PropertyChangedEventArgs>.AddHandler(_settingService.CurrentSetting.Day, "PropertyChanged", Day_PropertyChanged);
            WeakEventManager<ThemeSetting, PropertyChangedEventArgs>.AddHandler(_settingService.CurrentSetting.Night, "PropertyChanged", Night_PropertyChanged);

            // Commanding
            StartMonitoring = new RelayCommand(_ =>
            {
                StatusText = "Monitoring";
                IsWatching = true;
                NightLightWatcher.Start();
                ThemeWatcher.Start();
            }, _ => !NightLightWatcher.IsWatching);
            StopMonitoring = new RelayCommand(_ =>
            {
                StatusText = "Not Monitored";
                IsWatching = false;
                NightLightWatcher.Stop();
                ThemeWatcher.Stop();
            }, _ => NightLightWatcher.IsWatching);
        }

        private void Night_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NightUseSystemLightTheme = _settingService.CurrentSetting.Night.System == Theme.Light;
            NightUseAppLightTheme = _settingService.CurrentSetting.Night.App == Theme.Light;
        }

        private void Day_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            DayUseSystemLightTheme = _settingService.CurrentSetting.Day.System == Theme.Light;
            DayUseAppLightTheme = _settingService.CurrentSetting.Day.App == Theme.Light;
        }

        //public void Dispose()
        //{
        //    WeakEventManager<ThemeSetting, PropertyChangedEventArgs>.RemoveHandler(_settingService.CurrentSetting.Day, "PropertyChanged", Day_PropertyChanged);
        //    WeakEventManager<ThemeSetting, PropertyChangedEventArgs>.RemoveHandler(_settingService.CurrentSetting.Night, "PropertyChanged", Night_PropertyChanged);
        //}
    }
}
