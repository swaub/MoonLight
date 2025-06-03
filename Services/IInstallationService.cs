using MoonLight.Models;

namespace MoonLight.Services
{
    public interface IInstallationService
    {
        Task<bool> InstallApplicationAsync(Software software, InstallationOptions options);
        Task<bool> CreateSystemRestorePointAsync();
        void CancelInstallations();
    }
}