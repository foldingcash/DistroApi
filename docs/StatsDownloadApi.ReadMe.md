# Stats API

## Getting Started

The stats API is used to expose the stats, users, and team data from the stats database.

## Prerequisites

* .NET Core 2.1
* Microsoft SQL Server 2017

## Installation

### Publishing to Azure

1. Load StatsDownloadApi in VS
2. [Publish StatsDownloadApi.WebApi project](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/azure-apps/?view=aspnetcore-2.1)
3. Ensure connection string 'FoldingCoin' for the API is pointing to a StatsDownload database
4. [Configure additional application settings if needed](#settings)

### Settings

1. [Logging Settings](SettingsConfiguration.md#nlog-configuration)
2. [Database Settings](SettingsConfiguration.md#stats-download-database-connection-settings)
3. [Email Settings](SettingsConfiguration.md#stats-download-email-settings)

### Applications

1. WebApi
	* This is an API meant to provide services to various web applications

## [Using the API](StatsDownloadApi.UsingTheApi.md)
	
## Running the tests

### Unit Tests

Options for running the unit tests:
* Use ReSharper's Test Runner from within Visual Studio
* Use the nunit console runner, using a command such as:
```
nunit-console {path-to-assembly}
```
	
### Integration Tests

1. Load the database with a test dataset
2. Run API pointing to previous database
3. Run tests
	
## Deployment

### Api Hardware Requirements

* SDD 120GB+
* i5 Intel / Ryzen 5 AMD
* 16 GB