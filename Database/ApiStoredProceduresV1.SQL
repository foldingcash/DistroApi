IF OBJECT_ID('FoldingCoin.GetDistro') IS NOT NULL
	BEGIN
		DROP PROCEDURE [FoldingCoin].[GetDistro];
	END
GO

CREATE PROCEDURE [FoldingCoin].[GetDistro]
AS

BEGIN
	SELECT [BitcoinAddress]
	FROM [FoldingCoin].[Users]
	WHERE [BitcoinAddress] IS NOT NULL
END
GO