IF OBJECT_ID('FoldingCoin.UpdateToLatest') IS NOT NULL
	BEGIN
		DROP PROCEDURE [FoldingCoin].[UpdateToLatest];
	END
GO

CREATE PROCEDURE [FoldingCoin].[UpdateToLatest]
AS
BEGIN
	BEGIN TRY
		BEGIN TRANSACTION
			IF (SELECT COUNT(1) FROM [FoldingCoin].[Statuses] WHERE [Status] = 'FILE DOWNLOAD STARTED') = 0
				BEGIN
					INSERT INTO [FoldingCoin].[Statuses] ([Status],StatusDescription)
					VALUES ('FILE DOWNLOAD STARTED', 'The stats file download service has started.');
				END

			IF (SELECT COUNT(1) FROM [FoldingCoin].[Statuses] WHERE [Status] = 'FILE DOWNLOAD FINISHED') = 0
				BEGIN
					INSERT INTO [FoldingCoin].[Statuses] ([Status],StatusDescription)
					VALUES ('FILE DOWNLOAD FINISHED', 'The stats file download has finished.');
				END

			IF (SELECT COUNT(1) FROM [FoldingCoin].[Statuses] WHERE [Status] = 'FILE DOWNLOAD ERROR') = 0
				BEGIN
					INSERT INTO [FoldingCoin].[Statuses] ([Status],StatusDescription)
					VALUES ('FILE DOWNLOAD ERROR', 'There was an error during the file download process.');
				END

			IF (SELECT COUNT(1) FROM [FoldingCoin].[Statuses] WHERE [Status] = 'VALIDATION STARTED') = 0
				BEGIN
					INSERT INTO [FoldingCoin].[Statuses] ([Status],StatusDescription)
					VALUES ('VALIDATION STARTED', 'The validation of the file has started.');
				END

			IF (SELECT COUNT(1) FROM [FoldingCoin].[Statuses] WHERE [Status] = 'VALIDATED')  = 0
				BEGIN
					INSERT INTO [FoldingCoin].[Statuses] ([Status],StatusDescription)
					VALUES ('VALIDATED', 'The file has been validated of any issues.');
				END

			IF (SELECT COUNT(1) FROM [FoldingCoin].[Statuses] WHERE [Status] = 'VALIDATION ERROR') = 0
				BEGIN
					INSERT INTO [FoldingCoin].[Statuses] ([Status],StatusDescription)
					VALUES ('VALIDATION ERROR', 'There was an error during the file validation process.');
				END
		COMMIT
	END TRY
	BEGIN CATCH
		ROLLBACK;
		THROW;
	END CATCH
END
GO

-----------------------------------------------------------------

IF OBJECT_ID('FoldingCoin.GetFileDownloadStartedStatusId') IS NOT NULL
BEGIN
	DROP FUNCTION [FoldingCoin].[GetFileDownloadStartedStatusId];
END
GO

CREATE FUNCTION [FoldingCoin].[GetFileDownloadStartedStatusId] () RETURNS INT
AS
BEGIN
	DECLARE @StatusId INT;
	
	SELECT @StatusId = StatusId FROM [FoldingCoin].[Statuses] WHERE [Status] = 'FILE DOWNLOAD STARTED';
	
	RETURN @StatusId;
END
GO

-----------------------------------------------------------------

IF OBJECT_ID('FoldingCoin.GetFileDownloadFinishedStatusId') IS NOT NULL
BEGIN
	DROP FUNCTION [FoldingCoin].[GetFileDownloadFinishedStatusId];
END
GO

CREATE FUNCTION [FoldingCoin].[GetFileDownloadFinishedStatusId] () RETURNS INT
AS
BEGIN
	DECLARE @StatusId INT;
	
	SELECT @StatusId = StatusId FROM [FoldingCoin].[Statuses] WHERE [Status] = 'FILE DOWNLOAD FINISHED';
				 
	RETURN @StatusId;
END
GO

-----------------------------------------------------------------

IF OBJECT_ID('FoldingCoin.GetFileDownloadErrorStatusId') IS NOT NULL
BEGIN
	DROP FUNCTION [FoldingCoin].[GetFileDownloadErrorStatusId];
END
GO

CREATE FUNCTION [FoldingCoin].[GetFileDownloadErrorStatusId] () RETURNS INT
AS
BEGIN
	DECLARE @StatusId INT;
	
	SELECT @StatusId = StatusId FROM [FoldingCoin].[Statuses] WHERE [Status] = 'FILE DOWNLOAD ERROR';
				 
	RETURN @StatusId;
END
GO

-----------------------------------------------------------------

IF OBJECT_ID('FoldingCoin.GetValidationStartedStatusId') IS NOT NULL
BEGIN
	DROP FUNCTION [FoldingCoin].[GetValidationStartedStatusId];
END
GO

CREATE FUNCTION [FoldingCoin].[GetValidationStartedStatusId] () RETURNS INT
AS
BEGIN
	DECLARE @StatusId INT;
	
	SELECT @StatusId = StatusId FROM [FoldingCoin].[Statuses] WHERE [Status] = 'VALIDATION STARTED';
				 
	RETURN @StatusId;
END
GO

-----------------------------------------------------------------

IF OBJECT_ID('FoldingCoin.GetValidatedStatusId') IS NOT NULL
BEGIN
	DROP FUNCTION [FoldingCoin].[GetValidatedStatusId];
END
GO

CREATE FUNCTION [FoldingCoin].[GetValidatedStatusId] () RETURNS INT
AS
BEGIN
	DECLARE @StatusId INT;
	
	SELECT @StatusId = StatusId FROM [FoldingCoin].[Statuses] WHERE [Status] = 'VALIDATED';
				 
	RETURN @StatusId;
