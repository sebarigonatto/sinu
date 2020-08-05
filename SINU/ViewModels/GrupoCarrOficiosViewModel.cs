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
        [Required]
        [Display(Name = "Grupo(ID)")]
        [StringLength(10, MinimumLength = 5, ErrorMessage = "Debe ingresar máximo 10 digitos")]
        public string IdGrupoCarrOficio { set; get; }

        
        [Display(Name = "Carrera/Oficio (ID)")]
        public string IdCarreraOficio { set; get; }

        //este listado de carreras es el que uso cuando me dan un ID 
        [Display(Name = "Carreras")]
        public List<spCarrerasDelGrupo_Result> Carreras { set; get; }
        
        [Display(Name = "Personal")]
        public string Personal { set; get; }
       
        [Display(Name = "Descripción")]
        public string Descripcion { set; get; }        
        [Display(Name = "Listado de Carreras disponibles")]
        //este es el listado de carreras para mostrar (todas)
        public List<CarreraOficio> Carreras2 { set; get; }
        //genera el listado id de las carreras cuando se crea un nuevo grupo
        public List<int> SelectedIDs { get; set; }


    }

 }