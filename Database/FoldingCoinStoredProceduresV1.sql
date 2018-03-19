IF OBJECT_ID('FoldingCoin.UpdateToLatest') IS NOT NULL
BEGIN
	DROP PROCEDURE [FoldingCoin].[UpdateToLatest];
END
GO

CREATE PROCEDURE [FoldingCoin].[UpdateToLatest]
AS
BEGIN
	DECLARE @Version1000 NVARCHAR(50);

	SET @Version1000 = '1.0.0.0';

	IF (
			SELECT Count(1)
			FROM [FoldingCoin].[Versions]
			WHERE VersionNumber = @Version1000
			) = 0
	BEGIN
		INSERT INTO [FoldingCoin].[Versions] (
			VersionNumber
			,LastRun
			)
		VALUES (
			@Version1000
			,CURRENT_TIMESTAMP
			);

		DECLARE @Version1000Id INT;

		SET @Version1000Id = (
				SELECT VersionId
				FROM [FoldingCoin].[Versions]
				WHERE VersionNumber = @Version1000
				);

		INSERT INTO [FoldingCoin].[Statuses] (
			STATUS
			,StatusDescription
			)
		VALUES (
			'FILE DOWNLOAD STARTED'
			,'The stats file download service has started.'
			)
			,(
			'FILE DOWNLOAD FINISHED'
			,'The stats file download has finished.'
			)
			,(
			'FILE DOWNLOAD ERROR'
			,'There was an error during the file download process.'
			)
			,(
			'STATS UPLOAD STARTED'
			,'The stats upload has started.'
			)
			,(
			'STATS UPLOAD FINISHED'
			,'The stats upload has finished.'
			)
			,(
			'STATS UPLOAD ERROR'
			,'There was an error during the file download process.'
			);
	END
END
GO

IF OBJECT_ID('FoldingCoin.GetCurrentVersionId') IS NOT NULL
BEGIN
	DROP FUNCTION [FoldingCoin].[GetCurrentVersionId];
END
GO

CREATE FUNCTION [FoldingCoin].[GetCurrentVersionId] ()
RETURNS INT
AS
BEGIN
	RETURN FoldingCoin.GetVersion1000Id();
END
GO

IF OBJECT_ID('FoldingCoin.GetVersion1000Id') IS NOT NULL
BEGIN
	DROP FUNCTION [FoldingCoin].[GetVersion1000Id];
END
GO

CREATE FUNCTION [FoldingCoin].[GetVersion1000Id] ()
RETURNS INT
AS
BEGIN
	DECLARE @VersionNumberId INT;

	SET @VersionNumberId = (
			SELECT TOP 1 VersionId
			FROM [FoldingCoin].[Versions]
			ORDER BY VersionNumber ASC
			);

	RETURN @VersionNumberId;
END
GO

IF OBJECT_ID('FoldingCoin.GetFileDownloadStartedStatusId') IS NOT NULL
BEGIN
	DROP FUNCTION [FoldingCoin].[GetFileDownloadStartedStatusId];
END
GO

CREATE FUNCTION [FoldingCoin].[GetFileDownloadStartedStatusId] ()
RETURNS INT
AS
BEGIN
	DECLARE @StatusId INT;

	SET @StatusId = (
			SELECT StatusId
			FROM [FoldingCoin].[Statuses]
			WHERE STATUS = 'FILE DOWNLOAD STARTED'
			);

	RETURN @StatusId;
END
GO

IF OBJECT_ID('FoldingCoin.GetFileDownloadFinishedStatusId') IS NOT NULL
BEGIN
	DROP FUNCTION [FoldingCoin].[GetFileDownloadFinishedStatusId];
END
GO

CREATE FUNCTION [FoldingCoin].[GetFileDownloadFinishedStatusId] ()
RETURNS INT
AS
BEGIN
	DECLARE @StatusId INT;

	SET @StatusId = (
			SELECT StatusId
			FROM [FoldingCoin].[Statuses]
			WHERE STATUS = 'FILE DOWNLOAD FINISHED'
			);

	RETURN @StatusId;
END
GO

IF OBJECT_ID('FoldingCoin.GetFileDownloadErrorStatusId') IS NOT NULL
BEGIN
	DROP FUNCTION [FoldingCoin].[GetFileDownloadErrorStatusId];
