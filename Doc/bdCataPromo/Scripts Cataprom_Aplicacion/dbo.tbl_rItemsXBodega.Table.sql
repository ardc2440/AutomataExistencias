USE [Cataprom_aplicacion]
GO
/****** Object:  Table [dbo].[tbl_rItemsXBodega]    Script Date: 5/05/2018 12:17:49 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tbl_rItemsXBodega](
	[IdItemXColor] [int] NOT NULL,
	[IdBodega] [int] NOT NULL,
	[ItbCantidad] [int] NOT NULL,
	[ItbActivo] [char](1) NULL,
PRIMARY KEY CLUSTERED 
(
	[IdItemXColor] ASC,
	[IdBodega] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[tbl_rItemsXBodega]  WITH CHECK ADD  CONSTRAINT [FK_tbl_rItemsXBodega_tbl_rBodegas] FOREIGN KEY([IdBodega])
REFERENCES [dbo].[tbl_rBodegas] ([IdBodega])
GO
ALTER TABLE [dbo].[tbl_rItemsXBodega] CHECK CONSTRAINT [FK_tbl_rItemsXBodega_tbl_rBodegas]
GO
ALTER TABLE [dbo].[tbl_rItemsXBodega]  WITH CHECK ADD  CONSTRAINT [FK_tbl_rItemsXBodega_tbl_rItemsXColor] FOREIGN KEY([IdItemXColor])
REFERENCES [dbo].[tbl_rItemsXColor] ([IdItemxcolor])
GO
ALTER TABLE [dbo].[tbl_rItemsXBodega] CHECK CONSTRAINT [FK_tbl_rItemsXBodega_tbl_rItemsXColor]
GO
