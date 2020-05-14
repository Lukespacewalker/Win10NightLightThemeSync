using System;
using System.Management;
using System.Security.Principal;
using Microsoft.Win32;

namespace Win10NightLightThemeSync.Service
{
    public enum NightLightStatus { Disable, Enable }

    public delegate void NightLightStatusChangedHandler(NightLightStatus status);

    public class NightLightWatcher
    {
        private readonly ManagementEventWatcher _watcher;
        private static NightLightWatcher Instance { get; set; }

        public static bool IsWatching { get; private set; }

        private const string NightLightKeyPathForWMI =
            @"\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\CloudStore\\Store\\DefaultAccount\\Current\\default$windows.data.bluelightreduction.bluelightreductionstate\\windows.data.bluelightreduction.bluelightreductionstate";


        private const string NightLightKeyPathForWin32 =
            @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\CloudStore\Store\DefaultAccount\Current\default$windows.data.bluelightreduction.bluelightreductionstate\windows.data.bluelightreduction.bluelightreductionstate";

        private const string NightLightValueName = "Data";

        protected NightLightWatcher()
        {
            WindowsIdentity currentUser = WindowsIdentity.GetCurrent();
            WqlEventQuery query = new WqlEventQuery(
                "SELECT * FROM RegistryValueChangeEvent WHERE " +
                "Hive = 'HKEY_USERs'" +
                $"AND KeyPath = '{currentUser.User.Value}{NightLightKeyPathForWMI}' AND ValueName='{NightLightValueName}'");

            _watcher = new ManagementEventWatcher(query);
        }

        private static readonly byte[] _target = new byte[] { 0x43, 0x42, 0x01, 0x00 };

        public static NightLightStatus NightLightStatus
        {
            get
            {
                // 0x43 0x42 0x01 0x00
                byte[] nightLightData = Registry.GetValue(NightLightKeyPathForWin32, NightLightValueName, new byte[0]) as byte[];
                if (nightLightData == null) return NightLightStatus.Disable;
                var span = new Span<byte>(nightLightData);
                var nightLightByteIndex = span.LastIndexOf(_target) + _target.Length;

                return nightLightData[nightLightByteIndex] == 0x10 && nightLightData[nightLightByteIndex+1] == 0x0
                    ? NightLightStatus.Enable
                    : NightLightStatus.Disable;
            }
        }

        static NightLightWatcher()
        {
            NightLightWatcher.Instance = new NightLightWatcher();
        }

        public static void Start()
        {
            NightLightWatcher.Instance.StartInternal();
        }

        private void StartInternal()
        {
            _watcher.EventArrived += Watcher_EventArrived;
            _watcher.Start();
            IsWatching = true;
            WatchingStatusChanged?.Invoke(IsWatching);
        }

        public static void Stop()
        {
            NightLightWatcher.Instance.StopInternal();
        }

        private void StopInternal()
        {
            _watcher.EventArrived -= Watcher_EventArrived;
            _watcher.Stop();
            IsWatching = false;
            WatchingStatusChanged?.Invoke(IsWatching);
        }

        public static event Action<bool> WatchingStatusChanged; 

        public static event NightLightStatusChangedHandler NightLightStatusChanged;

        private void Watcher_EventArrived(object sender, EventArrivedEventArgs e)
        {
            NightLightStatusChanged?.Invoke(NightLightStatus);
        }
    }
}
