using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MoonLight.Models
{
    public class SoftwareCategory : INotifyPropertyChanged
    {
        public string Name { get; set; } = string.Empty;
        public ObservableCollection<Software> Applications { get; set; } = new ObservableCollection<Software>();
        public bool IsExpanded { get; set; } = false;
        public int SelectedCount => Applications?.Count(app => app.IsSelected) ?? 0;

        public event PropertyChangedEventHandler? PropertyChanged;

        public void NotifySelectedCountChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedCount)));
        }
    }
}