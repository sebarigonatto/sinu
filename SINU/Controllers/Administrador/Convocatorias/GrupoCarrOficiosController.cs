using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SINU.Models;
using SINU.ViewModels;
using System.Data.Entity.Core.Objects;
using System.ComponentModel.DataAnnotations;
using static SINU.ViewModels.GrupoCarrOficiosvm;
using System.Web.UI.WebControls;
using SINU.Authorize;

namespace SINU.Controllers.Administrador.Convocatorias
{
    [AuthorizacionPermiso("AdminMenu")]
    public class GrupoCarrOficiosController : Controller
    {
       
        private SINUEntities db = new SINUEntities();

        // GET: GrupoCarrOficios
        public ActionResult Index(string Mensaje)
        {
            ViewBag.Mensaje = Mensaje;
            List<GrupoCarrOficio> grupoCarrOficio = db.GrupoCarrOficio.ToList();
            for (int i = 0; i < grupoCarrOficio.Count(); i++)
            {
                //TODO de OTTINO: aca vamos a acceder a la tablita que cree que te trae esto.. lo tenes en varios lugares y te acordas que me dijiste de crear la tablita.. ya lo hice.. en vez de hacer esto traete la tablita
                switch (grupoCarrOficio[i].Personal)
                {
                    case "O":
                        grupoCarrOficio[i].Personal = "Oficiales";
                        break;
                    case "S":
                        grupoCarrOficio[i].Personal = "Suboficiales";
                        break;
                    case "M":
                        grupoCarrOficio[i].Personal = "Marinero";
                        break;
                }
            }
            return View(db.GrupoCarrOficio.ToList());
        }

