IF OBJECT_ID('FoldingCoin.UpdateToLatest') IS NOT NULL
	BEGIN
		DROP PROCEDURE [FoldingCoin].[UpdateToLatest];
	END
GO

CREATE PROCEDURE [FoldingCoin].[UpdateToLatest]
AS
BEGIN
	IF (SELECT COUNT(*) FROM [FoldingCoin].Statuses) <> 6
		BEGIN
			INSERT INTO [FoldingCoin].[Statuses] ([Status],StatusDescription)
			VALUES ('FILE DOWNLOAD STARTED', 'The stats file download service has started.')
				  ,('FILE DOWNLOAD FINISHED', 'The stats file download has finished.')
				  ,('FILE DOWNLOAD ERROR', 'There was an error during the file download process.')
				  ,('STATS UPLOAD STARTED', 'The stats upload has started.')
				  ,('STATS UPLOAD FINISHED', 'The stats upload has finished.')
				  ,('STATS UPLOAD ERROR', 'There was an error during the file download process.');
		END
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
	
	SELECT @StatusId = StatusId FROM [FoldingCoin].[Statuses] WHERE STATUS = 'FILE DOWNLOAD STARTED';
	
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
	
	SELECT @StatusId = StatusId FROM [FoldingCoin].[Statuses] WHERE STATUS = 'FILE DOWNLOAD FINISHED';
				 
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
	
	SELECT @StatusId = StatusId FROM [FoldingCoin].[Statuses] WHERE STATUS = 'FILE DOWNLOAD ERROR';
				 
	RETURN @StatusId;
END
GO

-----------------------------------------------------------------

IF OBJECT_ID('FoldingCoin.GetStatsUploadStartedStatusId') IS NOT NULL
BEGIN
	DROP FUNCTION [FoldingCoin].[GetStatsUploadStartedStatusId];
END
GO

CREATE FUNCTION [FoldingCoin].[GetStatsUploadStartedStatusId] () RETURNS INT
AS
BEGIN
	DECLARE @StatusId INT;
	
	SELECT @StatusId = StatusId	FROM [FoldingCoin].[Statuses] WHERE STATUS = 'STATS UPLOAD STARTED';

	RETURN @StatusId;
END
GO

-----------------------------------------------------------------

IF OBJECT_ID('FoldingCoin.GetStatsUploadFinishedStatusId') IS NOT NULL
BEGIN
	DROP FUNCTION [FoldingCoin].[GetStatsUploadFinishedStatusId];
END
GO

CREATE FUNCTION [FoldingCoin].[GetStatsUploadFinishedStatusId] () RETURNS INT
AS
BEGIN
	DECLARE @StatusId INT;
	
	SELECT @StatusId = StatusId	FROM [FoldingCoin].[Statuses] WHERE STATUS = 'STATS UPLOAD FINISHED';

	RETURN @StatusId;
END
GO

-----------------------------------------------------------------

IF OBJECT_ID('FoldingCoin.GetStatsUploadErrorStatusId') IS NOT NULL
BEGIN
	DROP FUNCTION [FoldingCoin].[GetStatsUploadErrorStatusId];
END
GO

CREATE FUNCTION [FoldingCoin].[GetStatsUploadErrorStatusId] () RETURNS INT
AS
BEGIN
	DECLARE @StatusId INT;
	
	SELECT @StatusId = StatusId	FROM [FoldingCoin].[Statuses] WHERE STATUS = 'STATS UPLOAD ERROR';

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
	
	SELECT @FileDownloadStartedStatusId = [FoldingCoin].[GetFileDownloadStartedStatusId]();
	
	INSERT INTO [FoldingCoin].[Files] ([FileName], FileExtension, FileData)
	VALUES (NULL, NULL, NULL);

	DECLARE @FileId INT;
	SELECT TOP 1 @FileId = @@Identity FROM [FoldingCoin].[Files];

	INSERT INTO [FoldingCoin].[Downloads] (StatusId, FileId, DownloadDateTime)
	VALUES (@FileDownloadStartedStatusId, @FileId, CURRENT_TIMESTAMP);
	
	SELECT TOP 1 @DownloadId = @@Identity FROM [FoldingCoin].[Downloads];
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
	,@FileName NVARCHAR(50)
	,@FileExtension NVARCHAR(5)
	,@FileData NVARCHAR(max)
AS
BEGIN
	DECLARE @FileId INT;
	
	SELECT @FileId = FileId FROM [FoldingCoin].[Downloads] WHERE DownloadId = @DownloadId;

	UPDATE [FoldingCoin].[Files] SET
			 [FileName] = @FileName
			,FileExtension = @FileExtension
			,FileData = @FileData
	WHERE FileId = @FileId;

	DECLARE @FileDownloadFinishedStatusId INT;

	UPDATE [FoldingCoin].[Downloads] SET
			 FileId = @FileId
			,StatusId = FoldingCoin.GetFileDownloadFinishedStatusId()
	WHERE DownloadId = @DownloadId;
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
	SELECT @FileDownloadErrorStatusId = FoldingCoin.GetfileDownloadErrorStatusId();

	UPDATE [FoldingCoin].[Downloads] 
	SET StatusId = @FileDownloadErrorStatusId
	WHERE DownloadId = @DownloadId;

	INSERT INTO [FoldingCoin].[Rejections] (DownloadId, LineNumber, Reason)
	VALUES (@DownloadId, NULL, @ErrorMessage);
