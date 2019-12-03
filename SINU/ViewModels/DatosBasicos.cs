using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SINU.Models;

namespace SINU.ViewModels
{

    public class DatosBasicos
    {
        public vPersona_DatosBasicos vPersona_DatosBasicos { get; set; }

        public List<Sexo> Sexo { get; set; }

        public List<vPeriodosInscrip> vPeriodosInscrips { get; set; }

        public List<OficinasYDelegaciones> OficinasYDelegaciones { get; set; }

    }
}