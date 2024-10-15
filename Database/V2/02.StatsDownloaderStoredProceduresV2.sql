IF OBJECT_ID('FoldingCash.UpdateToLatest') IS NOT NULL
	BEGIN
		DROP PROCEDURE [FoldingCash].[UpdateToLatest];
	END
GO

CREATE PROCEDURE [FoldingCash].[UpdateToLatest]
AS
BEGIN
	BEGIN TRY
		BEGIN TRANSACTION
			IF (SELECT COUNT(1) FROM [FoldingCash].[Statuses] WHERE [Status] = 'FILE DOWNLOAD STARTED') = 0
				BEGIN
					INSERT INTO [FoldingCash].[Statuses] ([Status],StatusDescription)
					VALUES ('FILE DOWNLOAD STARTED', 'The stats file download service has started.');
				END

			IF (SELECT COUNT(1) FROM [FoldingCash].[Statuses] WHERE [Status] = 'FILE DOWNLOAD FINISHED') = 0
				BEGIN
					INSERT INTO [FoldingCash].[Statuses] ([Status],StatusDescription)
					VALUES ('FILE DOWNLOAD FINISHED', 'The stats file download has finished.');
				END

			IF (SELECT COUNT(1) FROM [FoldingCash].[Statuses] WHERE [Status] = 'FILE DOWNLOAD ERROR') = 0
				BEGIN
					INSERT INTO [FoldingCash].[Statuses] ([Status],StatusDescription)
					VALUES ('FILE DOWNLOAD ERROR', 'There was an error during the file download process.');
				END

			IF (SELECT COUNT(1) FROM [FoldingCash].[Statuses] WHERE [Status] = 'FILE VALIDATION STARTED') = 0
				BEGIN
					INSERT INTO [FoldingCash].[Statuses] ([Status],StatusDescription)
					VALUES ('FILE VALIDATION STARTED', 'The validation of the file has started.');
				END

			IF (SELECT COUNT(1) FROM [FoldingCash].[Statuses] WHERE [Status] = 'FILE VALIDATED')  = 0
				BEGIN
					INSERT INTO [FoldingCash].[Statuses] ([Status],StatusDescription)
					VALUES ('FILE VALIDATED', 'The file has been validated of any issues.');
				END

			IF (SELECT COUNT(1) FROM [FoldingCash].[Statuses] WHERE [Status] = 'FILE VALIDATION ERROR') = 0
				BEGIN
					INSERT INTO [FoldingCash].[Statuses] ([Status],StatusDescription)
					VALUES ('FILE VALIDATION ERROR', 'There was an error during the file validation process.');
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

IF OBJECT_ID('FoldingCash.GetFileDownloadStartedStatusId') IS NOT NULL
BEGIN
	DROP FUNCTION [FoldingCash].[GetFileDownloadStartedStatusId];
END
GO

CREATE FUNCTION [FoldingCash].[GetFileDownloadStartedStatusId] () RETURNS INT
AS
BEGIN
	DECLARE @StatusId INT;
	
	SELECT @StatusId = StatusId FROM [FoldingCash].[Statuses] WHERE [Status] = 'FILE DOWNLOAD STARTED';
	
	RETURN @StatusId;
END
GO

-----------------------------------------------------------------

IF OBJECT_ID('FoldingCash.GetFileDownloadFinishedStatusId') IS NOT NULL
BEGIN
	DROP FUNCTION [FoldingCash].[GetFileDownloadFinishedStatusId];
END
GO

CREATE FUNCTION [FoldingCash].[GetFileDownloadFinishedStatusId] () RETURNS INT
AS
BEGIN
	DECLARE @StatusId INT;
	
	SELECT @StatusId = StatusId FROM [FoldingCash].[Statuses] WHERE [Status] = 'FILE DOWNLOAD FINISHED';
				 
	RETURN @StatusId;
END
GO

-----------------------------------------------------------------

IF OBJECT_ID('FoldingCash.GetFileDownloadErrorStatusId') IS NOT NULL
BEGIN
	DROP FUNCTION [FoldingCash].[GetFileDownloadErrorStatusId];
END
GO

CREATE FUNCTION [FoldingCash].[GetFileDownloadErrorStatusId] () RETURNS INT
AS
BEGIN
	DECLARE @StatusId INT;
	
	SELECT @StatusId = StatusId FROM [FoldingCash].[Statuses] WHERE [Status] = 'FILE DOWNLOAD ERROR';
				 
	RETURN @StatusId;
END
GO

-----------------------------------------------------------------

