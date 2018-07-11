IF OBJECT_ID('FoldingCoin.GetDistroUsers') IS NOT NULL
	BEGIN
		DROP PROCEDURE [FoldingCoin].[GetDistroUsers];
	END
GO

CREATE PROCEDURE [FoldingCoin].[GetDistroUsers] @StartDate DATE,
	@EndDate DATE
AS
BEGIN
	WITH WorkingDataSet AS
	(
		SELECT 
			[UserStatId], 
			[Points], 
			[WorkUnits], 
			[TeamMembers].[TeamMemberId], 
			[DownloadDateTime], 
			[BitcoinAddress]
		FROM [FoldingCoin].[UserStats]
			INNER JOIN [FoldingCoin].[FAHDataRuns] ON [FoldingCoin].[UserStats].[FAHDataRunId] = [FoldingCoin].[FAHDataRuns].[FAHDataRunId]
			INNER JOIN [FoldingCoin].[Downloads] ON [FoldingCoin].[Downloads].[DownloadId] = [FoldingCoin].[FAHDataRuns].[DownloadId]
			INNER JOIN [FoldingCoin].[TeamMembers] ON [FoldingCoin].[FAHDataRuns].[TeamMemberId] = [FoldingCoin].[TeamMembers].[TeamMemberId]
			INNER JOIN [FoldingCoin].[Users] ON [FoldingCoin].[TeamMembers].[UserId] = [FoldingCoin].[Users].[UserId]
		WHERE [FoldingCoin].[Downloads].[StatusId] = [FoldingCoin].GetStatsUploadFinishedStatusId() 
			AND DownloadDateTime >= @StartDate AND DownloadDateTime < DATEADD(day, 1, @EndDate)
	)
	SELECT 
		[BitcoinAddress] 
	FROM
	(
		SELECT DISTINCT [TeamMemberId], [BitcoinAddress],
			(MAX([Points]) OVER (PARTITION BY [TeamMemberId]) - MIN([Points]) OVER (PARTITION BY [TeamMemberId])) AS NewPoints,
			(MAX([WorkUnits]) OVER (PARTITION BY [TeamMemberId]) - MIN([WorkUnits]) OVER (PARTITION BY [TeamMemberId])) AS NewWorkUnits
		FROM WorkingDataSet
	) AS DistroUsers
	WHERE NewPoints <> 0 
		AND NewWorkUnits <> 0 
		AND BitcoinAddress IS NOT NULL
END
GO