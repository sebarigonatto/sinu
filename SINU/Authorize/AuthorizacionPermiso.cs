using SINU.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
        protected override void HandleUnauthorizedRequest(AuthorizationContext context)
        {
            ////RUTA DE LA VIEW A MOSTRAR EN CASO DE NO TENER PERMISO----

            //respuesta $.ayax
            //if (context.HttpContext.Request.IsAjaxRequest())
            //{
            //    var urlHelper = new UrlHelper(context.RequestContext);
            //    context.HttpContext.Response.StatusCode = 403;
            //    context.Result = new JsonResult
            //    {
            //        Data = new
            //        {
            //            Error = "NotAuthorized",
            //            NoAuth = urlHelper.Action("NoAutorizado", "Error")
            //        },
            //        JsonRequestBehavior = JsonRequestBehavior.AllowGet
            //    };
            //}
            //else
            {
                //respuesta para view
                context.Result = new RedirectResult(Url);
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
        protected override void HandleUnauthorizedRequest(AuthorizationContext context)
        {
            ////RUTA DE LA VIEW A MOSTRAR EN CASO DE NO TENER PERMISO----

            //respuesta $.ayax
            //if (context.HttpContext.Request.IsAjaxRequest())
            //{
            //    var urlHelper = new UrlHelper(context.RequestContext);
            //    context.HttpContext.Response.StatusCode = 403;
            //    context.Result = new JsonResult
            //    {
            //        Data = new
            //        {
            //            Error = "NotAuthorized",
            //            NoAuth = urlHelper.Action("NoAutorizado", "Error")
            //        },
            //        JsonRequestBehavior = JsonRequestBehavior.AllowGet
            //    };
            //}
            //else
            {
                //respuesta para view
                context.Result = new RedirectResult(Url);
            }
        }
    }
}