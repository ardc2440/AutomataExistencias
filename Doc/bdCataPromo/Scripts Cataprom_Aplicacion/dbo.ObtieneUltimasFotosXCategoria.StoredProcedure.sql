USE [Cataprom_aplicacion]
GO
/****** Object:  StoredProcedure [dbo].[ObtieneUltimasFotosXCategoria]    Script Date: 5/05/2018 12:17:49 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[ObtieneUltimasFotosXCategoria]
as
select I.IdItem,IC.idCategoria,c.Descripcion
from tbl_rItemsFoto I
join tbl_rItemsComplementos IC on I.IdItem= IC.IdItem
join tbl_rcategorias c on IC.idCategoria=c.idCategoria
join (select max(IdItem) idItem,idCategoria from tbl_rItemsComplementos group by idCategoria) as ultimo
on ultimo.idItem=I.idItem
GO
