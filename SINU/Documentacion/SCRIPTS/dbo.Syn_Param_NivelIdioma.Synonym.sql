USE [SINU]
GO
/****** Object:  Synonym [dbo].[Syn_Param_NivelIdioma]    Script Date: 01/06/2020 18:22:45 ******/
CREATE SYNONYM [dbo].[Syn_Param_NivelIdioma] FOR [Parametricas].[dbo].[T010_CONOC_IDIOMA]
GO
EXEC sys.sp_addextendedproperty @name=N'Descripcion', @value=N'Synonim que accede a la base Parametricas para ver Tabla T010_CONOC_IDIOMA' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'SYNONYM',@level1name=N'Syn_Param_NivelIdioma'
GO
