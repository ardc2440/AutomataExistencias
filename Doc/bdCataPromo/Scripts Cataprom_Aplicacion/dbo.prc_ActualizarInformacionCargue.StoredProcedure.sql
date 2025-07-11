USE [Cataprom_aplicacion]
GO
/****** Object:  StoredProcedure [dbo].[prc_ActualizarInformacionCargue]    Script Date: 5/05/2018 12:17:49 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[prc_ActualizarInformacionCargue]
AS
-- LINEAS
INSERT INTO tbl_rLineas (idLinea,LinCodigo,LinNombre,LinDemonio,LinActivo)
SELECT IDLINEA,CODLINEA,NOMLINEA,DEMONIO,ACTIVO
FROM IBASE_LINEAS
WHERE IDLINEA NOT IN (SELECT idLinea FROM tbl_rLineas)
UPDATE a
SET LinCodigo = b.CODLINEA, LinNombre = b.NOMLINEA, LinDemonio = b.DEMONIO, LinActivo = b.ACTIVO
FROM tbl_rLineas a
INNER JOIN IBASE_LINEAS b ON a.idLinea = b.IDLINEA
UPDATE a
SET LinActivo = 'N'
FROM tbl_rLineas a
WHERE a.idLinea NOT IN (SELECT IDLINEA FROM IBASE_LINEAS)
-- MONEDAS
INSERT INTO tbl_rMonedas (IdMoneda,MonNombre,MonActivo)
SELECT IDMONEDA,NOMMONEDA,'S'
FROM IBASE_MONEDAS
WHERE IDMONEDA NOT IN (SELECT idMoneda FROM tbl_rMonedas)
UPDATE a
SET MonNombre = b.NOMMONEDA
FROM tbl_rMonedas a
INNER JOIN IBASE_MONEDAS b ON a.IdMoneda = b.IDMONEDA
UPDATE a
SET MonActivo = 'N'
FROM tbl_rMonedas a
WHERE a.idMoneda NOT IN (SELECT IDMONEDA FROM IBASE_MONEDAS)
-- UNIDADES MEDIDA
INSERT INTO tbl_rUnidadesMedida (IdUnidadMedida,UniNombre,UniActivo)
SELECT IDUNIDAD,NOMBRE,'S'
FROM IBASE_UNIDADESMEDIDA
WHERE IDUNIDAD NOT IN (SELECT idUnidadMedida FROM tbl_rUnidadesMedida)
UPDATE a
SET UniNombre = b.NOMBRE
FROM tbl_rUnidadesMedida a
INNER JOIN IBASE_UNIDADESMEDIDA b ON a.IdUnidadMedida = b.IDUNIDAD
UPDATE a
SET UniActivo = 'N'
FROM tbl_rUnidadesMedida a
WHERE a.idUnidadMedida NOT IN (SELECT IDUNIDAD FROM IBASE_UNIDADESMEDIDA)
-- ITEMS
INSERT INTO tbl_rItems (IdItem,idLinea,ItmRefinterna,ItmNombre,ItmRefproveedor,ItmNomitemprov,ItmTipoitem,ItmCostofob,IdMoneda,ItmTipparte,ItmDeterminante,ItmObservaciones,ItmInventarioext,ItmCostocif,ItmVolumen,ItmPeso,IdUnidadfob,IdUnidadcif,ItmProdnac,
ItmActivo,ItmCatalogoVisible)
SELECT IDITEM,IDLINEA,REFINTERNA,NOMITEM,REFPROVEEDOR,NOMITEMPROV,TIPOITEM,COSTOFOB,IDMONEDA,TIPPARTE,DETERMINANTE,OBSERVACIONES,INVENTARIOEXT,COSTOCIF,VOLUMEN,PESO,IDUNIDADFOB,IDUNIDADCIF,PRODNAC,ACTIVO, CATALOGOVISIBLE
FROM IBASE_ITEMS
WHERE IDITEM NOT IN (SELECT IdItem FROM tbl_rItems)
UPDATE a
SET
idLinea = b.IDLINEA,
ItmRefinterna = b.REFINTERNA,
ItmNombre = b.NOMITEM,
ItmRefproveedor = b.REFPROVEEDOR,
ItmNomitemprov = b.NOMITEMPROV,
ItmTipoitem = b.TIPOITEM,
ItmCostofob = b.COSTOFOB,
IdMoneda = b.IDMONEDA,
ItmTipparte = b.TIPPARTE,
ItmDeterminante = b.DETERMINANTE,
ItmObservaciones = b.OBSERVACIONES,
ItmInventarioext = b.INVENTARIOEXT,
ItmCostocif = b.COSTOCIF,
ItmVolumen = b.VOLUMEN,
ItmPeso = b.PESO,
IdUnidadfob = b.IDUNIDADFOB,
IdUnidadcif = b.IDUNIDADCIF,
ItmProdnac = b.PRODNAC,
ItmActivo = b.ACTIVO,
ItmCatalogoVisible=B.CATALOGOVISIBLE
FROM tbl_rItems a
INNER JOIN IBASE_ITEMS b ON a.IdItem = b.IDITEM
UPDATE a
SET ItmActivo = 'N'
FROM tbl_rItems a
WHERE a.IdItem NOT IN (SELECT IDITEM FROM IBASE_ITEMS)
--TABLA DE COLORES
INSERT INTO tbl_rColores (ColNombre)
SELECT distinct NOMCOLOR FROM IBASE_ITEMSXCOLOR
WHERE NOT EXISTS (SELECT ColNombre FROM tbl_rColores WHERE ColNombre=IBASE_ITEMSXCOLOR.NOMCOLOR)
-- ITEMS X COLOR
INSERT INTO tbl_rItemsXColor (IdItemxcolor,IdItem,ItcRefitemxcolor,ItcRefintitemxcolor,
ItcNombre,ItcNomitemxcolorprov,ItcObservaciones,ItcColor,ItcCantpedida,ItcCantidad,
ItcCantreservada,ItcCantpedidapan,ItcCantreservadapan,ItcCantidadpan,ItcActivo,ItcAgotado,
ItcCantproceso)
SELECT IDITEMXCOLOR,IBASE_ITEMSXCOLOR.IDITEM,REFITEMXCOLOR,REFINTITEMXCOLOR,NOMCOLOR,NOMITEMXCOLORPROV,
IBASE_ITEMSXCOLOR.OBSERVACIONES,COLOR,CANTPEDIDA,CANTIDAD,CANTRESERVADA,CANTPEDIDAPAN,CANTRESERVADAPAN,
CANTIDADPAN,IBASE_ITEMSXCOLOR.ACTIVO,AGOTADO,CANTPROCESO
FROM IBASE_ITEMSXCOLOR
join IBASE_ITEMS on IBASE_ITEMSXCOLOR.IDITEM=IBASE_ITEMS.IDITEM
WHERE IDITEMXCOLOR NOT IN (SELECT IdItemxcolor FROM tbl_rItemsXColor)

UPDATE a
SET
IdItemxcolor = b.IDITEMXCOLOR,
IdItem = b.IDITEM,
ItcRefitemxcolor = b.REFITEMXCOLOR,
ItcRefintitemxcolor = b.REFINTITEMXCOLOR,
ItcNombre = b.NOMCOLOR,
ItcNomitemxcolorprov = b.NOMITEMXCOLORPROV,
ItcObservaciones = b.OBSERVACIONES,
ItcColor = b.COLOR,
ItcCantpedida = b.CANTPEDIDA,
ItcCantidad = b.CANTIDAD,
ItcCantreservada = b.CANTRESERVADA,
ItcCantpedidapan = b.CANTPEDIDAPAN,
ItcCantreservadapan = b.CANTRESERVADAPAN,
ItcCantidadpan = b.CANTIDADPAN,
ItcActivo = b.ACTIVO,
ItcAgotado = b.AGOTADO
FROM tbl_rItemsXColor a
INNER JOIN IBASE_ITEMSXCOLOR b ON a.IdItemxcolor = b.IDITEMXCOLOR
UPDATE a
SET ItcActivo = 'N'
FROM tbl_rItemsXColor a
WHERE a.IdItemxcolor NOT IN (SELECT IDITEMXCOLOR FROM IBASE_ITEMSXCOLOR)
-- BODEGAS
INSERT INTO tbl_rBodegas (IdBodega,BodNombre,BodActivo)
SELECT IDBODEGA,NOMBODEGA,'S'
FROM IBASE_BODEGAS
WHERE IDBODEGA NOT IN (SELECT IdBodega FROM tbl_rBodegas)
UPDATE a
SET BodNombre = b.NOMBODEGA
FROM tbl_rBodegas a
INNER JOIN IBASE_BODEGAS b ON a.IdBodega = b.IDBODEGA
UPDATE a
SET BodActivo = 'N'
FROM tbl_rBodegas a
WHERE a.IdBodega NOT IN (SELECT IDBODEGA FROM IBASE_BODEGAS)
-- ITEMS X BODEGAS
INSERT INTO tbl_rItemsXBodega (IdItemXColor,IdBodega,ItbCantidad,ItbActivo)
SELECT b.IDITEMXCOLOR,b.IDBODEGA,b.CANTIDAD,'S'
FROM IBASE_ITEMSXBODEGA b
WHERE NOT EXISTS (SELECT 1 FROM tbl_rItemsXBodega
WHERE IdItemXColor = b.IDITEMXCOLOR
AND IdBodega = b.IDBODEGA )
AND EXISTS (SELECT 1 FROM tbl_rItemsXColor
where tbl_rItemsXColor.IdItemxcolor=b.IDITEMXCOLOR)
UPDATE a
SET ItbCantidad = b.CANTIDAD
FROM tbl_rItemsXBodega a
INNER JOIN IBASE_ITEMSXBODEGA b ON a.IdItemXColor = b.IDITEMXCOLOR AND a.IdBodega = b.IDBODEGA
UPDATE a
SET ItbActivo = 'N'
FROM tbl_rItemsXBodega a
WHERE NOT EXISTS (SELECT 1 FROM IBASE_ITEMSXBODEGA
WHERE a.IdItemXColor = IDITEMXCOLOR
AND a.IdBodega = IDBODEGA )
-- CLIENTES
INSERT INTO tbl_rClientes (IdCliente,CliNumidentifica,CliNombre,CliMail1,CliMail2,CliMail3,CliEnviarmail,CliActivo)
SELECT IDCLIENTE,NUMIDENTIFICA,NOMCLIENTE,MAIL1,MAIL2,MAIL3,ENVIARMAIL,'S'
FROM IBASE_CLIENTES
WHERE IDCLIENTE NOT IN (SELECT IdCliente FROM tbl_rClientes)
UPDATE a
SET CliNumidentifica = b.NUMIDENTIFICA,
CliNombre = b.NOMCLIENTE,
CliMail1 = MAIL1,
CliMail2 = MAIL2,
CliMail3 = MAIL3,
CliEnviarmail = ENVIARMAIL
FROM tbl_rClientes a
INNER JOIN IBASE_CLIENTES b ON a.IdCliente = b.IDCLIENTE
UPDATE a
SET CliActivo = 'N'
FROM tbl_rClientes a
WHERE a.IdCliente NOT IN (SELECT IDCLIENTE FROM IBASE_CLIENTES)
--embalaje
INSERT INTO tbl_rEmbalajes(idEmbalaje, IdItem, peso, altura, ancho, largo, cantidadCaja)
SELECT A.IDEMBALAJE, A.IDITEM, A.PESO, A.ALTURA, A.ANCHO, A.LARGO, A.CANTIDAD
FROM IBASE_EMBALAJE A
join tbl_rItems on a.IdItem=tbl_rItems.IdItem
WHERE NOT exists (SELECT IdItem FROM tbl_rEmbalajes where a.IdItem=tbl_rEmbalajes.IdItem)
UPDATE A
SET
IdItem=B.IDITEM,
peso=B.PESO,
altura=B.ALTURA,
ancho=B.ANCHO,
largo=B.LARGO,
cantidadCaja=B.CANTIDAD
from tbl_rEmbalajes A
INNER JOIN IBASE_EMBALAJE B ON A.idEmbalaje=B.IDEMBALAJE 
GO
