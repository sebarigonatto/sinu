﻿using System;
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
using static SINU.ViewModels.GrupoCarrOficiosvm;
using System.Web.UI.WebControls;

namespace SINU.Controllers.Administrador.Convocatorias
{
    public class GrupoCarrOficiosController : Controller
    {
        private SINUEntities db = new SINUEntities();

        // GET: GrupoCarrOficios
        public ActionResult Index(string Mensaje)
        {
            ViewBag.Mensaje = Mensaje;
            return View(db.GrupoCarrOficio.ToList());
        }
        // este es el ORIGINAL
        // GET: GrupoCarrOficios/Details/5
        //public ActionResult Details(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    GrupoCarrOficio grupoCarrOficio = db.GrupoCarrOficio.Find(id);
        //    List < CarreraOficio > listado = spCarrerasDelGrupo_Result(id);

        //    if (grupoCarrOficio == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(grupoCarrOficio);
        //}

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
            //ViewBag.Carreras = db.spCarrerasDelGrupo(id).ToList();
            GrupoCarrOficiosvm datosgrupocarroficio = new GrupoCarrOficiosvm()
            {

                IdGrupoCarrOficio = grupoCarrOficio.IdGrupoCarrOficio,
                Descripcion = grupoCarrOficio.Descripcion,
                Personal = grupoCarrOficio.Personal,
                Carreras = db.spCarrerasDelGrupo(id,"").ToList()

            };            
            return View(datosgrupocarroficio);
        }
        public ActionResult Create()
        {
            GrupoCarrOficiosvm prueba = new GrupoCarrOficiosvm { Carreras2 = db.CarreraOficio.ToList() };
                       return View(prueba);
        }

