using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SINU.ViewModels;
using SINU.Models;
using System.Web.Mvc;

namespace SINU.ViewModels
{
    public class GrupoCarrOficiosvm
    {
       public string IdGrupoCarrOficio { set; get; }
       public string IdCarreraOficio { set; get; }
        //este listado de carreras es el que uso cuando me dan un ID
       public List<spCarrerasDelGrupo_Result> Carreras { set; get; }
        public string Personal { set; get; }
        public string Descripcion { set; get; }
        //este es el listado de carreras para mostrar (todas)
        public List<CarreraOficio> Carreras2 { set; get; }
        //genera el listado id de las carreras cuando se crea un nuevo grupo
        public List<int> SelectedIDs { get; set; }


    }

 }