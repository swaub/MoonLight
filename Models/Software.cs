using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MoonLight.Models
{
    public class Software : INotifyPropertyChanged
    {
        private bool _isSelected;

        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string DownloadUrl { get; set; } = string.Empty;
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