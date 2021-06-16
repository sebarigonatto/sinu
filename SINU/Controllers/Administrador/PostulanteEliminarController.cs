using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
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
    public class PostulanteEliminarController : Controller
    {
        private SINUEntities db = new SINUEntities();


        // GET: PostulanteEliminar
        public ActionResult Index()
        {
            var DeleYMod = new DelegacionModalidadVm
            {
                Delegaciones=db.OficinasYDelegaciones
                               .Select(m=> new SelectListItem { Value= m.IdOficinasYDelegaciones.ToString(),Text=m.Nombre}).ToList(),
                Modalidad= db.Modalidad
                             .Select(m => new SelectListItem { Value = m.IdModalidad, Text = m.IdModalidad }).ToList()
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
                return Json(new { success=true, msg="Postulante eliminado correctamente" });
            }
            catch (Exception ex)
            {

                return Json(new { success = false, msg = "Error en la eliminacion, intentelo nuevamente." });
                
            }
        }

        // POST: PostulanteEliminar/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            vInscripcionDetalleUltInsc vInscripcionDetalleUltInsc = db.vInscripcionDetalleUltInsc.Find(id);
            db.vInscripcionDetalleUltInsc.Remove(vInscripcionDetalleUltInsc);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        public static object ConvertList(List<object> value, Type type)
        {
            var containedType = type.GenericTypeArguments.First();
            return value.Select(item => Convert.ChangeType(item, containedType)).ToList();
        }

        //CARGA DE TABLA CON AJAX
        public async Task<JsonResult> CustomServerSideSearchAction(DataTableAjaxPostModel model)
        {
            var filterExtras = JObject.Parse(model.extras);
            var deleValue = ((JValue)filterExtras.SelectToken("delegacion")).Value.ToString();
            int delegacion = deleValue==""?0: int.Parse(deleValue);
            string modalidad = (string)((JValue)filterExtras.SelectToken("modalidad")).Value<string>();

            //obtencion de tabla dinamicamente
            //var tableName = "Postulante";
            //var tableClassNameSpace = "SINU.Models";
            //using (var dbContext = new SINUEntities())
            //{
            //    var tableClassName = $"{tableClassNameSpace}.{tableName}";
            //    var dynamicTableType = Type.GetType(tableClassName);      // Type
            //    var dynamicTable = dbContext.Set(dynamicTableType);      // DbSet
                                
            //    var records = await dynamicTable.AsQueryable().ToListAsync();

            //    List<Postulante> listAsInt = records.Cast<Postulante>().ToList();

            //    var asdasd = listAsInt.Select(m => new { m.IdPersona, m.IdComoSeEntero }).ToList();

            //}



            int filteredResultsCount;
            int totalResultsCount;
            var res = YourCustomSearchFunc(model, out filteredResultsCount, out totalResultsCount);


            var result = new List<vInscripcionDetalleUltInsc>(res.Count);
            foreach (var m in res)
            {
                // simple remapping adding extra info to found dataset
                result.Add(new vInscripcionDetalleUltInsc
                {
                    IdPersona = m.IdPersona,
                    IdInscripcion = m.IdInscripcion,
                    Nombres = m.Nombres,
                    Apellido = m.Apellido,
                    DNI = m.DNI,
                    Email= m.Email,
                    Inscripto_En = m.Inscripto_En,
                    IdOficinasYDelegaciones= m.IdOficinasYDelegaciones,
                    Modalidad = m.Modalidad,
                });
            };

            //si envio en model.extras filtro con los combos

            result = result.Where(m => (delegacion>0?m.IdOficinasYDelegaciones == delegacion:true ) && m.Modalidad.Contains(modalidad)).ToList();


            return Json(new
            {
                // this is what datatables wants sending back
                draw = model.draw,
                recordsTotal = totalResultsCount,
                recordsFiltered = filteredResultsCount,
                data = result
            });
        }

  

        public IList<vInscripcionDetalleUltInsc> YourCustomSearchFunc(DataTableAjaxPostModel model, out int filteredResultsCount, out int totalResultsCount)
        {
            var searchBy = (model.search != null) ? model.search.value : null;
            var take = model.length>0? model.length: db.vInscripcionDetalleUltInsc.ToList().Count();
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
            var result = GetDataFromDbase(searchBy, take, skip, sortBy, sortDir, out filteredResultsCount, out totalResultsCount);
            if (result == null)
            {
                // empty collection...
                return new List<vInscripcionDetalleUltInsc>();
            }
            return result;
        }

        public List<vInscripcionDetalleUltInsc> GetDataFromDbase(string searchBy, int take, int skip, string sortBy, bool sortDir, out int filteredResultsCount, out int totalResultsCount)
        {
            // the example datatable used is not supporting multi column ordering
            // so we only need get the column order from the first column passed to us.        
            var whereClause = BuildDynamicWhereClause(searchBy);            
                      
            var result = db.vInscripcionDetalleUltInsc
                           .AsExpandable()
                           .Where(whereClause)                 
                           .OrderByDT(sortBy, sortDir) // have to give a default order when skipping .. so use the PK
                           .Skip(skip)
                           .Take(take)
                           .ToList();

            // now just get the count of items (without the skip and take) - eg how many could be returned with filtering
            filteredResultsCount = db.vInscripcionDetalleUltInsc.AsExpandable().Where(whereClause).Count();
            totalResultsCount = db.vInscripcionDetalleUltInsc.Count();

            return result;
        }

        private Expression<Func<vInscripcionDetalleUltInsc, bool>> BuildDynamicWhereClause(string searchValue)
        {
            // simple method to dynamically plugin a where clause
            var predicate = PredicateBuilder.New<vInscripcionDetalleUltInsc>(true); // true -where(true) return all
            if (String.IsNullOrWhiteSpace(searchValue) == false)
            {
                // as we only have 2 cols allow the user type in name 'firstname lastname' then use the list to search the first and last name of dbase
                var searchTerms = searchValue.Split(' ').ToList().ConvertAll(x => x.ToLower());

                predicate = predicate.Or(s => searchTerms.Any(srch => s.IdInscripcion.ToString().ToLower().Contains(srch)));
                predicate = predicate.Or(s => searchTerms.Any(srch => s.Nombres.ToLower().Contains(srch)));
                predicate = predicate.Or(s => searchTerms.Any(srch => s.Apellido.ToLower().Contains(srch)));
                predicate = predicate.Or(s => searchTerms.Any(srch => s.DNI.ToLower().Contains(srch)));
                predicate = predicate.Or(s => searchTerms.Any(srch => s.Email.ToString().ToLower().Contains(srch)));
                //predicate = predicate.Or(s => searchTerms.Any(srch => s.Inscripto_En.ToLower().Contains(srch)));
                //predicate = predicate.Or(s => searchTerms.Any(srch => s.Modalidad.ToString().ToLower().Contains(srch)));
            }

            return predicate;
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
