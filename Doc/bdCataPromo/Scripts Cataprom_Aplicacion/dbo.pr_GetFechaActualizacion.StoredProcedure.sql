USE [Cataprom_aplicacion]
GO
/****** Object:  StoredProcedure [dbo].[pr_GetFechaActualizacion]    Script Date: 5/05/2018 12:17:49 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[pr_GetFechaActualizacion]
AS
BEGIN
	SELECT * FROM tbl_rActualizacion ra	
END




GO
