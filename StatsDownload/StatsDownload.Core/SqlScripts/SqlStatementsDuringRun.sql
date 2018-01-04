-- Call this every time, the scripts will be updated if the database changes
EXEC [FoldingCoin].[UpdateToLatest];

DECLARE @DownloadId INT;

-- Call this to start when a new file download is started
EXEC [FoldingCoin].[NewFileDownloadStarted] @DownloadId OUTPUT;

-- This Id will be used later to update the status
--SELECT @DownloadId;

-- If an error occurs during the download process, update the status and provide an error message
--EXEC [FoldingCoin].[FileDownloadError] @DownloadId, 'File Download Error Message';

-- When the file download is finished, upload the file data
EXEC [FoldingCoin].[FileDownloadFinished] @DownloadId
	,'File Name'
	,'.ext'
	,'data';

-- Use this function to get the last file download date time
--SELECT [FoldingCoin].[GetLastFileDownloadDateTime]();

-- Use this view to get the download Ids of the downloads ready for stats upload
--SELECT DownloadId FROM [FoldingCoin].[DownloadsReadyForUpload];

-- Output parameters
DECLARE @FileName NVARCHAR(50);
DECLARE @FileExtension NVARCHAR(5);
DECLARE @FileData NVARCHAR(MAX);

-- Use this stored procedure to get the file data for a download
EXEC [FoldingCoin].[GetFileData] @DownloadId
	,@FileName OUTPUT
	,@FileExtension OUTPUT
	,@FileData OUTPUT;

-- The file name, extension, and data
--SELECT @FileName, @FileExtension, @FileData;

-- Use this to update the download status to show the stats upload has started
EXEC [FoldingCoin].[StartStatsUpload] @DownloadId;

-- If there is a problem during the stats upload process that prevents the file from being processed at all, update to stats upload error
--EXEC [FoldingCoin].[StatsUploadError] @DownloadId, 'Stats Upload Error Message'

-- If there is a problem processing a particular FAH user in the stats file, then update the user to have a rejection status
--EXEC [FoldingCoin].[AddUserRejection] @DownloadId, 'FriendlyName_TAG_BitcoinAddress', '199', 'A Bad User';

-- Use this to add user data
EXEC [FoldingCoin].[AddUserData] @DownloadId
	,'FriendlyName_TAG_BitcoinAddress'
	,'1000'
	,'100'
	,'100'
	,'FriendlyName'
	,'BitcoinAddress';

-- Use this to update the download status to show the stats upload has completed
EXEC [FoldingCoin].[StatsUploadFinished] @DownloadId;
