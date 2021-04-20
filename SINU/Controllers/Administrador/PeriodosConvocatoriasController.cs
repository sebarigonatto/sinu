using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SINU.Models;
using SINU.ViewModels;
using SINU.Authorize;
using System.ComponentModel.DataAnnotations;

namespace SINU.Controllers.Administrador
{
    [AuthorizacionPermiso("AdminMenu")]
    public class PeriodosConvocatoriasController : Controller
    {
        private SINUEntities db = new SINUEntities();

        // GET: PeriodosConvocatorias
        public ActionResult Index()
        {
            //return View(db.Convocatoria.ToList());
            return View(db.vConvocatoriaDetalles.ToList());
        }

        // GET: PeriodosConvocatorias/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //PeriodosConvocatorias periodosConvocatorias = db.PeriodosConvocatorias.Find(id);
           
            return View(db.vConvocatoriaDetalles.First(m=>m.IdConvocatoria==id));
        }

        // GET: PeriodosConvocatorias/Create
        public ActionResult Create()
        {
           
            CreacionConvocatoria convoca = new CreacionConvocatoria
            {
                Modalidades = db.vInstitucionModalidad.Where(m=>m.IdInstitucion!=1).ToList(),
                GrupoCarrOficio = new SelectList(db.GrupoCarrOficio, "IdGrupoCarrOficio", "Descripcion"),
            };
            //PeriodosConvocatorias convoca = new PeriodosConvocatorias();
            //ViewBag.IdGrupoCarrOficio = new SelectList(db.GrupoCarrOficio, "IdGrupoCarrOficio", "Descripcion");
            //ViewBag.IdModalidad = new SelectList(db.Modalidad, "IdModalidad", "Descripcion");
            //ViewBag.IdPeriodoInscripcion = new SelectList(db.PeriodosInscripciones, "IdPeriodoInscripcion", "IdPeriodoInscripcion");
            //convoca.Instituciones = new SelectList(db.Institucion.Where(m => m.IdInstitucion != 1).ToList(), "IdInstitucion", "NombreInst");

            #region Aclaracion: estis valores por defectos permiten que el modelo de valido, luego se tiene que verificar los valores correctos para grabar y demas
            //convoca.IdConvocatoria = 1;
            //convoca.IdPeriodoInscripcion = 1;
            //convoca.IdModalidad = "x";
            #endregion




            return View(convoca);
        }

        // POST: PeriodosConvocatorias/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreacionConvocatoria convoca)
        {
            var idperiodo = 0;
            var idModalidad = db.vInstitucionModalidad.First(m => m.IdInstitucion == convoca.FechaConvo.IdInstitucion).IdModalidad;
            convoca.FechaConvo.IdInstitucion = convoca.FechaConvo.IdInstitucion;
            convoca.Modalidades = db.vInstitucionModalidad.Where(m => m.IdInstitucion != 1).ToList();
            convoca.GrupoCarrOficio = new SelectList(db.GrupoCarrOficio, "IdGrupoCarrOficio", "Descripcion");
            if (ModelState.IsValid)
            {
                try
                {
                    //aca grabamos el periodo de inscripcion                    
                    //esta parte es fundamental para obtener el id y poder generar la covocatoria
                    PeriodosInscripciones PeriodoInscrip = new PeriodosInscripciones();                   
                    PeriodoInscrip.IdInstitucion = convoca.FechaConvo.IdInstitucion;
                    PeriodoInscrip.FechaInicio=convoca.FechaConvo.FechaInicioPeriodo;
                    PeriodoInscrip.FechaFinal = convoca.FechaConvo.FechaFinPeriodo;
                    db.PeriodosInscripciones.Add(PeriodoInscrip);
                    db.SaveChanges();
                    idperiodo = PeriodoInscrip.IdPeriodoInscripcion;
                }
                //27/10/2020 este catch es una prueba de internet, pero creo que no hace mas que caputrar el error
                //no lo investigue mucho, por lo que vi en el for iria dando mensajes de pantalla pero  talvez no era para mvc
                catch (DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            System.Console.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                            //aca guardo el error, pero si hay mas de uno, porque creo que lo relaciona con la validacion del viewmodel/metadata
                            //solo dejara el ultimo, no se como hacer un array com viewdata[X] asi tener el listado completo cuando es mas de uno
                            ViewData["error"]= validationError.ErrorMessage.ToString();
                        }
                    }
                    //vuelvo a cargar los datos seleccionados del usuario y pasarlo a la vista por modelo. falta pasar mensaje dle error                   
                    //ViewBag.IdGrupoCarrOficio = new SelectList(db.GrupoCarrOficio, "IdGrupoCarrOficio", "Descripcion");
                    //ViewBag.IdModalidad = new SelectList(db.Modalidad, "IdModalidad", "Descripcion");
                    //ViewBag.IdPeriodoInscripcion = new SelectList(db.PeriodosInscripciones, "IdPeriodoInscripcion", "IdPeriodoInscripcion");
                    //periodosConvocatorias.Instituciones = new SelectList(db.Institucion.Where(m => m.IdInstitucion != 1).ToList(), "IdInstitucion", "NombreInst");

                    //#region Aclaracion: estos valores por defectos permiten que el modelo de valido, luego se tiene que verificar los valores correctos para grabar y demas
                    //periodosConvocatorias.IdConvocatoria = 1;
                    //periodosConvocatorias.IdPeriodoInscripcion = 1;
                    //#endregion
                    
                    return View(convoca);
                }

