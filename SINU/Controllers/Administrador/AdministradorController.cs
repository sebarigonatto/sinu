using SINU.Authorize;
using SINU.Models;
using SINU.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace SINU.Controllers.Administrador
{
    [AuthorizacionPermiso("AdminMenu")]
    [Authorize]
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

            IEnumerable<SINU.Models.vUsuariosAdministrativos> usuarios = db.vUsuariosAdministrativos;//.Where(m=>true);//TRAIGO TODOS LOS TIPOS DE USUARIOS 
            return View("UsuariosAdministrativos",usuarios);
        }

        // GET: Administrador/Details/5
        public ActionResult Details(string Email)
        {
            vUsuariosAdministrativos Elegido = db.vUsuariosAdministrativos.First(m => m.Email==Email);

            return View(Elegido);
        }

        // GET: Administrador/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Administrador/Create
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<ActionResult> Create([Bind(Include = "Email,Nombre,Apellido,mr,Grado,Destino,Comentario,codGrupo,Password,ConfirmPassword,IdOficinasYDelegaciones")] vUsuariosAdministrativos usuario) //UsuarioVm usuarioVm) //(FormCollection collection)
        {
            try
            {             
                if (ModelState.IsValid)
                {
                    //cuando no existe el usuario creo uno nuevo con los datos que me dan
                    ApplicationUser user = (await UserManager.FindByNameAsync(usuario.Email)) ?? new ApplicationUser { UserName = usuario.Email, Email = usuario.Email };                    

                    //verificar si el año es ==1 ya existe el usuario sino recien lo voy a crear, por ahora dejo que aborte y luego decidimos que se hace
                    user.EmailConfirmed = true;
                    IdentityResult result ;

                    result = await UserManager.CreateAsync(user, usuario.Password);
                    if (result.Succeeded)
                    {
                        //Ingresa el regisrto de Usuario a la Base de Seguridad
                        var r = db.spIngresaASeguridad(usuario.Email, usuario.codGrupo, usuario.mr, usuario.Grado, usuario.Destino, usuario.Nombre, usuario.Apellido);
                        
                        if (usuario.IdOficinasYDelegaciones>0)
                        {
                            Usuario_OficyDeleg x = new Usuario_OficyDeleg
                            {
                                Email = usuario.Email,
                                //x.IdOficinasYDelegaciones = usuario.IdOficinasYDelegaciones.HasValue ? usuario.IdOficinasYDelegaciones.Value : 0;
                                //idoficinasydelegaciones puede ser nulo
                                //equivalente corto y moderno es con el operador ??:
                                IdOficinasYDelegaciones = usuario.IdOficinasYDelegaciones ?? 0
                            };
                            db.Usuario_OficyDeleg.Add(x);
                            db.SaveChanges();
                        }

                        //armo el modelo con los datos que quiero que aparescan en el  mail
                        var modelPlantilla = new ViewModels.PlantillaMailCuenta
                        {
                            Apellido = usuario.Apellido,
                            Email = usuario.Email,
                            Password = usuario.Password,
                            LinkConfirmacion = Url.Action("Login", "Account")
                        };
                        //la funcio para enviar devuelve un booleano, 
                        //necesita 4 paramtros:
                        //ViewModel, con la que se cargaran los datos en la Plantilla del mail
                        //Nombre de la Plantilla a Utilizar
                        //Id de la Persona para obtener el correo de destino
                        //Asusnto de Mail.
                        string seenvio = await Func.EnvioDeMail(modelPlantilla, "PlantillaMailCuenta", user.Id,null, "MailAsunto3",null,null);
                        //si llego aqui es que ya grabe en aspnetuser y en Usuario_OficyDeleg, así que vuelvo al listado 

                        return RedirectToAction("Index");
                    }

                }
                return View(usuario);
            }
            catch (Exception ex)
            {
                ex = new Exception("Correo Electronico existente, verifique los datos.");
                return View("Error", new System.Web.Mvc.HandleErrorInfo(ex, "Administrador", "Create"));
            }
        }

        // GET: Administrador/Edit/5
        public ActionResult Edit(string Email)
        {
            vUsuariosAdministrativos usuario;
            if (!String.IsNullOrEmpty(Email))
            {
                usuario = db.vUsuariosAdministrativos.First(m => m.Email == Email);
                if (usuario == null)
                {
                    //return HttpNotFound("ese numero de ID no se encontro en la tabla de xxx");
                    return View("Error", Func.ConstruyeError("Ese Email de Usuario no se encontró entre Usuarios Administrativos", "Administrador", "Edit"));
                    //o puedo mandarlo al index devuelta e ignorar el error
                    //return RedirectToAction("Index");
                }
                ////se coloca cualquier password que cumpla la regla solicitada y si el Usuario desea cambiar la password los borra y recarga               
                usuario.Password = "123456A_";
                usuario.ConfirmPassword = "123456A_"; 

                
                return View(usuario);

            }
            //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);               
            return View("Error", Func.ConstruyeError("Falta el Email que desea buscar entre los Usuarios", "Administrador", "Edit"));
            //o puedo mandarlo al index devuelta e ignorar el error
            //return RedirectToAction("Index");
       }

        // POST: Administrador/Edit/nnn@xx.com
        [HttpPost]
        public ActionResult Edit([Bind(Include = "Email,Nombre,Apellido,mr,Grado,Destino,Comentario,codGrupo,Password,ConfirmPassword,Delegacion,FechUltimaAct,IdOficinasYDelegaciones")] vUsuariosAdministrativos usuario)//(int id, FormCollection collection)
        {
            
            try
            {
                if (ModelState.IsValid)
                {
                    //240620 hasta este momento graba el cambio pero da un error al terminar de ejcutar el sp
                    db.spAdministradorEditar(usuario.Email, usuario.Destino, usuario.mr, usuario.Grado, usuario.Nombre, usuario.Apellido, usuario.codGrupo, usuario.IdOficinasYDelegaciones, usuario.Comentario);
                    return RedirectToAction("Index");
                }
                return View(usuario);
            }
            catch (Exception ex)
            {
                return View("Error", new System.Web.Mvc.HandleErrorInfo(ex, "Administrador", "Edit"));
            }
        }

        // GET: Administrador/Delete/5
        public ActionResult Delete(string Email)
        {
            vUsuariosAdministrativos usuario;
            if (!String.IsNullOrEmpty(Email))
            {
                usuario = db.vUsuariosAdministrativos.First(m => m.Email == Email);
                if (usuario == null)
                {
                    //return HttpNotFound("ese numero de ID no se encontro en la tabla de xxx");
                    return View("Error", Func.ConstruyeError("Ese Email de Usuario no se encontró entre Usuarios Administrativos", "Administrador", "Edit"));
                    //o puedo mandarlo al index devuelta e ignorar el error
                    //return RedirectToAction("Index");
                }
                return View(usuario);
            }
            //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);               
            return View("Error", Func.ConstruyeError("Falta el Email que desea buscar entre los Usuarios", "Administrador", "Edit"));
            //o puedo mandarlo al index devuelta e ignorar el error
            //return RedirectToAction("Index");
        }

        // POST: Administrador/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string Email)
        {
            try
            {
                    vUsuariosAdministrativos usuario;
                    if (!String.IsNullOrEmpty(Email))
                    {
                        usuario = db.vUsuariosAdministrativos.First(m => m.Email == Email);
                        if (usuario == null)
                        {
                            //return HttpNotFound("ese numero de ID no se encontro en la tabla de xxx");
                            return View("Error", Func.ConstruyeError("Ese Email de Usuario no se encontró entre Usuarios Administrativos", "Administrador", "Delete"));
                            //o puedo mandarlo al index devuelta e ignorar el error
                            //return RedirectToAction("Index");
                        }
                    //llamar a proceso de borrado
                    ///return View("Error", Func.ConstruyeError("Se inicia proceso borrado que NO ESTA HECHO "+Email, "Administrador", "Delete"));
                    //db.vUsuariosAdministrativos.Remove(db.vUsuariosAdministrativos.First(m => m.Email == Email));
                    //db.Usuario_OficyDeleg.Remove(db.Usuario_OficyDeleg.First(m => m.Email == Email));
                    //db.SaveChanges();
                    //return RedirectToAction("Index");
                    db.spUsuariosAdministrativosELIMINAR(Email, usuario.codGrupo, usuario.IdOficinasYDelegaciones);
                    return RedirectToAction("Index");
                }
                    //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);               
                    return View("Error", Func.ConstruyeError("Falta el Email que desea buscar entre los Usuarios", "Administrador", "Delete"));
                    //o puedo mandarlo al index devuelta e ignorar el error
                    //return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                return View("Error", new System.Web.Mvc.HandleErrorInfo(ex, "Administrador", "Delete"));
            }
        }
        //
        // GET: /Administrador/SetPassword
        public ActionResult SetPassword(string Email)
        {
            SetPasswordVM model = new SetPasswordVM
            {
                Email = Email
            };
            return View(model);
        }

        //
        // POST: /Administrador/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordVM model)
        {
            
            if (ModelState.IsValid)
            {
                ApplicationUser Usuario = UserManager.FindByEmail(model.Email);
                IdentityResult result =  await UserManager.RemovePasswordAsync(Usuario.Id);
                if (result.Succeeded)
                {
                    
                    result = await UserManager.AddPasswordAsync(Usuario.Id, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                }
                
            }
            //debo ver como mostrar los errores que aparecen en el result.errors
            // Si llegamos a este punto, es que se ha producido un error, volvemos a mostrar el formulario
            return View(model);
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
