# FLDCDotNet

## Getting Started

This is a C# implementation for downloading FAH statistics on a scheduled basis and loading the statistics into a database.

## Prerequisites

* .NET Framework 4.7.1
* .NET Core 2.0
* Microsoft SQL Server 2017

## Installing

1. Run publish on project to install
2. Take published files for installation
3. Update application configuration file
	* [Settings Doc](SettingsConfiguration.md)
4. Navigate to FLDCDotNet/Database 
	* Run the schema and stored procedure scripts against a database
5. Build the solution and run Unit Tests to ensure all pass
	* There are four applications available to use; two for production usage and two for testing
6. Ensure application's connection string pointed to the database created in step 1
7. Run application(s)

### Applications

1. StatsDownload.TestHarness
	* This is a windows forms application meant to provide a GUI interface to the file download and stats upload processes for rapid testing
2. StatsDownload.FileServer.TestHarness
	* This is a WCF application meant to provide a test server for the file download i.e. for mocking Stanford's server
3. StatsDownload.FileDownload.Console
	* This is a console application meant to be executed via command line or task scheduler and will execute only the file download portion of the stats download process
4. StatsDownload.StatsUpload.Console
	* This is a console application meant to be executed via command line or task scheduler and will execute only the stats upload portion of the stats download process

## Running the tests

1. Use ReSharper from Visual Studios
2. nunit-console {path-to-assembly}

## Deployment

### App Hardware Requirements

* HDD 120GB+
* i5 Intel / Ryzen 5 AMD
* 16 GB

### DB Hardware Requirements

* SSD 120GB+
* i5 Intel / Ryzen 5 AMD
* 16 GB

## Built With

* .NET Framework 4.7.1
* .NET Core 2.0
* Visual Studio 2017
* Microsoft SQL Server 2017
* ReSharper

## License

This project is licensed under the MIT License - see the [./LICENSE.md](LICENSE.md) file for details

## Acknowledgments

* SharpZipLib