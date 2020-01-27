using SINU.Authorize;
using SINU.Models;
using System.Linq;
using System.Web.Mvc;

namespace SINU.Controllers
{
    [AuthorizacionPermiso("RUDInscripcion")]
    public class DelegacionController : Controller
    {
        SINUEntities db = new SINUEntities();
        // GET: Delegacion
        public ActionResult Index()
        {// tomara los datos de incripciones correspondiente a la Delegacion /cuenta usario Asociado
         //cargo todos los registros que ayan valido la cuenta, y esten en la carga de los datos basicos
            var cargadatosbasicos = db.vInscripcionEtapaEstadoUltimoEstado.Where(m => m.IdSecuencia == 5).ToList();
            return View("Index",cargadatosbasicos);

            //return View();
        }

        // GET: Delegacion/Details/5
        public ActionResult Details(int? id)
        {
           

            if (id == null)
            {
                return View("Error", new System.Web.Mvc.HandleErrorInfo(new System.Exception("Falta el ID de Inscripcion que desea consultar."),"Delegacion","Details"));
            }

            //vInscripcionDetalle InscripcionElegida = db.vInscripcionDetalle.Find(id);

            ////por ahora hago esto
            var cargadatosbasicos = db.vInscripcionEtapaEstadoUltimoEstado.Where(m => m.IdInscripcionEtapaEstado == id).ToList();

            if (cargadatosbasicos == null)
            {
                return View("Error", new System.Web.Mvc.HandleErrorInfo(new System.Exception("Incorrecta la llamada a la vista detalle con el id " + id.ToString() + "==> NO EXISTE"), "Delegacion", "Details"));
               
            }
            vInscripcionEtapaEstadoUltimoEstado InscripcionElegida = cargadatosbasicos[0];
            return View(InscripcionElegida);

            //return PartialView(InscripcionElegida);

        }

        // GET: Delegacion/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Delegacion/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Delegacion/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Delegacion/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Delegacion/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Delegacion/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
