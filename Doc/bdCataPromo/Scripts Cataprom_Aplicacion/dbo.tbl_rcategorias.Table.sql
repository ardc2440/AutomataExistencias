USE [Cataprom_aplicacion]
GO
/****** Object:  Table [dbo].[tbl_rcategorias]    Script Date: 5/05/2018 12:17:49 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tbl_rcategorias](
	[idCategoria] [int] IDENTITY(1,1) NOT NULL,
	[Descripcion] [varchar](40) NULL,
	[activo] [bit] NULL DEFAULT ((1)),
PRIMARY KEY CLUSTERED 
(
	[idCategoria] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
