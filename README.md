# FLDCDotNet

### DB Hardware Requirements

* SSD 120GB+
* i5 Intel / Ryzen 5 AMD
* 16 GB

### App Hardware Requirements

* HDD 120GB+
* i5 Intel / Ryzen 5 AMD
* 16 GB

### Development Dependencies

* .NET Framework 4.7.1 (TestHarness and FileServer.TestHarness only)
* .NET Core 2.0
* Visual Studio 2017
* Microsoft SQL Server
* ReSharper

#### Project Requirements

* Projects meant to work with both .NET Framework and .NET Core should target .NET Standard (eg. StatsDownload.Core, etc.)
* Test projects should be targeting .NET Core
* Application projects that are able to use .NET Core should (eg. StatsDownload.FileDownload.Console, etc.)
* Windows Forms and WCF applications should target .NET Framework (eg. StatsDownload.TestHarness, etc.)

### Getting Started

1. Navigate to FLDCDotNet/Database 
	* Run the schema and stored procedure scripts against a database
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

#### Stats Download Email Settings

1. SmtpHost
	* The SMTP server to route emails through
2. Port
	* The SMTP server's port to communicate on
3. FromAddress
	* The address emails when be sent from
4. DisplayName
	* The display name the emails sent will have
5. Password
	* The password to access and use the SMTP server
6. Receivers
	* A semi-colon separated list of emails to send notifications to

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
	
#### Stats Upload Filter Settings

1. EnableNoPaymentAddressUsersFilter
	* The stats upload only processes users with a payment address
2. EnableZeroPointUsersFilter
	* The stats upload will filter out users with zero points
3. EnableWhitespaceNameUsersFilter
	* The stats upload will filter out users with a whitespace name
4. EnableGoogleUsersFilter
	* The stats upload will filter out users whoms name start with 'google'

#### Test Harness Only Settings

1. DisableMinimumWaitTime
	* Disables the minimum wait time feature
2. DisableSecureFilePayload
	* Disables trying to upgrade HTTP connections to HTTPS connections automatically
3. EnableOneHundredUsersFilter
	* The stats upload only processes one hundred users

#### File Server Test Harness Settings

1. FilePath
	* The file path to the stats file resource to return in a happy scenario
2. SleepInSeconds
	* The amount of time in seconds the service should sleep in the timeout scenario