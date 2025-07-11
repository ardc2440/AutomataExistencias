USE [Cataprom_aplicacion]
GO
/****** Object:  StoredProcedure [dbo].[prc_ConsultarItem]    Script Date: 5/05/2018 12:17:49 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[prc_ConsultarItem]      
(      
 @idItem int      
)      
as      
begin      
     
SELECT     I.IdItem, I.idLinea, I.ItmRefinterna, I.ItmNombre, I.ItmRefproveedor, I.ItmNomitemprov, I.ItmTipoitem, I.ItmCostofob, I.IdMoneda, I.ItmTipparte, I.ItmDeterminante,   
                      I.ItmObservaciones, I.ItmInventarioext, I.ItmCostocif, I.ItmVolumen, I.ItmPeso, I.IdUnidadfob, I.IdUnidadcif, I.ItmProdnac, I.ItmActivo, I.ItmCatalogoVisible,   
                      IC.Caracteristicas, 
                      peso = isnull(tbl_rEmbalajes.peso, 0), 
                      altura = isnull(tbl_rEmbalajes.altura, 0), 
                      ancho = isnull(tbl_rEmbalajes.ancho, 0), 
                      largo = isnull(tbl_rEmbalajes.largo, 0), 
                      cantidadCaja = isnull(tbl_rEmbalajes.cantidadCaja, 0)  
FROM         tbl_rItems AS I left JOIN  
                      tbl_rEmbalajes ON I.IdItem = tbl_rEmbalajes.IdItem LEFT OUTER JOIN  
                      tbl_rItemsComplementos AS IC ON I.IdItem = IC.IdItem  
WHERE     (I.IdItem = @idItem)  
end 
GO
