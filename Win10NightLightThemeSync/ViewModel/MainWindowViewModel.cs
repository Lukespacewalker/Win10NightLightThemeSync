using System;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Win10NightLightThemeSync.Helper;
using Win10NightLightThemeSync.Models;
using Win10NightLightThemeSync.Service;

namespace Win10NightLightThemeSync.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly ThemeService _themeService;
        private readonly SettingService _settingService;

        #region ThemeRadioButton

        private bool _nightUseAppLightTheme = false;
        public bool NightUseAppLightTheme
        {
            get => _nightUseAppLightTheme;
            set
            {
                _themeService.SetPreferredThemeInNightLight(null,value);
                SetAndRaiseIfChanged(ref _nightUseAppLightTheme, value);
            }
        }

        private bool _nightUseSystemLightTheme = false;
        public bool NightUseSystemLightTheme
        {
            get => _nightUseSystemLightTheme;
            set
            {
                _themeService.SetPreferredThemeInNightLight(value, null);
                SetAndRaiseIfChanged(ref _nightUseSystemLightTheme, value);
            }
        }

        private bool _dayUseAppLightTheme = true;
        public bool DayUseAppLightTheme {
            get => _dayUseAppLightTheme;
            set
            {
                _themeService.SetPreferredThemeOutNightLight(null, value);
                SetAndRaiseIfChanged(ref _dayUseAppLightTheme, value);
            }
        }

        private bool _dayUseSystemLightTheme = true;
        public bool DayUseSystemLightTheme {
            get => _dayUseSystemLightTheme;
            set
            {
                _themeService.SetPreferredThemeOutNightLight(value, null);
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
            set => _settingService.CurrentSetting.StartMinimized = value;
        }

        public bool Autorun {
            get => _settingService.CurrentSetting.Autorun;
            set => _settingService.CurrentSetting.Autorun = value;
        }

        public ICommand ShowOrHideWindowsCommand { get; private set; }
        public ICommand TerminateCommand { get; private set; }
        public ICommand StartMonitoring { get; private set; }
        public ICommand StopMonitoring { get; private set; }

        public MainWindowViewModel(ThemeService themeService, NightLightMonitor nightLightMonitor, SettingService settingService)
        {
            _themeService = themeService;
            _settingService = settingService;

            DayUseSystemLightTheme = settingService.CurrentSetting.Day.System == Theme.Light;
            NightUseSystemLightTheme = settingService.CurrentSetting.Night.System == Theme.Light;
            DayUseAppLightTheme = settingService.CurrentSetting.Day.App == Theme.Light;
            NightUseAppLightTheme = settingService.CurrentSetting.Night.App == Theme.Light;

            // Commanding
            StartMonitoring = new RelayCommand(_=>
            {
                StatusText = "Monitoring";
                IsWatching = true;
                nightLightMonitor.Start();
            }, _=>!nightLightMonitor.IsWatching);
            StopMonitoring = new RelayCommand(_=>
            {
                StatusText = "Not Monitored";
                IsWatching = false;
                nightLightMonitor.Stop();
            }, _ => nightLightMonitor.IsWatching);

            ShowOrHideWindowsCommand = new RelayCommand(_ =>
            {
                if (Application.Current.MainWindow.WindowState == WindowState.Minimized)
                {
                    SystemCommands.RestoreWindow(Application.Current.MainWindow);
                    Application.Current.MainWindow.Activate();
                }
                else
                {
                    SystemCommands.MinimizeWindow(Application.Current.MainWindow);
                }
            });

            TerminateCommand = new RelayCommand(_ => Application.Current.Shutdown());
        }
    }
}
