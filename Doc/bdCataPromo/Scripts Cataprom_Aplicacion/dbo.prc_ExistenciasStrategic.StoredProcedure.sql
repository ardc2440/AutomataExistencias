USE [Cataprom_aplicacion]
GO
/****** Object:  StoredProcedure [dbo].[prc_ExistenciasStrategic]    Script Date: 5/05/2018 12:17:49 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[prc_ExistenciasStrategic]
as
begin
	select a.iditem[consecutivo], b.ItmRefinterna [codigo]  ,color,sum(cantidad) Cantidad  
	from tbl_rexistencias a
	join tbl_rItems b on a.iditem=b.IdItem
	group by a.iditem, b.ItmRefinterna  ,color
end
GO
