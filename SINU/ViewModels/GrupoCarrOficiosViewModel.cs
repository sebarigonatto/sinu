using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SINU.ViewModels;
using SINU.Models;

namespace SINU.ViewModels
{
    public class GrupoCarrOficiosvm
    {
       public string IdGrupoCarrOficio { set; get; }
       public string IdCarreraOficio { set; get; }
       public List<spCarrerasDelGrupo_Result> Carreras { set; get; }
        public string Personal { set; get; }
        public string Descripcion { set; get; }

    }
    //public class CarrOficiosvm
    //{
    //    public string IdCarreraOficio { set; get; }
    //    public string CarreraUoficio { set; get; }
    //    public string Personal { set; get; }

    //}
    
}