END
GO

CREATE FUNCTION [FoldingCoin].[GetFileDownloadErrorStatusId] ()
RETURNS INT
AS
BEGIN
	DECLARE @StatusId INT;

	SET @StatusId = (
			SELECT StatusId
			FROM [FoldingCoin].[Statuses]
			WHERE STATUS = 'FILE DOWNLOAD ERROR'
			);

	RETURN @StatusId;
END
GO

IF OBJECT_ID('FoldingCoin.GetStatsUploadStartedStatusId') IS NOT NULL
BEGIN
	DROP FUNCTION [FoldingCoin].[GetStatsUploadStartedStatusId];
END
GO

CREATE FUNCTION [FoldingCoin].[GetStatsUploadStartedStatusId] ()
RETURNS INT
AS
BEGIN
	DECLARE @StatusId INT;

	SET @StatusId = (
			SELECT StatusId
			FROM [FoldingCoin].[Statuses]
			WHERE STATUS = 'STATS UPLOAD STARTED'
			);

	RETURN @StatusId;
END
GO

IF OBJECT_ID('FoldingCoin.GetStatsUploadFinishedStatusId') IS NOT NULL
BEGIN
	DROP FUNCTION [FoldingCoin].[GetStatsUploadFinishedStatusId];
END
GO

CREATE FUNCTION [FoldingCoin].[GetStatsUploadFinishedStatusId] ()
RETURNS INT
AS
BEGIN
	DECLARE @StatusId INT;

	SET @StatusId = (
			SELECT StatusId
			FROM [FoldingCoin].[Statuses]
			WHERE STATUS = 'STATS UPLOAD FINISHED'
			);

	RETURN @StatusId;
END
GO

IF OBJECT_ID('FoldingCoin.GetStatsUploadErrorStatusId') IS NOT NULL
BEGIN
	DROP FUNCTION [FoldingCoin].[GetStatsUploadErrorStatusId];
END
GO

CREATE FUNCTION [FoldingCoin].[GetStatsUploadErrorStatusId] ()
RETURNS INT
AS
BEGIN
	DECLARE @StatusId INT;

	SET @StatusId = (
			SELECT StatusId
			FROM [FoldingCoin].[Statuses]
			WHERE STATUS = 'STATS UPLOAD ERROR'
			);

	RETURN @StatusId;
END
GO

IF OBJECT_ID('FoldingCoin.GetLastFileDownloadDateTime') IS NOT NULL
BEGIN
	DROP FUNCTION [FoldingCoin].[GetLastFileDownloadDateTime];
END
GO

CREATE FUNCTION [FoldingCoin].[GetLastFileDownloadDateTime] ()
RETURNS DATETIME
AS
BEGIN
	DECLARE @DownloadDateTime DATETIME;

	SET @DownloadDateTime = (
			SELECT TOP (1) DownloadDateTime
			FROM [FoldingCoin].[Downloads]
			WHERE StatusId = FoldingCoin.GetFileDownloadFinishedStatusId()
			ORDER BY DownloadDateTime DESC
			);

	RETURN @DownloadDateTime;
END
GO

IF OBJECT_ID('FoldingCoin.GetFAHDataId') IS NOT NULL
BEGIN
	DROP PROCEDURE [FoldingCoin].[GetFAHDataId];
END
GO

CREATE PROCEDURE [FoldingCoin].[GetFAHDataId] @FAHUserName NVARCHAR(50)
	,@TotalPoints NVARCHAR(50)
	,@WorkUnits NVARCHAR(50)
	,@TeamNumber NVARCHAR(50)
	,@FAHDataId INT OUTPUT
AS
BEGIN
	IF (
			(
				SELECT COUNT(1)
				FROM [FoldingCoin].[FAHData]
				WHERE UserName = @FAHUserName
				) = 0
			)
	BEGIN
		INSERT INTO [FoldingCoin].[FAHData] (
			UserName
			,TotalPoints
			,WorkUnit
			,TeamNumber
			)
		VALUES (
			@FAHUserName
			,@TotalPoints
			,@WorkUnits
			,@TeamNumber
			);

		SET @FAHDataId = (
				SELECT TOP 1 @@Identity
				FROM [FoldingCoin].[FAHData]
				);
	END
	ELSE
	BEGIN
		SET @FAHDataId = (
				SELECT FAHDataId
				FROM [FoldingCoin].[FAHData]
				WHERE UserName = @FAHUserName
				);
	END
