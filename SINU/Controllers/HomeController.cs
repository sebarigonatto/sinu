using SINU.Models;
using SINU.RefWebCPA;
using SINU.ViewModels;
using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;

namespace SINU.Controllers
{
    public class HomeController : Controller
    {
        SINUEntities db = new SINUEntities();
        public ActionResult Index()
        {
            string autorizacion = "201abe0636a5bf59e44159128323256d1ac62150";
            string consulta = @"<?xml version=""1.0"" encoding=""iso-8859-1""?><CLEANSING><QUERY><TABLE>STREET</TABLE><PROVINCE>BUENOS AIRES</PROVINCE><CITY>LA PLATA</CITY><NEIGHBOURHOOD>TOLOSA</NEIGHBOURHOOD><STREET>CALLE 1</STREET></QUERY></CLEANSING>";
            string respuesta;

            using (RefWebCPA.CleansingService cliente = new CleansingService())
            {
                //var respuesta=cliente.exec("201abe0636a5bf59e44159128323256d1ac62150", @"<?xml version=""1.0"" encoding=""iso-8859-1""?><CLEANSING><QUERY><TABLE>DISTRICT</TABLE><PROVINCE>JUJUY</PROVINCE><DISTRICT>%</DISTRICT></QUERY></CLEANSING>");
                respuesta = cliente.exec(autorizacion, consulta);
                Console.WriteLine(respuesta.ToString());
            };

            XDocument doc1 = XDocument.Parse(respuesta);

            var list3 = doc1.Root.Descendants("PROVINCE")
                                .Elements("ROW")
                               .Select(element => element.Value)
                               .ToList();






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