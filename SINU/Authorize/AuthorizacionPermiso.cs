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
        private readonly string[] funciones;


        public AuthorizacionPermiso(params string[] funcion)
        {
            this.funciones = funcion;

        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            //LOGICA DE VALIDACION segun funcion -------------------------------------
            List<spValidarUsuario_Result> permiso = db.spValidarUsuario(httpContext.User.Identity.Name, funciones[0]).ToList();
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
                base.HandleUnauthorizedRequest(filterContext);
            }
        }
    }



    public class AuthorizacionGrupo : AuthorizeAttribute

    {
        private const string Url = "~/Error/AccionNoAutorizada";
        SINUEntities db = new SINUEntities();
        private readonly string[] grupos;


        public AuthorizacionGrupo(params string[] grupo)
        {
            this.grupos = grupo;

        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            //verifico en la vSeguridad_Grupos_Usuarios, si el usuario actual pertenece al grupo pasado como parametro
            var codUsuario = httpContext.User.Identity.Name;
            var Listgrupo = db.vSeguridad_Grupos_Usuarios.Where(m => m.codUsuario == codUsuario).Select(m=>m.codGrupo.Trim()).ToList();
            bool pertenece = Listgrupo.Contains("Postulante");
            //si devuelve 1 registro o mas pertenece al grupo indicado
            return (pertenece);
        }
  
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
                base.HandleUnauthorizedRequest(filterContext);
            }
        }
    }
  
}  

  