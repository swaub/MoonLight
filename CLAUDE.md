# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

MoonLight is a Windows silent installer management application built with WPF and .NET 8.0. It provides functionality similar to Ninite, allowing users to download and silently install multiple Windows applications in batch operations.

## Build and Development Commands

### Build Commands
```bash
# Build the project
dotnet build

# Build in Release mode
dotnet build -c Release

# Run the application
dotnet run

# Clean the build
dotnet clean
```

### Testing
Currently no test projects are configured. When tests are added, use:
```bash
dotnet test
```

## Architecture and Code Structure

### Technology Stack
- **Framework**: .NET 8.0 Windows Desktop
- **UI Framework**: WPF (Windows Presentation Foundation)
- **Language**: C# with nullable reference types enabled
- **Target Platform**: Windows only (net8.0-windows)

### Project Structure
The project follows a standard WPF application structure:
- `App.xaml` / `App.xaml.cs` - Application entry point and global resources
- `MainWindow.xaml` / `MainWindow.xaml.cs` - Main application window
- Currently a minimal skeleton implementation with empty MainWindow

### Key Requirements from Documentation

The project has detailed requirements documented in:
- `PROJECT INSTRUCTIONS.md` - Comprehensive feature requirements
- `Technical Requirements.md` - Performance targets and quality standards
- `User Interface and Layout.md` - UI design specifications
- `Software Catalog Reference Data.md` - List of applications to support

Major components to be implemented:
1. **UI Components**: Header, search, application grid, options panel, progress bars, logging area
2. **Download Manager**: HTTP downloads with resume support, progress tracking
3. **Installation Engine**: Silent installation for various installer types (MSI, InnoSetup, NSIS, etc.)
4. **Application Catalog**: Categorized software database with download URLs and silent install arguments
5. **Modes**: Batch installation and single installation modes

### Performance Requirements
- Startup time: < 2 seconds
- Memory usage: < 200MB idle
- Executable size: < 5MB
- Support 5 concurrent downloads
- UI responsiveness: < 16ms for interactions

### Development Guidelines
- Follow WPF MVVM pattern for UI architecture
- Implement proper async/await for long-running operations
- Use dependency injection for services
- Ensure proper error handling and logging throughout
- Validate all user inputs
- Use HTTPS for all downloads with certificate validation