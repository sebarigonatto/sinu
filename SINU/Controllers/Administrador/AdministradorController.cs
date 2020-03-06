using SINU.Authorize;
using SINU.Models;
using SINU.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using System.Threading.Tasks;

namespace SINU.Controllers.Administrador
{
    [AuthorizacionPermiso("AdminMenu")]

    public class AdministradorController : Controller
    {
        private SINUEntities db = new SINUEntities();

       private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        public AdministradorController()
        {
        }

        public AdministradorController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
           
        }


        // GET: Administrador
        public ActionResult Index()
        {
            string menu = Func.CorrespondeMenu();        
            IEnumerable<SINU.Models.vUsuariosAdministrativos> usuarios = db.vUsuariosAdministrativos.Where(m=>true);//TRAIGO TODOS LOS TIPOS DE USUARIOS 
            return View("UsuariosAdministrativos",usuarios);
        }

        // GET: Administrador/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Administrador/Create
        public ActionResult Create()
        {
            ////creando la lista para la vista Create usa combo de las oficinas de ingreso y delegaciones si es DELEGACION
            //ViewBag.OficinaYDelegacion = new SelectList(db.OficinasYDelegaciones.ToList(), "IdOficinasYDelegaciones", "Nombre");

            return View();
        }

        // POST: Administrador/Create
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<ActionResult> Create([Bind(Include = "Email,Nombre,Apellido,mr,Grado,Destino,Comentario,codGrupo,Password,ConfirmPassword,IdOficinasYDelegaciones")] UsuarioVm usuarioVm) //(FormCollection collection)
        {
            try
            {             
                if (ModelState.IsValid)
                {
                    //cuando no existe el usuario creo uno nuevo con los datos que me dan
                    ApplicationUser user = (await UserManager.FindByNameAsync(usuarioVm.Email)) ?? new ApplicationUser { UserName = usuarioVm.Email, Email = usuarioVm.Email };                    

                    //verificar si el año es ==1 ya existe el usuario sino recien lo voy a crear


                    user.EmailConfirmed = true;
                    IdentityResult result ;

                    result = await UserManager.CreateAsync(user, usuarioVm.Password);
                    if (result.Succeeded)
                    {
                        //Ingresa el regisrto de Usuario a la Base de Seguridad
                        var r = db.spIngresaASeguridad(usuarioVm.Email, usuarioVm.codGrupo, usuarioVm.mr, usuarioVm.Grado, usuarioVm.Destino, usuarioVm.Nombre, usuarioVm.Apellido);
                        if (usuarioVm.IdOficinasYDelegaciones>0)
                        {
                            Usuario_OficyDeleg x=  new Usuario_OficyDeleg();
                            x.Email = usuarioVm.Email; x.IdOficinasYDelegaciones = usuarioVm.IdOficinasYDelegaciones;
                            db.Usuario_OficyDeleg.Add(x);
                            db.SaveChanges();
                        }
                        return RedirectToAction("Index");
                    }

                }
                return View(usuarioVm);
            }
            catch (Exception ex)
            {
                return View("Error", new System.Web.Mvc.HandleErrorInfo(ex, "Administrador", "Create"));
            }
        }

        // GET: Administrador/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Administrador/Edit/5
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

        // GET: Administrador/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Administrador/Delete/5
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
