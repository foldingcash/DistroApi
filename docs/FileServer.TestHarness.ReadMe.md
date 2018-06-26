# File Server Test Harness

The File Server Test Harness allows for mocking a file server for use with the Stats Download. It provides URIs for testing different permutations.

## URIs

* /decompressable.bz2
	* Returns the text "decompressable" as a stream.
* /fail_download.bz2
	* Throws a NotImplementedException
* /daily_user_summary.txt.bz2
	* Returns a valid statistics file.
* /invalid_folder.txt.bz2
	* Returns a compressed file with an invalid folder.
* /invalid_file.txt.bz2
	* Returns an invalid compressed file.
* /timeout.bz2
	* Sleeps a pre-determined amount of time. If not specified in the app.config, the value is 100 seconds.

## Settings

* FilePath
	* The file path to the stats file resource to return in a happy scenario
* SleepInSeconds
	* The amount of time in seconds the service should sleep in the timeout scenario. If not specified, the default value is 100 seconds.
