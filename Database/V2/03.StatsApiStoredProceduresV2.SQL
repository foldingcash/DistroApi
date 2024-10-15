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
	SELECT 
		DownloadId, 
		DownloadDateTime, 
		FilePath 
	FROM [FoldingCash].[ValidatedFiles] 
	WHERE 
		DATEADD(dd, 0, DATEDIFF(dd, 0, DownloadDateTime)) >= @StartDate
	AND 
		DATEADD(dd, 0, DATEDIFF(dd, 0, DownloadDateTime)) <= @EndDate
END
GO

-----------------------------------------------------------------