        // POST: GrupoCarrOficios/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdGrupoCarrOficio,Personal,Descripcion,SelectedIDs,IdGCOOriginal,SelIDsEdit")]  GrupoCarrOficiosvm
 grupoCarrOficiovm )
        {
            string stgCarreras = String.Join(",", grupoCarrOficiovm.SelectedIDs);
            ObjectParameter ObjMensaje = new ObjectParameter("Mensaje","");
            grupoCarrOficiovm.Esinsert = true;
            //grupoCarrOficiovm.IdGCOOriginal = grupoCarrOficiovm.IdGrupoCarrOficio;
            if (ModelState.IsValid)
            {
                //03 agosto, graba en grupo carrera oficio
                // aca iria un sp donde le paso todo el listado de carreras y 
                //el id del grupo carrera oficio para asignarle a las mismas.
                //db.GrupoCarrOficio.Add(grupoCarrOficio);
                //db.SaveChanges(); 
                
               db.spGrupoYAgrupacionCarreras(grupoCarrOficiovm.IdGrupoCarrOficio, grupoCarrOficiovm.Personal, grupoCarrOficiovm.Descripcion, grupoCarrOficiovm.IdGCOOriginal, grupoCarrOficiovm.Esinsert, stgCarreras, ObjMensaje);
                //aca debo MANIPULAR al MensajeDevuelto.Value.ToString()
                String mens = ObjMensaje.Value.ToString();
                switch (mens)
                {
                    case string a when a.Contains("Exito"):
                        return RedirectToAction("Index",new { Mensaje = ObjMensaje.Value.ToString() }); //write "<div>Custom Value 1</div>"
                        
                    //case string a when a.Contains("Error"):
                    //    return RedirectToAction("Create", new { Mensaje = ObjMensaje.Value.ToString() }); //write "<div>Custom Value 1</div>" //write "<span>Custom Value 2</span>"

                }
                 //aca haria un case of segun lo recibido en el mensaje (supongo)
                 //lo mando al index si hay exito o queda en la pantalla de create con el error
                
            }
            grupoCarrOficiovm.Carreras2 = db.CarreraOficio.ToList();
            ViewBag.Mensaje = ObjMensaje.Value.ToString();

            return View(grupoCarrOficiovm);
        }

        // GET: GrupoCarrOficios/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GrupoCarrOficio grupoCarrOficio = db.GrupoCarrOficio.Find(id);
            if (grupoCarrOficio == null)
            {
                return HttpNotFound();
            }
            GrupoCarrOficiosvm NuevogrupocarroficioVM = new GrupoCarrOficiosvm {
                IdGrupoCarrOficio = grupoCarrOficio.IdGrupoCarrOficio,
                IdGCOOriginal=grupoCarrOficio.IdGrupoCarrOficio,
                Personal = grupoCarrOficio.Personal,
                Descripcion = grupoCarrOficio.Descripcion,
                Carreras = db.spCarrerasDelGrupo(id, "").ToList(),
                Carreras2 = db.CarreraOficio.ToList(),                
        };          
            //creo lista para compara y marcar como checkeada
            NuevogrupocarroficioVM.Carreras3 = new List<CheckBoxes>();
                    //cargo la lista que se va a mostrar checkeada
            for (int i = 0; i < NuevogrupocarroficioVM.Carreras2.Count(); i++)
            {
                //ListItem li = new ListItem(value, value);
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
            NuevogrupocarroficioVM.SelIDsEdit = NuevogrupocarroficioVM.SelectedIDs;
    
            return View(NuevogrupocarroficioVM);
        }

        // POST: GrupoCarrOficios/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdGrupoCarrOficio,Descripcion,Personal,SelectedIDs,IdGCOOriginal")] GrupoCarrOficiosvm grupoCarrOficiovm)
        {            
            //grupoCarrOficiovm.Carreras2 = db.CarreraOficio.ToList();
        
            string stgCarreras = "";
            if (grupoCarrOficiovm.SelectedIDs==null)
            {
                 stgCarreras = String.Join(",", grupoCarrOficiovm.SelIDsEdit);    
            }
            else
            {
                stgCarreras = String.Join(",", grupoCarrOficiovm.SelectedIDs);
            }
           
            ObjectParameter ObjMensaje = new ObjectParameter("Mensaje", "");
            grupoCarrOficiovm.IdCarreraOficio = "";
            grupoCarrOficiovm.Esinsert = false;

            if (ModelState.IsValid)
            {
                try
                {
                    db.spGrupoYAgrupacionCarreras(grupoCarrOficiovm.IdGrupoCarrOficio, grupoCarrOficiovm.Personal, grupoCarrOficiovm.Descripcion, stgCarreras, grupoCarrOficiovm.Esinsert, grupoCarrOficiovm.IdGCOOriginal, ObjMensaje);
               

                //aca debo MANIPULAR al MensajeDevuelto.Value.ToString()
                String mens = ObjMensaje.Value.ToString();
                switch (mens)
                {
                    case string a when a.Contains("Exito"):
                        return RedirectToAction("Index", new { Mensaje = ObjMensaje.Value.ToString() } );
                    //write "<div>Custom Value 1</div>"
                case string a when a.Contains("Error"):
                    {
                        grupoCarrOficiovm.Carreras2 = db.CarreraOficio.ToList();
                        grupoCarrOficiovm.ErrorDevuelto = ObjMensaje.Value.ToString();
                        return View(grupoCarrOficiovm);
                   };
                    //case string a when a.Contains("Error"):
                    //    return RedirectToAction("Create", new { Mensaje = ObjMensaje.Value.ToString() }); //write "<div>Custom Value 1</div>" //write "<span>Custom Value 2</span>"

            }
                    //aca haria un case of segun lo recibido en el mensaje (supongo)
                    //lo mando al index si hay exito o queda en la pantalla de creat
                    //grupoCarrOficiovm.Carreras2= db.CarreraOficio.ToList();
                }
                catch (Exception ex)// esto es una prueba ..quiero provocar un error y que venga por aca si falla el mail
                {
                    //HttpContext.Session["funcion"] = ex.Message; //no se debe usar session hay que crear el System.Web.Mvc.HandleErrorInfo

                    return View("Error", new System.Web.Mvc.HandleErrorInfo(ex, "GrupoCarreraOficio", "Edit"));
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
            GrupoCarrOficio grupoCarrOficio = db.GrupoCarrOficio.Find(id);
            db.GrupoCarrOficio.Remove(grupoCarrOficio);
            db.SaveChanges();
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

    }
}