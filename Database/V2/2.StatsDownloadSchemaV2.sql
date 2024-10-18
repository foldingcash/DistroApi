USE [FoldingCash]
GO
/****** Object:  Schema [FoldingCash]    Script Date: 6/29/2019 4:00:40 PM ******/
CREATE SCHEMA [FoldingCash]
GO
/****** Object:  Table [FoldingCash].[Downloads]    Script Date: 6/29/2019 4:00:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [FoldingCash].[Downloads](
	[DownloadId] [int] IDENTITY(1,1) NOT NULL,
	[StatusId] [int] NOT NULL,
	[FileId] [int] NULL,
	[DownloadDateTime] [datetime] NOT NULL,
 CONSTRAINT [PK_Downloads] PRIMARY KEY CLUSTERED 
(
	[DownloadId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [FoldingCash].[Files]    Script Date: 6/29/2019 4:00:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [FoldingCash].[Files](
	[FileId] [int] IDENTITY(1,1) NOT NULL,
	[FilePath] [nvarchar](250) NOT NULL,
	[FileName] [nvarchar](50) NOT NULL,
	[FileExtension] [nvarchar](5) NOT NULL,
 CONSTRAINT [PK_Files] PRIMARY KEY CLUSTERED 
(
	[FileId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [FoldingCash].[Rejections]    Script Date: 6/29/2019 4:00:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [FoldingCash].[Rejections](
	[RejectionId] [int] IDENTITY(1,1) NOT NULL,
	[DownloadId] [int] NOT NULL,
	[Reason] [nvarchar](500) NOT NULL,
 CONSTRAINT [PK_Rejections] PRIMARY KEY CLUSTERED 
(
	[RejectionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [FoldingCash].[Statuses]    Script Date: 6/29/2019 4:00:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [FoldingCash].[Statuses](
	[StatusId] [int] IDENTITY(1,1) NOT NULL,
	[Status] [nvarchar](50) NOT NULL,
	[StatusDescription] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_Statuses] PRIMARY KEY CLUSTERED 
(
	[StatusId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [FoldingCash].[Downloads]  WITH CHECK ADD  CONSTRAINT [FK_Downloads_Files] FOREIGN KEY([FileId])
REFERENCES [FoldingCash].[Files] ([FileId])
GO
ALTER TABLE [FoldingCash].[Downloads] CHECK CONSTRAINT [FK_Downloads_Files]
GO
ALTER TABLE [FoldingCash].[Downloads]  WITH CHECK ADD  CONSTRAINT [FK_Downloads_Statuses] FOREIGN KEY([StatusId])
REFERENCES [FoldingCash].[Statuses] ([StatusId])
GO
ALTER TABLE [FoldingCash].[Downloads] CHECK CONSTRAINT [FK_Downloads_Statuses]
GO
ALTER TABLE [FoldingCash].[Rejections]  WITH CHECK ADD  CONSTRAINT [FK_Rejections_Downloads] FOREIGN KEY([DownloadId])
REFERENCES [FoldingCash].[Downloads] ([DownloadId])
GO
ALTER TABLE [FoldingCash].[Rejections] CHECK CONSTRAINT [FK_Rejections_Downloads]
GO
USE [master]
GO
ALTER DATABASE [FoldingCash] SET  READ_WRITE 
GO
