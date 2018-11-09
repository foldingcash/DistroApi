# Stats Downloader Setup

## Getting Started

The stats downloader setup is used to create a Windows installation package to install the stats downloader.

## Prerequisites

* .NET Framework 4.7.1
* WiX

## Preparing for a Release

### Update Version Number

1. Update all projects
	* StatsDownload, StatsDownloadSetup, and StatsDownloadApi
	* Do not update WxsMerger unless modified
2. Update Product.wxs.template

### Update .NET Core Version

If the .NET Core version has changed, then update the batch script that is havesting the source to pull from the correct folder (e.g. netcoreapp2.0 -> netcoreapp2.1).

## Creating the Installation Package

1. Load StatsDownload solution
2. Run unit tests and ensure all tests pass
3. Load StatsDownloadSetup solution
4. Publish StatsDownloadSetup solution
	* Update target as needed e.g. Windows x86, Windows x64

## Getting SHA256 Hash

### Windows 10 Using Powershell

1. Use Get-FileHash Command
	* Ex. PS C:\> Get-FileHash C:\StatsDownload-x64.msi -ALGO SHA256