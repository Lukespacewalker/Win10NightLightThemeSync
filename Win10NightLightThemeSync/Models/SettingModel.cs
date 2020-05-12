using System;
using System.Collections.Generic;
using System.Text;

namespace Win10NightLightThemeSync.Models
{
    public enum Theme
    {
        Dark, Light
    }

    public class SettingModel
    {
        public ThemeSetting Night { get; set; } = new ThemeSetting { App = Theme.Dark , System = Theme.Dark};
        public ThemeSetting Day { get; set; } = new ThemeSetting { App = Theme.Light, System = Theme.Light };
    }

    public class ThemeSetting
    {
        public Theme System { get; set; }
        public Theme App { get; set; }
    }
}
