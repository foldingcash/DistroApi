IF OBJECT_ID('FoldingCoin.GetFoldingUsers') IS NOT NULL
	BEGIN
		DROP PROCEDURE [FoldingCoin].[GetFoldingUsers];
	END
GO

CREATE PROCEDURE [FoldingCoin].[GetFoldingUsers] @StartDate DATE,
	@EndDate DATE
AS
BEGIN
	DECLARE @StartDateTime DATETIME;
	DECLARE @EndDateTime DATETIME;

	--This is converted to add 12 hours because of FLDC specific implementation needs
	SET @StartDateTime = DATEADD(HOUR, 12, CONVERT(DATETIME, @StartDate));
	SET @EndDateTime = DATEADD(DAY, 1, DATEADD(HOUR, 12, CONVERT(DATETIME, @EndDate)));

	WITH StartStats AS 
	(
		SELECT
			UserStats.UserStatId,  
			ISNULL(UserStats.Points, 0) AS Points,
			ISNULL(UserStats.WorkUnits, 0) AS WorkUnits,  
			TeamMembers.TeamMemberId,   
			Users.BitcoinAddress,
			Users.FriendlyName,
			Downloads.DownloadDateTime
		FROM FoldingCoin.UserStats UserStats
			INNER JOIN FoldingCoin.FAHDataRuns FAHDataRuns ON UserStats.FAHDataRunId = FAHDataRuns.FAHDataRunId 
			INNER JOIN FoldingCoin.Downloads Downloads ON Downloads.DownloadId = FAHDataRuns.DownloadId 
			INNER JOIN FoldingCoin.TeamMembers TeamMembers ON FAHDataRuns.TeamMemberId = TeamMembers.TeamMemberId 
			INNER JOIN FoldingCoin.Users Users ON TeamMembers.UserId = Users.UserId 
			INNER JOIN 
			(
				SELECT    
					TeamMembers.TeamMemberId,   
					Users.BitcoinAddress,
					MAX(Downloads.DownloadDateTime) AS MaxDate
				FROM FoldingCoin.UserStats UserStats
					INNER JOIN FoldingCoin.FAHDataRuns FAHDataRuns ON UserStats.FAHDataRunId = FAHDataRuns.FAHDataRunId 
					INNER JOIN FoldingCoin.Downloads Downloads ON Downloads.DownloadId = FAHDataRuns.DownloadId 
					INNER JOIN FoldingCoin.TeamMembers TeamMembers ON FAHDataRuns.TeamMemberId = TeamMembers.TeamMemberId 
					INNER JOIN FoldingCoin.Users Users ON TeamMembers.UserId = Users.UserId 
				WHERE 
					Downloads.StatusId = FoldingCoin.GetStatsUploadFinishedStatusId()  
					AND Downloads.DownloadDateTime < @StartDateTime
				GROUP BY
					TeamMembers.TeamMemberId,   
					Users.BitcoinAddress
			) StartingDate ON 
				StartingDate.TeamMemberID = TeamMembers.TeamMemberID
				AND StartingDate.BitcoinAddress = Users.BitcoinAddress
				AND StartingDate.MaxDate = Downloads.DownloadDateTime
	)
	,EndStats AS 
	(
		SELECT
			UserStats.UserStatId,  
			ISNULL(UserStats.Points, 0) AS Points,  
			ISNULL(UserStats.WorkUnits, 0) AS WorkUnits,  
			TeamMembers.TeamMemberId,   
			Users.BitcoinAddress,
			Users.FriendlyName,
			Downloads.DownloadDateTime
		FROM FoldingCoin.UserStats UserStats
			INNER JOIN FoldingCoin.FAHDataRuns FAHDataRuns ON UserStats.FAHDataRunId = FAHDataRuns.FAHDataRunId 
			INNER JOIN FoldingCoin.Downloads Downloads ON Downloads.DownloadId = FAHDataRuns.DownloadId 
			INNER JOIN FoldingCoin.TeamMembers TeamMembers ON FAHDataRuns.TeamMemberId = TeamMembers.TeamMemberId 
			INNER JOIN FoldingCoin.Users Users ON TeamMembers.UserId = Users.UserId 
			INNER JOIN 
			(
				SELECT    
					TeamMembers.TeamMemberId,   
					Users.BitcoinAddress,
					MAX(Downloads.DownloadDateTime) AS MaxDate
				FROM FoldingCoin.UserStats UserStats
					INNER JOIN FoldingCoin.FAHDataRuns FAHDataRuns ON UserStats.FAHDataRunId = FAHDataRuns.FAHDataRunId 
					INNER JOIN FoldingCoin.Downloads Downloads ON Downloads.DownloadId = FAHDataRuns.DownloadId 
					INNER JOIN FoldingCoin.TeamMembers TeamMembers ON FAHDataRuns.TeamMemberId = TeamMembers.TeamMemberId 
					INNER JOIN FoldingCoin.Users Users ON TeamMembers.UserId = Users.UserId 
				WHERE 
					Downloads.StatusId = FoldingCoin.GetStatsUploadFinishedStatusId()  
					AND Downloads.DownloadDateTime >= @StartDateTime
					AND Downloads.DownloadDateTime < @EndDateTime
				GROUP BY
					TeamMembers.TeamMemberId,   
					Users.BitcoinAddress
			) EndDate ON 
				EndDate.TeamMemberID = TeamMembers.TeamMemberID
				AND EndDate.BitcoinAddress = Users.BitcoinAddress
				AND EndDate.MaxDate = Downloads.DownloadDateTime
	)
	SELECT 
		FoldingUsers.TeamMemberId,
		FoldingUsers.FriendlyName,
		FoldingUsers.BitcoinAddress,
		FoldingUsers.PointsGained,
		FoldingUsers.WorkUnitsGained,
		FoldingUsers.StartPoints,
		FoldingUsers.StartWorkUnits,
		FoldingUsers.[EndPoints],
		FoldingUsers.EndWorkUnits
	FROM 
	(
		SELECT  
			EndStats.TeamMemberID,
			EndStats.FriendlyName,
			EndStats.BitcoinAddress,
			ISNULL(EndStats.Points - StartStats.Points, EndStats.Points) AS PointsGained,
			ISNULL(EndStats.WorkUnits - StartStats.WorkUnits, EndStats.WorkUnits) AS WorkUnitsGained,
			ISNULL(StartStats.Points, 0) AS StartPoints,
			ISNULL(StartStats.WorkUnits, 0) AS StartWorkUnits,
			EndStats.Points AS [EndPoints],
			EndStats.WorkUnits AS EndWorkUnits
		FROM EndStats 
			LEFT JOIN StartStats ON 
				EndStats.TeamMemberID = StartStats.TeamMemberID
				AND EndStats.BitcoinAddress = StartStats.BitcoinAddress
	) AS FoldingUsers
	WHERE FoldingUsers.PointsGained <> 0  
		AND FoldingUsers.WorkUnitsGained <> 0  
		AND FoldingUsers.BitcoinAddress IS NOT NULL
