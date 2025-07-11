USE [Cataprom_aplicacion]
GO
/****** Object:  StoredProcedure [dbo].[prc_consultaExistencias]    Script Date: 5/05/2018 12:17:49 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[prc_consultaExistencias]
(  
 @idItem INT
)   
AS  
BEGIN  
 SELECT DISTINCT ie.IDITEMXCOLOR,  
        ie.IDITEM,  
        ie.COLOR [COLOR],  
        (ISNULL(bl.Itccantidad, 0) -ISNULL(zf.CANTIDAD, 0)) -ISNULL(bl.ItcCantpedida, 0)  
        -ISNULL(bl.ItcCantreservada, 0) [BODEGALOCAL],  
        ISNULL(zf.CANTIDAD, 0) [ZONAFRANCA],  
        ISNULL((bl.Itccantidad -zf.CANTIDAD) + ISNULL(zf.CANTIDAD, 0) -ISNULL(bl.ItcCantpedida, 0)-ISNULL(bl.ItcCantreservada, 0), 0) [TOTAL],  
        it.CANTIDADREC [CANTIDAD],  
        it.FECHAESTRECIBO [FECHARECIBIDO],  
        it.FECHA [FECHAACTUALIZACION],  
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
 WHERE  ie.IDITEM = @idItem  
 ORDER BY  
        ie.COLOR, ie.IDITEMXCOLOR, it.FECHAESTRECIBO
END 


GO
