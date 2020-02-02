IF OBJECT_ID('FoldingCoin.ValidatedFiles') IS NOT NULL
	BEGIN
		DROP VIEW [FoldingCoin].[ValidatedFiles];
	END
GO

CREATE VIEW [FoldingCoin].[ValidatedFiles]
AS
	SELECT D.DownloadId, D.DownloadDateTime, F.FilePath FROM [FoldingCoin].[Downloads] D
	INNER JOIN [FoldingCoin].[Files] F ON D.FileId = F.FileId
	WHERE StatusId = FoldingCoin.GetFileValidatedStatusId();
GO

-----------------------------------------------------------------

IF OBJECT_ID('FoldingCoin.GetValidatedFiles') IS NOT NULL
	BEGIN
		DROP PROCEDURE [FoldingCoin].[GetValidatedFiles];
	END
GO

CREATE PROCEDURE [FoldingCoin].[GetValidatedFiles] @StartDate DATE,
	@EndDate DATE
AS
BEGIN
	DECLARE @StartDateTime DATETIME;
	DECLARE @EndDateTime DATETIME;

	--This is converted to alter the hours because of FLDC specific implementation needs
	SET @StartDateTime = DATEADD(HOUR, 11, CONVERT(DATETIME, @StartDate));
	SET @EndDateTime = DATEADD(DAY, 1, DATEADD(HOUR, 12, CONVERT(DATETIME, @EndDate)));

	SELECT 
		DownloadId, 
		DownloadDateTime, 
		FilePath 
	FROM [FoldingCoin].[ValidatedFiles] 
	WHERE 
		DownloadDateTime >= @StartDateTime 
	AND 
		DownloadDateTime < @EndDateTime;
END
GO

-----------------------------------------------------------------