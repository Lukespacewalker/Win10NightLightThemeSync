using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Unity;
using Win10NightLightThemeSync.Service;
using Win10NightLightThemeSync.ViewModel;

namespace Win10NightLightThemeSync
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IUnityContainer Container = new UnityContainer().AddExtension(new Diagnostic());

        public App()
        {
            Container.RegisterSingleton<SettingService>();
            Container.RegisterSingleton<NightLightMonitor>();
            Container.RegisterSingleton<ThemeService>();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            var nightLight = Container.Resolve<NightLightMonitor>();
            nightLight.Stop();
            var settingService = Container.Resolve<SettingService>();
            Task.WaitAll(settingService.SaveConfigFile());
            base.OnExit(e);
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            var settingService = Container.Resolve<SettingService>();
            await settingService.OpenConfigFile();
            var nightLight = Container.Resolve<NightLightMonitor>();
            nightLight.Start();
            var mainWindowViewModel = Container.Resolve<MainWindowViewModel>();
            var window = new MainWindow {DataContext = mainWindowViewModel};
            window.Show();
            if (settingService.CurrentSetting.StartMinimized)
            {
                SystemCommands.MinimizeWindow(window);
            }
            //base.OnStartup(e);
        }
    }


}
