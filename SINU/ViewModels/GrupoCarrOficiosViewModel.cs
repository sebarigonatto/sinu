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
        [StringLength(10, MinimumLength = 5, ErrorMessage = "Debe ingresar mínimo 5 Carácteres (máximo 10)")]
        public string IdGrupoCarrOficio { set; get; }     
        [Display(Name = "Carrera/Oficio (ID)")]
        public string IdCarreraOficio { set; get; }
        //este listado de carreras es el que uso cuando me dan un ID 
        //[Required(ErrorMessage = "Please enter student name.")]
        [Display(Name = "Carreras")]
        public List<spCarrerasDelGrupo_Result> Carreras { set; get; }
        [Required(ErrorMessage = "Seleccione tipo de Personal.")]
        [Display(Name = "Personal")]
        public string Personal { set; get; }
        [Required(ErrorMessage = "Ingrese una descripción.")]
        [Display(Name = "Descripción")]
        public string Descripcion { set; get; }        
        [Display(Name = "Listado de Carreras disponibles")]
        //este es el listado de carreras para mostrar (todas)
        public List<CarreraOficio> Carreras2 { set; get; }
        //genera el listado id de las carreras cuando se crea un nuevo grupo
        [Required(ErrorMessage = "Seleccione una o mas carreras.")]
        public List<int> SelectedIDs { get; set; }
        //public List<CheckBoxes> SelectedBoxes { get; set; }
        public bool Esinsert { get; set; }
        public string IdGCOOriginal { get; set; }
        
        public List<int> SelIDsEdit { get; set; }
        //para usar checkbox
        public List<CheckBoxes> Carreras3 { get; set; }
        public string ErrorDevuelto { set; get; }
        [Required(ErrorMessage = "Elija o cree una nueva restricción")]
        [Display(Name = "Restricción de Grupo")]
        public int IdResGrupo { set; get; }
    }

    public class CheckBoxes
    {
        public string Text { get; set; }
        public int Value { get; set; }
        public bool Checked { get; set; }
    }

}
