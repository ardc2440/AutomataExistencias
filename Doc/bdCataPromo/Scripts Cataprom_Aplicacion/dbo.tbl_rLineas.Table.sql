USE [Cataprom_aplicacion]
GO
/****** Object:  Table [dbo].[tbl_rLineas]    Script Date: 5/05/2018 12:17:49 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tbl_rLineas](
	[idLinea] [int] NOT NULL,
	[LinCodigo] [varchar](10) NOT NULL,
	[LinNombre] [varchar](30) NOT NULL,
	[LinDemonio] [char](1) NULL,
	[LinActivo] [char](1) NULL,
PRIMARY KEY CLUSTERED 
(
	[idLinea] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
