USE [Cataprom_aplicacion]
GO
/****** Object:  Table [dbo].[tbl_rOrdenCompra]    Script Date: 5/05/2018 12:17:49 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tbl_rOrdenCompra](
	[IdOrdenCompra] [int] IDENTITY(1,1) NOT NULL,
	[OrdNumero] [int] NULL,
	[OrdSolicitante] [varchar](100) NULL,
	[OrdFechaSolicitud] [varchar](10) NULL,
	[OrdFechaEntrega] [varchar](10) NULL,
	[OrdNit] [varchar](15) NULL,
	[OrdDireccion] [varchar](100) NULL,
	[OrdCiudad] [varchar](100) NULL,
	[OrdTelefono] [varchar](30) NULL,
	[OrdCelular] [varchar](11) NULL,
	[OrdFax] [varchar](20) NULL,
	[OrdContacto] [varchar](50) NULL,
	[OrdEmail] [varchar](100) NULL,
	[OrdFormaPago] [varchar](20) NULL,
	[OrdObservaciones] [varchar](500) NULL,
PRIMARY KEY CLUSTERED 
(
	[IdOrdenCompra] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
