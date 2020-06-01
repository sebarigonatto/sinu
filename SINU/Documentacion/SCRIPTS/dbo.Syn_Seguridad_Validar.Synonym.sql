USE [SINU]
GO
/****** Object:  Synonym [dbo].[Syn_Seguridad_Validar]    Script Date: 01/06/2020 18:22:45 ******/
CREATE SYNONYM [dbo].[Syn_Seguridad_Validar] FOR [Seguridad].[dbo].[spValidarPermisos]
GO
EXEC sys.sp_addextendedproperty @name=N'Descripcion', @value=N'Synonim que accede al sp de la base Seguridad para ver si tiene o no permiso el usuario en la aplicacion' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'SYNONYM',@level1name=N'Syn_Seguridad_Validar'
GO
