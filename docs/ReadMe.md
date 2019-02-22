# FLDCDotNet

[![Build Status](https://foldingcoin.visualstudio.com/StatsDownloadDotNet/_apis/build/status/TestStatsDownloadApi%20-%20CI?branchName=develop)](https://foldingcoin.visualstudio.com/StatsDownloadDotNet/_build/latest?definitionId=1&branchName=develop)
[![Deployment Status](https://foldingcoin.vsrm.visualstudio.com/_apis/public/Release/badge/7276dea2-ed05-49df-b912-05a27a323bb1/1/1)](https://foldingcoin.vsrm.visualstudio.com/_apis/public/Release/badge/7276dea2-ed05-49df-b912-05a27a323bb1/1/1)

## Getting Started

This is a C# implementation for downloading FAH statistics on a scheduled basis, parsing and loading the statistics into a database, and parsing metadata out of a FAH user's name. An API is used to expose the data in the database.

## Built With

* Visual Studio 2017
* Microsoft SQL Server 2017
* ReSharper
* WiX

## Solutions

1. [StatsDownload](StatsDownload.ReadMe.md)
	* The downloader interfaces with the StatsDownload database to upload the database with user statistics
2. [StatsDownloadSetup](StatsDownloadSetup.ReadMe.md)
	* The downloader setup creates an installation package for Windows
3. [StatsDownloadApi](StatsDownloadApi.ReadMe.md)
	* The API interfaces with the StatsDownload database to return the data within based on query parameters

## Versioning

Each application should be using the same version on release. We use [semantic versioning](https://semver.org/) for determining when and how to increase the verison number.

## Issues

Feel free to submit issues and enhancement requests.

## Coding Style

In general, we follow the [MSDN coding guidelines](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions) and use ReSharper for code formatting.

## Contributing

Refer to the project's style guidelines for submitting patches and additions. In general, we follow the "fork-and-pull" Git workflow.

1. Fork the repo on GitHub
2. Clone the project to your own machine
3. Commit changes to your own branch
4. Run R#er, commit cleanup
5. Push your work back up to your fork
6. Submit a pull request so that we can review your changes

NOTE: Be sure to merge the latest from "upstream" before making a pull request!

## License

This project is licensed under the MIT License - see the [LICENSE](../LICENSE) file for details

## Acknowledgments

* SharpZipLib
