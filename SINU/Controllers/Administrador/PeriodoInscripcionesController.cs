using SINU.Authorize;
using SINU.Models;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System;
using SINU.ViewModels;
using System.Collections.Generic;

namespace SINU.Controllers.Administrador
{
    [AuthorizacionPermiso("CRUDParam")]
    public class PeriodoInscripcionesController : Controller
    {

        private SINUEntities db = new SINUEntities();
        // GET: PeriodoInscripciones
        public ActionResult Index()
        {
            return View(db.PeriodosInscripciones.ToList());
        }

        //Toma el id y muestra los datos al selccionar una fila
        // GET: PeriodoInscripciones/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PeriodosInscripciones periodosInscripciones = db.PeriodosInscripciones.Find(id);

            if (periodosInscripciones == null)
            {
                return HttpNotFound();
            }
            return View(periodosInscripciones);
        }

        //Carga valores para dropdownlist 
        // GET: PeriodoInscripciones/Create
        public ActionResult Create()
        {
            //excluyo un registro de las tabla Institucion, "Necesito Orientacion"
            AdministradorVm datos = new AdministradorVm();
            datos.Instituciones = new SelectList(db.Institucion.Where(m => m.IdInstitucion != 1).ToList(), "IdInstitucion", "NombreInst");
            return View(datos);
        }   

        //Accion que de da de alta un nuevo periodo de inscripcion
        // POST: PeriodoInscripciones/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AdministradorVm ListaParametros) // no nos sale el bind pero lo hacemos con FormControls
        {
            if (ModelState.IsValid)
            {
                db.PeriodosInscripciones.Add(ListaParametros.PeriodosInscripcionesVm);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            var Institucion = db.Institucion.Where(m => m.IdInstitucion != 1).ToList();
            ListaParametros.Instituciones = new SelectList(Institucion, "IdInstitucion", "NombreInst");
            return View(ListaParametros);
        }

        //Toma el id de una fila y muestra los datos para modicarlos
        // GET: PeriodoInscripciones/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return View("Error", Func.ConstruyeError("Falta el NRO de ID que desea buscar en la tabla de Periodo de Inscripciones", "PeriodoInscripciones", "Edit"));
            }
            AdministradorVm dato = new AdministradorVm()
            {
                PeriodosInscripcionesVm = db.PeriodosInscripciones.Find(id),
                Instituciones = new SelectList(db.Institucion.Where(m => m.IdInstitucion != 1).ToList(), "IdInstitucion", "NombreInst")
            };
          
            if (dato.PeriodosInscripcionesVm == null)
            {
                return View("Error", Func.ConstruyeError("Ese numero de ID no se encontro en la tabla de Periodo de Inscripciones", "PeriodoInscripciones", "Edit"));
            }
            return View(dato);
        }

        //Accion que guarda los datos modificados de una fila
        // POST: PeriodoInscripciones/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AdministradorVm PeriodosInscripciones )//aqui funciono el bind por que le paso el IdPeriodoInscripcion
        {
            if (ModelState.IsValid)
            {
                PeriodosInscripciones periodo = PeriodosInscripciones.PeriodosInscripcionesVm;
                db.Entry(periodo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            PeriodosInscripciones.Instituciones = new SelectList(db.Institucion.Where(m => m.IdInstitucion != 1).ToList(), "IdInstitucion", "NombreInst");
            return View(PeriodosInscripciones);
        }

        //Toma una id y muesta los datos de la fila que se va dar de baja
        // GET: PeriodoInscripciones/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return View("Error", Func.ConstruyeError("Falta el NRO de ID que desea buscar en la tabla de Periodo de Inscripciones", "PeriodoInscripciones", "Edit"));
            }
            PeriodosInscripciones periodosInscripciones = db.PeriodosInscripciones.Find(id);

            if (periodosInscripciones == null)
            {
                return View("Error", Func.ConstruyeError("Ese numero de ID no se encontro en la tabla de Periodo de Inscripciones", "PeriodoInscripciones", "Edit"));
            }

            //aca es una prueba casi funcional del eliminar, creando viewmodel y poniendo los datos en cada parte segun la condicion de existencia de los datos
            EliminarPeriodo PeriodoEliar = new EliminarPeriodo(); ;
            PeriodoEliar.PeriodosInscripcionesVm = periodosInscripciones;
            List<vConvocatoriaDetalles> x = db.vConvocatoriaDetalles.Where(i => i.IdPeriodoInscripcion == id).ToList();
            if (x != null)
            {
                PeriodoEliar.ConvocatorVm = x;

            };
            return View(PeriodoEliar);
        }

        //Falta agregar el codigo para que se confirme la baja de una fila
        // POST: PeriodoInscripciones/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)//falta incluir el codigo para poder borrar un registro
        {
            try
            {
                
                var reg = db.PeriodosInscripciones.Find(id);
                DateTime hoy = DateTime.Today;
                if (reg.FechaInicio > DateTime.Today && reg.FechaFinal > DateTime.Today)
                {
                    db.PeriodosInscripciones.Remove(reg);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    throw new IndexOutOfRangeException();
                    //Exception ex = new Exception("Correo Electronico existente, verifique los datos.");
                }
                //return RedirectToAction("Index");
            }
            catch (Exception ex )
            {
                ex = new Exception("No puede eliminar un Periodo cuyo rango sea Anterior a la fecha Actual.");
                return View("Error", new System.Web.Mvc.HandleErrorInfo(ex, "PeriodoInscripciones", "Index"));
            }
        }
    }
}
