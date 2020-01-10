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
namespace SINU.Controllers
{
    public class HomeController : Controller
    {
        SINUEntities db= new SINUEntities();
        public ActionResult Index() {
            string autorizacion = "201abe0636a5bf59e44159128323256d1ac62150";
            string consulta = @"<?xml version=""1.0"" encoding=""iso-8859-1""?><CLEANSING><QUERY><TABLE>DISTRICT</TABLE><PROVINCE>JUJUY</PROVINCE><DISTRICT>%</DISTRICT></QUERY></CLEANSING>";
            String respuesta;
           
            using (RefWebCPA.CleansingService cliente = new CleansingService())
            {
                //var respuesta=cliente.exec("201abe0636a5bf59e44159128323256d1ac62150", @"<?xml version=""1.0"" encoding=""iso-8859-1""?><CLEANSING><QUERY><TABLE>DISTRICT</TABLE><PROVINCE>JUJUY</PROVINCE><DISTRICT>%</DISTRICT></QUERY></CLEANSING>");
                 respuesta = cliente.exec(autorizacion, consulta);
                Console.WriteLine(respuesta.ToString());
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