USE [Cataprom_aplicacion]
GO
/****** Object:  Table [dbo].[tbl_rEmbalajes]    Script Date: 5/05/2018 12:17:49 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tbl_rEmbalajes](
	[idEmbalaje] [int] NOT NULL,
	[IdItem] [int] NULL,
	[peso] [numeric](10, 1) NULL,
	[altura] [numeric](10, 1) NULL,
	[ancho] [numeric](10, 1) NULL,
	[largo] [numeric](10, 1) NULL,
	[cantidadCaja] [varchar](10) NULL,
PRIMARY KEY CLUSTERED 
(
	[idEmbalaje] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
