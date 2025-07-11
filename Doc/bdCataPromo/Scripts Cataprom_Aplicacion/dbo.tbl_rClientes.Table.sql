USE [Cataprom_aplicacion]
GO
/****** Object:  Table [dbo].[tbl_rClientes]    Script Date: 5/05/2018 12:17:49 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tbl_rClientes](
	[IdCliente] [int] NOT NULL,
	[CliNumidentifica] [varchar](15) NOT NULL,
	[CliNombre] [varchar](50) NOT NULL,
	[CliMail1] [varchar](250) NULL,
	[CliMail2] [varchar](250) NULL,
	[CliMail3] [varchar](250) NULL,
	[CliEnviarmail] [char](1) NOT NULL,
	[CliActivo] [char](1) NULL,
PRIMARY KEY CLUSTERED 
(
	[IdCliente] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
