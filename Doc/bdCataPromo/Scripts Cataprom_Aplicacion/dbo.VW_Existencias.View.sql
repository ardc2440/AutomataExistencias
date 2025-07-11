USE [Cataprom_aplicacion]
GO
/****** Object:  View [dbo].[VW_Existencias]    Script Date: 5/05/2018 12:17:49 a. m. ******/
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
