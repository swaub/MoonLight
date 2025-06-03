using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace MoonLight.Models
{
    public class InstallationOptions : INotifyPropertyChanged
    {
        private bool _createSystemRestorePoint = true;
        private bool _disableDesktopShortcuts = true;
        private bool _disableStartMenuShortcuts = false;
        private bool _disableAutoStart = true;
        private bool _cleanUpInstallers = true;
        private bool _runAsAdministrator = true;
        private string _downloadLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", "MoonLight");
        private FileOrganization _fileOrganization = FileOrganization.KeepIndividualFiles;
        private bool _useSystemProxy = false;
        private int _timeoutSeconds = 300;

        public bool CreateSystemRestorePoint 
        { 
            get => _createSystemRestorePoint;
            set { _createSystemRestorePoint = value; OnPropertyChanged(); }
        }
        
        public bool DisableDesktopShortcuts 
        { 
            get => _disableDesktopShortcuts;
            set { _disableDesktopShortcuts = value; OnPropertyChanged(); }
        }
        
        public bool DisableStartMenuShortcuts 
        { 
            get => _disableStartMenuShortcuts;
            set { _disableStartMenuShortcuts = value; OnPropertyChanged(); }
        }
        
        public bool DisableAutoStart 
        { 
            get => _disableAutoStart;
            set { _disableAutoStart = value; OnPropertyChanged(); }
        }
        
        public bool CleanUpInstallers 
        { 
            get => _cleanUpInstallers;
            set { _cleanUpInstallers = value; OnPropertyChanged(); }
        }
        
        public bool RunAsAdministrator 
        { 
            get => _runAsAdministrator;
            set { _runAsAdministrator = value; OnPropertyChanged(); }
        }
        
        public string DownloadLocation 
        { 
            get => _downloadLocation;
            set { _downloadLocation = value; OnPropertyChanged(); }
        }
        
        public FileOrganization FileOrganization 
        { 
            get => _fileOrganization;
            set { _fileOrganization = value; OnPropertyChanged(); }
        }
        
        public bool UseSystemProxy 
        { 
            get => _useSystemProxy;
            set { _useSystemProxy = value; OnPropertyChanged(); }
        }
        
        public int TimeoutSeconds 
        { 
            get => _timeoutSeconds;
            set { _timeoutSeconds = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public enum FileOrganization
    {
        KeepIndividualFiles,
        CompressToCategorizedZip
    }
}