IF OBJECT_ID('FoldingCash.GetFileValidationStartedStatusId') IS NOT NULL
BEGIN
	DROP FUNCTION [FoldingCash].[GetFileValidationStartedStatusId];
END
GO

CREATE FUNCTION [FoldingCash].[GetFileValidationStartedStatusId] () RETURNS INT
AS
BEGIN
	DECLARE @StatusId INT;
	
	SELECT @StatusId = StatusId FROM [FoldingCash].[Statuses] WHERE [Status] = 'FILE VALIDATION STARTED';
				 
	RETURN @StatusId;
END
GO

-----------------------------------------------------------------

IF OBJECT_ID('FoldingCash.GetFileValidatedStatusId') IS NOT NULL
BEGIN
	DROP FUNCTION [FoldingCash].[GetFileValidatedStatusId];
END
GO

CREATE FUNCTION [FoldingCash].[GetFileValidatedStatusId] () RETURNS INT
AS
BEGIN
	DECLARE @StatusId INT;
	
	SELECT @StatusId = StatusId FROM [FoldingCash].[Statuses] WHERE [Status] = 'FILE VALIDATED';
				 
	RETURN @StatusId;
END
GO

-----------------------------------------------------------------

IF OBJECT_ID('FoldingCash.GetFileValidationErrorStatusId') IS NOT NULL
BEGIN
	DROP FUNCTION [FoldingCash].[GetFileValidationErrorStatusId];
END
GO

CREATE FUNCTION [FoldingCash].[GetFileValidationErrorStatusId] () RETURNS INT
AS
BEGIN
	DECLARE @StatusId INT;
	
	SELECT @StatusId = StatusId FROM [FoldingCash].[Statuses] WHERE [Status] = 'FILE VALIDATION ERROR';
				 
	RETURN @StatusId;
END
GO

-----------------------------------------------------------------

IF OBJECT_ID('FoldingCash.GetLastFileDownloadDateTime') IS NOT NULL
BEGIN
	DROP FUNCTION [FoldingCash].[GetLastFileDownloadDateTime];
END
GO

CREATE FUNCTION [FoldingCash].[GetLastFileDownloadDateTime] () RETURNS DATETIME
AS
BEGIN
	DECLARE @DownloadDateTime DATETIME;
	
	SELECT TOP (1) @DownloadDateTime = DownloadDateTime	FROM [FoldingCash].[Downloads]
	WHERE StatusId <> FoldingCash.GetFileDownloadStartedStatusId()
	AND StatusId <> FoldingCash.GetFileDownloadErrorStatusId()
	AND StatusId <> FoldingCash.GetFileValidationErrorStatusId()
	ORDER BY DownloadDateTime DESC;

	RETURN @DownloadDateTime;
END
GO

-----------------------------------------------------------------

IF OBJECT_ID('FoldingCash.FileDownloadStarted') IS NOT NULL
BEGIN
	DROP PROCEDURE [FoldingCash].[FileDownloadStarted];
END
GO

CREATE PROCEDURE [FoldingCash].[FileDownloadStarted] @DownloadId INT OUTPUT
AS
BEGIN
	DECLARE @FileDownloadStartedStatusId INT;

	BEGIN TRY
		BEGIN TRANSACTION
			SELECT @FileDownloadStartedStatusId = [FoldingCash].[GetFileDownloadStartedStatusId]();
	
			INSERT INTO [FoldingCash].[Downloads] (StatusId, FileId, DownloadDateTime)
			VALUES (@FileDownloadStartedStatusId, NULL, GETUTCDATE());
	
			SELECT TOP 1 @DownloadId = @@Identity FROM [FoldingCash].[Downloads];
		COMMIT
	END TRY
	BEGIN CATCH
		ROLLBACK;
		THROW;
	END CATCH
END
GO

-----------------------------------------------------------------

IF OBJECT_ID('FoldingCash.FileDownloadFinished') IS NOT NULL
	BEGIN
		DROP PROCEDURE [FoldingCash].[FileDownloadFinished];
	END
GO

CREATE PROCEDURE [FoldingCash].[FileDownloadFinished] 
	 @DownloadId INT
	,@FilePath NVARCHAR(250)
	,@FileName NVARCHAR(50)
	,@FileExtension NVARCHAR(5)
