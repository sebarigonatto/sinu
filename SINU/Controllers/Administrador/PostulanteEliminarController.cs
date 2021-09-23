using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using LinqKit;
using Newtonsoft.Json.Linq;
using SINU.Models;
using SINU.ViewModels;
using static SINU.Models.AjaxDataTableModel;

namespace SINU.Controllers.Administrador
{
    [Authorize(Roles = "Administrador")]
    public class PostulanteEliminarController : Controller
    {
        private SINUEntities db = new SINUEntities();

       
        private string tableClassNameSpace = "SINU.Models";


        // GET: PostulanteEliminar
        public ActionResult Index()
        {


            var DeleYMod = new PostulantesAdminVM
            {
                Delegaciones = db.OficinasYDelegaciones.Select(m => new SelectListItem { Value = m.Nombre, Text = m.Nombre }).ToList(),

                Modalidad = db.Modalidad.Select(m => new SelectListItem { Value = m.IdModalidad, Text = m.IdModalidad }).ToList(),

                TablaVista = "vInscripcionDetalleUltInsc",

                //Columnas = new List<Column> {
                //    new Column { data = "IdPersona", visible = false, title="idPersona"},
                //    new Column { data = "IdInscripcion", title="N° de Pre-Inscripción" },
                //    new Column { data = "Nombres", orderable = false, title="Nombres" },
                //    new Column { data = "Apellido", orderable = false, title="Apellido" },
                //    new Column { data = "DNI", orderable = false, title="DNI" },
                //    new Column { data = "Email", orderable = false, title="Email" },
                //    new Column { data = "Inscripto_En", searchable = false, title="Delegacion" },
                //    new Column { data = "Modalidad", searchable = false, title="Modalidad" },
                //},
                   Columnas = new List<Column> {
                    ColumnaDTAjax("IdPersona"),
                    ColumnaDTAjax("IdInscripcion"),
                    ColumnaDTAjax( "Nombres",visible: true,true),
                    ColumnaDTAjax( "Apellido",visible: true,true),
                    ColumnaDTAjax("DNI",visible:true,true),
                    ColumnaDTAjax("Email",visible: true ,true),
                    ColumnaDTAjax("Inscripto_En",visible:true, nombreDisplay:"Delegacion"),
                    ColumnaDTAjax("Modalidad", visible:true )
                }
            };
            return View(DeleYMod);
        }



        [HttpPost]
        public JsonResult PostulanteDelete(int idPostulante, string comentario)
        {
            try
            {
                //recuperado para el sp_PostulanteEliminar
                string emailResponsable = HttpContext.User.Identity.Name;
                string emailPostulante = db.Persona.Find(idPostulante).Email;

                var result = db.Sp_PostulanteELIMINAR(emailPostulante, true, comentario, emailResponsable);
                return Json(new { success = true, msg = "Postulante eliminado correctamente" });
            }
            catch (Exception ex)
            {

                return Json(new { success = false, msg = "Error en la eliminacion, intentelo nuevamente." });

            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
