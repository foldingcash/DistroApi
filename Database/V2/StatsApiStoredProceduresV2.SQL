IF OBJECT_ID('FoldingCash.ValidatedFiles') IS NOT NULL
	BEGIN
		DROP VIEW [FoldingCash].[ValidatedFiles];
	END
GO

CREATE VIEW [FoldingCash].[ValidatedFiles]
AS
	SELECT D.DownloadId, D.DownloadDateTime, F.FilePath FROM [FoldingCash].[Downloads] D
	INNER JOIN [FoldingCash].[Files] F ON D.FileId = F.FileId
	WHERE StatusId = FoldingCash.GetFileValidatedStatusId();
GO

-----------------------------------------------------------------

IF OBJECT_ID('FoldingCash.GetValidatedFiles') IS NOT NULL
	BEGIN
		DROP PROCEDURE [FoldingCash].[GetValidatedFiles];
	END
GO

CREATE PROCEDURE [FoldingCash].[GetValidatedFiles] @StartDate DATE,
	@EndDate DATE
AS
BEGIN
	DECLARE @StartDateTime DATETIME;
	DECLARE @EndDateTime DATETIME;

	--This is converted to alter the hours because of FoldingCash specific implementation needs
	SET @StartDateTime = DATEADD(HOUR, 11, CONVERT(DATETIME, @StartDate));
	SET @EndDateTime = DATEADD(DAY, 1, DATEADD(HOUR, 12, CONVERT(DATETIME, @EndDate)));

	SELECT 
		DownloadId, 
		DownloadDateTime, 
		FilePath 
	FROM [FoldingCash].[ValidatedFiles] 
	WHERE 
		DownloadDateTime >= @StartDateTime 
	AND 
		DownloadDateTime < @EndDateTime;
END
GO

-----------------------------------------------------------------