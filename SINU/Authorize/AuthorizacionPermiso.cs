using SINU.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        public AuthorizacionPermiso(string funcion)
        {
            this.Funcion = funcion;
            //this.funciones = funcion;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {

            //LOGICA DE VALIDACION segun funcion 
            List<spValidarUsuario_Result> permiso = db.spValidarUsuario(httpContext.User.Identity.Name, Funcion).ToList();
            //esta funciones son solo ejecutadas por los POSTULANTES
            var fun = new[] { "CreaEditaDatosP", "EliminarDatosP", "ModificarSecuenciaP" };
            if (fun.Contains(Funcion))
            {
                var IDpersonaActual = db.AspNetUsers.FirstOrDefault(m => m.Email == httpContext.User.Identity.Name).Postulante.First().IdPersona;
                var IDpersonaDatos = (httpContext.Request.Form.Count > 1) ? int.Parse(httpContext.Request.Form[1]) : int.Parse(httpContext.Request.QueryString[0]);
                
                if (IDpersonaActual != IDpersonaDatos)
                {
                    //if ((db.Postulante.FirstOrDefault(m=>m.IdPersona==IDpersonaDatos) != null) ? db.Postulante.Find(IDpersonaDatos).FechaRegistro.Date.Year == System.DateTime.Now.Year : false) return false;
                    //verifico si son familiares
                    if (db.Familiares.Where(m => m.IdPersona == IDpersonaDatos && m.IdPostulantePersona == IDpersonaActual).ToList().Count() == 0) return false;
                }

            }
            //verifico los datos a mostrar a un ususario Postulante sea los suyos y no de otro
            else if (Funcion == "ListarRP" && httpContext.User.IsInRole("Postulante") && httpContext.Request.QueryString.Count > 0 && httpContext.Request.QueryString[0]!= "0")
            {
                if (httpContext.Request.QueryString.ToString().Contains("ID_persona"))
                {
                    var IDpersonaActual = db.AspNetUsers.FirstOrDefault(m => m.Email == httpContext.User.Identity.Name).Postulante.First().IdPersona;

                    int id = int.Parse(httpContext.Request.QueryString[0]);
                    if ((id != IDpersonaActual) && (db.Familiares.Where(m => m.IdPersona == id && m.IdPostulantePersona == IDpersonaActual).ToList().Count() < 1))
                    {
                        return false;
                    }
                }
            }

            //----------------------------------si devuelve algo esta autorizado caso contrario no tiene permiso de esa funcion-----------------------
            return (permiso.Count() > 0);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.Request.IsAuthenticated)
            {
                filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Sin Autorización");
            

            }
            else
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
    }

}

