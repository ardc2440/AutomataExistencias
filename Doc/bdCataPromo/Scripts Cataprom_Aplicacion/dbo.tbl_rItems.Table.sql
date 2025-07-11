USE [Cataprom_aplicacion]
GO
/****** Object:  Table [dbo].[tbl_rItems]    Script Date: 5/05/2018 12:17:49 a. m. ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[tbl_rItems]  WITH CHECK ADD  CONSTRAINT [FK_tbl_rItems_tbl_rLineas] FOREIGN KEY([idLinea])
REFERENCES [dbo].[tbl_rLineas] ([idLinea])
GO
ALTER TABLE [dbo].[tbl_rItems] CHECK CONSTRAINT [FK_tbl_rItems_tbl_rLineas]
GO
ALTER TABLE [dbo].[tbl_rItems]  WITH CHECK ADD  CONSTRAINT [FK_tbl_rItems_tbl_rMonedas] FOREIGN KEY([IdMoneda])
REFERENCES [dbo].[tbl_rMonedas] ([IdMoneda])
GO
ALTER TABLE [dbo].[tbl_rItems] CHECK CONSTRAINT [FK_tbl_rItems_tbl_rMonedas]
GO
ALTER TABLE [dbo].[tbl_rItems]  WITH CHECK ADD  CONSTRAINT [FK_tbl_rItems_tbl_rUnidadesMedida] FOREIGN KEY([IdUnidadcif])
REFERENCES [dbo].[tbl_rUnidadesMedida] ([IdUnidadMedida])
GO
ALTER TABLE [dbo].[tbl_rItems] CHECK CONSTRAINT [FK_tbl_rItems_tbl_rUnidadesMedida]
GO
ALTER TABLE [dbo].[tbl_rItems]  WITH CHECK ADD  CONSTRAINT [FK_tbl_rItems_tbl_rUnidadesMedida1] FOREIGN KEY([IdUnidadfob])
REFERENCES [dbo].[tbl_rUnidadesMedida] ([IdUnidadMedida])
GO
ALTER TABLE [dbo].[tbl_rItems] CHECK CONSTRAINT [FK_tbl_rItems_tbl_rUnidadesMedida1]
GO
