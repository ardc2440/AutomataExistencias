USE [Cataprom_aplicacion]
GO
/****** Object:  Table [dbo].[tbl_rItemsComplementos]    Script Date: 5/05/2018 12:17:49 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tbl_rItemsComplementos](
	[IdItem] [int] NOT NULL,
	[idCategoria] [int] NULL,
	[Caracteristicas] [varchar](1000) NULL,
 CONSTRAINT [PK__tbl_rItemsComple__7E02B4CC] PRIMARY KEY CLUSTERED 
(
	[IdItem] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[tbl_rItemsComplementos]  WITH CHECK ADD  CONSTRAINT [FK_tbl_rItemsComplementos_tbl_rcategorias] FOREIGN KEY([idCategoria])
REFERENCES [dbo].[tbl_rcategorias] ([idCategoria])
GO
ALTER TABLE [dbo].[tbl_rItemsComplementos] CHECK CONSTRAINT [FK_tbl_rItemsComplementos_tbl_rcategorias]
GO
ALTER TABLE [dbo].[tbl_rItemsComplementos]  WITH CHECK ADD  CONSTRAINT [FK_tbl_rItemsComplementos_tbl_rItems] FOREIGN KEY([IdItem])
REFERENCES [dbo].[tbl_rItems] ([IdItem])
GO
ALTER TABLE [dbo].[tbl_rItemsComplementos] CHECK CONSTRAINT [FK_tbl_rItemsComplementos_tbl_rItems]
GO
