USE [Cataprom_aplicacion]
GO
/****** Object:  Table [dbo].[tbl_rItemsXColor]    Script Date: 5/05/2018 12:17:49 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tbl_rItemsXColor](
	[IdItemxcolor] [int] NOT NULL,
	[IdItem] [int] NOT NULL,
	[ItcRefitemxcolor] [varchar](30) NOT NULL,
	[ItcRefintitemxcolor] [varchar](10) NULL,
	[ItcNombre] [varchar](30) NOT NULL,
	[ItcNomitemxcolorprov] [varchar](30) NULL,
	[ItcObservaciones] [varchar](250) NULL,
	[ItcColor] [varchar](30) NULL,
	[ItcCantpedida] [int] NOT NULL,
	[ItcCantidad] [int] NOT NULL,
	[ItcCantreservada] [int] NOT NULL,
	[ItcCantpedidapan] [int] NOT NULL,
	[ItcCantreservadapan] [int] NOT NULL,
	[ItcCantidadpan] [int] NOT NULL,
	[ItcActivo] [char](1) NULL,
	[ItcAgotado] [char](1) NULL,
	[ItcCantproceso] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[IdItemxcolor] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[tbl_rItemsXColor]  WITH CHECK ADD  CONSTRAINT [FK_tbl_rItemsXColor_tbl_rItems] FOREIGN KEY([IdItem])
REFERENCES [dbo].[tbl_rItems] ([IdItem])
GO
ALTER TABLE [dbo].[tbl_rItemsXColor] CHECK CONSTRAINT [FK_tbl_rItemsXColor_tbl_rItems]
GO
