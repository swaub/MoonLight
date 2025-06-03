namespace MoonLight.Models
{
    public class DownloadProgress
    {
        public string FileName { get; set; } = string.Empty;
        public long TotalBytes { get; set; }
        public long DownloadedBytes { get; set; }
        public double ProgressPercentage => TotalBytes > 0 ? (DownloadedBytes * 100.0) / TotalBytes : 0;
        public double DownloadSpeedMBps { get; set; }
        public TimeSpan EstimatedTimeRemaining { get; set; }
        public DownloadStatus Status { get; set; }
        public string StatusMessage { get; set; } = string.Empty;
    }

    public enum DownloadStatus
    {
        Pending,
        Downloading,
        Completed,
        Failed,
        Cancelled
    }
}