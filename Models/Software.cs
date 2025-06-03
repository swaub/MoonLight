using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MoonLight.Models
{
    public class Software : INotifyPropertyChanged
    {
        private bool _isSelected;
        private double _downloadProgress;
        private double _installProgress;
        private string _statusMessage = string.Empty;
        private bool _isProcessing;

        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string DownloadUrl { get; set; } = string.Empty;
        public List<string> FallbackUrls { get; set; } = new List<string>();
        public string InstallArguments { get; set; } = string.Empty;
        public InstallerType InstallerType { get; set; }
        public int EstimatedSizeMB { get; set; }
        
        public bool IsSelected 
        { 
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        public double DownloadProgress
        {
            get => _downloadProgress;
            set
            {
                if (_downloadProgress != value)
                {
                    _downloadProgress = value;
                    OnPropertyChanged();
                }
            }
        }

        public double InstallProgress
        {
            get => _installProgress;
            set
            {
                if (_installProgress != value)
                {
                    _installProgress = value;
                    OnPropertyChanged();
                }
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                if (_statusMessage != value)
                {
                    _statusMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsProcessing
        {
            get => _isProcessing;
            set
            {
                if (_isProcessing != value)
                {
                    _isProcessing = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public enum InstallerType
    {
        EXE,
        MSI,
        InnoSetup,
        NSIS
    }
}