END
GO

-----------------------------------------------------------------

IF OBJECT_ID('FoldingCoin.GetValidationErrorStatusId') IS NOT NULL
BEGIN
	DROP FUNCTION [FoldingCoin].[GetValidationErrorStatusId];
END
GO

CREATE FUNCTION [FoldingCoin].[GetValidationErrorStatusId] () RETURNS INT
AS
BEGIN
	DECLARE @StatusId INT;
	
	SELECT @StatusId = StatusId FROM [FoldingCoin].[Statuses] WHERE [Status] = 'VALIDATION ERROR';
				 
	RETURN @StatusId;
END
GO

-----------------------------------------------------------------

IF OBJECT_ID('FoldingCoin.GetLastFileDownloadDateTime') IS NOT NULL
BEGIN
	DROP FUNCTION [FoldingCoin].[GetLastFileDownloadDateTime];
END
GO

CREATE FUNCTION [FoldingCoin].[GetLastFileDownloadDateTime] () RETURNS DATETIME
AS
BEGIN
	DECLARE @DownloadDateTime DATETIME;
	
	SELECT TOP (1) @DownloadDateTime = DownloadDateTime	FROM [FoldingCoin].[Downloads]
	WHERE StatusId <> FoldingCoin.GetFileDownloadStartedStatusId()
	AND StatusId <> FoldingCoin.GetFileDownloadErrorStatusId()
	ORDER BY DownloadDateTime DESC;

	RETURN @DownloadDateTime;
END
GO

-----------------------------------------------------------------

IF OBJECT_ID('FoldingCoin.NewFileDownloadStarted') IS NOT NULL
BEGIN
	DROP PROCEDURE [FoldingCoin].[NewFileDownloadStarted];
END
GO

CREATE PROCEDURE [FoldingCoin].[NewFileDownloadStarted] @DownloadId INT OUTPUT
AS
BEGIN
	DECLARE @FileDownloadStartedStatusId INT;

	BEGIN TRY
		BEGIN TRANSACTION
			SELECT @FileDownloadStartedStatusId = [FoldingCoin].[GetFileDownloadStartedStatusId]();
	
			INSERT INTO [FoldingCoin].[Downloads] (StatusId, FileId, DownloadDateTime)
			VALUES (@FileDownloadStartedStatusId, NULL, GETUTCDATE());
	
			SELECT TOP 1 @DownloadId = @@Identity FROM [FoldingCoin].[Downloads];
		COMMIT
	END TRY
	BEGIN CATCH
		ROLLBACK;
		THROW;
	END CATCH
END
GO

-----------------------------------------------------------------

IF OBJECT_ID('FoldingCoin.FileDownloadFinished') IS NOT NULL
	BEGIN
		DROP PROCEDURE [FoldingCoin].[FileDownloadFinished];
	END
GO

CREATE PROCEDURE [FoldingCoin].[FileDownloadFinished] 
	 @DownloadId INT
	,@FileLocation NVARCHAR(250)
	,@FileName NVARCHAR(50)
	,@FileExtension NVARCHAR(5)
AS
BEGIN
	DECLARE @FileId INT;

	BEGIN TRY
		BEGIN TRANSACTION
			INSERT INTO [FoldingCoin].[Files] ([FileLocation], [FileName], [FileExtension])
			VALUES (@FileLocation, @FileName, @FileExtension);

			SELECT TOP 1 @FileId = @@Identity FROM [FoldingCoin].[Files];

			DECLARE @FileDownloadFinishedStatusId INT;

			UPDATE [FoldingCoin].[Downloads] SET
					 FileId = @FileId
					,StatusId = FoldingCoin.GetFileDownloadFinishedStatusId()
			WHERE DownloadId = @DownloadId;
		COMMIT
	END TRY
	BEGIN CATCH
		ROLLBACK;
		THROW;
	END CATCH
END
GO

-----------------------------------------------------------------

IF OBJECT_ID('FoldingCoin.FileDownloadError') IS NOT NULL
	BEGIN
		DROP PROCEDURE [FoldingCoin].[FileDownloadError];
	END
GO

CREATE PROCEDURE [FoldingCoin].[FileDownloadError] @DownloadId INT ,@ErrorMessage NVARCHAR(500)
AS
BEGIN
	DECLARE @FileDownloadErrorStatusId INT;

	BEGIN TRY
		BEGIN TRANSACTION
			SELECT @FileDownloadErrorStatusId = [FoldingCoin].GetFileDownloadErrorStatusId();

			UPDATE [FoldingCoin].[Downloads] 
			SET StatusId = @FileDownloadErrorStatusId
			WHERE DownloadId = @DownloadId;

			INSERT INTO [FoldingCoin].[Rejections] (DownloadId, LineNumber, Reason)
			VALUES (@DownloadId, NULL, @ErrorMessage);
		COMMIT
	END TRY
	BEGIN CATCH
		ROLLBACK;
		THROW;
	END CATCH
END
GO

-----------------------------------------------------------------

IF OBJECT_ID('FoldingCoin.ValidatedDownloads') IS NOT NULL
	BEGIN
		DROP VIEW [FoldingCoin].[ValidatedDownloads];
	END
GO

CREATE VIEW [FoldingCoin].[ValidatedDownloads]
AS
	SELECT DownloadId  FROM [FoldingCoin].[Downloads] D
	INNER JOIN [FoldingCoin].[Files] F ON D.FileId = F.FileId
	WHERE StatusId = FoldingCoin.GetValidatedStatusId();
GO
