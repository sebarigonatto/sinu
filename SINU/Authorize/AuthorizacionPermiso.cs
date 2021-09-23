using SINU.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SINU.Authorize
{
    //clase para restringir el uso de metodos a ususarios que debe cumplir ciertos requisitos
    public class AuthorizacionPermiso : AuthorizeAttribute
    {
        private const string Url = "~/Error/Forbidden";
        SINUEntities db = new SINUEntities();

        public string Funcion { get; set; }

        //private readonly [] funciones;

        //// public AuthorizacionPermiso(params string[] funcion)
        //public AuthorizacionPermiso(string funcion)
        //{
        //    this.Funcion = funcion;
        //    //this.funciones = funcion;
        //}

        // public AuthorizacionPermiso(params string[] funcion)
        public AuthorizacionPermiso(string funcion)
        {
            this.Funcion = funcion;
            
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            //lista con las secuencias en las cuales el postualante posee control de sus datos
            var secuenciaPostulante = new[] { 6, 13};
            //LOGICA DE VALIDACION segun funcion 
            List<spValidarUsuario_Result> permiso = db.spValidarUsuario(httpContext.User.Identity.Name, Funcion).ToList();
            //esta funciones son solo ejecutadas por los POSTULANTES
            var fun = new[] { "CreaEditaDatosP", "EliminarDatosP", "ModificarSecuenciaP" };
            if (fun.Contains(Funcion))
            {
                if (!httpContext.User.IsInRole("Postulante"))
                {
                    return false;
                }

                var IDpersonaActual = db.AspNetUsers.FirstOrDefault(m => m.Email == httpContext.User.Identity.Name).Postulante.First().IdPersona;
                var IDpersonaDatos = (httpContext.Request.Form.Count > 1) ? int.Parse(httpContext.Request.Form[1]) : int.Parse(httpContext.Request.QueryString[0]);


                //verifico si el id del postulante logueado es igual al del registro a modificar
                // en caso de ser distintos verifico si son familiares y poseen relacion 
                if (IDpersonaActual != IDpersonaDatos)
                {
                    //if ((db.Postulante.FirstOrDefault(m=>m.IdPersona==IDpersonaDatos) != null) ? db.Postulante.Find(IDpersonaDatos).FechaRegistro.Date.Year == System.DateTime.Now.Year : false) return false;
                    //verifico si son familiares
                    if (db.Familiares.Where(m => m.IdPersona == IDpersonaDatos && m.IdPostulantePersona == IDpersonaActual).ToList().Count() == 0) return false;
                }

                //datos del postulante
                var datos = db.vInscripcionEtapaEstadoUltimoEstado.Where(m => m.IdPersona == IDpersonaActual)
                                                                  .OrderBy(m=>m.FechaInscripcion)
                                                                  .ToList().Last();

                //si veo que es una usuaria con convocatoria vencida, permito que realice la reinscripcion
                if (!(bool)datos.Activa)
                {
                    return true;
                }

                //verifico que la secuancia en la que esta el postulante sea una en la que cargue o modifique sus datos
                if (!secuenciaPostulante.Contains(datos.IdSecuencia))
                {
                    return false;
                }

            }
            //verifico los datos a mostrar a un ususario Postulante sea los suyos y no de otro
            else if (Funcion == "ListarRP" && httpContext.User.IsInRole("Postulante") && httpContext.Request.QueryString.Count > 0 && httpContext.Request.QueryString[0] != "0")
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
                        Data = new { success = false, msg = "No posee autorización para realizar esta acción, refresca la pagina e intentalo nuevamente." }
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

