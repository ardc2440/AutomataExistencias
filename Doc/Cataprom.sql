USE [Cataprom_aplicacion]
GO
/****** Object:  Table [dbo].[tbl_sTransito]    Script Date: 05/11/2018 19:52:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tbl_sTransito](
	[IDITEMTRANSITO] [int] NOT NULL,
	[FECHAESTRECIBO] [datetime] NOT NULL,
	[CANTIDADREC] [int] NOT NULL,
	[FECHA] [datetime] NOT NULL,
	[ACTIVIDAD] [varchar](200) NOT NULL,
	[IDITEMXCOLOR] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[IDITEMTRANSITO] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tbl_rUnidadesMedida]    Script Date: 05/11/2018 19:52:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tbl_rUnidadesMedida](
	[IdUnidadMedida] [int] NOT NULL,
	[UniNombre] [varchar](30) NOT NULL,
	[UniActivo] [char](1) NULL,
PRIMARY KEY CLUSTERED 
(
	[IdUnidadMedida] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tbl_rMonedas]    Script Date: 05/11/2018 19:52:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tbl_rMonedas](
	[IdMoneda] [int] NOT NULL,
	[MonNombre] [varchar](30) NOT NULL,
	[MonActivo] [char](1) NULL,
PRIMARY KEY CLUSTERED 
(
	[IdMoneda] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
INSERT [dbo].[tbl_rMonedas] ([IdMoneda], [MonNombre], [MonActivo]) VALUES (13, N'Euro', N'A')
/****** Object:  Table [dbo].[tbl_rLineas]    Script Date: 05/11/2018 19:52:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tbl_rLineas](
	[idLinea] [int] NOT NULL,
	[LinCodigo] [varchar](10) NOT NULL,
	[LinNombre] [varchar](30) NOT NULL,
	[LinDemonio] [char](1) NULL,
	[LinActivo] [char](1) NULL,
PRIMARY KEY CLUSTERED 
(
	[idLinea] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tbl_rexistencias]    Script Date: 05/11/2018 19:52:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tbl_rexistencias](
	[IBASE_IDITEMEXISTENCIAS] [int] NOT NULL,
	[IDITEMXCOLOR] [int] NOT NULL,
	[IDITEM] [int] NOT NULL,
	[COLOR] [varchar](30) NOT NULL,
	[CANTIDAD] [int] NOT NULL,
	[BODEGA] [varchar](30) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[IBASE_IDITEMEXISTENCIAS] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[IBASE_ITEMEXISTENCIAS]    Script Date: 05/11/2018 19:52:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[IBASE_ITEMEXISTENCIAS](
	[IBASE_IDITEMEXISTENCIAS] [int] IDENTITY(1,1) NOT NULL,
	[IDITEMXCOLOR] [int] NOT NULL,
	[IDITEM] [int] NOT NULL,
	[COLOR] [varchar](30) NOT NULL,
	[CANTIDAD] [int] NOT NULL,
	[BODEGA] [varchar](30) NOT NULL,
 CONSTRAINT [PK_IBASE_ITEMEXISTENCIAS] PRIMARY KEY CLUSTERED 
(
	[IBASE_IDITEMEXISTENCIAS] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET IDENTITY_INSERT [dbo].[IBASE_ITEMEXISTENCIAS] ON
INSERT [dbo].[IBASE_ITEMEXISTENCIAS] ([IBASE_IDITEMEXISTENCIAS], [IDITEMXCOLOR], [IDITEM], [COLOR], [CANTIDAD], [BODEGA]) VALUES (6, 317, 101, N'Rojo Frost', 0, N'Bodega Local')
SET IDENTITY_INSERT [dbo].[IBASE_ITEMEXISTENCIAS] OFF
/****** Object:  Table [dbo].[tbl_rItems]    Script Date: 05/11/2018 19:52:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tbl_rItems](
	[IdItem] [int] NOT NULL,
	[idLinea] [int] NOT NULL,
	[ItmRefinterna] [varchar](10) NOT NULL,
	[ItmNombre] [varchar](50) NOT NULL,
	[ItmRefproveedor] [varchar](25) NOT NULL,
	[ItmNomitemprov] [varchar](50) NOT NULL,
	[ItmTipoitem] [char](1) NOT NULL,
	[ItmCostofob] [decimal](18, 10) NOT NULL,
	[IdMoneda] [int] NOT NULL,
	[ItmTipparte] [char](1) NULL,
	[ItmDeterminante] [char](1) NULL,
	[ItmObservaciones] [varchar](250) NULL,
	[ItmInventarioext] [char](1) NOT NULL,
	[ItmCostocif] [varchar](200) NULL,
	[ItmVolumen] [decimal](18, 10) NULL,
	[ItmPeso] [decimal](18, 10) NULL,
	[IdUnidadfob] [int] NULL,
	[IdUnidadcif] [int] NULL,
	[ItmProdnac] [char](1) NULL,
	[ItmActivo] [char](1) NULL,
	[ItmCatalogoVisible] [char](1) NULL,
PRIMARY KEY CLUSTERED 
(
	[IdItem] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tbl_rItemsXColor]    Script Date: 05/11/2018 19:52:53 ******/
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
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  View [dbo].[VW_Existencias]    Script Date: 05/11/2018 19:52:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[VW_Existencias]
AS
SELECT DISTINCT 
        ie.IDITEM,  
        ie.COLOR [COLOR],  
        (ISNULL(bl.Itccantidad, 0) -ISNULL(zf.CANTIDAD, 0)) -ISNULL(bl.ItcCantpedida, 0)  
        -ISNULL(bl.ItcCantreservada, 0) [BODEGALOCAL],  
        ISNULL(zf.CANTIDAD, 0) [ZONAFRANCA],  
        ISNULL((bl.Itccantidad -zf.CANTIDAD) + ISNULL(zf.CANTIDAD, 0) -ISNULL(bl.ItcCantpedida, 0)-ISNULL(bl.ItcCantreservada, 0), 0) [TOTAL],  
        it.CANTIDADREC [CANTIDAD],  
        CONVERT(VARCHAR(12),it.FECHAESTRECIBO, 112) [FECHARECIBIDO],  /*Se cambio Format por Conver por version de sql*/
        CONVERT(VARCHAR(12),it.FECHA, 112) [FECHAACTUALIZACION],  /*Se cambio Format por Conver por version de sql*/
        isnull(it.actividad, '') [STATUS]  
 FROM   tbl_rexistencias ie  
        JOIN tbl_rItemsXColor bl  
             ON  ie.IDITEMXCOLOR = bl.IdItemxcolor  
        LEFT JOIN (  
                 SELECT IDITEMXCOLOR,  
                        CANTIDAD,  
                        BODEGA  
                 FROM   tbl_rexistencias  
                 WHERE  BODEGA = 'Zona Franca'  
             )                   AS zf  
             ON  ie.IDITEMXCOLOR = zf.IDITEMXCOLOR  
        LEFT JOIN tbl_sTransito  AS it  
             ON  ie.IDITEMXCOLOR = it.IDITEMXCOLOR
