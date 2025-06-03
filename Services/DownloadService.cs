using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using MoonLight.Models;

namespace MoonLight.Services
{
    public class DownloadService : IDownloadService
    {
        private readonly HttpClient _httpClient;
        private CancellationTokenSource? _cancellationTokenSource;
        private readonly ILoggingService _loggingService;

        public DownloadService()
        {
            _loggingService = new LoggingService();
            var handler = new HttpClientHandler
            {
                UseProxy = WebRequest.DefaultWebProxy != null,
                Proxy = WebRequest.DefaultWebProxy,
                AllowAutoRedirect = true
            };
            _httpClient = new HttpClient(handler) { Timeout = TimeSpan.FromMinutes(30) };
            
            // Add user agent to avoid 403 errors
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
        }

        public async Task<string> DownloadFileAsync(Software software, InstallationOptions options, IProgress<DownloadProgress>? progress = null)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = _cancellationTokenSource.Token;

            try
            {
                if (software.DownloadUrl.StartsWith("file://", StringComparison.OrdinalIgnoreCase))
                {
                    var localPath = software.DownloadUrl.Substring(7);
                    if (File.Exists(localPath))
                    {
                        _loggingService.LogInfo($"Using local installer: {localPath}");
                        return localPath;
                    }
                    throw new FileNotFoundException($"Local installer not found: {localPath}");
                }

                Directory.CreateDirectory(options.DownloadLocation);
                var fileName = GetFileNameFromUrl(software.DownloadUrl) ?? $"{software.Name.Replace(" ", "")}_installer.exe";
                var filePath = Path.Combine(options.DownloadLocation, fileName);

                if (File.Exists(filePath))
                {
                    _loggingService.LogInfo($"Installer already exists: {fileName}");
                    return filePath;
                }

                var tempPath = filePath + ".tmp";
                long existingLength = 0;

                if (File.Exists(tempPath))
                {
                    existingLength = new FileInfo(tempPath).Length;
                }

                using var request = new HttpRequestMessage(HttpMethod.Get, software.DownloadUrl);
                if (existingLength > 0)
                {
                    request.Headers.Range = new System.Net.Http.Headers.RangeHeaderValue(existingLength, null);
                }

                using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
                
                if (response.StatusCode == HttpStatusCode.RequestedRangeNotSatisfiable)
                {
                    File.Move(tempPath, filePath, true);
                    return filePath;
                }

                response.EnsureSuccessStatusCode();

                var totalBytes = response.Content.Headers.ContentLength ?? -1;
                if (response.StatusCode == HttpStatusCode.PartialContent)
                {
                    totalBytes += existingLength;
                }

                using (var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken))
                using (var fileStream = new FileStream(tempPath, existingLength > 0 ? FileMode.Append : FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    var buffer = new byte[8192];
                    var downloadedBytes = existingLength;
                    var stopwatch = Stopwatch.StartNew();
                    var lastProgressUpdate = DateTime.Now;

                    while (true)
                    {
                        var bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                        if (bytesRead == 0) break;

                        await fileStream.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                        downloadedBytes += bytesRead;

                        if ((DateTime.Now - lastProgressUpdate).TotalMilliseconds >= 100 && progress != null)
                        {
                            var downloadSpeed = downloadedBytes / stopwatch.Elapsed.TotalSeconds / 1024 / 1024;
                            var remainingBytes = totalBytes - downloadedBytes;
                            var estimatedTime = downloadSpeed > 0 ? TimeSpan.FromSeconds(remainingBytes / (downloadSpeed * 1024 * 1024)) : TimeSpan.Zero;

                            progress.Report(new DownloadProgress
                            {
                                FileName = fileName,
                                TotalBytes = totalBytes,
                                DownloadedBytes = downloadedBytes,
                                DownloadSpeedMBps = downloadSpeed,
                                EstimatedTimeRemaining = estimatedTime,
                                Status = DownloadStatus.Downloading,
                                StatusMessage = $"Downloading {fileName}"
                            });

                            lastProgressUpdate = DateTime.Now;
                        }
                    }
                }

                // Ensure file is closed before moving
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                File.Move(tempPath, filePath);
                
                progress?.Report(new DownloadProgress
                {
                    FileName = fileName,
                    TotalBytes = totalBytes,
                    DownloadedBytes = totalBytes,
                    Status = DownloadStatus.Completed,
                    StatusMessage = $"Completed: {fileName}"
                });

                return filePath;
            }
            catch (OperationCanceledException)
            {
                _loggingService.LogWarning($"Download cancelled: {software.Name}");
                throw;
            }
            catch (Exception ex)
            {
                _loggingService.LogError($"Download failed for {software.Name}: {ex.Message}");
                progress?.Report(new DownloadProgress
                {
                    Status = DownloadStatus.Failed,
                    StatusMessage = $"Failed: {ex.Message}"
                });
                throw;
            }
        }

        public void CancelDownloads()
        {
            _cancellationTokenSource?.Cancel();
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