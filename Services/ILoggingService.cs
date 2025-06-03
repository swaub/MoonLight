namespace MoonLight.Services
{
    public interface ILoggingService
    {
        event EventHandler<string>? LogAdded;
        void LogInfo(string message);
        void LogWarning(string message);
        void LogError(string message);
        void LogDebug(string message);
    }
}