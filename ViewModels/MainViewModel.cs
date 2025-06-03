using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using MoonLight.Models;
using MoonLight.Services;

namespace MoonLight.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ISoftwareCatalogService _catalogService;
        private readonly IDownloadService _downloadService;
        private readonly IInstallationService _installationService;
        private readonly ILoggingService _loggingService;

        private bool _isBatchMode = true;
        private string _searchText = string.Empty;
        private string _statusText = "";
        private double _overallProgress;
        private double _currentOperationProgress;
        private string _currentOperationText = string.Empty;
        private ObservableCollection<string> _logEntries = new ObservableCollection<string>();
        private ObservableCollection<SoftwareCategory> _categories = new ObservableCollection<SoftwareCategory>();
        private ObservableCollection<SoftwareCategory> _filteredCategories = new ObservableCollection<SoftwareCategory>();
        private InstallationOptions _installationOptions = new InstallationOptions();
        private Software? _selectedSingleSoftware;
        private bool _isOperationInProgress;
        private string _singleModeInstallerPath = string.Empty;
        private string _singleModeInstallArguments = string.Empty;
        private bool _autoDetectInstallerType = true;

        public MainViewModel()
        {
            _catalogService = new SoftwareCatalogService();
            _downloadService = new DownloadService();
            _installationService = new InstallationService();
            _loggingService = new LoggingService();

            InitializeCommands();
            LoadSoftwareCatalog();
            _loggingService.LogAdded += OnLogAdded;
        }

        public bool IsBatchMode
        {
            get => _isBatchMode;
            set
            {
                if (SetProperty(ref _isBatchMode, value))
                {
                    OnPropertyChanged(nameof(IsSingleMode));
                }
            }
        }

        public bool IsSingleMode => !IsBatchMode;

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    FilterSoftware();
                }
            }
        }

        public string StatusText
        {
            get => _statusText;
            set => SetProperty(ref _statusText, value);
        }

        public double OverallProgress
        {
            get => _overallProgress;
            set => SetProperty(ref _overallProgress, value);
        }

        public double CurrentOperationProgress
        {
            get => _currentOperationProgress;
            set => SetProperty(ref _currentOperationProgress, value);
        }

        public string CurrentOperationText
        {
            get => _currentOperationText;
            set => SetProperty(ref _currentOperationText, value);
        }

        public ObservableCollection<string> LogEntries
        {
            get => _logEntries;
            set => SetProperty(ref _logEntries, value);
        }

        public ObservableCollection<SoftwareCategory> Categories
        {
            get => _categories;
            set => SetProperty(ref _categories, value);
        }

        public ObservableCollection<SoftwareCategory> FilteredCategories
        {
            get => _filteredCategories;
            set => SetProperty(ref _filteredCategories, value);
        }

        public InstallationOptions InstallationOptions
        {
            get => _installationOptions;
            set => SetProperty(ref _installationOptions, value);
        }

        public bool KeepIndividualFiles
        {
            get => InstallationOptions.FileOrganization == FileOrganization.KeepIndividualFiles;
            set
            {
                if (value)
                {
                    InstallationOptions.FileOrganization = FileOrganization.KeepIndividualFiles;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CompressToCategorizedZip));
                }
            }
        }

        public bool CompressToCategorizedZip
        {
            get => InstallationOptions.FileOrganization == FileOrganization.CompressToCategorizedZip;
            set
            {
                if (value)
                {
                    InstallationOptions.FileOrganization = FileOrganization.CompressToCategorizedZip;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(KeepIndividualFiles));
                }
            }
        }

        public Software? SelectedSingleSoftware
        {
            get => _selectedSingleSoftware;
            set
            {
                if (SetProperty(ref _selectedSingleSoftware, value))
                {
                    if (value != null && AutoDetectInstallerType)
                    {
                        SingleModeInstallerPath = value.DownloadUrl;
                        SingleModeInstallArguments = value.InstallArguments;
                    }
                }
            }
        }

        public string SingleModeInstallerPath
        {
            get => _singleModeInstallerPath;
            set => SetProperty(ref _singleModeInstallerPath, value);
        }

        public string SingleModeInstallArguments
        {
            get => _singleModeInstallArguments;
            set => SetProperty(ref _singleModeInstallArguments, value);
        }

        public bool AutoDetectInstallerType
        {
            get => _autoDetectInstallerType;
            set => SetProperty(ref _autoDetectInstallerType, value);
        }

        public bool IsOperationInProgress
        {
            get => _isOperationInProgress;
            set => SetProperty(ref _isOperationInProgress, value);
        }

        public int SelectedApplicationCount => Categories.Sum(c => c.SelectedCount);
        public int TotalSizeMB => Categories.SelectMany(c => c.Applications).Where(a => a.IsSelected).Sum(a => a.EstimatedSizeMB);
        
        public IEnumerable<Software> AllApplications => Categories.SelectMany(c => c.Applications);

        public ICommand SelectAllCommand { get; private set; } = null!;
        public ICommand DeselectAllCommand { get; private set; } = null!;
        public ICommand DownloadSelectedCommand { get; private set; } = null!;
        public ICommand InstallSelectedCommand { get; private set; } = null!;
        public ICommand CancelOperationCommand { get; private set; } = null!;
        public ICommand ExportLogCommand { get; private set; } = null!;
        public ICommand BrowseDownloadLocationCommand { get; private set; } = null!;
        public ICommand ClearSearchCommand { get; private set; } = null!;
        public ICommand BrowseSingleModeInstallerCommand { get; private set; } = null!;

        private void InitializeCommands()
        {
            SelectAllCommand = new RelayCommand(_ => SelectAll());
            DeselectAllCommand = new RelayCommand(_ => DeselectAll());
            DownloadSelectedCommand = new RelayCommand(async _ => await DownloadSelected(), _ => !IsOperationInProgress);
            InstallSelectedCommand = new RelayCommand(async _ => await InstallSelected(), _ => !IsOperationInProgress);
            CancelOperationCommand = new RelayCommand(_ => CancelOperation(), _ => IsOperationInProgress);
            ExportLogCommand = new RelayCommand(_ => ExportLog());
            BrowseDownloadLocationCommand = new RelayCommand(_ => BrowseDownloadLocation());
            ClearSearchCommand = new RelayCommand(_ => SearchText = string.Empty);
            BrowseSingleModeInstallerCommand = new RelayCommand(_ => BrowseSingleModeInstaller());
        }

        private void LoadSoftwareCatalog()
        {
            var catalog = _catalogService.GetSoftwareCatalog();
            Categories.Clear();
            foreach (var category in catalog)
            {
                Categories.Add(category);
                
                // Subscribe to category changes
                category.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(SoftwareCategory.SelectedCount))
                    {
                        OnPropertyChanged(nameof(SelectedApplicationCount));
                        OnPropertyChanged(nameof(TotalSizeMB));
                    }
                };
                
                // Subscribe to each application's property changes
                foreach (var app in category.Applications)
                {
                    app.PropertyChanged += (s, e) =>
                    {
                        if (e.PropertyName == nameof(Software.IsSelected))
                        {
                            OnPropertyChanged(nameof(SelectedApplicationCount));
                            OnPropertyChanged(nameof(TotalSizeMB));
                        }
                    };
                }
            }
            FilteredCategories = new ObservableCollection<SoftwareCategory>(Categories);
            _loggingService.LogInfo("Application initialized successfully");
        }

        private void FilterSoftware()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredCategories = new ObservableCollection<SoftwareCategory>(Categories);
                return;
            }

            var filtered = new ObservableCollection<SoftwareCategory>();
            foreach (var category in Categories)
            {
                var filteredApps = category.Applications
                    .Where(app => app.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                                  app.Description.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (filteredApps.Any())
                {
                    var filteredCategory = new SoftwareCategory
                    {
                        Name = category.Name,
                        IsExpanded = category.IsExpanded,
                        Applications = new ObservableCollection<Software>(filteredApps)
                    };
                    filtered.Add(filteredCategory);
                }
            }
            FilteredCategories = filtered;
        }

        private void SelectAll()
        {
            foreach (var category in FilteredCategories)
            {
                foreach (var app in category.Applications)
                {
                    app.IsSelected = true;
                }
            }
        }

        private void DeselectAll()
        {
            foreach (var category in Categories)
            {
                foreach (var app in category.Applications)
                {
                    app.IsSelected = false;
                }
            }
        }

        private async Task DownloadSelected()
        {
            IsOperationInProgress = true;
            StatusText = "Starting downloads...";
            _loggingService.LogInfo("Starting download operation");

            var selectedApps = GetSelectedApplications();

            if (!selectedApps.Any())
            {
                _loggingService.LogWarning("No applications selected for download");
                StatusText = "No applications selected";
                IsOperationInProgress = false;
                return;
            }

            OverallProgress = 0;
            int completed = 0;

            foreach (var app in selectedApps)
            {
                CurrentOperationText = $"Downloading {app.Name}...";
                _loggingService.LogInfo($"Starting download: {app.Name} ({app.EstimatedSizeMB}MB)");

                try
                {
                    var progress = new Progress<DownloadProgress>(p =>
                    {
                        CurrentOperationProgress = p.ProgressPercentage;
                        if (p.DownloadSpeedMBps > 0)
                        {
                            CurrentOperationText = $"Downloading {app.Name} - {p.ProgressPercentage:F1}% ({p.DownloadSpeedMBps:F1} MB/s)";
                        }
                    });

                    await _downloadService.DownloadFileAsync(app, InstallationOptions, progress);
                    _loggingService.LogInfo($"Successfully downloaded: {app.Name}");
                    completed++;
                }
                catch (Exception ex)
                {
                    _loggingService.LogError($"Failed to download {app.Name}: {ex.Message}");
                }

                OverallProgress = (completed * 100.0) / selectedApps.Count;
            }

            StatusText = $"Downloaded {completed} of {selectedApps.Count} applications";
            IsOperationInProgress = false;
            CurrentOperationText = string.Empty;
            CurrentOperationProgress = 0;
        }

        private async Task InstallSelected()
        {
            IsOperationInProgress = true;
            StatusText = "Starting installations...";
            _loggingService.LogInfo("Starting installation operation");

            var selectedApps = GetSelectedApplications();

            if (!selectedApps.Any())
            {
                _loggingService.LogWarning("No applications selected for installation");
                StatusText = "No applications selected";
                IsOperationInProgress = false;
                return;
            }

            if (InstallationOptions.CreateSystemRestorePoint)
            {
                CurrentOperationText = "Creating system restore point...";
                _loggingService.LogInfo("Creating system restore point");
                await _installationService.CreateSystemRestorePointAsync();
            }

            OverallProgress = 0;
            int completed = 0;

            foreach (var app in selectedApps)
            {
                CurrentOperationText = $"Installing {app.Name}...";
                _loggingService.LogInfo($"Starting installation: {app.Name}");

                try
                {
                    await _installationService.InstallApplicationAsync(app, InstallationOptions);
                    _loggingService.LogInfo($"Successfully installed: {app.Name}");
                    completed++;
                }
                catch (Exception ex)
                {
                    _loggingService.LogError($"Failed to install {app.Name}: {ex.Message}");
                }

                OverallProgress = (completed * 100.0) / selectedApps.Count;
                CurrentOperationProgress = 100;
            }

            StatusText = $"Installed {completed} of {selectedApps.Count} applications";
            IsOperationInProgress = false;
            CurrentOperationText = string.Empty;
            CurrentOperationProgress = 0;
        }

        private void CancelOperation()
        {
            _downloadService.CancelDownloads();
            _installationService.CancelInstallations();
            StatusText = "Operation cancelled";
            IsOperationInProgress = false;
            _loggingService.LogWarning("User cancelled operation");
        }

        private void ExportLog()
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                FileName = $"MoonLight_Log_{DateTime.Now:yyyyMMdd_HHmmss}.txt"
            };

            if (dialog.ShowDialog() == true)
            {
                File.WriteAllLines(dialog.FileName, LogEntries);
                _loggingService.LogInfo($"Log exported to: {dialog.FileName}");
            }
        }

        private void BrowseDownloadLocation()
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog
            {
                Description = "Select download location",
                SelectedPath = InstallationOptions.DownloadLocation
            };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                InstallationOptions.DownloadLocation = dialog.SelectedPath;
                _loggingService.LogInfo($"Download location changed to: {dialog.SelectedPath}");
            }
        }

        private void BrowseSingleModeInstaller()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Installer files (*.exe;*.msi)|*.exe;*.msi|All files (*.*)|*.*",
                Title = "Select installer file"
            };

            if (dialog.ShowDialog() == true)
            {
                SingleModeInstallerPath = dialog.FileName;
                if (AutoDetectInstallerType && SelectedSingleSoftware == null)
                {
                    SingleModeInstallArguments = DetectInstallerArguments(dialog.FileName);
                }
            }
        }

        private string DetectInstallerArguments(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            if (extension == ".msi")
            {
                return "/quiet /norestart";
            }
            return "/S";
        }

        private List<Software> GetSelectedApplications()
        {
            if (IsBatchMode)
            {
                return Categories.SelectMany(c => c.Applications).Where(a => a.IsSelected).ToList();
            }
            else if (!string.IsNullOrWhiteSpace(SingleModeInstallerPath))
            {
                var software = SelectedSingleSoftware ?? new Software
                {
                    Id = "custom",
                    Name = Path.GetFileNameWithoutExtension(SingleModeInstallerPath),
                    Category = "Custom",
                    DownloadUrl = SingleModeInstallerPath,
                    InstallArguments = SingleModeInstallArguments,
                    InstallerType = DetectInstallerType(SingleModeInstallerPath),
                    EstimatedSizeMB = 0
                };
                
                if (!SingleModeInstallerPath.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    software.DownloadUrl = "file://" + SingleModeInstallerPath;
                }
                software.InstallArguments = SingleModeInstallArguments;
                
                return new List<Software> { software };
            }
            
            return new List<Software>();
        }

        private InstallerType DetectInstallerType(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            return extension == ".msi" ? InstallerType.MSI : InstallerType.EXE;
        }

        private void OnLogAdded(object? sender, string logEntry)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                LogEntries.Add(logEntry);
                if (LogEntries.Count > 1000)
                {
                    LogEntries.RemoveAt(0);
                }
            });
        }
    }
}