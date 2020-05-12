using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;
using Win10NightLightThemeSync.Helper;
using Win10NightLightThemeSync.Models;

namespace Win10NightLightThemeSync.Service
{
    public class SettingService
    {
        public SettingModel CurrentSetting { get; set; } = new SettingModel();

        public async Task OpenConfigFile()
        {
            if (File.Exists("appsetting.json"))
            {
                using FileStream stream = File.Open("appsetting.json", FileMode.Open, FileAccess.Read);
                var result = await JsonHelper.Deserialize<SettingModel>(stream);
                CurrentSetting = result ?? CurrentSetting;
            }
        }

        public async Task SaveConfigFile()
        {
            SetStartup();
            using FileStream stream = File.Open("appsetting.json", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            stream.SetLength(0);
            stream.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            await JsonHelper.SerializeToFileStream(CurrentSetting, stream, CancellationToken.None).ConfigureAwait(false);
            stream.Flush();
        }

        private void SetStartup()
        {
            string keyName = @"Software\Microsoft\Windows\CurrentVersion\Run";
            string appName = "Win10NightLightThemeSync";
            if (CurrentSetting.Autorun)
            {
                string currentPath = System.Reflection.Assembly.GetEntryAssembly().Location;
                Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Run", appName, currentPath, RegistryValueKind.String);
            }
            else
            {
                using RegistryKey key = Registry.CurrentUser.OpenSubKey(keyName, true);
                key?.DeleteValue(appName);
            }
        }
    }
}
