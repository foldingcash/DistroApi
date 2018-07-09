IF OBJECT_ID('FoldingCoin.GetDistroUsers') IS NOT NULL
	BEGIN
		DROP PROCEDURE [FoldingCoin].[GetDistroUsers];
	END
GO

CREATE PROCEDURE [FoldingCoin].[GetDistroUsers]
AS

BEGIN
	SELECT [BitcoinAddress]
	FROM [FoldingCoin].[Users]
	WHERE [BitcoinAddress] IS NOT NULL
END
GO