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
        private string _path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Win10NightLightThemeSync\";

        public async Task OpenConfigFile()
        {
            if (File.Exists(_path + "appsetting.json"))
            {
                using FileStream stream = File.Open(_path + "appsetting.json", FileMode.Open, FileAccess.Read);
                var result = await JsonHelper.Deserialize<SettingModel>(stream);
                CurrentSetting = result ?? CurrentSetting;
            }
        }

        public async Task SaveConfigFile()
        {
            SetStartup();
            Directory.CreateDirectory(_path);
            using FileStream stream = File.Open(_path + "appsetting.json", FileMode.OpenOrCreate, FileAccess.ReadWrite);
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
                string exeFilePath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
                Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Run", appName, exeFilePath, RegistryValueKind.String);
            }
            else
            {
                using RegistryKey key = Registry.CurrentUser.OpenSubKey(keyName, true);
                try
                {
                    key?.DeleteValue(appName);
                }
                catch (ArgumentException) { } //System.ArgumentException: No value exists with that name.
            }
        }
    }
}
