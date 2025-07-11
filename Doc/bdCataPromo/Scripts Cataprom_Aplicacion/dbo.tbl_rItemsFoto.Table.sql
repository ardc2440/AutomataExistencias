USE [Cataprom_aplicacion]
GO
/****** Object:  Table [dbo].[tbl_rItemsFoto]    Script Date: 5/05/2018 12:17:49 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tbl_rItemsFoto](
	[IdItem] [int] NOT NULL,
	[ItmFoto] [image] NULL,
	[itmNombreArchivo] [varchar](100) NULL,
	[itmCaracteristicas] [varchar](200) NULL,
	[itmPlanoTecnico] [image] NULL,
	[itmNombrePlano] [varchar](30) NULL,
 CONSTRAINT [PK_tbl_rItemsFoto] PRIMARY KEY CLUSTERED 
(
	[IdItem] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[tbl_rItemsFoto]  WITH CHECK ADD  CONSTRAINT [FK_tbl_rItemsFoto_tbl_rItems1] FOREIGN KEY([IdItem])
REFERENCES [dbo].[tbl_rItems] ([IdItem])
GO
ALTER TABLE [dbo].[tbl_rItemsFoto] CHECK CONSTRAINT [FK_tbl_rItemsFoto_tbl_rItems1]
GO
