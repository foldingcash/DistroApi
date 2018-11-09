# Stats Downloader

## Getting Started

The stats downloader is used to download the stats data on a scheduled basis.

## Prerequisites

* .NET Core 2.1
* Microsoft SQL Server 2017

## Installation

### Installing on Windows w/ Installer

1. Install using latest available installation package
2. [Update applications configuration file](SettingsConfiguration.md)
3. Navigate to FLDCDotNet/Database
	1. Run the schema script to create a database (update path to database)
	2. Run the stored procedure script against the database
4. Create a task for each application
	1. [Search for Task Scheduler](InstallPics/Windows_SearchTask.PNG)
	2. [Create Task](InstallPics/Windows_CreateTask.PNG)
	3. [Sample Task Properties](InstallPics/Windows_TaskProperties.PNG)
	4. [Sample Task Trigger](InstallPics/Windows_Trigger.PNG)
	5. [Sample Task Action](InstallPics/Windows_Action.PNG)
5. Use a batch script calling the appropriate application

```
dotnet StatsDownload.FileDownload.Console.dll >> Log.txt
```
```
dotnet StatsDownload.StatsUpload.Console.dll >> Log.txt
```

### Settings

1. [Logging Settings](SettingsConfiguration.md#logging-settings)
2. [Database Settings](SettingsConfiguration.md#stats-download-database-connection-settings)
3. [Email Settings](SettingsConfiguration.md#stats-download-email-settings)
4. [Download Settings](SettingsConfiguration.md#file-download-settings)
5. [Upload Settings](SettingsConfiguration.md#stats-upload-filter-settings)
6. [Test Settings](SettingsConfiguration.md#test-harness-only-settings)

### Applications

1. StatsDownload.TestHarness
	* This is a windows forms application meant to provide a GUI interface to the file download and stats upload processes for rapid testing
2. [StatsDownload.FileServer.TestHarness](FileServer.TestHarness.ReadMe.md)
	* This is a WCF application meant to provide a test server for the file download i.e. for mocking Stanford's server
3. StatsDownload.FileDownload.Console
	* This is a console application meant to be executed via command line or task scheduler and will execute only the file download portion of the stats download process
4. StatsDownload.StatsUpload.Console
	* This is a console application meant to be executed via command line or task scheduler and will execute only the stats upload portion of the stats download process
	
## Errors

### Failed Reasons

|           Failed Reason             | Failed Reason                                                                                                                                                                                        |
|:-----------------------------------:|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
|               None                  | No failures were encountered.                                                                                                                                                                        |
|        DatabaseUnavailable          | The database is unavailable. Ensure the database is available, reachable, and the connection string is correct.                                                                                      |
|    DatabaseMissingRequiredObjects   | The database is available but is missing required objects to run. Ensure the required objects are available in the database and try again.                                                           |
|       MinimumWaitTimeNotMet         | The minimum wait time was not satisified. Wait more time before trying to download the stats file again. The minimum wait time is by default one hour or overriden in the application configuration. |
|      RequiredSettingsInvalid        | Required settings are not configured or invalid. [Ensure all required settings are configured and valid.](#settings)                                                                                 |
|        FileDownloadTimeout          | There was a timeout download the stats file. Try to download file again and if the error occurs again, then try increasing the download timeout.                                                     |
|       FileDownloadNotFound          | The stats file was not found at the configured URI. Ensure the URI is correct and accesible, then try again.                                                                                         |
|   FileDownloadFailedDecompression   | There was a problem decompressing the stats file. The file was left on disk for analysis. Ensure the stats file is compressed using BZip compression.                                                |
|      InvalidStatsFileUpload         | The stats file failed header validation. Ensure the DateTime and header are available and correctly formatted.                                                                                       |
|    UnexpectedDatabaseException      | There was an unhandled database exception. Try again and if the error occurs again, then check the logs for more details about the database exception. This is likely an unhandled scenario or bug.  |
|       UnexpectedException           | There was an unhandled exception. Try again and if the error occurs again, then check the logs for more details about the exception. This is likely an unhandled scenario or bug.                    |

### Rejection Reasons

|     Rejection Reason         | Rejection Reason                                                                                                                              |
|:----------------------------:|-----------------------------------------------------------------------------------------------------------------------------------------------|
|           None               | No rejections were encountered.                                                                                                               |
|     UnexpectedFormat         | There was an unexpected number of values in the user record. Ensure the user record is correctly formatted.                                   |
|       FailedParsing          | One of the user's values failed conversion. Ensure user's fields are correctly typed and within upper/lower bound limits for it's type.       |
|    FahNameExceedsMaxSize     | The user's FAH name exceeded the max FAH name size. Have the user shorten their FAH name.                                                     |
|  FriendlyNameExceedsMaxSize  | The user's friendly name exceeded the max friendly name size. Have the user shorten their friendly name.                                      |
| BitcoinAddressExceedsMaxSize | The user's bitcoin address exceeded the max bitcoin address size. Have the user shorten their bitcoin address.                                |
|    FailedAddToDatabase       | There was an error while adding the user record to the database. Check the database and logs for more details about the reason for rejection. |

## Running the tests

### Unit Tests

Options for running the unit tests:
* Use ReSharper's Test Runner from within Visual Studio
* Use the nunit console runner, using a command such as:
```
nunit-console {path-to-assembly}
```
	
### Integration Tests

1. Use the FileServer.TestHarness for integration tests
	* Several endpoints exist for testing various scenarios
2. Update FileServer.TestHarness settings
	* [FileServer Test Harness ReadMe](FileServer.TestHarness.ReadMe.md#settings)
	
## Deployment

### App Hardware Requirements

* HDD 120GB+
* i5 Intel / Ryzen 5 AMD
* 16 GB

### DB Hardware Requirements

* SSD 120GB+
* i5 Intel / Ryzen 5 AMD
* 16 GB