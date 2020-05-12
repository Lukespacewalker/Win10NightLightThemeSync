using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Win10DarkThemeNightLightSync.Helper;
using Win10DarkThemeNightLightSync.Models;

namespace Win10DarkThemeNightLightSync.Service
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
            using FileStream stream = File.Open("appsetting.json", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            stream.SetLength(0);
            stream.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            await JsonHelper.SerializeToFileStream(CurrentSetting, stream, CancellationToken.None).ConfigureAwait(false);
            stream.Flush();
        }
    }
}
