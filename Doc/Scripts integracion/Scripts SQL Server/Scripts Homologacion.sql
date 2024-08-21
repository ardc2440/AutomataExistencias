CREATE TABLE [dbo].[Items_Win](
	[IdItem] [int] NOT NULL,
	[IdLinea] [int] NOT NULL,
	[Referencia_Interna] [varchar](10) NOT NULL,
	[NomItem] [varchar](50) NOT NULL,
	Activo BIT NOT NULL,
 CONSTRAINT PK_Items_Win PRIMARY KEY (IdItem)
)
GO

CREATE TABLE [dbo].[ItemsColor_Win](
	[IdItemxColor] [int] NOT NULL,
	[IdItem] [int] NOT NULL,
	[NomColor] [varchar](50) NOT NULL,
	Activo BIT NOT NULL,
 CONSTRAINT PK_ItemsColor_Win PRIMARY KEY (IdItemxColor),
 CONSTRAINT FK_ItemsColor_Win_Item FOREIGN KEY(IdItem) REFERENCES [dbo].[Items_Win] (IdItem) 
)
GO

CREATE TABLE [dbo].[Embalajes_Win](
	[IdItem] [int] NOT NULL,
	[IdEmbalaje] [Int] NOT NULL
 CONSTRAINT PK_Embalaje_Win PRIMARY KEY (IdEmbalaje),
 CONSTRAINT FK_Embalaje_Win_Item FOREIGN KEY(IdItem) REFERENCES [dbo].[Items_Win] (IdItem) 
)
GO

CREATE TABLE [dbo].[Monedas_Win](
	[IdMoneda] [int] NOT NULL,
	[NomMoneda] [VARCHAR](30) NOT NULL
 CONSTRAINT PK_Monedas_Win PRIMARY KEY (IdMoneda) 
)
GO

CREATE TABLE [dbo].[UnidadesMedida_Win](
	[IdUnidad] [int] NOT NULL,
	[NomUnidad] [VARCHAR](30) NOT NULL
 CONSTRAINT PK_UnidadesMedida_Win PRIMARY KEY (IdUnidad) 
)
GO

CREATE VIEW Items_Homologados_View
AS
SELECT a.ITEM_ID Item_Id, ISNULL(b.IdItem, a.ITEM_ID) Item_Id_Homologado, ISNULL(b.IdLinea, a.Line_Id) Line_Id_Homologada,CASE WHEN b.IdItem IS NULL THEN 0 ELSE 1 END Homologado   
  FROM items a
  LEFT JOIN Items_Win b on b.Referencia_Interna = a.INTERNAL_REFERENCE
GO

CREATE VIEW Item_References_Homologados_View
AS
SELECT a.ITEM_ID, b.ITEM_ID_HOMOLOGADO, REFERENCE_ID, ISNULL(c.IdItemxColor,REFERENCE_ID )REFERENCE_ID_HOMOLOGADO, REFERENCE_CODE, CASE WHEN c.IdItemxColor IS NULL THEN 0 ELSE 1 END Homologado   
  FROM Item_references a
  JOIN Items_Homologados_View b ON b.Item_Id = a.ITEM_ID
  LEFT JOIN ItemsColor_Win c ON c.IdItem = b.Item_Id_Homologado
                            AND c.NomColor = a.REFERENCE_NAME
GO

CREATE VIEW Packaging_Homologados_View
AS
SELECT a.Packaging_Id, ISNULL(c.IdEmbalaje, a.Packaging_Id) Packaging_Id_Homologado, b.ITEM_ID_HOMOLOGADO, CASE WHEN c.IdEmbalaje IS NULL THEN 0 ELSE 1 END Homologado 
  FROM packaging a
  JOIN Items_Homologados_View b ON b.Item_Id = a.ITEM_ID
  LEFT JOIN Embalajes_Win c ON c.IdItem = b.Item_Id_Homologado
GO

CREATE VIEW Currencies_Homologados_View
AS
SELECT a.Currency_Id, ISNULL(b.IdMoneda, a.Currency_Id) Currency_Id_Homologado, CASE WHEN b.IdMoneda IS NULL THEN 0 ELSE 1 END Homologado 
  FROM Currencies a
  LEFT JOIN Monedas_Win B ON b.NomMoneda = a.CURRENCY_NAME
GO

CREATE VIEW Measure_Units_Homologados_View
AS
SELECT a.MEASURE_UNIT_ID, ISNULL(b.IdUnidad, a.MEASURE_UNIT_ID) Measure_Unit_Id_Homologado, CASE WHEN b.IdUnidad IS NULL THEN 0 ELSE 1 END Homologado 
  FROM measure_units a
  LEFT JOIN UnidadesMedida_Win B ON b.NomUnidad = a.MEASURE_UNIT_NAME
GO