GO
/****** Object:  ForeignKey [FK_tbl_rItems_tbl_rLineas]    Script Date: 05/11/2018 19:52:53 ******/
ALTER TABLE [dbo].[tbl_rItems]  WITH CHECK ADD  CONSTRAINT [FK_tbl_rItems_tbl_rLineas] FOREIGN KEY([idLinea])
REFERENCES [dbo].[tbl_rLineas] ([idLinea])
GO
ALTER TABLE [dbo].[tbl_rItems] CHECK CONSTRAINT [FK_tbl_rItems_tbl_rLineas]
GO
/****** Object:  ForeignKey [FK_tbl_rItems_tbl_rMonedas]    Script Date: 05/11/2018 19:52:53 ******/
ALTER TABLE [dbo].[tbl_rItems]  WITH CHECK ADD  CONSTRAINT [FK_tbl_rItems_tbl_rMonedas] FOREIGN KEY([IdMoneda])
REFERENCES [dbo].[tbl_rMonedas] ([IdMoneda])
GO
ALTER TABLE [dbo].[tbl_rItems] CHECK CONSTRAINT [FK_tbl_rItems_tbl_rMonedas]
GO
/****** Object:  ForeignKey [FK_tbl_rItems_tbl_rUnidadesMedida]    Script Date: 05/11/2018 19:52:53 ******/
ALTER TABLE [dbo].[tbl_rItems]  WITH CHECK ADD  CONSTRAINT [FK_tbl_rItems_tbl_rUnidadesMedida] FOREIGN KEY([IdUnidadcif])
REFERENCES [dbo].[tbl_rUnidadesMedida] ([IdUnidadMedida])
GO
ALTER TABLE [dbo].[tbl_rItems] CHECK CONSTRAINT [FK_tbl_rItems_tbl_rUnidadesMedida]
GO
/****** Object:  ForeignKey [FK_tbl_rItems_tbl_rUnidadesMedida1]    Script Date: 05/11/2018 19:52:53 ******/
ALTER TABLE [dbo].[tbl_rItems]  WITH CHECK ADD  CONSTRAINT [FK_tbl_rItems_tbl_rUnidadesMedida1] FOREIGN KEY([IdUnidadfob])
REFERENCES [dbo].[tbl_rUnidadesMedida] ([IdUnidadMedida])
GO
ALTER TABLE [dbo].[tbl_rItems] CHECK CONSTRAINT [FK_tbl_rItems_tbl_rUnidadesMedida1]
GO
/****** Object:  ForeignKey [FK_tbl_rItemsXColor_tbl_rItems]    Script Date: 05/11/2018 19:52:53 ******/
ALTER TABLE [dbo].[tbl_rItemsXColor]  WITH CHECK ADD  CONSTRAINT [FK_tbl_rItemsXColor_tbl_rItems] FOREIGN KEY([IdItem])
REFERENCES [dbo].[tbl_rItems] ([IdItem])
GO
ALTER TABLE [dbo].[tbl_rItemsXColor] CHECK CONSTRAINT [FK_tbl_rItemsXColor_tbl_rItems]
GO
