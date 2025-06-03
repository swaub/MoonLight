using MoonLight.Models;

namespace MoonLight.Services
{
    public interface IDownloadService
    {
        Task<string> DownloadFileAsync(Software software, InstallationOptions options, IProgress<DownloadProgress>? progress = null);
        void CancelDownloads();
    }
}