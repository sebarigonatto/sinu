using SINU.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SINU.Authorize
{
    public class AuthorizacionPermiso : AuthorizeAttribute

    {
        SINUEntities db = new SINUEntities();
        private readonly string[] funciones;


        public AuthorizacionPermiso(params string[] funcion)
        {
            this.funciones = funcion;

        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            //LOGICA DE VALIDACION segun fucion -------------------------------------
            List<spValidarUsuario_Result> permiso = db.spValidarUsuario(httpContext.User.Identity.Name, funciones[0]).ToList();
            //HttpContext.Current.Session["funcion"] = funciones[0];
            //----------------------------------si devuelve algo esta autorizado cso contrario no tiene permiso de esa funcion-----------------------
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
                context.Result = new RedirectResult("~/Error/AccionNoAutorizada");
            }
        }
    }
}