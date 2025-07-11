USE [Cataprom_aplicacion]
GO
/****** Object:  Table [dbo].[tbl_rItemAdjuntos]    Script Date: 5/05/2018 12:17:49 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tbl_rItemAdjuntos](
	[idItemAdjuntos] [int] IDENTITY(1,1) NOT NULL,
	[idItem] [int] NOT NULL,
	[ItaArchivo] [varbinary](max) NULL,
	[itaNombreArchivo] [varchar](100) NULL,
	[itaDescripcion] [varchar](100) NULL,
 CONSTRAINT [PK_tbl_rItemAdjuntos] PRIMARY KEY CLUSTERED 
(
	[idItemAdjuntos] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[tbl_rItemAdjuntos]  WITH CHECK ADD  CONSTRAINT [FK_tbl_rItemAdjuntos_tbl_rItems] FOREIGN KEY([idItem])
REFERENCES [dbo].[tbl_rItems] ([IdItem])
GO
ALTER TABLE [dbo].[tbl_rItemAdjuntos] CHECK CONSTRAINT [FK_tbl_rItemAdjuntos_tbl_rItems]
GO
