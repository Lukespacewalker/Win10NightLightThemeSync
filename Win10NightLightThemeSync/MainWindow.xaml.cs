using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SourceChord.FluentWPF;
using Unity;
using Win10NightLightThemeSync.Helper;
using Win10NightLightThemeSync.Service;
using Win10NightLightThemeSync.ViewModel;
using Application = System.Windows.Application;

namespace Win10NightLightThemeSync
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private BitmapFrame _darkIcon;
        private BitmapFrame _lightIcon;
        private Icon _darkIco;
        private Icon _lightIco;

        private readonly NightLightMonitor _monitor = App.Container.Resolve<NightLightMonitor>();
        private NotifyIcon _tray = new NotifyIcon();

        public MainWindow()
        {
            PrepIcon();
            InitializeComponent();
            SetIcon();
            // Tray
            _tray.Visible = true;
            _tray.Text = Application.Current.MainWindow.Title;

            _tray.MouseClick += (sender, args) =>
            {
                if (args.Button == MouseButtons.Left)
                    ((MainWindowViewModel)DataContext).ShowOrHideWindowsCommand.Execute(null);
            };
            _tray.ContextMenuStrip = new ContextMenuStrip();
            _tray.ContextMenuStrip.Items.Add("&Setting", null, ((sender, args) => ((MainWindowViewModel)DataContext).ShowOrHideWindowsCommand.Execute(null)));
            _tray.ContextMenuStrip.Items.Add("E&xit", null, (sender, args) => ((MainWindowViewModel)DataContext).TerminateCommand.Execute(null));
            // Event
            SystemTheme.ThemeChanged += SystemTheme_ThemeChanged;
            _monitor.WatchingStatusChanged += _monitor_WatchingStatusChanged;
            this.StateChanged += MainWindow_StateChanged;
        }


        private void _monitor_WatchingStatusChanged(bool isWatching)
        {
            _tray.Visible = isWatching;
            /*
            if(isWatching)_tray.Text = $"[Monitoring] {Application.Current.MainWindow.Title}";
            else _tray.Text = $"[Not Monitored] {Application.Current.MainWindow.Title}";
            */
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (_monitor.IsWatching)
            {
                e.Cancel = true;
                SystemCommands.MinimizeWindow(this);
            }
            else base.OnClosing(e);
        }

        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.ShowInTaskbar = false;
            }
            else if (this.WindowState == WindowState.Normal)
            {
                this.ShowInTaskbar = true;
            }
        }

        private void PrepIcon()
        {
            var darkURI = new Uri("pack://application:,,,/Assets/icon_dark.ico", UriKind.RelativeOrAbsolute);
            var lightURI = new Uri("pack://application:,,,/Assets/icon_light.ico", UriKind.RelativeOrAbsolute);
            _darkIcon = BitmapFrame.Create(darkURI);
            _lightIcon = BitmapFrame.Create(lightURI);
            _darkIco = GenIcon(darkURI);
            _lightIco = GenIcon(lightURI);
        }

        private Icon GenIcon(Uri uri)
        {
            var sri = Application.GetResourceStream(uri);
            return new Icon(sri.Stream);
            /*
            var bitmap = new Bitmap(sri.Stream);
            var handle = bitmap.GetHicon();
            return System.Drawing.Icon.FromHandle(handle);
            */
        }

        private void SetIcon()
        {
            if (SystemTheme.WindowsTheme == WindowsTheme.Dark)
            {
                this.Icon = _darkIcon;
                _tray.Icon = _darkIco;
            }
            else
            {
                this.Icon = _lightIcon;
                _tray.Icon = _lightIco;
            }
        }

        private void SystemTheme_ThemeChanged(object sender, EventArgs e)
        {
            SetIcon();
        }
    }
}
