USE [Cataprom_aplicacion]
GO
/****** Object:  Table [dbo].[tbl_rColores]    Script Date: 5/05/2018 12:17:49 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tbl_rColores](
	[idColor] [int] IDENTITY(1,1) NOT NULL,
	[ColNombre] [varchar](50) NOT NULL,
	[ColHexadecimal2] [varchar](50) NULL,
	[ColHexadecimal] [varchar](50) NULL,
	[ColCaracteristicas] [varchar](100) NULL,
 CONSTRAINT [PK_tbl_rColores] PRIMARY KEY CLUSTERED 
(
	[idColor] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_tbl_rColores_1] UNIQUE NONCLUSTERED 
(
	[ColNombre] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
