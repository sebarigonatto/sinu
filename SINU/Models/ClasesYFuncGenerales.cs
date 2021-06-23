using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using Microsoft.Ajax.Utilities;
using SINU.ViewModels;
using System.Windows.Media.Imaging;
using Microsoft.AspNet.Identity;
using System.Windows.Documents;


namespace SINU.Models
{

    /// <summary>Listado de Funciones prácticas que pueden ser usadas desde cualquier parte del sistema
    /// </summary>

    public class Func
    {
        private static ApplicationUserManager _userManager;
        private static SINUEntities db = new SINUEntities();

        private static ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            set
            {
                _userManager = value;
            }
        }
        public Func()
        {
        }

        public Func(ApplicationUserManager userManager)
        {
            UserManager = userManager;

        }
        /// <summary>Nombre de la Identidad que corresponde al usuario del contexto actual
        /// 
        /// </summary>
        /* private*/
        //readonly String Usu = HttpContext.Current.User.Identity.Name;

        /// <summary>Observa el Usuario conectado actualmente én qué grupo de seguridad se encuentra
        /// Si aún no fue agregado a seguridad o si aún no ha iniciado sesion contendrá el perfil de una persona NO IDENTIFICADA
        /// Si inició sesion sin problemas es que está en seguridad y tiene un GRUPO ASIGNADO 
        /// por ende devolvera el nombre del archivo de vista parcial que está en carpeta Shared del tipo _MenuPerfilXxxxx
        /// OJO: Cada nombre de Grupo debe tener su archivo _MenuPerfilGrupo
        /// </summary>
        /// <returns></returns>
        public static String CorrespondeMenu()
        {
            String Respuesta = "";

            Respuesta = String.IsNullOrEmpty(HttpContext.Current.User.Identity.Name) ? "_MenuPerfilNoIde" : f_BuscaGrupo();


            return Respuesta;
        }
        /// <summary>Busca especificamente en la base de Seguridad a qué Grupo fue Asignado el Nombre de Usuario actual
        /// Acá llega teniendo la seguridad que es un nombre distinto de nulo o vacio.
        /// Si no lo encuentra en la tabla de Seguridad devuelve _MenuPerfilNoIde (pues no está identificado para que entre a la aplicacion)
        /// Si lo encontro devuelve el nombre de la PartialView que le corresponde segun el nombre de Grupo. 
        /// OJO: Cada nombre de Grupo debe tener su archivo _MenuPerfilGrupo
        /// </summary>
        /// <returns></returns>
        private static string f_BuscaGrupo()
        {

            String Respuesta = "";
            List<vSeguridad_Grupos_Usuarios> grupo = db.vSeguridad_Grupos_Usuarios.Where(m => m.codUsuario == HttpContext.Current.User.Identity.Name).ToList();
            Respuesta = (grupo.Count > 0) ? ("_MenuPerfil" + (grupo[0].codGrupo.Trim()).Substring(0, 5)) : "_MenuPerfilNoIde";

            return Respuesta;
        }
        public static string f_BuscaIcono()
        {
            String Respuesta = "";
            SINUEntities db = new SINUEntities();
            List<vSeguridad_Grupos_Usuarios> grupo = (db.vSeguridad_Grupos_Usuarios.Where(m => m.codUsuario == HttpContext.Current.User.Identity.Name).ToList());
            //-------------averigua el icono que le corresponde al grupo-------------------------
            string perfil = (grupo.Count > 0) ? grupo[0].codGrupo.Trim() : "NoIdentificado";
            Configuracion conf = db.Configuracion.First(b => (b.NombreDato == "IconoGrupo") && b.UsoDato.Contains(perfil));
            Respuesta = conf.ValorDato;
            //-------------fin-------------------------------------------------------------------

            return Respuesta;
        }
        /// <summary>Construye el objeto error que maneja nuestra View de error del sistema
        /// En realidad debieramos aprender a usar el AddError
        /// </summary>
        /// <param name="mnsg">Mensaje que deseo que aparezca principalmente</param>
        /// <param name="cntrlr">Nombre del controlador donde ocurrio el Problema</param>
        /// <param name="actn">Nombre de la Acción donde ocurrió el problema</param>
        /// <returns></returns>
        public static HandleErrorInfo ConstruyeError(String mnsg, string cntrlr, string actn)
        {
            Exception x = new Exception(mnsg);
            System.Web.Mvc.HandleErrorInfo UnError = new System.Web.Mvc.HandleErrorInfo(x, cntrlr, actn);
            return UnError;
        }

