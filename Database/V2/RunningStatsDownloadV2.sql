-- Call this every time, the scripts will be updated if the database changes
PRINT 'Updating to latest'
EXEC [FoldingCoin].[UpdateToLatest]; --TODO: Remove this requirement

-- Use this function to get the last file download date time
--PRINT 'Getting last file download datetime'
--SELECT [FoldingCoin].[GetLastFileDownloadDateTime]();

DECLARE @DownloadId INT;

-- Call this to start when a new file download is started
PRINT 'Starting a new download'
EXEC [FoldingCoin].[FileDownloadStarted] @DownloadId OUTPUT;

-- This Id will be used later to update the status
--SELECT @DownloadId;

-- If an error occurs during the download process, update the status and provide an error message
--PRINT 'File download error'
--EXEC [FoldingCoin].[FileDownloadError] @DownloadId, 'File Download Error Message';

-- When the file download is finished, upload the file data
PRINT 'File download finished'
EXEC [FoldingCoin].[FileDownloadFinished] @DownloadId
	,'\\storage\location\unprocessed\FileName.ext'
	,'FileName'
	,'.ext';

PRINT 'File validation started'
EXEC [FoldingCoin].[FileValidationStarted] @DownloadId

DECLARE @ValidatedDateTime DATETIME;

SELECT @ValidatedDateTime = GETUTCDATE();
--SELECT @ValidatedDateTime = DATEFROMPARTS(2019, 01, 01); --year/month/day

PRINT 'File validated'
EXEC [FoldingCoin].[FileValidated] @DownloadId
	,@ValidatedDateTime
	,'\\storage\location\processed\FileName.ext'
	,'FileName'
	,'.ext';

--PRINT 'File validation error'
--EXEC [FoldingCoin].[FileValidationError] @DownloadId, 'File Validation Error';

--------------- The API starts here ---------------

-- Use this view to get the file paths of the validated files ready for API use
DECLARE @StartDate DATE;
DECLARE @EndDate DATE;

SELECT @StartDate = DATEADD(month, -1, GETUTCDATE());
SELECT @EndDate = GETUTCDATE();

PRINT 'Selecting downloads ready for upload'
EXEC [FoldingCoin].[GetValidatedFiles] @StartDate, @EndDate;