using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using Win10NightLightThemeSync.Annotations;
using Win10NightLightThemeSync.Helper;
using Win10NightLightThemeSync.ViewModel;

namespace Win10NightLightThemeSync.Models
{
    public enum Theme
    {
        Dark, Light
    }

    public class ThemeChangedEventArg : EventArgs
    {
        private readonly Theme _newTheme;
        public Theme NewTheme => _newTheme;

        public ThemeChangedEventArg(Theme newTheme)
        {
            _newTheme = newTheme;
        }
    }

    public class SettingModel : NotificableObject
    {
        private bool _startMinimized;
        private bool _autorun;

        public ThemeSetting Night { get; set; } = new ThemeSetting { App = Theme.Dark , System = Theme.Dark};
        public ThemeSetting Day { get; set; } = new ThemeSetting { App = Theme.Light, System = Theme.Light };

        public bool StartMinimized
        {
            get => _startMinimized;
            set => SetAndRaiseIfChanged(ref _startMinimized, value);

        }

        public bool Autorun
        {
            get => _autorun;
            set => SetAndRaiseIfChanged(ref _autorun, value);
        }

    }

    public class ThemeSetting : NotificableObject
    {
        private Theme _system;
        private Theme _app;

        public Theme System
        {
            get => _system;
            set => SetAndRaiseIfChanged(ref _system,value);
        }

        public Theme App
        {
            get => _app;
            set => SetAndRaiseIfChanged(ref _app, value);
        }

    }
}
