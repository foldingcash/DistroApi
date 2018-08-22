# Stats Downloader Setup

## Getting Started

The stats downloader setup is used to create a Windows installation package to install the stats downloader.

## Prerequisites

* .NET Framework 4.7.1
* WiX

## Installation Package

### Creating the Installation Package

1. Load StatsDownload solution
2. Run unit tests and ensure all tests pass
3. Load StatsDownloadSetup solution
4. Publish StatsDownloadSetup solution
	* Update target as needed e.g. Windows x86, Windows x64