# FLDCDotNet

### Required Dependencies

* .NET Framework 4.5.2
* Visual Studio 2015
* ReSharper
* Microsoft SQL Server

### Getting Started

1. Navigate to FLDCDotNet/Database 
	* Restore FoldingCoin.bak to an instance of SQL Server
	* Alternatively, run the schema and stored procedure scripts against a database
2. Build the solution and run Unit Tests to ensure all pass
	* There are four applications available to use; two for production usage and two for testing
3. Ensure application's connection string pointed to the database created in step 1
4. Run application(s)

### Applications

1. StatsDownload.TestHarness
	* This is a windows forms application meant to provide a GUI interface to the file download and stats upload processes for rapid testing
2. StatsDownload.FileServer.TestHarness
	* This is a WCF application meant to provide a test server for the file download i.e. for mocking Stanford's server
3. StatsDownload.FileDownload.Console
	* This is a console application meant to be executed via command line or task scheduler and will execute only the file download portion of the stats download process
4. StatsDownload.StatsUpload.Console
	* This is a console application meant to be executed via command line or task scheduler and will execute only the stats upload portion of the stats download process

#### Stats Download Settings

1. FoldingCoin
	* A connection string to the database loaded with the schema and stored procedures

#### File Download Settings

1. DownloadUri
	* The uri to the stats file resource
2. DownloadTimeoutSeconds
	* The amount of time in seconds we will try to download the stats file
3. DownloadDirectory
	* The directory to store the download files and process them for upload
4. AcceptAnySslCert
	* Accept all SSL certificates regardless if they are valid
5. MinimumWaitTimeInHours
	* The amount of time in hours we should wait between file downloads

#### Stats Download Emailing Settings

1. SmtpHost
2. Port
3. FromAddress
4. DisplayName
5. Password
6. Receivers

#### Test Harness Only Settings

1. DisableMinimumWaitTime
2. DisableSecureFilePayload

#### File Server Test Harness Settings

1. FilePath
	* The file path to the stats file resource to return in a happy scenario
2. SleepInSeconds
	* The amount of time in seconds the service should sleep in the timeout scenario