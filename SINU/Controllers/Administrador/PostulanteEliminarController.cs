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
using static SINU.Models.ModelDataTable;

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

                Columnas = new List<Column> {
                    new Column { data = "IdPersona", visible = false, name = "int", title="idPersona"},
                    new Column { data = "IdInscripcion", name = "int", title="N° de Pre-Inscripción" },
                    new Column { data = "Nombres", orderable = false, name = "string" , title="Nombres" },
                    new Column { data = "Apellido", orderable = false, name = "string" , title="Apellido" },
                    new Column { data = "DNI", orderable = false, name = "string" , title="DNI" },
                    new Column { data = "Email", orderable = false, name = "string" , title="Email" },
                    new Column { data = "Inscripto_En", searchable = false, name = "string" , title="Delegacion" },
                    new Column { data = "Modalidad", searchable = false, name = "string" , title="Modalidad" },
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

                //var result = db.Sp_PostulanteELIMINAR(emailPostulante, true, comentario, emailResponsable);
                return Json(new { success = true, msg = "Postulante eliminado correctamente" });
            }
            catch (Exception ex)
            {

                return Json(new { success = false, msg = "Error en la eliminacion, intentelo nuevamente." });

            }
        }


        //CARGA DE TABLA CON AJAX
        public async Task<JsonResult> CustomServerSideSearchActionAsync(DataTableAjaxPostModel model)
        {
            try
            {
                             
                var result = await YourCustomSearchFuncAsync(model);
                               
                return Json(new
                {
                    // this is what datatables wants sending back
                    draw = model.draw,
                    recordsTotal = result.totalResultsCount,
                    recordsFiltered = result.filteredResultsCount,
                    data = result.result
                });
            }
            catch (Exception)
            {

                throw;
            }


        }



        public async Task<parametro> YourCustomSearchFuncAsync(DataTableAjaxPostModel model)
        {
            try
            {

                var searchBy = (model.search != null) ? model.search.value : null;
                var take = model.length > 0 ? model.length : (await db.Set(Type.GetType($"{tableClassNameSpace}.{model.tablaVista}")).ToListAsync()).Count();
                var skip = model.start;

                string sortBy = "";
                bool sortDir = true;

                if (model.order != null)
                {
                    // in this example we just default sort on the 1st column
                    sortBy = model.columns[model.order[0].column].data;
                    sortDir = model.order[0].dir.ToLower() == "asc";
                }

                // search the dbase taking into consideration table sorting and paging
                var result = await GetDataFromDbaseAsync(model, searchBy, take, skip, sortBy, sortDir);
                //if (result == null)
                //{
                //    // empty collection...
                //    return new List<vInscripcionDetalleUltInsc>();
                //}
                return result;
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<parametro> GetDataFromDbaseAsync(DataTableAjaxPostModel model, string searchBy, int take, int skip, string sortBy, bool sortDir)
        {
            try
            {
              
                parametro para = new parametro();

                string dirSort = sortDir ? "ASC" : "DESC";

                //armo un string con las columnas requeridad para relizar un select
                string selectColumn = "";

                int columnCount = 0;

                foreach (var columna in model.columns)
                {
                    columnCount++;
                    selectColumn += (columna.name == "int" ? $"(Int32({columna.data})).ToString() as {columna.data}" : columna.data) + (columnCount < model.columns.Count() ? "," : "");
                }


                //armo el where 
                string[] searchWhereDT = new string[] { };
                string whereDT = "";

                // where con la variable searchBy
                if (searchBy != null)
                {
                    searchWhereDT = searchBy.Split(' ').ToList().ConvertAll(m => m.ToLower()).ToArray();

                    int indexParametro = 0;
                    
                    foreach (var param in searchWhereDT)
                    {
                        string wheres = "";
                        int indexColumna = 0;
                        foreach (var columna in model.columns.Where(m => m.searchable))
                        {
                            indexColumna++;
                            wheres += $"it.{columna.data}.Contains(@{indexParametro})" + (indexColumna < model.columns.Where(m => m.searchable).Count() ? " or " : "");                           
                        }
                        indexParametro++;

                        whereDT += $"({wheres}) {(indexParametro < searchWhereDT.Count()?" and ":"")}";                        
                    }                    
                }
                else//trigo todos los elementos
                {
                    whereDT = "true";
                }

                //where si existen filtros extras
                string[] searchWhereExtras = new string[] { };
                string whereExtras = "";              

                if (model.filtrosExtras.Where(m=>m.Value!=null).Count()>0)
                {
                    searchWhereExtras = model.filtrosExtras.Select(m => m.Value).ToArray();
                    //clausula where
                    int indexExtras = 0;
                    foreach (var filtros in model.filtrosExtras)
                    {
                        indexExtras++;
                        whereExtras += (filtros.Value!=null ? $"{filtros.Text}.Contains(@{indexExtras-1})":"true") + (indexExtras < model.filtrosExtras.Count() ? " and " : "");
                    }
                }
                else
                {
                    whereExtras = "true";
                }

            
                para.result = await db.Set(Type.GetType($"{tableClassNameSpace}.{model.tablaVista}")).Select($"new({selectColumn})")
                                      .Where(whereDT, searchWhereDT)
                                      .Where(whereExtras, searchWhereExtras)
                                      .OrderBy($"{sortBy} {dirSort}")
                                      .Skip(skip)
                                      .Take(take)
                                      .ToListAsync();

                para.totalResultsCount = db.Set(Type.GetType($"{tableClassNameSpace}.{model.tablaVista}"))                                           
                                           .Count();
                               
                para.filteredResultsCount = db.Set(Type.GetType($"{tableClassNameSpace}.{model.tablaVista}"))
                                              .Select($"new({selectColumn})")
                                              .Where(whereDT, searchWhereDT)
                                              .Where(whereExtras, searchWhereExtras)
                                              .Count();

                return para;
            }
            catch (Exception ex)
            {

                throw;
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