                //segunda parte de la pantalla, cuando me da ok el periodo de inscripcion, es decir
                //lo graba, paso a crear la convocatoria con el periodo de inscripcion nuevo
                try
                {
                    Convocatoria NuevConvocatoria = new Convocatoria();                            
                    NuevConvocatoria.IdGrupoCarrOficio = convoca.FechaConvo.IdGrupoCarrOficio;
                    //aca cargo en una variable el id del periodo de inscripcion anteriormente grabado
                    //lo busco por los datos ingresados, asi puedo pasarlo como dato para la nueva CONVOCATORIA


                    NuevConvocatoria.IdPeriodoInscripcion = idperiodo;

                    //modalidad
                    NuevConvocatoria.IdModalidad = idModalidad;
                            NuevConvocatoria.Fecha_Inicio_Proceso = convoca.FechaConvo.FechaInicioPeriodo;
                            NuevConvocatoria.Fecha_Fin_Proceso = convoca.FechaConvo.FechaFinProceso;
                            ViewBag.eliminar = NuevConvocatoria.IdPeriodoInscripcion;
                            db.Convocatoria.Add(NuevConvocatoria);
                            db.SaveChanges();
                            return RedirectToAction("Index", "Convocatorias");
                }
                catch (DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            System.Console.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                            ViewData["error"] = validationError.ErrorMessage.ToString();
                        }
                    }
               
                    //elimino el periodo de inscripcion creado, para poder volver a crearlo nuevamente 
                    //y que  no incurra en error de existencia en este punto
                    PeriodosInscripciones EliminarPerConv = db.PeriodosInscripciones.Find(ViewBag.eliminar);
                    db.PeriodosInscripciones.Remove(EliminarPerConv);
                    db.SaveChanges();

                    //#region Aclaracion: estos valores por defectos permiten que el modelo de valido, luego se tiene que verificar los valores correctos para grabar y demas
                    //periodosConvocatorias.IdConvocatoria = 1;
                    //periodosConvocatorias.IdPeriodoInscripcion = 1;
                    //#endregion
                    return View(convoca);
                }           
            }

            return View(convoca);
        }

        // GET: PeriodosConvocatorias/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var convo = db.vConvocatoriaDetalles.First(m => m.IdConvocatoria == id);
            FechaConvocatoria convocatoria = new FechaConvocatoria
            {
                FechaFinPeriodo= convo.FechaFinal,
                FechaInicioPeriodo= convo.FechaInicio,
                FechaFinProceso= (DateTime)convo.Fecha_Fin_Proceso,
                IdGrupoCarrOficio= convo.IdGrupoCarrOficio,
                IdInstitucion= convo.IdInstitucion,
                IdModalidad= convo.Modalidad,
                IDConvo= convo.IdConvocatoria,
                IdPeriodo=convo.IdPeriodoInscripcion

            };
            var personal = db.Modalidad.First(m => m.IdModalidad == convo.IdModalidad).Personal;
            CreacionConvocatoria convoca = new CreacionConvocatoria
            {
                Modalidades = db.vInstitucionModalidad.Where(m => m.IdInstitucion != 1).ToList(),
                GrupoCarrOficio = new SelectList(db.GrupoCarrOficio.Where(m=>m.Personal== personal), "IdGrupoCarrOficio", "Descripcion"),
                FechaConvo= convocatoria
              
            };
            ////PeriodosConvocatorias periodosConvocatorias = db.PeriodosConvocatorias.Find(id);

            return View(convoca);
        }

        // POST: PeriodosConvocatorias/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CreacionConvocatoria convoca)
        {
            var idModalidad = db.vInstitucionModalidad.First(m => m.IdInstitucion == convoca.FechaConvo.IdInstitucion).IdModalidad;
            convoca.FechaConvo.IdInstitucion = convoca.FechaConvo.IdInstitucion;
            convoca.Modalidades = db.vInstitucionModalidad.Where(m => m.IdInstitucion != 1).ToList();
            convoca.GrupoCarrOficio = new SelectList(db.GrupoCarrOficio, "IdGrupoCarrOficio", "Descripcion");
            var convocatoria = db.vConvocatoriaDetalles.First(m => m.IdConvocatoria == convoca.FechaConvo.IDConvo);
           
            try
            {
                if (ModelState.IsValid)
                {
                    var periodos = db.PeriodosInscripciones.Where(m => m.IdPeriodoInscripcion != convocatoria.IdPeriodoInscripcion && m.IdInstitucion == convoca.FechaConvo.IdInstitucion && (convoca.FechaConvo.FechaInicioPeriodo >= m.FechaInicio) && (convoca.FechaConvo.FechaFinPeriodo <= m.FechaFinal)).ToList();
                    if (periodos.Count() == 0)
                    {

                        var convo = db.Convocatoria.First(m => m.IdConvocatoria == convocatoria.IdConvocatoria);
                        convo.Fecha_Inicio_Proceso = convoca.FechaConvo.FechaInicioPeriodo;
                        convo.Fecha_Fin_Proceso = convoca.FechaConvo.FechaFinProceso;
                        convo.IdModalidad = db.Institucion.First(m => m.IdInstitucion == convoca.FechaConvo.IdInstitucion).IdModalidad;
                        convo.PeriodosInscripciones.FechaInicio = convoca.FechaConvo.FechaInicioPeriodo;
                        convo.PeriodosInscripciones.FechaFinal = convoca.FechaConvo.FechaFinPeriodo;
                        convo.IdGrupoCarrOficio = convoca.FechaConvo.IdGrupoCarrOficio;

                        db.Entry(convo).State = EntityState.Modified;

                        db.SaveChanges();

                        return RedirectToAction("Details",new { id= convo.IdConvocatoria });
                    }

                    return View(convoca);
                }
               
                return View(convoca);
            }
            catch (Exception ex)
            {

                return View(convoca);
            }
           
           
        }

        // GET: PeriodosConvocatorias/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            return View(db.vConvocatoriaDetalles.First(m => m.IdConvocatoria == id));
        }

        // POST: PeriodosConvocatorias/Delete/5
        
        public ActionResult DeleteConfirmed(int id)
        {
            Convocatoria convo = db.Convocatoria.Find(id);
            if (convo != null)
            {
                db.Convocatoria.Remove(convo);
                db.PeriodosInscripciones.Remove(db.PeriodosInscripciones.First(m => m.IdPeriodoInscripcion == convo.IdPeriodoInscripcion));
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public JsonResult DevolverCArrerasFiltradas(string ModalidadId)
        {
            using (db = new SINUEntities())
            {
                if (ModalidadId != "")
                {

                    var ModTipoPersonal = db.Modalidad.Where(x => x.IdModalidad == ModalidadId).Select(m => m.Personal).ToList();
                    string y = ModTipoPersonal[0].ToString();
                    var grupo_carreras = db.GrupoCarrOficio.Where(x => x.Personal == y).Select(m => new SelectListItem
                    {
                        Value = m.IdGrupoCarrOficio,
                        Text = m.Descripcion
                    }).ToList();

                    return Json(grupo_carreras, JsonRequestBehavior.AllowGet);
                }
                return Json("", JsonRequestBehavior.AllowGet);
                //carrerasFiltradas
            }
        }

        public JsonResult FiltrarInstituciones(int IdInstitucion)
        {


            using (db = new SINUEntities())
            {
                    //esta logica era para filtrar desde Institucion a Modalidad, pero ese paso intermedio lo sacamos
                    //y directamente Institucion filtra Carrera Oficio
                    //string y = (db.Institucion.Where(x => x.IdInstitucion == InstitucionID).Select(m => m.IdModalidad).ToList())[0].ToString();                    
                    //var grupo_carreras = db.Modalidad.Where(x => x.IdModalidad == y).Select(x => new SelectListItem
                    //{
                    //    Value = x.IdModalidad,
                    //    Text = x.Descripcion
                    //}).ToList();
                    
                    string z = db.vInstitucionModalidad.Where(x => x.IdInstitucion == IdInstitucion).Select(m=>m.IdPersonal).FirstOrDefault();
                    var grupo_carreras = db.GrupoCarrOficio.Where(x => x.Personal == z).Select(m => new SelectListItem
                    {
                        Value = m.IdGrupoCarrOficio,
                        Text = m.Descripcion
                    }).ToList();
                    return Json(grupo_carreras, JsonRequestBehavior.AllowGet);
             }
             
                //carrerasFiltradas
            
        }
    }
}
