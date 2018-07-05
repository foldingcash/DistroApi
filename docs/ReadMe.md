# FLDCDotNet

## Getting Started

This is a C# implementation for downloading FAH statistics on a scheduled basis, parsing and loading the statistics into a database, and parsing metadata out of a FAH user's name. An API is used to expose the data in the database.

## Solutions

1. [StatsDownload](StatsDownload.ReadMe.md)
	* The downloader interfaces with the StatsDownload database to upload the database with user statistics
2. [StatsDownloadApi](StatsDownloadApi.ReadMe.md)
	* The API interfaces with the StatsDownload database to return the data within based on query parameters

## Built With

* Visual Studio 2017
* Microsoft SQL Server 2017
* ReSharper

## License

This project is licensed under the MIT License - see the [LICENSE](../LICENSE) file for details

## Acknowledgments

* SharpZipLib
