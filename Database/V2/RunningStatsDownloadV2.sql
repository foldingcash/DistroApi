-- Call this every time, the scripts will be updated if the database changes
PRINT 'Updating to latest'
EXEC [FoldingCoin].[UpdateToLatest];

DECLARE @DownloadId INT;

-- Call this to start when a new file download is started
PRINT 'Starting a new download'
EXEC [FoldingCoin].[NewFileDownloadStarted] @DownloadId OUTPUT;

-- This Id will be used later to update the status
--SELECT @DownloadId;

-- If an error occurs during the download process, update the status and provide an error message
--PRINT 'File download error'
--EXEC [FoldingCoin].[FileDownloadError] @DownloadId, 'File Download Error Message';

-- When the file download is finished, upload the file data
PRINT 'File download finished'
EXEC [FoldingCoin].[FileDownloadFinished] @DownloadId
	,'\\storage\location'
	,'FileName'
	,'.ext';

PRINT 'File validation started'
EXEC [FoldingCoin].[FileValidationStarted] @DownloadId

DECLARE @ValidatedDateTime DATETIME;

SELECT @ValidatedDateTime = GETUTCDATE();

PRINT 'File validated'
EXEC [FoldingCoin].[FileValidated] @DownloadId
	,@ValidatedDateTime
	,'\\storage\location\processed'
	,'NewFileName'
	,'.ext';

--PRINT 'File validation error'
--EXEC [FoldingCoin].[FileValidationError] @DownloadId, 'File Validation Error';

-- Use this function to get the last file download date time
--PRINT 'Getting last file download datetime'
--SELECT [FoldingCoin].[GetLastFileDownloadDateTime]();

-- Use this view to get the download Ids of the downloads ready for stats upload
PRINT 'Selecting downloads ready for upload'
SELECT DownloadId FROM [FoldingCoin].[ValidatedFiles];