END
GO

-----------------------------------------------------------------

IF OBJECT_ID('FoldingCoin.GetMembers') IS NOT NULL
	BEGIN
		DROP PROCEDURE [FoldingCoin].[GetMembers];
	END
GO

CREATE PROCEDURE [FoldingCoin].[GetMembers] @StartDate DATETIME,
	@EndDate DATETIME
AS
BEGIN
	DECLARE @StartDateTime DATETIME;
	DECLARE @EndDateTime DATETIME;

	--This is converted to add 12 hours because of FLDC specific implementation needs
	SET @StartDateTime = DATEADD(HOUR, 12, CONVERT(DATETIME, @StartDate));
	SET @EndDateTime = DATEADD(DAY, 1, DATEADD(HOUR, 12, CONVERT(DATETIME, @EndDate)));

	WITH StartStats AS 
	(
		SELECT
			UserStats.UserStatId,
			ISNULL(UserStats.Points, 0) AS Points,
			ISNULL(UserStats.WorkUnits, 0) AS WorkUnits,
			TeamMembers.TeamMemberId,  
			Teams.TeamNumber,
			Users.UserName,
			Users.FriendlyName,
			Users.BitcoinAddress,
			Downloads.DownloadDateTime
		FROM FoldingCoin.UserStats UserStats
			INNER JOIN FoldingCoin.FAHDataRuns FAHDataRuns ON UserStats.FAHDataRunId = FAHDataRuns.FAHDataRunId 
			INNER JOIN FoldingCoin.Downloads Downloads ON Downloads.DownloadId = FAHDataRuns.DownloadId 
			INNER JOIN FoldingCoin.TeamMembers TeamMembers ON FAHDataRuns.TeamMemberId = TeamMembers.TeamMemberId 
			INNER JOIN FoldingCoin.Teams Teams ON TeamMembers.TeamId = Teams.TeamId
			INNER JOIN FoldingCoin.Users Users ON TeamMembers.UserId = Users.UserId 
			INNER JOIN 
			(
				SELECT    
					TeamMembers.TeamMemberId,   
					Users.BitcoinAddress,
					MAX(Downloads.DownloadDateTime) AS MaxDate
				FROM FoldingCoin.UserStats UserStats
					INNER JOIN FoldingCoin.FAHDataRuns FAHDataRuns ON UserStats.FAHDataRunId = FAHDataRuns.FAHDataRunId 
					INNER JOIN FoldingCoin.Downloads Downloads ON Downloads.DownloadId = FAHDataRuns.DownloadId 
					INNER JOIN FoldingCoin.TeamMembers TeamMembers ON FAHDataRuns.TeamMemberId = TeamMembers.TeamMemberId 
					INNER JOIN FoldingCoin.Teams Teams ON TeamMembers.TeamId = Teams.TeamId
					INNER JOIN FoldingCoin.Users Users ON TeamMembers.UserId = Users.UserId 
				WHERE 
					Downloads.StatusId = FoldingCoin.GetStatsUploadFinishedStatusId()  
					AND Downloads.DownloadDateTime < @StartDateTime
				GROUP BY
					TeamMembers.TeamMemberId,   
					Users.BitcoinAddress
			) StartingDate ON 
				StartingDate.TeamMemberID = TeamMembers.TeamMemberID
				AND StartingDate.MaxDate = Downloads.DownloadDateTime
	)
	,EndStats AS 
	(
		SELECT
			UserStats.UserStatId,
			Users.UserName,  
			ISNULL(UserStats.Points, 0) AS Points,  
			ISNULL(UserStats.WorkUnits, 0) AS WorkUnits,  
			TeamMembers.TeamMemberId,  
			Teams.TeamNumber, 
			Users.BitcoinAddress,
			Users.FriendlyName,
			Downloads.DownloadDateTime
		FROM FoldingCoin.UserStats UserStats
			INNER JOIN FoldingCoin.FAHDataRuns FAHDataRuns ON UserStats.FAHDataRunId = FAHDataRuns.FAHDataRunId 
			INNER JOIN FoldingCoin.Downloads Downloads ON Downloads.DownloadId = FAHDataRuns.DownloadId 
			INNER JOIN FoldingCoin.TeamMembers TeamMembers ON FAHDataRuns.TeamMemberId = TeamMembers.TeamMemberId 
			INNER JOIN FoldingCoin.Teams Teams ON TeamMembers.TeamId = Teams.TeamId
			INNER JOIN FoldingCoin.Users Users ON TeamMembers.UserId = Users.UserId 
			INNER JOIN 
			(
				SELECT    
					TeamMembers.TeamMemberId,   
					Users.BitcoinAddress,
					MAX(Downloads.DownloadDateTime) AS MaxDate
				FROM FoldingCoin.UserStats UserStats
					INNER JOIN FoldingCoin.FAHDataRuns FAHDataRuns ON UserStats.FAHDataRunId = FAHDataRuns.FAHDataRunId 
					INNER JOIN FoldingCoin.Downloads Downloads ON Downloads.DownloadId = FAHDataRuns.DownloadId 
					INNER JOIN FoldingCoin.TeamMembers TeamMembers ON FAHDataRuns.TeamMemberId = TeamMembers.TeamMemberId 
					INNER JOIN FoldingCoin.Users Users ON TeamMembers.UserId = Users.UserId 
				WHERE 
					Downloads.StatusId = FoldingCoin.GetStatsUploadFinishedStatusId()  
					AND Downloads.DownloadDateTime >= @StartDateTime
					AND Downloads.DownloadDateTime < @EndDateTime
				GROUP BY
					TeamMembers.TeamMemberId,   
					Users.BitcoinAddress
			) EndDate ON 
				EndDate.TeamMemberID = TeamMembers.TeamMemberID
				AND EndDate.MaxDate = Downloads.DownloadDateTime
	)
	SELECT 
		FoldingUsers.UserName,
		FoldingUsers.TeamNumber,
		FoldingUsers.FriendlyName,
		FoldingUsers.BitcoinAddress,
		FoldingUsers.PointsGained,
		FoldingUsers.WorkUnitsGained,
		FoldingUsers.StartPoints,
		FoldingUsers.StartWorkUnits,
		FoldingUsers.[EndPoints],
		FoldingUsers.EndWorkUnits
	FROM 
	(
		SELECT
			EndStats.UserName,
			EndStats.TeamNumber,
			EndStats.FriendlyName,
			EndStats.BitcoinAddress,
			ISNULL(EndStats.Points - StartStats.Points, EndStats.Points) AS PointsGained,
			ISNULL(EndStats.WorkUnits - StartStats.WorkUnits, EndStats.WorkUnits) AS WorkUnitsGained,
			ISNULL(StartStats.Points, 0) AS StartPoints,
			ISNULL(StartStats.WorkUnits, 0) AS StartWorkUnits,
			EndStats.Points AS [EndPoints],
			EndStats.WorkUnits AS EndWorkUnits
		FROM EndStats 
			LEFT JOIN StartStats ON 
				EndStats.TeamMemberID = StartStats.TeamMemberID
	) AS FoldingUsers
END
GO

-----------------------------------------------------------------

IF OBJECT_ID('FoldingCoin.GetTeams') IS NOT NULL
	BEGIN
		DROP PROCEDURE [FoldingCoin].[GetTeams];
	END
GO

CREATE PROCEDURE [FoldingCoin].[GetTeams]
AS
BEGIN
	SELECT TeamNumber, TeamNumber AS TeamName FROM [FoldingCoin].[Teams];
END
GO