        // GET: GrupoCarrOficios/Create
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);               
                return View("Error", Func.ConstruyeError("Falta el ID que desea buscar entre los Grupos de Carreras", "GrupoCarrOficios", "Details"));
                //o puedo mandarlo al index devuelta e ignorar el error
                //return RedirectToAction("Index");
            }
            GrupoCarrOficio grupoCarrOficio = db.GrupoCarrOficio.Find(id);
            if (grupoCarrOficio == null)
            {
                //return HttpNotFound();
                return View("Error", Func.ConstruyeError("Ese ID de Grupo no se encontro en la tabla de GrupoCarrOficio", "GrupoCarrOficios", "Details"));
            }
            var idresgrupoNuevo = grupoCarrOficio.ResGrupo.Where(m => m.IdResGrupo > 0).Select(m => m.IdResGrupo);
            GrupoCarrOficiosvm datosgrupocarroficio = new GrupoCarrOficiosvm()
            {

                IdGrupoCarrOficio = grupoCarrOficio.IdGrupoCarrOficio,
                Descripcion = grupoCarrOficio.Descripcion,
                Personal = grupoCarrOficio.Personal,
                Carreras = db.spCarrerasDelGrupo(id,"").ToList(),
                IdResGrupo = idresgrupoNuevo.FirstOrDefault()
                };
            List<vRestriccionesGrupo> lstResGrupo = db.vRestriccionesGrupo.ToList();
            ViewBag.lstresGrupo = lstResGrupo;
            return View(datosgrupocarroficio);
        }

        public static int le;
        public ActionResult Create(int Length)
        {
            le = Length;
            GrupoCarrOficiosvm prueba = new GrupoCarrOficiosvm { Carreras2 = db.CarreraOficio.ToList() };
            List <vRestriccionesGrupo> lstResGrupo = db.vRestriccionesGrupo.ToList();
            ViewBag.lstresGrupo = lstResGrupo;
                       return View(prueba);
        }

        // POST: GrupoCarrOficios/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdGrupoCarrOficio,Personal,Descripcion,SelectedIDs,IdResGrupo")]  GrupoCarrOficiosvm
 grupoCarrOficiovm )
        {

            ObjectParameter ObjMensaje = new ObjectParameter("Mensaje", "");
            //if (grupoCarrOficiovm.IdResGrupo!=0)
            //{
            //grupoCarrOficiovm.IdResGrupo = "";
            //}
            if (ModelState.IsValid)
            {
                string stgCarreras = String.Join(",", grupoCarrOficiovm.SelectedIDs);
                grupoCarrOficiovm.Esinsert = true;
                
                db.spGrupoYAgrupacionCarreras(grupoCarrOficiovm.IdGrupoCarrOficio, grupoCarrOficiovm.Personal,
                  grupoCarrOficiovm.Descripcion, stgCarreras,grupoCarrOficiovm.IdResGrupo, grupoCarrOficiovm.Esinsert,
                  grupoCarrOficiovm.IdGCOOriginal, ObjMensaje);
                //aca debo MANIPULAR al MensajeDevuelto.Value.ToString()
                String mens = ObjMensaje.Value.ToString();
                switch (mens)
                {
                    case string a when a.Contains("Exito"):
                        if (le == 5) return Redirect("../PeriodosConvocatorias/Create");
                        return RedirectToAction("Index", new { Mensaje = ObjMensaje.Value.ToString() }); //write "<div>Custom Value 1</div>"                            
                }
                //aca haria un case of segun lo recibido en el mensaje (supongo)
                //lo mando al index si hay exito o queda en la pantalla de create con el error

            }
            List<vRestriccionesGrupo> lstResGrupo = db.vRestriccionesGrupo.ToList();
            ViewBag.lstresGrupo = lstResGrupo;
            grupoCarrOficiovm.Carreras2= db.CarreraOficio.ToList();
            ViewBag.Mensaje = "No se creó registro. Verifique errores mencionados.";
            return View(grupoCarrOficiovm);
}
        static public String Listid;
        static public int Listint;
        // GET: GrupoCarrOficios/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Listid = id;
            Listint++;
            GrupoCarrOficio grupoCarrOficio = db.GrupoCarrOficio.Find(id);
            if (grupoCarrOficio == null)
            {
                return HttpNotFound();
            }
            List<CarreraOficio> lst = new List<CarreraOficio>();
            lst = db.CarreraOficio.ToList();
            ViewBag.Carreras = lst;


            var idresgrupoNuevo = grupoCarrOficio.ResGrupo.Where(m=>m.IdResGrupo>0).Select(m=> m.IdResGrupo);
            
            GrupoCarrOficiosvm NuevogrupocarroficioVM = new GrupoCarrOficiosvm {
                IdGrupoCarrOficio = grupoCarrOficio.IdGrupoCarrOficio,
                IdGCOOriginal=grupoCarrOficio.IdGrupoCarrOficio,
                Personal = grupoCarrOficio.Personal,
                Descripcion = grupoCarrOficio.Descripcion,
                Carreras = db.spCarrerasDelGrupo(id, "").ToList(),
                Carreras2 = lst.Where(m => m.Personal == grupoCarrOficio.Personal).ToList(),//lstCarreras2.ToList()/*db.CarreraOficio.ToList()*/,                
                IdResGrupo= idresgrupoNuevo.FirstOrDefault()

        };
        //creo lista para compara y marcar como checkeada
        NuevogrupocarroficioVM.Carreras3 = new List<CheckBoxes>();
                    //cargo la lista que se va a mostrar checkeada
            for (int i = 0; i < NuevogrupocarroficioVM.Carreras2.Count(); i++)
            {
                
                CheckBoxes li = new CheckBoxes { Text = NuevogrupocarroficioVM.Carreras2[i].CarreraUoficio, Value = NuevogrupocarroficioVM.Carreras2[i].IdCarreraOficio };
                NuevogrupocarroficioVM.Carreras3.Add(li);
                    
            };           
            for (int i = 0; i < NuevogrupocarroficioVM.Carreras.Count(); i++)
            {
                for (int z = 0; z < NuevogrupocarroficioVM.Carreras2.Count(); z++)
                {
                    if (NuevogrupocarroficioVM.Carreras[i].CarreraUoficio == NuevogrupocarroficioVM.Carreras2[z].CarreraUoficio)
                    {
                        NuevogrupocarroficioVM.Carreras3[z].Checked = true;
                        NuevogrupocarroficioVM.Carreras3[z].Text = NuevogrupocarroficioVM.Carreras2[z].CarreraUoficio;
                        NuevogrupocarroficioVM.Carreras3[z].Value = NuevogrupocarroficioVM.Carreras2[z].IdCarreraOficio;
                    }
                }

            }
            NuevogrupocarroficioVM.SelectedIDs = new List<int>();
            for (int i = 0; i < NuevogrupocarroficioVM.Carreras.Count(); i++)
            {
                 //int li = new int () {Value = Convert.ToInt32(NuevogrupocarroficioVM.Carreras[i].IdCarreraOficio) }
                NuevogrupocarroficioVM.SelectedIDs.Add(NuevogrupocarroficioVM.Carreras[i].IdCarreraOficio);

            }
            NuevogrupocarroficioVM.SelIDsEdit = NuevogrupocarroficioVM.SelectedIDs; List<vRestriccionesGrupo> lstResGrupo = db.vRestriccionesGrupo.ToList();
            ViewBag.lstresGrupo = lstResGrupo;

            return View(NuevogrupocarroficioVM);
        }

        // POST: GrupoCarrOficios/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdGrupoCarrOficio,Descripcion,Personal,SelectedIDs,IdGCOOriginal,IdResGrupo")] GrupoCarrOficiosvm grupoCarrOficiovm)
        {
            
            string stgCarreras = "";
            try
            {
               
                if (grupoCarrOficiovm.SelectedIDs == null)
                {
                    stgCarreras = String.Join(",", grupoCarrOficiovm.SelIDsEdit);
                }
                else
                {
                    stgCarreras = String.Join(",", grupoCarrOficiovm.SelectedIDs);
                }
            }
            catch (Exception ex)// esto es una prueba ..quiero provocar un error y que venga por aca si falla el mail
            {
                //HttpContext.Session["funcion"] = ex.Message; //no se debe usar session hay que crear el System.Web.Mvc.HandleErrorInfo

                return View("Error", new System.Web.Mvc.HandleErrorInfo(ex, "GrupoCarreraOficio", "Create"));
            }


            ObjectParameter ObjMensaje = new ObjectParameter("Mensaje", "");
            grupoCarrOficiovm.IdCarreraOficio = "";
            grupoCarrOficiovm.Esinsert = false;

            if (ModelState.IsValid)
            {
                try
                {
                    db.spGrupoYAgrupacionCarreras(grupoCarrOficiovm.IdGrupoCarrOficio,
                        grupoCarrOficiovm.Personal, grupoCarrOficiovm.Descripcion,
                        stgCarreras, grupoCarrOficiovm.IdResGrupo, grupoCarrOficiovm.Esinsert, grupoCarrOficiovm.IdGCOOriginal,
                        ObjMensaje);


                    //aca debo MANIPULAR al MensajeDevuelto.Value.ToString()
                    String mens = ObjMensaje.Value.ToString();
                    switch (mens)
                    {
                        case string a when a.Contains("Exito"):
                            return RedirectToAction("Index", new { Mensaje = ObjMensaje.Value.ToString() });
                        
                        case string a when a.Contains("Error"):
                            {
                                grupoCarrOficiovm.Carreras2 = db.CarreraOficio.ToList();
                                grupoCarrOficiovm.ErrorDevuelto = ObjMensaje.Value.ToString();
                                return View(grupoCarrOficiovm);
                            };
                            
                    }                   
                }
                catch (Exception ex)// esto es una prueba ..quiero provocar un error y que venga por aca si falla el mail
                {
                    //HttpContext.Session["funcion"] = ex.Message; //no se debe usar session hay que crear el System.Web.Mvc.HandleErrorInfo
                    //return View("Error", new System.Web.Mvc.HandleErrorInfo(ex, "GrupoCarreraOficio", "Create"));
                    grupoCarrOficiovm.Carreras = db.spCarrerasDelGrupo(grupoCarrOficiovm.IdGCOOriginal, "").ToList();
                    GrupoCarrOficio grupoCarrOficio = db.GrupoCarrOficio.Find(grupoCarrOficiovm.IdGCOOriginal);
                    string idCArreras2 = grupoCarrOficio.Personal;
                    //List<spCarrerasDelGrupo_Result> lst = new List<spCarrerasDelGrupo_Result>();                    
                    List<CarreraOficio> lst = new List<CarreraOficio>();
                    List<CarreraOficio> lstCarreras2 = new List<CarreraOficio>();
                    lst = db.CarreraOficio.ToList();
                    for (int i = 0; i < lst.Count; i++)
                    {
                        if (lst[i].Personal == idCArreras2)
                        {
                            CarreraOficio itemlst = new CarreraOficio { CarreraUoficio = lst[i].CarreraUoficio, IdCarreraOficio = lst[i].IdCarreraOficio };
                            lstCarreras2.Add(itemlst);
                        }
                    }                 
                    grupoCarrOficiovm.Carreras2 = lstCarreras2.ToList();
                    grupoCarrOficiovm.Carreras3 = new List<CheckBoxes>();
                    //cargo la lista que se va a mostrar checkeada
                    for (int i = 0; i < grupoCarrOficiovm.Carreras2.Count(); i++)
                    {                       
                        CheckBoxes li = new CheckBoxes { Text = grupoCarrOficiovm.Carreras2[i].CarreraUoficio, Value = grupoCarrOficiovm.Carreras2[i].IdCarreraOficio };
                        grupoCarrOficiovm.Carreras3.Add(li);

                    };
                    grupoCarrOficiovm.ErrorDevuelto = ObjMensaje.Value.ToString();
                    return View(grupoCarrOficiovm);
                }
            }
            return View(grupoCarrOficiovm);
        }


        // GET: GrupoCarrOficios/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GrupoCarrOficio grupoCarrOficio = db.GrupoCarrOficio.Find(id);
            switch (grupoCarrOficio.Personal)
            {
                case "O":grupoCarrOficio.Personal = "Oficiales";
                    break;
                case "S":
                    grupoCarrOficio.Personal = "Suboficiales";
                    break;
                case "M":
                    grupoCarrOficio.Personal = "Marinero";
                    break;
            }
            if (grupoCarrOficio == null)
            {
                return HttpNotFound();
            }
            return View(grupoCarrOficio);
        }

        // POST: GrupoCarrOficios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            try
            {

                GrupoCarrOficio grupoCarrOficio = db.GrupoCarrOficio.Find(id);
                db.GrupoCarrOficio.Remove(grupoCarrOficio);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            catch (Exception ex)// esto es una prueba ..quiero provocar un error y que venga por aca si falla el mail
            {
                //HttpContext.Session["funcion"] = ex.Message; //no se debe usar session hay que crear el System.Web.Mvc.HandleErrorInfo

                return View("Error", new System.Web.Mvc.HandleErrorInfo(ex, "GrupoCarreraOficio", "Edit"));
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

        //esto es para usar Json y cargar la lista filtrada

        //	DevolverCarrerasFiltradas TPersonal
        public JsonResult DevolverCArrerasFiltradas(string RegionId)
        {
            using (db = new SINUEntities())
            {
                //	carreras					Tpersonal
                var carreras = db.CarreraOficio.Where(x => x.Personal == RegionId).Select(m => new SelectListItem
                {
                    Value = m.IdCarreraOficio.ToString(),
                    Text = m.CarreraUoficio
                }).ToList();
                return Json(carreras, JsonRequestBehavior.AllowGet);
                //carrerasFiltradas
            }
        }

        //public JsonResult DevolverRestricciones(int RegionId)
        //{
        //    using (db = new SINUEntities())
        //    {
        //        //carreras Tpersonal
        //        //var Restriccion = db.CarreraOficio.Where(x => x.Personal == RegionId).Select(m => new SelectListItem
        //        //{
        //        //    Value = m.IdCarreraOficio.ToString(),
        //        //    Text = m.CarreraUoficio
        //        //}).ToList();
        //        var Restriccion = db.ResGrupo.Where(x => x.IdResGrupo == RegionId).Select(x => new SelectListItem
        //        {
        //            Value = x.IdResGrupo.ToString(),
        //            Text = "Restricción n° "+x.IdResGrupo.ToString()
        //        }).ToList();
        //        return Json(Restriccion, JsonRequestBehavior.AllowGet);
        //        //carrerasFiltradas
        //    }
        //}
    }
}
