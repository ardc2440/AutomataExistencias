USE [Cataprom_aplicacion]
GO
/****** Object:  StoredProcedure [dbo].[prc_ItemsStrategic]    Script Date: 5/05/2018 12:17:49 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[prc_ItemsStrategic]
as
begin
	declare @tbcolores table (iditem int, colores varchar(900))
	declare @iditem1 int
	declare @iditem int
	declare @colores varchar(900)
	Declare Cur cursor for
		select IdItem,ItcNombre
		from tbl_rItemsXColor
		OPEN Cur
		FETCH Cur INTO @iditem,@colores
		WHILE @@FETCH_STATUS = 0
		BEGIN
			
			if @iditem1=@iditem
			begin
				update @tbcolores set colores=colores+','+@colores
				where iditem=@iditem
			end
			else
			begin
			  insert into @tbcolores(iditem,colores)
			  values (@iditem,@colores)
			  set @iditem1=@iditem
			end
		
			FETCH Cur INTO @iditem,@colores
		end
		Close Cur
		deallocate Cur
	select ItmRefinterna [codigo],ItmNombre[nombre],Caracteristicas,colores ,a.iditem[consecutivo] 
	from tbl_rItems a
	join tbl_rItemsComplementos b on a.IdItem=b.iditem
	join @tbcolores c on a.IdItem=c.iditem
end
GO
