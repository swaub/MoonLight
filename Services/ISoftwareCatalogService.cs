using MoonLight.Models;

namespace MoonLight.Services
{
    public interface ISoftwareCatalogService
    {
        List<SoftwareCategory> GetSoftwareCatalog();
        Software? GetSoftwareById(string id);
    }
}