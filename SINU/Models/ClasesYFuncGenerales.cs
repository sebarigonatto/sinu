using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SINU.Models
{
   
    /// <summary>Listado de Funciones prácticas que pueden ser usadas desde cualquier parte del sistema
    /// </summary>
    public class Func
    {
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
            SINUEntities db = new SINUEntities();
            List<vSeguridad_Grupos_Usuarios> grupo = (db.vSeguridad_Grupos_Usuarios.Where(m => m.codUsuario== HttpContext.Current.User.Identity.Name).ToList());
            Respuesta = (grupo.Count > 0) ? ("_MenuPerfil"+(grupo[0].codGrupo.Trim()).Substring(0,5)) : "_MenuPerfilNoIde"; 
            return Respuesta;
        }
        public static string f_BuscaIcono()
        {
            String Respuesta = "";
            SINUEntities db = new SINUEntities();
            List<vSeguridad_Grupos_Usuarios> grupo = (db.vSeguridad_Grupos_Usuarios.Where(m => m.codUsuario == HttpContext.Current.User.Identity.Name).ToList());
            if (grupo.Count == 0) //usuario desconocido
                Respuesta = "fas fa-user-slash";
            else
            {
                if (grupo[0].codGrupo.Trim()=="Administrador")
                {
                    Respuesta = "fas fa-user-secret";
                }
                if (grupo[0].codGrupo.Trim() == "Delegacion")
                {
                    Respuesta = "fas fa-user-tie";
                }

            }
            return Respuesta;
        }
        /// <summary>Construye el objeto error que maneja nuestra View de error del sistema
        /// En realidad debieramos aprender a usar el AddError
        /// </summary>
        /// <param name="mnsg">Mensaje que deseo que aparezca principalmente</param>
        /// <param name="cntrlr">Nombre del controlador donde ocurrio el Problema</param>
        /// <param name="actn">Nombre de la Acción donde ocurrió el problema</param>
        /// <returns></returns>
        public static System.Web.Mvc.HandleErrorInfo ConstruyeError(String mnsg, string cntrlr, string actn)
        {
            Exception x = new Exception(mnsg);
            System.Web.Mvc.HandleErrorInfo UnError = new System.Web.Mvc.HandleErrorInfo(x, cntrlr, actn);

            return UnError;
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

    public class Usuario
    {
        public string Apellido { get; set; }
        public string Nombres { get; set; }
        public string Destino { get; set; }
        public string Grado { get; set; }
        public string MR { get; set; }
        public string codGrupo { get; set; }
        public string Email { get; set; }
        public int IdOficinasYDelegaciones { get; set; }

        public virtual OficinasYDelegaciones OficinasYDelegaciones { get; set; }

    }
}