END
GO

IF OBJECT_ID('FoldingCoin.NewFileDownloadStarted') IS NOT NULL
BEGIN
	DROP PROCEDURE [FoldingCoin].[NewFileDownloadStarted];
END
GO

CREATE PROCEDURE [FoldingCoin].[NewFileDownloadStarted] @DownloadId INT OUTPUT
AS
BEGIN
	DECLARE @FileDownloadStartedStatusId INT;

	SET @FileDownloadStartedStatusId = (
			SELECT [FoldingCoin].[GetFileDownloadStartedStatusId]()
			);

	INSERT INTO [FoldingCoin].[Files] (
		FileName
		,FileExtension
		,FileData
		)
	VALUES (
		''
		,''
		,''
		);

	DECLARE @FileId INT;

	SET @FileId = (
			SELECT TOP 1 @@Identity
			FROM [FoldingCoin].[Files]
			);

	INSERT INTO [FoldingCoin].[Downloads] (
		StatusId
		,FileId
		,DownloadDateTime
		,ErrorMessage
		)
	VALUES (
		@FileDownloadStartedStatusId
		,@FileId
		,CURRENT_TIMESTAMP
		,''
		);

	SET @DownloadId = (
			SELECT TOP 1 @@Identity
			FROM [FoldingCoin].[Downloads]
			);

	RETURN 0;
END
GO

IF OBJECT_ID('FoldingCoin.FileDownloadFinished') IS NOT NULL
BEGIN
	DROP PROCEDURE [FoldingCoin].[FileDownloadFinished];
END
GO

CREATE PROCEDURE [FoldingCoin].[FileDownloadFinished] @DownloadId INT
	,@FileName NVARCHAR(50)
	,@FileExtension NVARCHAR(5)
	,@FileData NVARCHAR(max)
AS
BEGIN
	DECLARE @FileId INT;

	SET @FileId = (
			SELECT FileId
			FROM [FoldingCoin].[Downloads]
			WHERE DownloadId = @DownloadId
			);

	UPDATE [FoldingCoin].[Files]
	SET FileName = @FileName
		,FileExtension = @FileExtension
		,FileData = @FileData
	WHERE FileId = @FileId;

	DECLARE @FileDownloadFinishedStatusId INT;

	SET @FileDownloadFinishedStatusId = (
			SELECT FoldingCoin.GetFileDownloadFinishedStatusId()
			);

	UPDATE [FoldingCoin].[Downloads]
	SET FileId = @FileId
		,StatusId = @FileDownloadFinishedStatusId
	WHERE DownloadId = @DownloadId;

	RETURN 0;
END
GO

IF OBJECT_ID('FoldingCoin.FileDownloadError') IS NOT NULL
BEGIN
	DROP PROCEDURE [FoldingCoin].[FileDownloadError];
END
GO

CREATE PROCEDURE [FoldingCoin].[FileDownloadError] @DownloadId INT
	,@ErrorMessage NVARCHAR(500)
AS
BEGIN
	DECLARE @FileDownloadErrorStatusId INT;

	SET @FileDownloadErrorStatusId = (
			SELECT FoldingCoin.GetfileDownloadErrorStatusId()
			);

	UPDATE [FoldingCoin].[Downloads]
	SET StatusId = @FileDownloadErrorStatusId
		,ErrorMessage = @ErrorMessage
	WHERE DownloadId = @DownloadId;

	RETURN 0;
END
GO

IF OBJECT_ID('FoldingCoin.DownloadsReadyForUpload') IS NOT NULL
BEGIN
	DROP VIEW [FoldingCoin].[DownloadsReadyForUpload];
END
GO

CREATE VIEW [FoldingCoin].[DownloadsReadyForUpload]
AS
SELECT DownloadId
FROM [FoldingCoin].[Downloads] D
INNER JOIN [FoldingCoin].[Files] F ON D.FileId = F.FileId
WHERE StatusId = FoldingCoin.GetFileDownloadFinishedStatusId();
GO

