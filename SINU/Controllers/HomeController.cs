using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SINU.Models;
using SINU.ViewModels;
using SINU.RefWebCPA;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace SINU.Controllers
{
    public class HomeController : Controller
    {
        SINUEntities db= new SINUEntities();
        public ActionResult Index() {
            string autorizacion = "201abe0636a5bf59e44159128323256d1ac62150";
            string consulta = @"<?xml version=""1.0"" encoding=""iso-8859-1""?>
                                <CLEANSING>
                                <QUERY>
                                        <TABLE>DISTRICT</TABLE>
                                        <PROVINCE>JUJUY</PROVINCE>
                                        <DISTRICT>%</DISTRICT>
                                </QUERY>
                                </CLEANSING>";
            String respuesta;
           
            using (RefWebCPA.CleansingService cliente = new CleansingService())
            {
                respuesta = cliente.exec(autorizacion, consulta);
                
                //para salida por consola de visual studio
                System.Diagnostics.Debug.WriteLine(respuesta.ToString());
                //utlizar un XMLserializer para convertir la respuesta de tipo string a objeto del tipo a ser deserializado y acceder a sus propiedades
                // la clase objeto se llama RESPONSE se genera a partir del xml valido obtenido como repuesta, creando un clase vacia y haciendo el pegado especial menu EDITAR>PEGADO ESPECIAL> PEGAR XML COMO CLASE
                XmlSerializer serializer = new XmlSerializer(typeof(RESPONSE));
                // Declaracion del objectO  del tipo a ser deserializado
                RESPONSE distritos;
                //XmlReader xmlReaderRespuesta = new XmlNodeReader(respuestaXML);
                StringReader respuestastringReader = new StringReader(respuesta);
             
                // Se invoca al metodo Deserialize para restablecer el estado del objeto Reponse que representa los distritos de una provincia consultado
                distritos = (RESPONSE)serializer.Deserialize(respuestastringReader);

                int i = 0;
                while (i < distritos.DISTRICT.Length)
                {// acceso al objeto y sus propiedades
                    System.Diagnostics.Debug.WriteLine(distritos.DISTRICT[i].DESCRIPTION);
                    System.Diagnostics.Debug.WriteLine(distritos.DISTRICT[i].PROVINCE);
                    i++;
                }

                /* con este string XML se genero el archivo RESPONSE.CS utilizando el copiado especial
                 <?xml version="1.0" encoding="ISO-8859-1"?>
                    <RESPONSE>
	                    <DISTRICT>
		                    <ROW INDEX="0">
			                    <PROVINCE>JUJUY</PROVINCE>
			                    <DESCRIPTION>DR MANUEL BELGRANO</DESCRIPTION>
		                    </ROW>
		                    <ROW INDEX="1">
			                    <PROVINCE>JUJUY</PROVINCE>
			                    <DESCRIPTION>EL CARMEN</DESCRIPTION>
		                    </ROW>
		                    <ROW INDEX="2">
			                    <PROVINCE>JUJUY</PROVINCE>
			                    <DESCRIPTION>SAN ANTONIO</DESCRIPTION>
		                    </ROW>
		                    <ROW INDEX="3">
			                    <PROVINCE>JUJUY</PROVINCE>
			                    <DESCRIPTION>SAN PEDRO</DESCRIPTION>
		                    </ROW>
		                    <ROW INDEX="4">
			                    <PROVINCE>JUJUY</PROVINCE>
			                    <DESCRIPTION>SANTA BARBARA</DESCRIPTION>
		                    </ROW>
	                    </DISTRICT>
                    </RESPONSE>
                */



            }
            ;
            ViewBag.TextoPAGINAPRINCIPAL = db.Configuracion.FirstOrDefault(b => b.NombreDato == "TextoPAGINAPRINCIPAL").ValorDato;
            return View(db.vPeriodosInscrip.ToList());
        }
        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            vContacto myModel = new vContacto();
            myModel.Configuracion = db.Configuracion.ToList();
            myModel.listoficinas = db.OficinasYDelegaciones.ToList();
            return View(myModel);
        }

        public ActionResult Vaciar()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Vaciar(string email)
        {
            try
            {
                var res = db.Vaciar(email, 1);
            }
            catch (Exception ex)
            {
                ViewBag.vaciar = ex;
                return View();
            }
            ViewBag.vaciar = "Exito, en la eliminacion de los registros del correo:  " + email;
            return View();
        }
    }
}