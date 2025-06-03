using System.Diagnostics;
using System.IO;
using System.Management;
using Microsoft.Win32;
using MoonLight.Models;

namespace MoonLight.Services
{
    public class InstallationService : IInstallationService
    {
        private readonly ILoggingService _loggingService;
        private readonly IDownloadService _downloadService;
        private CancellationTokenSource? _cancellationTokenSource;

        public InstallationService()
        {
            _loggingService = new LoggingService();
            _downloadService = new DownloadService();
        }

        public async Task<bool> InstallApplicationAsync(Software software, InstallationOptions options)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = _cancellationTokenSource.Token;

            try
            {
                string installerPath;
                
                if (software.DownloadUrl.StartsWith("file://", StringComparison.OrdinalIgnoreCase))
                {
                    installerPath = software.DownloadUrl.Substring(7);
                    if (!File.Exists(installerPath))
                    {
                        throw new FileNotFoundException($"Installer not found: {installerPath}");
                    }
                }
                else
                {
                    var fileName = GetFileNameFromUrl(software.DownloadUrl) ?? $"{software.Name.Replace(" ", "")}_installer.exe";
                    installerPath = Path.Combine(options.DownloadLocation, fileName);
                    
                    if (!File.Exists(installerPath))
                    {
                        _loggingService.LogInfo($"Installer not found locally, downloading: {software.Name}");
                        installerPath = await _downloadService.DownloadFileAsync(software, options);
                    }
                }

                _loggingService.LogInfo($"Starting installation: {software.Name}");

                var processInfo = new ProcessStartInfo
                {
                    FileName = GetInstallerExecutable(installerPath, software.InstallerType),
                    Arguments = GetInstallArguments(installerPath, software),
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                if (options.RunAsAdministrator)
                {
                    processInfo.Verb = "runas";
                    processInfo.UseShellExecute = true;
                    processInfo.RedirectStandardOutput = false;
                    processInfo.RedirectStandardError = false;
                }

                using var process = Process.Start(processInfo);
                if (process == null)
                {
                    throw new InvalidOperationException("Failed to start installer process");
                }

                var completed = await Task.Run(() => process.WaitForExit(options.TimeoutSeconds * 1000), cancellationToken);
                
                if (!completed)
                {
                    process.Kill();
                    throw new TimeoutException($"Installation timed out after {options.TimeoutSeconds} seconds");
                }

                if (process.ExitCode != 0)
                {
                    _loggingService.LogWarning($"Installer returned non-zero exit code: {process.ExitCode}");
                }

                if (options.DisableAutoStart)
                {
                    await DisableAutoStartAsync(software.Name);
                }

                if (options.CleanUpInstallers && File.Exists(installerPath))
                {
                    try
                    {
                        File.Delete(installerPath);
                        _loggingService.LogInfo($"Cleaned up installer: {Path.GetFileName(installerPath)}");
                    }
                    catch (Exception ex)
                    {
                        _loggingService.LogWarning($"Failed to clean up installer: {ex.Message}");
                    }
                }

                _loggingService.LogInfo($"Successfully installed: {software.Name}");
                return true;
            }
            catch (OperationCanceledException)
            {
                _loggingService.LogWarning($"Installation cancelled: {software.Name}");
                return false;
            }
            catch (Exception ex)
            {
                _loggingService.LogError($"Installation failed for {software.Name}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> CreateSystemRestorePointAsync()
        {
            try
            {
                return await Task.Run(() =>
                {
                    var scope = new ManagementScope(@"\\localhost\root\default");
                    using var systemRestore = new ManagementClass(scope, new ManagementPath("SystemRestore"), null);
                    using var inParams = systemRestore.GetMethodParameters("CreateRestorePoint");
                    
                    inParams["Description"] = $"MoonLight Installation - {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
                    inParams["RestorePointType"] = 0; // APPLICATION_INSTALL
                    inParams["EventType"] = 100; // BEGIN_SYSTEM_CHANGE

                    using var outParams = systemRestore.InvokeMethod("CreateRestorePoint", inParams, null);
                    var returnValue = Convert.ToInt32(outParams["ReturnValue"]);
                    
                    if (returnValue == 0)
                    {
                        _loggingService.LogInfo("System restore point created successfully");
                        return true;
                    }
                    else
                    {
                        _loggingService.LogWarning($"Failed to create system restore point. Return value: {returnValue}");
                        return false;
                    }
                });
            }
            catch (Exception ex)
            {
                _loggingService.LogError($"Error creating system restore point: {ex.Message}");
                return false;
            }
        }

        public void CancelInstallations()
        {
            _cancellationTokenSource?.Cancel();
        }

        private string GetInstallerExecutable(string installerPath, InstallerType installerType)
        {
            return installerType == InstallerType.MSI ? "msiexec.exe" : installerPath;
        }

        private string GetInstallArguments(string installerPath, Software software)
        {
            if (software.InstallerType == InstallerType.MSI)
            {
                return $"/i \"{installerPath}\" {software.InstallArguments}";
            }
            return software.InstallArguments;
        }

        private async Task DisableAutoStartAsync(string applicationName)
        {
            await Task.Run(() =>
            {
                try
                {
                    var runKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
                    if (runKey != null)
                    {
                        var valuesToRemove = new List<string>();
                        foreach (var valueName in runKey.GetValueNames())
                        {
                            var value = runKey.GetValue(valueName)?.ToString() ?? "";
                            if (value.Contains(applicationName, StringComparison.OrdinalIgnoreCase))
                            {
                                valuesToRemove.Add(valueName);
                            }
                        }

                        foreach (var valueName in valuesToRemove)
                        {
                            runKey.DeleteValue(valueName, false);
                            _loggingService.LogInfo($"Removed auto-start entry: {valueName}");
                        }
                        runKey.Close();
                    }

                    var runKeyLM = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
                    if (runKeyLM != null)
                    {
                        var valuesToRemove = new List<string>();
                        foreach (var valueName in runKeyLM.GetValueNames())
                        {
                            var value = runKeyLM.GetValue(valueName)?.ToString() ?? "";
                            if (value.Contains(applicationName, StringComparison.OrdinalIgnoreCase))
                            {
                                valuesToRemove.Add(valueName);
                            }
                        }

                        foreach (var valueName in valuesToRemove)
                        {
                            runKeyLM.DeleteValue(valueName, false);
                            _loggingService.LogInfo($"Removed auto-start entry: {valueName}");
                        }
                        runKeyLM.Close();
                    }
                }
                catch (Exception ex)
                {
                    _loggingService.LogWarning($"Failed to disable auto-start for {applicationName}: {ex.Message}");
                }
            });
        }

        private string? GetFileNameFromUrl(string url)
        {
            try
            {
                // Handle special cases for download URLs
                if (url.Contains("chrome", StringComparison.OrdinalIgnoreCase))
                    return "ChromeSetup.exe";
                if (url.Contains("firefox", StringComparison.OrdinalIgnoreCase))
                    return "FirefoxSetup.exe";
                if (url.Contains("edge", StringComparison.OrdinalIgnoreCase))
                    return "EdgeSetup.msi";
                if (url.Contains("discord", StringComparison.OrdinalIgnoreCase))
                    return "DiscordSetup.exe";
                if (url.Contains("teams", StringComparison.OrdinalIgnoreCase))
                    return "TeamsSetup.exe";
                if (url.Contains("vscode", StringComparison.OrdinalIgnoreCase) || url.Contains("code.visualstudio", StringComparison.OrdinalIgnoreCase))
                    return "VSCodeSetup.exe";
                if (url.Contains("spotify", StringComparison.OrdinalIgnoreCase))
                    return "SpotifySetup.exe";
                    
                var uri = new Uri(url);
                var fileName = Path.GetFileName(uri.LocalPath);
                
                // If we get a generic name or no extension, add .exe
                if (string.IsNullOrWhiteSpace(fileName) || !fileName.Contains('.'))
                {
                    return "installer.exe";
                }
                
                return fileName;
            }
            catch
            {
                return "installer.exe";
            }
        }
    }
}