IF OBJECT_ID('FoldingCoin.GetFileData') IS NOT NULL
BEGIN
	DROP PROCEDURE [FoldingCoin].[GetFileData];
END
GO

CREATE PROCEDURE [FoldingCoin].[GetFileData] @DownloadId INT
	,@FileName NVARCHAR(50) OUTPUT
	,@FileExtension NVARCHAR(5) OUTPUT
	,@FileData NVARCHAR(max) OUTPUT
AS
BEGIN
	SELECT @FileName = FileName
		,@FileExtension = FileExtension
		,@FileData = FileData
	FROM [FoldingCoin].[Downloads] D
	INNER JOIN [FoldingCoin].[Files] F ON D.FileId = F.FileId
	WHERE DownloadId = @DownloadId;

	RETURN 0;
END
GO

IF OBJECT_ID('FoldingCoin.StartStatsUpload') IS NOT NULL
BEGIN
	DROP PROCEDURE [FoldingCoin].[StartStatsUpload];
END
GO

CREATE PROCEDURE [FoldingCoin].[StartStatsUpload] @DownloadId INT
AS
BEGIN
	UPDATE [FoldingCoin].[Downloads]
	SET StatusId = FoldingCoin.GetStatsUploadStartedStatusId()
	WHERE DownloadId = @DownloadId

	RETURN 0;
END
GO

IF OBJECT_ID('FoldingCoin.AddUserData') IS NOT NULL
BEGIN
	DROP PROCEDURE [FoldingCoin].[AddUserData];
END
GO

CREATE PROCEDURE [FoldingCoin].[AddUserData] @DownloadId INT
	,@FAHUserName NVARCHAR(50)
	,@TotalPoints NVARCHAR(50)
	,@WorkUnits NVARCHAR(50)
	,@TeamNumber NVARCHAR(50)
	,@FriendlyName NVARCHAR(50)
	,@BitcoinAddress NVARCHAR(50)
AS
BEGIN
	DECLARE @UserId INT;

	IF (
			(
				SELECT COUNT(1)
				FROM [FoldingCoin].[Users]
				WHERE UserName = @FAHUserName
				) = 0
			)
	BEGIN
		INSERT INTO [FoldingCoin].[Users] (
			UserName
			,FriendlyName
			,BitcoinAddress
			)
		VALUES (
			@FAHUserName
			,@FriendlyName
			,@BitcoinAddress
			);

		SET @UserId = (
				SELECT TOP 1 @@Identity
				FROM [FoldingCoin].[Users]
				);
	END
	ELSE
	BEGIN
		SET @UserId = (
				SELECT UserId
				FROM [FoldingCoin].[Users]
				WHERE UserName = @FAHUserName
				);

		IF (
				(
					SELECT COUNT(1)
					FROM [FoldingCoin].[Users]
					WHERE UserId = @UserId
						AND FriendlyName = @FriendlyName
						AND BitcoinAddress = @BitcoinAddress
					) = 0
				)
		BEGIN
			UPDATE [FoldingCoin].[Users]
			SET FriendlyName = @FriendlyName
				,BitcoinAddress = @BitcoinAddress
			WHERE UserId = @UserId;
		END
	END

	DECLARE @FAHDataId INT;

	EXEC [FoldingCoin].[GetFAHDataId] @FAHUserName
		,@TotalPoints
		,@WorkUnits
		,@TeamNumber
		,@FAHDataId OUTPUT;

	DECLARE @RunDataId INT;

	IF (
			(
				SELECT COUNT(1)
				FROM [FoldingCoin].[FAHDataRuns]
				WHERE DownloadId = @DownloadId
					AND FAHDataId = @FAHDataId
					AND VersionId = FoldingCoin.GetCurrentVersionId()
				) = 0
			)
	BEGIN
		INSERT INTO [FoldingCoin].[RunData] (
			VersionId
			,RunDate
			,LastRunTime
			,NumberOfRuns
			)
		VALUES (
			FoldingCoin.GetCurrentVersionId()
			,CAST(CURRENT_TIMESTAMP AS DATE)
			,CAST(CURRENT_TIMESTAMP AS TIME)
			,1
			);

		SET @RunDataId = (
				SELECT TOP 1 @@Identity
				FROM [FoldingCoin].[RunData]
				);

		INSERT INTO [FoldingCoin].[FAHDataRuns] (
			FAHDataId
			,DownloadId
			,VersionId
			,RunDataId
			)
		VALUES (
			@FAHDataId
			,@DownloadId
			,FoldingCoin.GetCurrentVersionId()
			,@RunDataId
			);
	END
	ELSE
	BEGIN
		SET @RunDataId = (
				SELECT RunDataId
				FROM [FoldingCoin].[FAHDataRuns]
				WHERE DownloadId = @DownloadId
					AND FAHDataId = @FAHDataId
					AND VersionId = FoldingCoin.GetCurrentVersionId()
				);
	END

	--Question: How to get start and end? Should/can this be linked to download Id instead
	INSERT INTO [FoldingCoin].[UserStats] (
		UserId
		,StartDateTime
		,EndDateTime
		,Points
		)
	VALUES (
		@UserId
		,CURRENT_TIMESTAMP
		,CURRENT_TIMESTAMP
		,@TotalPoints
		);

	DECLARE @TeamId INT;

	IF (
			(
				SELECT COUNT(1)
				FROM [FoldingCoin].[Teams]
				WHERE TeamNumber = @TeamNumber
				) = 0
			)
	BEGIN
		INSERT INTO [FoldingCoin].[Teams] (
			TeamNumber
			,TeamName
			)
		VALUES (
			@TeamNumber
			,''
			);

		SET @TeamId = (
				SELECT TOP 1 @@Identity
				FROM [FoldingCoin].[Teams]
				);
	END
	ELSE
	BEGIN
		SET @TeamId = (
				SELECT TeamId
				FROM [FoldingCoin].[Teams]
				WHERE TeamNumber = @TeamNumber
				);
	END

	IF (
			(
				SELECT COUNT(1)
				FROM [FoldingCoin].[TeamMembers]
				WHERE TeamId = @TeamId
					AND UserId = @UserId
				) = 0
			)
	BEGIN
		INSERT INTO [FoldingCoin].[TeamMembers] (
			TeamId
			,UserId
			)
		VALUES (
			@TeamId
			,@UserId
			);
	END

	RETURN 0;
