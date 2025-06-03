================================================================================
                    MOONLIGHT SILENT INSTALLER - C# IMPLEMENTATION
                             PROJECT INSTRUCTIONS
================================================================================

PROJECT OVERVIEW
=================

Create a native C# application that replicates the functionality of a Windows 
software installer management tool. This application should provide a modern, 
efficient alternative to services like Ninite, with enhanced features and 
better performance characteristics.

The application will be built using WPF .NET 8.0 for the graphical user interface to 
ensure cross-platform compatibility while maintaining native performance 
characteristics.

CORE FUNCTIONALITY REQUIREMENTS
================================

Primary Purpose
---------------
The application manages the automated downloading and silent installation of 
popular Windows software packages. Users can select multiple applications from 
a categorized list, customize installation options, and execute batch 
operations with minimal user intervention.

User Interface Requirements
---------------------------

Main Window Layout
The interface should be organized into distinct functional areas:

- Header Section: Application branding, version information, and brief 
  description displayed in a prominent banner area
- Mode Selection: Toggle between batch installation (multiple apps) and single 
  installation modes using radio buttons or tab interface
- Search Interface: Real-time filtering of available applications with text 
  entry field and clear button
- Application Selection Grid: Categorized display of available software with 
  checkboxes organized in a scrollable tree view or grid layout
- Options Panel: Installation and download configuration controls presented 
  in organized groups with appropriate widges
- Action Controls: Download, install, and cancel operation buttons prominently 
  displayed with clear visual hierarchy
- Progress Display: Dual progress bars for overall and current operation 
  status with percentage and time estimates
- Logging Area: Real-time operation feedback and detailed logging output in 
  scrollable text view with syntax highlighting
- Status Bar: Current operation status and application version displayed at 
  bottom of window

