USE [Cataprom_aplicacion]
GO
/****** Object:  View [dbo].[VW_DATOS_ITEMS]    Script Date: 5/05/2018 12:17:49 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [dbo].[VW_DATOS_ITEMS]
AS
select ItmRefinterna,ItmNombre,ItmRefproveedor,ItmTipoitem,i.IdItem,
ItmFoto,itmNombreArchivo, isnull(cat.idCategoria,0) [idLinea], Descripcion [LinNombre],Caracteristicas,
I.ItmCatalogoVisible, i.ItmActivo
from dbo.tbl_rItems I
LEFT join tbl_rItemsFoto IFt on i.IdItem=Ift.IdItem
left join tbl_rItemsComplementos c on c.IdItem=I.IdItem
left join tbl_rcategorias cat on c.idCategoria=cat.idCategoria
GO