AS
BEGIN
	DECLARE @FileId INT;

	BEGIN TRY
		BEGIN TRANSACTION
			INSERT INTO [FoldingCash].[Files] ([FilePath], [FileName], [FileExtension])
			VALUES (@FilePath, @FileName, @FileExtension);

			SELECT TOP 1 @FileId = @@Identity FROM [FoldingCash].[Files];

			DECLARE @FileDownloadFinishedStatusId INT;

			UPDATE [FoldingCash].[Downloads] SET
					 FileId = @FileId
					,StatusId = FoldingCash.GetFileDownloadFinishedStatusId()
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

IF OBJECT_ID('FoldingCash.FileDownloadError') IS NOT NULL
	BEGIN
		DROP PROCEDURE [FoldingCash].[FileDownloadError];
	END
GO

CREATE PROCEDURE [FoldingCash].[FileDownloadError] @DownloadId INT ,@ErrorMessage NVARCHAR(500)
AS
BEGIN
	DECLARE @FileDownloadErrorStatusId INT;

	BEGIN TRY
		BEGIN TRANSACTION
			SELECT @FileDownloadErrorStatusId = [FoldingCash].GetFileDownloadErrorStatusId();

			UPDATE [FoldingCash].[Downloads] 
			SET StatusId = @FileDownloadErrorStatusId
			WHERE DownloadId = @DownloadId;

			INSERT INTO [FoldingCash].[Rejections] (DownloadId, Reason)
			VALUES (@DownloadId, @ErrorMessage);
		COMMIT
	END TRY
	BEGIN CATCH
		ROLLBACK;
		THROW;
	END CATCH
END
GO

-----------------------------------------------------------------

IF OBJECT_ID('FoldingCash.FileValidationStarted') IS NOT NULL
	BEGIN
		DROP PROCEDURE [FoldingCash].[FileValidationStarted];
	END
GO

CREATE PROCEDURE [FoldingCash].[FileValidationStarted] @DownloadId INT
AS
BEGIN
	DECLARE @FileValidationStartedStatusId INT;

	BEGIN TRY
		BEGIN TRANSACTION
			SELECT @FileValidationStartedStatusId = [FoldingCash].GetFileValidationStartedStatusId();

			UPDATE [FoldingCash].[Downloads] 
			SET StatusId = @FileValidationStartedStatusId
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

IF OBJECT_ID('FoldingCash.FileValidated') IS NOT NULL
	BEGIN
		DROP PROCEDURE [FoldingCash].[FileValidated];
	END
GO

CREATE PROCEDURE [FoldingCash].[FileValidated]
	@DownloadId INT
	,@FileUtcDateTime DATETIME
	,@FilePath NVARCHAR(250)
	,@FileName NVARCHAR(50)
	,@FileExtension NVARCHAR(5)
AS
BEGIN
	DECLARE @FileValidatedStatusId INT;
	DECLARE @FileId INT;

	BEGIN TRY
		BEGIN TRANSACTION
			SELECT @FileValidatedStatusId = [FoldingCash].GetFileValidatedStatusId();
			SELECT @FileId = FileId FROM [FoldingCash].[Files];

			UPDATE [FoldingCash].[Downloads] 
			SET StatusId = @FileValidatedStatusId, DownloadDateTime = @FileUtcDateTime
			WHERE DownloadId = @DownloadId;

			UPDATE [FoldingCash].[Files]
			SET FilePath = @FilePath, [FileName] = @FileName, FileExtension = @FileExtension
			WHERE FileId = @FileId;
		COMMIT
	END TRY
	BEGIN CATCH
		ROLLBACK;
		THROW;
	END CATCH
END
GO

-----------------------------------------------------------------

IF OBJECT_ID('FoldingCash.FileValidationError') IS NOT NULL
	BEGIN
		DROP PROCEDURE [FoldingCash].[FileValidationError];
	END
GO

CREATE PROCEDURE [FoldingCash].[FileValidationError] 
	@DownloadId INT 
	,@ErrorMessage NVARCHAR(500)
AS
BEGIN
	DECLARE @FileValidationErrorStatusId INT;

	BEGIN TRY
		BEGIN TRANSACTION
			SELECT @FileValidationErrorStatusId = [FoldingCash].GetFileValidationErrorStatusId();

			UPDATE [FoldingCash].[Downloads] 
			SET StatusId = @FileValidationErrorStatusId
			WHERE DownloadId = @DownloadId;

			INSERT INTO [FoldingCash].[Rejections] (DownloadId, Reason)
			VALUES (@DownloadId, @ErrorMessage);
		COMMIT
	END TRY
	BEGIN CATCH
		ROLLBACK;
		THROW;
	END CATCH
END
GO

-----------------------------------------------------------------