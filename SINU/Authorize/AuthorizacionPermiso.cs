using SINU.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SINU.Authorize
{

    public class AuthorizacionPermiso : AuthorizeAttribute
    {
        private const string Url = "~/Error/AccionNoAutorizada";
        SINUEntities db = new SINUEntities();   
 
        public string Funcion { get; set; }
        //private readonly [] funciones;

        // public AuthorizacionPermiso(params string[] funcion)
        public AuthorizacionPermiso( string funcion)
        {
            this.Funcion = funcion;
            //this.funciones = funcion;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            
            //LOGICA DE VALIDACION segun funcion 
            List<spValidarUsuario_Result> permiso = db.spValidarUsuario(httpContext.User.Identity.Name, Funcion).ToList();
            //----------------------------------si devuelve algo esta autorizado caso contrario no tiene permiso de esa funcion-----------------------
            return (permiso.Count() > 0);
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.Result = new JsonResult
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    Data = new { success = false, msg = "No tenes autorizacion para realizar esta accion!!!" }
                };
            }
            else
            {
                filterContext.Result = new RedirectResult(Url);
            }
        }
    }

   
    public class AuthorizacionRol : AuthorizeAttribute
    {
        private const string Url = "~/Error/AccionNoAutorizada";

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.Result = new JsonResult
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    Data = new { redirectTo = FormsAuthentication.LoginUrl }
                };
            }
            else
            {
                filterContext.Result = new RedirectResult(Url);
            }
        }
    }
  
}  

  