END
GO

-----------------------------------------------------------------

IF OBJECT_ID('FoldingCoin.DownloadsReadyForUpload') IS NOT NULL
	BEGIN
		DROP VIEW [FoldingCoin].[DownloadsReadyForUpload];
	END
GO

CREATE VIEW [FoldingCoin].[DownloadsReadyForUpload]
AS
	SELECT DownloadId  FROM [FoldingCoin].[Downloads] D
	INNER JOIN [FoldingCoin].[Files] F ON D.FileId = F.FileId
	WHERE StatusId = FoldingCoin.GetFileDownloadFinishedStatusId();
GO

-----------------------------------------------------------------

IF OBJECT_ID('FoldingCoin.GetFileData') IS NOT NULL
	BEGIN
		DROP PROCEDURE [FoldingCoin].[GetFileData];
	END
GO

CREATE PROCEDURE [FoldingCoin].[GetFileData] 
	 @DownloadId INT
	,@FileName NVARCHAR(50) OUTPUT
	,@FileExtension NVARCHAR(5) OUTPUT
	,@FileData NVARCHAR(max) OUTPUT
AS

BEGIN
	SELECT @FileName = [FileName]
		  ,@FileExtension = FileExtension
		  ,@FileData = FileData
	FROM [FoldingCoin].[Downloads] D
	INNER JOIN [FoldingCoin].[Files] F ON D.FileId = F.FileId
	WHERE DownloadId = @DownloadId;
END
GO

-----------------------------------------------------------------

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
	WHERE DownloadId = @DownloadId;
END
GO

-----------------------------------------------------------------

IF OBJECT_ID('FoldingCoin.AddUserData') IS NOT NULL
BEGIN
	DROP PROCEDURE [FoldingCoin].[AddUserData];
END
GO

CREATE PROCEDURE [FoldingCoin].[AddUserData]
	 @DownloadId INT
	,@FAHUserName NVARCHAR(50)
	,@TotalPoints BIGINT
	,@WorkUnits BIGINT
	,@TeamNumber BIGINT
	,@FriendlyName NVARCHAR(50)
	,@BitcoinAddress NVARCHAR(50)
