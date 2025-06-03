using System.Collections.ObjectModel;
using MoonLight.Models;

namespace MoonLight.Services
{
    public class SoftwareCatalogService : ISoftwareCatalogService
    {
        private readonly Dictionary<string, Software> _softwareDict = new Dictionary<string, Software>();
        private readonly List<SoftwareCategory> _categories = new List<SoftwareCategory>();

        public SoftwareCatalogService()
        {
            InitializeCatalog();
        }

        public List<SoftwareCategory> GetSoftwareCatalog()
        {
            return _categories;
        }

        public Software? GetSoftwareById(string id)
        {
            return _softwareDict.TryGetValue(id, out var software) ? software : null;
        }

        private void InitializeCatalog()
        {
            var webBrowsers = new SoftwareCategory { Name = "Web Browsers" };
            AddSoftware(webBrowsers, "chrome", "Google Chrome", "Most popular web browser with excellent performance and extension support",
                "https://dl.google.com/chrome/install/ChromeStandaloneSetup64.exe", "/silent /install", InstallerType.EXE, 85);
            AddSoftware(webBrowsers, "firefox", "Mozilla Firefox", "Open-source browser focused on privacy and customization",
                "https://download.mozilla.org/?product=firefox-stub&os=win&lang=en-US", "/S", InstallerType.EXE, 55);
            AddSoftware(webBrowsers, "edge", "Microsoft Edge", "Microsoft's modern browser built on Chromium engine",
                "https://msedge.sf.dl.delivery.mp.microsoft.com/filestreamingservice/files/latest/MicrosoftEdgeEnterpriseX64.msi", "/quiet", InstallerType.MSI, 110);
            AddSoftware(webBrowsers, "opera", "Opera Browser", "Feature-rich browser with built-in VPN and ad blocking",
                "https://download.opera.com/download/get/?id=42784&location=413&nothanks=yes&sub=marine", "--all-users --silent=1 --launch-browser=0", InstallerType.EXE, 80);
            AddSoftware(webBrowsers, "brave", "Brave Browser", "Privacy-focused browser that blocks ads and trackers by default",
                "https://laptop-updates.brave.com/latest/winx64", "/silent /install", InstallerType.EXE, 90);
            _categories.Add(webBrowsers);

            var communication = new SoftwareCategory { Name = "Communication" };
            AddSoftware(communication, "discord", "Discord", "Popular voice and text chat platform for communities and gaming",
                "https://discord.com/api/downloads/distributions/app/installers/latest?channel=stable&platform=win&arch=x86", "-s", InstallerType.EXE, 75);
            AddSoftware(communication, "teams", "Microsoft Teams", "Business communication and collaboration platform",
                "https://go.microsoft.com/fwlink/?linkid=2196106", "-p", InstallerType.EXE, 120);
            AddSoftware(communication, "zoom", "Zoom", "Video conferencing and online meeting platform",
                "https://zoom.us/client/5.17.5.2543/ZoomInstallerFull.msi?archType=x64", "/quiet /qn /norestart", InstallerType.MSI, 60);
            AddSoftware(communication, "anydesk", "AnyDesk Remote Desktop", "Fast remote desktop software for technical support and access",
                "https://download.anydesk.com/AnyDesk.exe", "--install \"C:\\Program Files (x86)\\AnyDesk\" --silent --start-with-win --create-shortcuts --create-desktop-icon", InstallerType.EXE, 4);
            AddSoftware(communication, "parsec", "Parsec Gaming", "Low-latency remote desktop optimized for gaming and creative work",
                "https://builds.parsecgaming.com/package/parsec-windows.exe", "/silent /percomputer", InstallerType.EXE, 20);
            _categories.Add(communication);

            var development = new SoftwareCategory { Name = "Development Tools" };
            AddSoftware(development, "vscode", "Visual Studio Code", "Lightweight but powerful source code editor with extensive extension support",
                "https://code.visualstudio.com/sha/download?build=stable&os=win32-x64-user", "/VERYSILENT /MERGETASKS=!runcode", InstallerType.InnoSetup, 85);
            AddSoftware(development, "vs2022", "Visual Studio 2022 Community", "Full-featured IDE for Windows development with comprehensive tooling",
                "https://aka.ms/vs/17/release/vs_community.exe", "--add Microsoft.VisualStudio.Workload.CoreEditor --quiet --wait --norestart", InstallerType.EXE, 1500);
            AddSoftware(development, "python", "Python 3.13", "Popular programming language with extensive library ecosystem",
                "https://www.python.org/ftp/python/3.13.0/python-3.13.0-amd64.exe", "/quiet InstallAllUsers=1 PrependPath=1 DefaultCustomInstall=1", InstallerType.EXE, 25);
            AddSoftware(development, "notepadpp", "Notepad++", "Advanced text and source code editor with syntax highlighting",
                "https://github.com/notepad-plus-plus/notepad-plus-plus/releases/download/v8.6/npp.8.6.Installer.x64.exe", "/S", InstallerType.NSIS, 15);
            _categories.Add(development);

            var gaming = new SoftwareCategory { Name = "Gaming" };
            AddSoftware(gaming, "steam", "Steam", "Leading digital game distribution platform",
                "https://cdn.akamai.steamstatic.com/client/installer/SteamSetup.exe", "/S", InstallerType.NSIS, 3);
            AddSoftware(gaming, "epicgames", "Epic Games Launcher", "Digital storefront and launcher for Epic Games titles",
                "https://launcher-public-service-prod06.ol.epicgames.com/launcher/api/installer/download/EpicGamesLauncherInstaller.msi", "/S", InstallerType.MSI, 120);
            AddSoftware(gaming, "gog", "GOG Galaxy", "DRM-free game platform with optional client features",
                "https://webinstallers.gog.com/download/GOG_Galaxy_2.0.exe", "/VERYSILENT /SUPPRESSMSGBOXES /NORESTART /SP-", InstallerType.InnoSetup, 300);
            AddSoftware(gaming, "battlenet", "Battle.net", "Blizzard Entertainment's game launcher and social platform",
                "https://www.battle.net/download/getInstallerForGame?os=win&locale=enUS&version=LIVE&gameProgram=BATTLENET_APP", "--lang=enUS --installpath=\"C:\\Program Files (x86)\\Battle.net\" --silent", InstallerType.EXE, 25);
            AddSoftware(gaming, "ubisoft", "Ubisoft Connect", "Ubisoft's game launcher and digital distribution platform",
                "https://ubistatic3-a.akamaihd.net/orbit/launcher_installer/UbisoftConnectInstaller.exe", "/S", InstallerType.NSIS, 100);
            _categories.Add(gaming);

            var media = new SoftwareCategory { Name = "Media and Entertainment" };
            AddSoftware(media, "vlc", "VLC Media Player", "Versatile media player supporting virtually all video and audio formats",
                "https://get.videolan.org/vlc/3.0.20/win64/vlc-3.0.20-win64.exe", "/S /L=1033", InstallerType.NSIS, 40);
            AddSoftware(media, "spotify", "Spotify", "Music streaming service with millions of songs and podcasts",
                "https://download.scdn.co/SpotifySetup.exe", "/silent", InstallerType.EXE, 120);
            AddSoftware(media, "itunes", "iTunes", "Apple's media player and device management software",
                "https://secure-appldnld.apple.com/itunes12/001-97787-20210421-F0E5A3C2-A2C9-11EB-A40B-A128318AD179/iTunes64Setup.exe", "/quiet /norestart", InstallerType.EXE, 240);
            AddSoftware(media, "audacity", "Audacity", "Free, open-source audio editing and recording software",
                "https://github.com/audacity/audacity/releases/download/Audacity-3.4.2/audacity-win-3.4.2-64bit.exe", "/VERYSILENT /NORESTART", InstallerType.InnoSetup, 35);
            AddSoftware(media, "kodi", "Kodi Media Center", "Open-source media center for organizing and playing media content",
                "https://mirrors.kodi.tv/releases/windows/win64/kodi-20.3-Nexus-x64.exe", "/S", InstallerType.NSIS, 85);
            _categories.Add(media);

            var utilities = new SoftwareCategory { Name = "System Utilities" };
            AddSoftware(utilities, "7zip", "7-Zip Archive Manager", "High-compression file archiver supporting multiple formats",
                "https://www.7-zip.org/a/7z2404-x64.exe", "/S", InstallerType.EXE, 3);
            AddSoftware(utilities, "winrar", "WinRAR", "Popular file compression and extraction utility",
                "https://www.win-rar.com/fileadmin/winrar-versions/winrar/winrar-x64-700.exe", "/S", InstallerType.EXE, 3);
            AddSoftware(utilities, "cpuz", "CPU-Z System Information", "System information utility showing detailed hardware specifications",
                "https://download.cpuid.com/cpu-z/cpu-z_2.12-en.exe", "/SILENT", InstallerType.NSIS, 9);
            AddSoftware(utilities, "ccleaner", "CCleaner System Cleaner", "System optimization and privacy tool for cleaning temporary files",
                "https://download.ccleaner.com/ccsetup600.exe", "/S", InstallerType.NSIS, 40);
            AddSoftware(utilities, "hwinfo", "HWiNFO Hardware Information", "Comprehensive hardware analysis and monitoring tool",
                "https://www.sac.sk/download/utildiag/hwi_758.exe", "/SILENT", InstallerType.NSIS, 12);
            _categories.Add(utilities);

            var security = new SoftwareCategory { Name = "Security" };
            AddSoftware(security, "malwarebytes", "Malwarebytes Anti-Malware", "Anti-malware software specializing in detecting and removing threats",
                "https://www.malwarebytes.com/api/downloads/mb-windows", "/VERYSILENT /SUPPRESSMSGBOXES /NORESTART /SP-", InstallerType.InnoSetup, 80);
            AddSoftware(security, "avast", "Avast Free Antivirus", "Free antivirus protection with real-time scanning and threat detection",
                "https://www.avast.com/en-us/download-thank-you.php?product=fav&locale=en-us", "", InstallerType.EXE, 280);
            _categories.Add(security);

            var cloudStorage = new SoftwareCategory { Name = "Cloud Storage" };
            AddSoftware(cloudStorage, "dropbox", "Dropbox", "Cloud storage service with file synchronization and sharing",
                "https://www.dropbox.com/download?plat=win&full=1", "/S", InstallerType.EXE, 140);
            AddSoftware(cloudStorage, "onedrive", "Microsoft OneDrive", "Microsoft's cloud storage integrated with Windows and Office",
                "https://go.microsoft.com/fwlink/?linkid=844652", "/silent /allusers", InstallerType.EXE, 30);
            AddSoftware(cloudStorage, "googledrive", "Google Drive", "Google's cloud storage with desktop synchronization client",
                "https://dl.google.com/drive-file-stream/GoogleDriveSetup.exe", "", InstallerType.EXE, 60);
            _categories.Add(cloudStorage);

            var fileTransfer = new SoftwareCategory { Name = "File Transfer" };
            AddSoftware(fileTransfer, "filezilla", "FileZilla FTP Client", "Open-source FTP, FTPS, and SFTP client",
                "https://download.filezilla-project.org/client/FileZilla_3.66.4_win64_sponsored-setup.exe", "/S", InstallerType.EXE, 15);
            AddSoftware(fileTransfer, "winscp", "WinSCP Secure Copy", "SFTP and SCP client for secure file transfer",
                "https://winscp.net/download/WinSCP-6.1.2-Setup.exe", "/VERYSILENT", InstallerType.InnoSetup, 10);
            AddSoftware(fileTransfer, "putty", "PuTTY SSH Client", "SSH and telnet client for secure remote connections",
                "https://the.earth.li/~sgtatham/putty/latest/w64/putty-64bit-0.79-installer.msi", "/q", InstallerType.MSI, 5);
            _categories.Add(fileTransfer);

            var productivity = new SoftwareCategory { Name = "Productivity" };
            AddSoftware(productivity, "libreoffice", "LibreOffice Suite", "Free office suite compatible with Microsoft Office formats",
                "https://download.libreoffice.org/libreoffice/stable/7.6.4/win/x86_64/LibreOffice_7.6.4_Win_x86-64.msi", "/qn /norestart", InstallerType.MSI, 320);
            AddSoftware(productivity, "foxit", "Foxit PDF Reader", "Fast PDF reader with annotation and form-filling capabilities",
                "https://www.foxit.com/downloads/latest.html?product=Foxit-Reader&platform=Windows&version=&package_type=exe&language=English", "/verysilent", InstallerType.EXE, 110);
            AddSoftware(productivity, "sumatra", "SumatraPDF Reader", "Lightweight PDF, eBook, and document viewer",
                "https://www.sumatrapdfreader.org/dl/rel/3.4.6/SumatraPDF-3.4.6-64-install.exe", "-s -all-users", InstallerType.EXE, 6);
            _categories.Add(productivity);

            var runtimes = new SoftwareCategory { Name = "Runtimes and Dependencies" };
            AddSoftware(runtimes, "vcredist64", "Visual C++ 2015-2022 Redistributable x64", "Required runtime components for applications built with Visual Studio",
                "https://aka.ms/vs/17/release/vc_redist.x64.exe", "/install /quiet /norestart", InstallerType.EXE, 25);
            AddSoftware(runtimes, "vcredist86", "Visual C++ 2015-2022 Redistributable x86", "32-bit runtime components for legacy applications",
                "https://aka.ms/vs/17/release/vc_redist.x86.exe", "/install /quiet /norestart", InstallerType.EXE, 14);
            AddSoftware(runtimes, "dotnet8", ".NET Desktop Runtime 8.0 x64", "Runtime for .NET 8 desktop applications",
                "https://download.microsoft.com/download/a/b/c/abc123/windowsdesktop-runtime-8.0.0-win-x64.exe", "/install /quiet /norestart", InstallerType.EXE, 65);
            AddSoftware(runtimes, "dotnet9", ".NET Desktop Runtime 9.0 x64", "Latest .NET runtime for modern desktop applications",
                "https://download.microsoft.com/download/a/b/c/abc123/windowsdesktop-runtime-9.0.0-win-x64.exe", "/install /quiet /norestart", InstallerType.EXE, 70);
            AddSoftware(runtimes, "java17", "Java AdoptOpenJDK 17 x64", "Open-source Java development kit and runtime",
                "https://github.com/adoptium/temurin17-binaries/releases/download/jdk-17.0.9%2B9/OpenJDK17U-jdk_x64_windows_hotspot_17.0.9_9.msi", "/qn /norestart", InstallerType.MSI, 170);
            AddSoftware(runtimes, "dotnet6", ".NET Desktop Runtime 6.0", "Runtime for .NET 6 applications with long-term support",
                "https://download.visualstudio.microsoft.com/download/pr/b9cfcd1b-8f26-4b95-8a81-dbd07383fa2e/3df1b2dde5c2cdb2772151f06a62f1b8/windowsdesktop-runtime-6.0.27-win-x64.exe", "/install /quiet /norestart", InstallerType.EXE, 60);
            _categories.Add(runtimes);

            var network = new SoftwareCategory { Name = "Network and Torrents" };
            AddSoftware(network, "qbittorrent", "qBittorrent", "Open-source BitTorrent client with clean interface",
                "https://sourceforge.net/projects/qbittorrent/files/qbittorrent-win32/qbittorrent-4.6.2/qbittorrent_4.6.2_x64_setup.exe", "/S", InstallerType.NSIS, 45);
            _categories.Add(network);

            var creative = new SoftwareCategory { Name = "Creative Software" };
            AddSoftware(creative, "gimp", "GIMP Image Editor", "Powerful open-source image editing and photo manipulation software",
                "https://download.gimp.org/mirror/pub/gimp/v2.10/windows/gimp-2.10.36-setup.exe", "/VERYSILENT /SUPPRESSMSGBOXES /NORESTART /SP-", InstallerType.InnoSetup, 250);
            AddSoftware(creative, "paintnet", "Paint.NET", "User-friendly image and photo editing software with layer support",
                "https://www.dotpdn.com/downloads/pdn.html", "/auto", InstallerType.EXE, 40);
            AddSoftware(creative, "blender", "Blender 3D Creation Suite", "Professional 3D modeling, animation, and rendering software",
                "https://www.blender.org/download/release/Blender4.0/blender-4.0.2-windows-x64.msi/", "/qb", InstallerType.MSI, 350);
            _categories.Add(creative);
        }

        private void AddSoftware(SoftwareCategory category, string id, string name, string description,
            string downloadUrl, string installArgs, InstallerType installerType, int sizeMB)
        {
            var software = new Software
            {
                Id = id,
                Name = name,
                Description = description,
                Category = category.Name,
                DownloadUrl = downloadUrl,
                InstallArguments = installArgs,
                InstallerType = installerType,
                EstimatedSizeMB = sizeMB,
                IsSelected = false
            };

            software.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(Software.IsSelected))
                {
                    category.NotifySelectedCountChanged();
                }
            };

            category.Applications.Add(software);
            _softwareDict[id] = software;
        }
    }
}