USE [Cataprom_aplicacion]
GO
/****** Object:  Table [dbo].[tbl_tnoticias]    Script Date: 5/05/2018 12:17:49 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tbl_tnoticias](
	[idNoticia] [int] IDENTITY(1,1) NOT NULL,
	[notEncabezado] [varchar](100) NULL,
	[notCuerpo] [ntext] NULL,
	[notFechaPublicacion] [datetime] NULL CONSTRAINT [DF_tbl_tnoticias_notFechaPublicacion]  DEFAULT (getdate()),
	[notActivo] [bit] NULL CONSTRAINT [DF_tbl_tnoticias_notActivo]  DEFAULT ((1)),
	[notPrioridad] [int] NULL,
	[notAdjunto] [varchar](100) NULL,
 CONSTRAINT [PK_tbl_tnoticias] PRIMARY KEY CLUSTERED 
(
	[idNoticia] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