END
GO

IF OBJECT_ID('FoldingCoin.AddUserRejection') IS NOT NULL
BEGIN
	DROP PROCEDURE [FoldingCoin].[AddUserRejection];
END
GO

CREATE PROCEDURE [FoldingCoin].[AddUserRejection] @DownloadId INT
	,@FAHUserName NVARCHAR(50)
	,@LineNumber INT
	,@RejectionReason NVARCHAR(500)
AS
BEGIN
	DECLARE @FAHDataId INT;

	EXEC [FoldingCoin].[GetFAHDataId] @FAHUserName
		,''
		,''
		,''
		,@FAHDataId OUTPUT;

	INSERT INTO [FoldingCoin].[Rejections] (
		FAHDataId
		,VersionId
		,LineNumber
		,Reason
		)
	VALUES (
		@FAHDataId
		,FoldingCoin.GetCurrentVersionId()
		,@LineNumber
		,@RejectionReason
		);

	RETURN 0;
END
GO

IF OBJECT_ID('FoldingCoin.StatsUploadFinished') IS NOT NULL
BEGIN
	DROP PROCEDURE [FoldingCoin].[StatsUploadFinished];
END
GO

CREATE PROCEDURE [FoldingCoin].[StatsUploadFinished] @DownloadId INT
AS
BEGIN
	UPDATE [FoldingCoin].[Downloads]
	SET StatusId = FoldingCoin.GetStatsUploadFinishedStatusId()
	WHERE DownloadId = @DownloadId

	RETURN 0;
END
GO

IF OBJECT_ID('FoldingCoin.StatsUploadError') IS NOT NULL
BEGIN
	DROP PROCEDURE [FoldingCoin].[StatsUploadError];
END
GO

CREATE PROCEDURE [FoldingCoin].[StatsUploadError] @DownloadId INT
	,@ErrorMessage NVARCHAR(500)
AS
BEGIN
	UPDATE [FoldingCoin].[Downloads]
	SET StatusId = FoldingCoin.GetStatsUploadErrorStatusId()
		,ErrorMessage = @ErrorMessage
	WHERE DownloadId = @DownloadId

	RETURN 0;
END
GO