AS
BEGIN
	DECLARE @TeamId INT;
	DECLARE @UserId INT;
	DECLARE @TeamMemberId INT;
	DECLARE @FAHDataId INT;
	DECLARE @FAHDataRunId INT;
	
	IF (SELECT COUNT(1) FROM [FoldingCoin].[Teams]	WHERE TeamNumber = @TeamNumber) = 0
		BEGIN
			INSERT INTO [FoldingCoin].[Teams] (TeamNumber, TeamName)
			VALUES (@TeamNumber, NULL);

			SELECT TOP 1 @TeamId = @@Identity FROM [FoldingCoin].[Teams];
		END
	ELSE
		BEGIN
			SELECT @TeamId = TeamId FROM [FoldingCoin].[Teams] WHERE TeamNumber = @TeamNumber;
		END
		
	IF (SELECT COUNT(1) FROM [FoldingCoin].[Users] WHERE UserName = @FAHUserName) = 0
		BEGIN
			INSERT INTO [FoldingCoin].[Users] (UserName, FriendlyName, BitcoinAddress)
			VALUES (@FAHUserName, @FriendlyName, @BitcoinAddress);

			SELECT TOP 1 @UserId = @@Identity FROM [FoldingCoin].[Users];

			INSERT INTO [FoldingCoin].[TeamMembers] (TeamId, UserId)
			VALUES (@TeamId, @UserId);
			
			SELECT TOP 1 @TeamMemberId = @@Identity FROM [FoldingCoin].[TeamMembers];

			INSERT INTO [FoldingCoin].[FAHData] (UserName, TotalPoints, WorkUnits, TeamNumber)
			VALUES (@FAHUserName, @TotalPoints, @WorkUnits, @TeamNumber);

			SELECT TOP 1 @FAHDataId = @@Identity FROM [FoldingCoin].[FAHData];

			INSERT INTO [FoldingCoin].[FAHDataRuns] (FAHDataId, DownloadId, TeamMemberId)
			VALUES (@FAHDataId, @DownloadId, @TeamMemberId);

			SELECT TOP 1 @FAHDataRunId = @@Identity FROM [FoldingCoin].[FAHDataRuns];

			INSERT INTO [FoldingCoin].[UserStats] (FAHDataRunId, Points, WorkUnits)
			VALUES (@FAHDataRunId, @TotalPoints, @WorkUnits);
		END
	ELSE
		BEGIN
			SELECT @UserId = UserId FROM [FoldingCoin].[Users] WHERE UserName = @FAHUserName;

			UPDATE [FoldingCoin].[Users]
			SET FriendlyName = @FriendlyName, BitcoinAddress = @BitcoinAddress
			WHERE (UserId = @UserId AND FriendlyName <> @FriendlyName) 
			   OR (UserId = @UserId AND BitcoinAddress <> @BitcoinAddress)

			IF (SELECT COUNT(1) FROM [FoldingCoin].[TeamMembers] WHERE TeamId = @TeamId AND UserId = @UserId) = 0
				BEGIN
					INSERT INTO [FoldingCoin].[TeamMembers] (TeamId, UserId)
					VALUES (@TeamId, @UserId);
		
					SELECT TOP 1 @TeamMemberId = @@Identity FROM [FoldingCoin].[TeamMembers];
				END
			ELSE
				BEGIN
					SELECT @TeamMemberId = TeamMemberId FROM [FoldingCoin].[TeamMembers] WHERE TeamId = @TeamId AND UserId = @UserId;
				END
			
			IF (SELECT COUNT(1) FROM [FoldingCoin].[FAHData] WHERE UserName = @FAHUserName AND TeamNumber = @TeamNumber) = 0
				BEGIN
					INSERT INTO [FoldingCoin].[FAHData] (UserName, TotalPoints, WorkUnits, TeamNumber)
					VALUES (@FAHUserName, @TotalPoints, @WorkUnits, @TeamNumber);
		
					SELECT TOP 1 @FAHDataId = @@Identity FROM [FoldingCoin].[FAHData];
				END
			ELSE
				BEGIN
					UPDATE [FoldingCoin].[FAHData]
					SET TotalPoints = @TotalPoints, WorkUnits = @WorkUnits
					WHERE UserName = @FAHUserName AND TeamNumber = @TeamNumber;
		
					SELECT @FAHDataId = FAHDataId FROM [FoldingCoin].[FAHData] WHERE UserName = @FAHUserName AND TeamNumber = @TeamNumber;
				END
				
			IF (SELECT COUNT(1)	FROM [FoldingCoin].[FAHDataRuns] WHERE DownloadId = @DownloadId AND FAHDataId = @FAHDataId) = 0
				BEGIN
					INSERT INTO [FoldingCoin].[FAHDataRuns] (FAHDataId, DownloadId, TeamMemberId)
					VALUES (@FAHDataId, @DownloadId, @TeamMemberId);
		
					SELECT TOP 1 @FAHDataRunId = @@Identity FROM [FoldingCoin].[FAHDataRuns];
				END
			ELSE
				BEGIN
					SELECT @FAHDataRunId = FAHDataRunId FROM [FoldingCoin].[FAHDataRuns] WHERE DownloadId = @DownloadId AND FAHDataId = @FAHDataId;
				END

			IF (SELECT COUNT(1) FROM [FoldingCoin].[UserStats] INNER JOIN [FoldingCoin].[FAHDataRuns] ON [FoldingCoin].[UserStats].[FAHDataRunId] = [FoldingCoin].[FAHDataRuns].[FAHDataRunId] WHERE TeamMemberId = @TeamMemberId AND Points >= @TotalPoints AND WorkUnits >= @WorkUnits) = 0
				BEGIN
					INSERT INTO [FoldingCoin].[UserStats] (FAHDataRunId, Points, WorkUnits)
					VALUES (@FAHDataRunId, @TotalPoints, @WorkUnits);
				END
		END
END
GO

-----------------------------------------------------------------

IF OBJECT_ID('FoldingCoin.AddUserRejection') IS NOT NULL
	BEGIN
		DROP PROCEDURE [FoldingCoin].[AddUserRejection];
	END
GO

CREATE PROCEDURE [FoldingCoin].[AddUserRejection] 
	 @DownloadId INT
	,@LineNumber INT
	,@RejectionReason NVARCHAR(500)
AS
BEGIN	
	INSERT INTO [FoldingCoin].[Rejections] (DownloadId, LineNumber, Reason)
	VALUES (@DownloadId, @LineNumber, @RejectionReason);
END
GO

-----------------------------------------------------------------

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
END
GO

-----------------------------------------------------------------

IF OBJECT_ID('FoldingCoin.StatsUploadError') IS NOT NULL
	BEGIN
		DROP PROCEDURE [FoldingCoin].[StatsUploadError];
	END
GO

CREATE PROCEDURE [FoldingCoin].[StatsUploadError] @DownloadId INT, @ErrorMessage NVARCHAR(500)
AS
BEGIN
	UPDATE [FoldingCoin].[Downloads]
	SET StatusId = FoldingCoin.GetStatsUploadErrorStatusId()
	WHERE DownloadId = @DownloadId;
	
	INSERT INTO [FoldingCoin].[Rejections] (DownloadId, LineNumber, Reason)
	VALUES (@DownloadId, NULL, @ErrorMessage);
END
GO