        /// <summary>Construyo el cuerpo de Email y lo ENVIO,como primera Opcion enviar ID_AspNetUser en el caso de no tenerlo enviar ID_Persona. Ej: var xx = Func.EnvioDeMail(x, "MailPrueba", null, datos.IdPersona, "MailAsunto4")</summary>
        /// <param name="ModeloPlantilla">Modelo con los datos que apareceran en el email</param>
        /// <param name="Plantilla">Nombre de la plantilla que se utilizara para el armado del Email</param>
        /// <param name="ID_AspNetUser">ID_AspNetUser de la persona a enviar el mail, segun la tabla AspNetUsers, acepta null</param>
        /// <param name="ID_Persona">O el IdPersona, acepta null</param>
        /// <param name="Asunto">Asunto del Mail, que se obtiene de la Tabla Configuracio</param>
        /// <param name="ID_Delegacion">Id de la Delegacion que se enviara el mail.</param>
        /// <returns></returns>
        public static async Task<string> EnvioDeMail(PlantillaMail ModeloPlantilla, string Plantilla, string? ID_AspNetUser, int? ID_Persona, string Asunto, int? ID_Delegacion, List<string>? mails)
        {
            try
            {
                ID_AspNetUser ??= "";
                ID_Persona ??= 0;
                int idInscrip = 0;
                List<string> correos = new List<string>();

                DatosResponsable datos= new DatosResponsable();
                string Rol= "Delegacion", Modo="To";
                                
                Postulante postulante = db.Postulante.FirstOrDefault(m => m.IdPersona == ID_Persona || m.IdAspNetUser == ID_AspNetUser);
                if (postulante != null)
                {
                    ID_AspNetUser = postulante.IdAspNetUser;
                    idInscrip = db.vPostPersonaEtapaEstadoUltimoEstado.First(m=>m.IdPersona==postulante.IdPersona).IdInscripcionEtapaEstado;
                    Rol = "Postulante";
                }
                else if (ID_Delegacion != null)
                {
                    var mailDelegacion = db.vUsuariosAdministrativos.Where(m => m.IdOficinasYDelegaciones == ID_Delegacion).ToList();

                    foreach (vUsuariosAdministrativos dele in mailDelegacion)
                    {
                        correos.Add(dele.Email);
                    }
                    
                }
                else if(ID_AspNetUser == "" && ID_Delegacion == null)
                {
                    foreach (var item in mails)
                    {
                        correos.Add(item);
                    }
                    string correo = correos[0];
                    var id_per = db.Persona.FirstOrDefault(m => m.Email == correo).IdPersona;
                    postulante = db.Postulante.Find(id_per);
                    idInscrip = db.vPostPersonaEtapaEstadoUltimoEstado.First(m => m.IdPersona == postulante.IdPersona).IdInscripcionEtapaEstado;
                    Rol = "Postulante";
                    Modo = "Bcc";
                }

                //cargo los datos del pie del correo
                switch (Rol)
                {
                    case "Delegacion":
                        datos = new DatosResponsable()
                        {
                            Apellido = ModeloPlantilla.Apellido,
                            ResponsableMail = db.Configuracion.FirstOrDefault(m => m.NombreDato == "ResponsableMail").ValorDato,
                            ResponsablePisoOfic = db.Configuracion.FirstOrDefault(m => m.NombreDato == "ResponsablePisoOfic").ValorDato,
                            ResponsableTelefonoEinterno = db.Configuracion.FirstOrDefault(m => m.NombreDato == "ResponsableTelefonoEinterno").ValorDato
                        };
                        break;
                    case "Postulante":
                        var OfiDeleg =  db.OficinasYDelegaciones.Find(db.Inscripcion.Find(idInscrip).IdDelegacionOficinaIngresoInscribio);
                        datos = new DatosResponsable()
                        {
                            Apellido = ModeloPlantilla.Apellido.ToUpper(),
                            ResponsableMail = OfiDeleg.Email1,
                            ResponsablePisoOfic = OfiDeleg.Provincia + ", " + OfiDeleg.Localidad + ", " + OfiDeleg.Direccion,
                            ResponsableTelefonoEinterno = OfiDeleg.Telefono,
                            ResponsableCelular = OfiDeleg.Celular
                        };
                        break;
                }

                //ENVIO de COREO con plantilla razor (*.cshtml) https://github.com/Antaris/RazorEngine
                var configuracion = new TemplateServiceConfiguration
                {
                    TemplateManager = new ResolvePathTemplateManager(new[] { "Plantillas" }),
                    DisableTempFileLocking = true

                };
                Engine.Razor = RazorEngineService.Create(configuracion);

                string Carpeta = AppDomain.CurrentDomain.BaseDirectory;
                string LayautCarpeta = $"{Carpeta}Plantillas\\LayoutPlantillaMail.cshtml";
                //preparo el layout del correo
                string HtmlLayout = Engine.Razor.RunCompile(LayautCarpeta, null, datos);
                //preparo el cuerpo del correo
                string cuerpoMail = Engine.Razor.RunCompile($"{Carpeta}Plantillas\\" + Plantilla + ".cshtml", null, ModeloPlantilla);
                //var templateHtml = templateService.Parse(template, ModeloPlantilla, null, null);
                var finalHtml = HtmlLayout.Replace("Mail_CUERPO", cuerpoMail);

                //cargo el asunto desde la base de datos
                string asunto = db.Configuracion.FirstOrDefault(b => b.NombreDato == Asunto).ValorDato;



                var result = await EmailService.SendEmail(ID_AspNetUser, asunto, finalHtml, correos, Modo);
                 
                return result;
            }
            catch (Exception x)
            {
                throw;
            }
        }

        /// <summary>Modelo de funcion o rutina para armar
        /// </summary>
        /// <param name="ID">Recibe el IdPostulante par Verificar que sea el mismo del usurio que esta logueado y </param>
        public static bool VerificoUsuario(int ID)
        {
            //proceso de la funcion o rutina que se quiera crear..
            //si es una funcion deben cambiar el void por la clase que devuelve
            //obvio puede no tener parametros
            return true;
        }

        /// <summary>Modelo de funcion o rutina para armar
        /// </summary>
        /// <param name="parametro">tipo de dato que recibe para operar</param>
        public static void funcion(object parametro)
        {
            //proceso de la funcion o rutina que se quiera crear..
            //si es una funcion deben cambiar el void por la clase que devuelve
            //obvio puede no tener parametros
            return;
        }

    }

}