Visual Design Principles
Implement a clean, modern interface with professional color scheme. Use blue 
primary colors (#3498DB, #2980B9) with green accent (#2ECC71) for action 
elements. Maintain consistent spacing, typography, and visual hierarchy 
throughout the interface and theming capabilities.

Application Categories and Software Database
============================================

Category Structure
------------------
Organize software into logical groupings:

WEB BROWSERS
- Google Chrome
- Mozilla Firefox  
- Microsoft Edge
- Opera Browser
- Brave Browser

COMMUNICATION
- Discord
- Microsoft Teams
- Zoom
- AnyDesk Remote Desktop
- Parsec Gaming

DEVELOPMENT TOOLS
- Visual Studio Code
- Visual Studio 2022 Community
- Python 3.13
- Notepad++

GAMING
- Steam
- Epic Games Launcher
- GOG Galaxy
- Battle.net
- Ubisoft Connect

MEDIA AND ENTERTAINMENT
- VLC Media Player
- Spotify
- iTunes
- Audacity
- Kodi Media Center

SYSTEM UTILITIES
- 7-Zip Archive Manager
- WinRAR
- CPU-Z System Information
- CCleaner System Cleaner
- HWiNFO Hardware Information

SECURITY
- Malwarebytes Anti-Malware
- Avast Free Antivirus

CLOUD STORAGE
- Dropbox
- Microsoft OneDrive
- Google Drive

FILE TRANSFER
- FileZilla FTP Client
- WinSCP Secure Copy
- PuTTY SSH Client

PRODUCTIVITY
- LibreOffice Suite
- Foxit PDF Reader
- SumatraPDF Reader

RUNTIMES AND DEPENDENCIES
- Visual C++ 2015-2022 Redistributable x64
- Visual C++ 2015-2022 Redistributable x86
- .NET Desktop Runtime 8.0 x64
- .NET Desktop Runtime 9.0 x64
- Java AdoptOpenJDK 17 x64
- .NET Desktop Runtime 6.0

NETWORK AND TORRENTS
- qBittorrent

CREATIVE SOFTWARE
- GIMP Image Editor
- Paint.NET
- Blender 3D Creation Suite

Application Data Structure
Each application entry must contain:
- Application name and display information for user interface
- Category assignment for organizational purposes
- Download URL providing direct link to installer package
- Silent installation arguments specific to installer type and framework
- Estimated download size in megabytes for user planning
- Installer type classification (MSI, EXE, InnoSetup, NSIS, etc.)

INSTALLATION MANAGEMENT SYSTEM
===============================

Installer Type Detection and Handling
--------------------------------------
Implement automatic detection for major installer frameworks:

MSI INSTALLERS
Execute via msiexec.exe with standardized silent parameters. Use arguments 
like "/quiet /norestart ALLUSERS=1" for system-wide silent installation.

INNOSETUP INSTALLERS  
Use "/VERYSILENT /SUPPRESSMSGBOXES /NORESTART /SP-" arguments for completely 
silent installation without user prompts.

NSIS INSTALLERS
Apply "/S" silent installation flag which is the standard for Nullsoft 
Installer System packages.

GENERIC EXE INSTALLERS
Use "/quiet /norestart" as fallback arguments for unknown installer types, 
with automatic detection attempting to identify specific frameworks through 
file header analysis.

Silent Installation Arguments
Provide pre-configured silent installation arguments for each application, 
with automatic detection fallback when possible. Support argument 
customization for advanced users who need specific installation behaviors.

Installation Options
Implement comprehensive installation control options:

SYSTEM RESTORE POINTS
Create restoration points before installation begins using Windows System 
Restore APIs to provide safety net for users.

SHORTCUT MANAGEMENT  
Disable desktop and start menu shortcut creation through installer arguments 
and post-installation cleanup when arguments are insufficient.

AUTO-START CONTROL
Prevent applications from automatically starting with Windows by modifying 
registry entries and startup folder contents after installation.

INSTALLER CLEANUP
Automatically remove downloaded installers after successful installation to 
conserve disk space and maintain system cleanliness.

ADMINISTRATIVE PRIVILEGES
Ensure proper elevation for system-level installations through UAC integration 
and privilege management.

DOWNLOAD MANAGEMENT SYSTEM
===========================

HTTP Download Engine
--------------------
Implement robust downloading capabilities with:

PROGRESS TRACKING
Real-time download progress with byte-level accuracy, speed calculations, and 
time-to-completion estimates displayed to users.

RESUME SUPPORT
Handle interrupted downloads gracefully by detecting partial files and 
resuming from last successful position using HTTP range requests.

TIMEOUT CONFIGURATION
User-configurable connection and download timeouts to handle various network 
conditions and user preferences.

PROXY SUPPORT
Integration with system proxy settings to work correctly in corporate and 
restricted network environments.

ERROR HANDLING
Comprehensive error detection and recovery mechanisms including automatic 
retry logic with exponential backoff for transient failures.

File Organization Options
Provide flexible file management:

INDIVIDUAL FILES
Keep downloaded installers as separate files in user-specified directory 
structure for manual management and reuse.

CATEGORY ARCHIVES
Compress downloads into ZIP files organized by software category for efficient 
storage and distribution.

CUSTOM LOCATION
User-selectable download directory with proper permission validation and 
automatic directory creation when necessary.

AUTOMATIC CLEANUP
Optional removal of installers after successful installation with user control 
over cleanup timing and behavior.

OPERATIONAL MODES
=================

Batch Installation Mode
-----------------------
The primary operational mode supporting:

MULTIPLE SELECTION
Grid-based interface for selecting multiple applications
or similar widget with checkbox columns for easy selection management.

CATEGORY ORGANIZATION
Visual grouping of applications by type using expandable tree structure or 
tabbed interface for logical organization.

SEARCH FILTERING
Real-time search across all available applications with immediate visual 
feedback and highlighting of matching results.

BULK OPERATIONS
Select all and deselect all functionality with category-level selection 
options for efficient bulk management.

PROGRESS TRACKING
Overall progress across entire batch operation with individual operation 
status and comprehensive completion estimates.

Single Installation Mode
------------------------
Alternative mode for individual application handling:

APPLICATION SELECTION
Dropdown menu or list selection for choosing specific software with detailed 
information display for selected application.

CUSTOM INSTALLER SUPPORT
Manual installer path specification through file dialog integration allowing 
users to install software not in the predefined catalog.

ARGUMENT CUSTOMIZATION
Direct editing of installation parameters through text entry fields with 
syntax validation and helpful examples.

AUTO-DETECTION
Automatic installer type recognition and argument suggestion based on file 
analysis and known patterns.

PROGRESS MONITORING AND LOGGING
================================

Progress Display System
-----------------------
Implement dual progress tracking:

OVERALL PROGRESS
Shows completion status across entire batch operation with accurate percentage 
calculation and time estimates based on current operation speeds.

CURRENT OPERATION
Displays progress for individual download or installation with real-time 
updates of bytes transferred, current speed, and operation-specific status.

STATUS MESSAGES
Real-time text updates describing current activity in human-readable format 
with appropriate detail level for user understanding.

OPERATION CANCELLATION
User-initiated operation termination with proper cleanup of partial downloads, 
temporary files, and system state restoration.

Comprehensive Logging
---------------------
Provide detailed operational logging:

MULTIPLE LOG LEVELS
Info, Warning, Error, and Debug message classification with appropriate 
filtering and display options for different user needs.

TIMESTAMPED ENTRIES
Precise timing information for all operations enabling detailed analysis of 
performance and troubleshooting of issues.

FILE OUTPUT
Persistent log files stored in application data directory with automatic 
rotation and size management to prevent unlimited growth.

UI DISPLAY
Real-time log display within application interface
syntax highlighting for different message types and severity levels.

EXPORT FUNCTIONALITY
Save logs to user-specified locations for troubleshooting and support 
purposes with multiple format options (plain text, structured data).

SYSTEM INTEGRATION FEATURES
============================

Windows Integration
-------------------
Leverage Windows-specific capabilities:

ADMINISTRATOR ELEVATION
Proper UAC handling for system-level operations with minimal privilege 
escalation and clear user communication about privilege requirements.

SYSTEM RESTORE
Integration with Windows System Restore functionality for safe system 
modification with automatic restore point creation before major operations.

REGISTRY MANAGEMENT
Safe registry modifications for auto-start control and application 
configuration with proper backup and restoration capabilities.

PROCESS MANAGEMENT
Proper handling of installer process execution and monitoring with timeout 
handling and resource cleanup for failed or hung processes.

Security Considerations
-----------------------
Implement appropriate security measures:

CODE SIGNING VERIFICATION
Validate downloaded installers when possible using Windows authenticode 
verification to ensure installer integrity and authenticity.

SECURE DOWNLOADS
Use HTTPS for all download operations with proper certificate validation to 
prevent man-in-the-middle attacks and ensure data integrity.

PRIVILEGE MANAGEMENT
Minimize elevated privilege scope and duration, only requesting administrative 
access when necessary for specific operations.

INPUT VALIDATION
Sanitize all user inputs and external data to prevent injection attacks and 
ensure application stability under malicious input conditions.

TECHNICAL REQUIREMENTS
=======================

Performance Objectives
-----------------------
- Startup Time: Application launch under 2 seconds from cold start
- Memory Usage: Minimal RAM footprint during idle state, efficient memory 
  management during operations
- Download Speed: Maximize network throughput utilization without overwhelming 
  system resources or competing applications
- File Size: Executable under 5MB without external dependencies for easy 
  distribution and storage

Platform Requirements
---------------------
- Target Platform: Windows 10/11 (64-bit primary, 32-bit compatible)
- Dependencies: runtime libraries with minimal additional library requirements  
- Compatibility: Support for Windows 7 SP1 and newer where feasible without 
  compromising primary platform performance

Quality Standards
-----------------
- Error Handling: Graceful failure modes with user-friendly error messages 
  that provide actionable information for problem resolution
- Resource Management: Proper cleanup of file handles, network connections, 
  and memory allocation with comprehensive leak detection and prevention
- Threading: Non-blocking UI during long-running operations using loop integration and proper thread coordination
- Data Validation: Comprehensive input validation and sanitization for all 
  user inputs and external data sources

SUCCESS CRITERIA
=================

The completed application should demonstrate:

FUNCTIONAL COMPLETENESS
All described features working reliably across supported Windows platforms 
with comprehensive testing validation and user acceptance criteria.

PERFORMANCE EXCELLENCE  
Noticeably faster operation compared to reference implementation with 
measurable improvements in startup time, memory usage, and operation speed.

USER EXPERIENCE
Intuitive interface requiring minimal learning curve with consistent behavior, 
clear feedback, and efficient workflow design that enhances user productivity.

RELIABILITY
Stable operation across diverse system configurations with comprehensive error 
handling, graceful degradation, and robust recovery from failure conditions.

MAINTAINABILITY
Clean, well-documented code structure enabling future enhancements with clear 
module separation, comprehensive commenting, and standardized coding practices.