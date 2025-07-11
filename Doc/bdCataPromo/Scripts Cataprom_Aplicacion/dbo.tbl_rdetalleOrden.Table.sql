USE [Cataprom_aplicacion]
GO
/****** Object:  Table [dbo].[tbl_rdetalleOrden]    Script Date: 5/05/2018 12:17:49 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tbl_rdetalleOrden](
	[IdDetalle] [int] IDENTITY(1,1) NOT NULL,
	[IdOrdenCompra] [int] NULL,
	[detReferencia] [varchar](20) NULL,
	[detDescripcion] [varchar](100) NULL,
	[detCantidad] [int] NULL,
	[detPrecioUnitario] [numeric](12, 2) NULL,
	[detPrecioTotal] [numeric](12, 2) NULL,
	[detColor] [varchar](30) NULL,
PRIMARY KEY CLUSTERED 
(
	[IdDetalle] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[tbl_rdetalleOrden]  WITH CHECK ADD  CONSTRAINT [FK_tbl_rdetalle_Orden_tbl_rOrdenCompra] FOREIGN KEY([IdOrdenCompra])
REFERENCES [dbo].[tbl_rOrdenCompra] ([IdOrdenCompra])
GO
ALTER TABLE [dbo].[tbl_rdetalleOrden] CHECK CONSTRAINT [FK_tbl_rdetalle_Orden_tbl_rOrdenCompra]
GO
