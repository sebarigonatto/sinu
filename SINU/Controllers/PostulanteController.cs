﻿using Microsoft.Ajax.Utilities;
using QRCoder;
using SINU.Authorize;
using SINU.Models;
using SINU.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using System.ServiceModel.Dispatcher;
using Newtonsoft.Json.Linq;
using System.Web.Script.Serialization;
using System.Windows.Media;
using RazorEngine.Compilation;

namespace SINU.Controllers
{

    [Authorize]
    public class PostulanteController : Controller
    {
        SINUEntities db = new SINUEntities();

        //----------------------------------PAGINA PRINCIPAL----------------------------------------------------------------------//
        //ver este atributo de autorizacion si corresponde o no
        //[Authorize(Roles = "Postulante")]

        public ActionResult Index(int? ID_Postulante)
        {
            //error cdo existe uno registrado antes de los cambios de secuencia
            try
            {
                IDPersonaVM pers = new IDPersonaVM
                {
                    ID_PER = (ID_Postulante != null) ? (int)ID_Postulante : db.Persona.FirstOrDefault(m => m.Email == HttpContext.User.Identity.Name.ToString()).IdPersona,

                };
                pers.OfiDele = db.OficinasYDelegaciones.FirstOrDefault(mbox => mbox.IdOficinasYDelegaciones == db.Inscripcion.FirstOrDefault(m => m.IdPostulantePersona == pers.ID_PER).IdDelegacionOficinaIngresoInscribio);
                Session["DeleConsul"] = ID_Postulante != null;

                //cargo los ID de las etapas por las que paso el postulante
                pers.EtapaTabs = db.vPostulanteEtapaEstado.Where(id => id.IdPostulantePersona == pers.ID_PER).OrderBy(m => m.IdEtapa).DistinctBy(id => id.IdEtapa).Select(id => id.IdEtapa).ToList();
                //cargo esto ID etapas en un string
                pers.EtapaTabs.ForEach(m => pers.IDETAPA += m + ",");
                //busco el IDinscripcion del postulante logueado
                int idInscri = db.Inscripcion.FirstOrDefault(m => m.IdPostulantePersona == pers.ID_PER).IdInscripcion;
                //creo array con las secuecias por las que el Postulante
                List<int> Secuencias = db.InscripcionEtapaEstado.OrderByDescending(m => m.Fecha).Where(m => m.IdInscripcionEtapaEstado == idInscri).Select(m => m.IdSecuencia).ToList();
                ViewBag.ULTISECU = Secuencias[0];
                //verifico si se lo postulo o no en la entrevista
                pers.NoPostulado = (Secuencias[0] == 12);
                //ver como mostrar esta pantalla de si fue 
                if (pers.NoPostulado) ViewBag.TextNoAsignado = (db.Inscripcion.FirstOrDefault(m => m.IdPostulantePersona == pers.ID_PER).IdPreferencia == 6) ? db.Configuracion.FirstOrDefault(m => m.NombreDato == "MailCuerpo4NoPostulado2").ValorDato : db.Configuracion.FirstOrDefault(m => m.NombreDato == "MailCuerpo4NoPostulado1").ValorDato;
                ;
                pers.ProcesoInterrumpido = (Secuencias[0] == 24);

                //verifico si la validacion esta en curso o no para el bloqueo de la Pantalla de Documentacion
                ViewBag.ValidacionEnCurso = (Secuencias[0] == 14);
                //Boolenao de si paso por validacion
                Session["ValidoUnaVez"] = (Secuencias.IndexOf(14) != -1) && (Secuencias[0] == 13);

                //Cargo listado con las solapas de documentacion "abiertas o cerradas"
                var PantallasEstadoProblemas = new List<Array>();
                db.spTildarPantallaParaPostulate(pers.ID_PER).ForEach(m => PantallasEstadoProblemas.Add(new object[] { m.Pantalla, m.Abierta, m.CantComentarios }));
                pers.ListProblemaCantPantalla = PantallasEstadoProblemas;
                ViewBag.PantallasEstadoProblemas2 = JsonConvert.SerializeObject(PantallasEstadoProblemas);

                //cargo listado de problemas de pantallas que le corresponde al postulante
                //pers.ListProblemaPantalla = db.DataProblemaPantalla.Where(m => m.IdPostulantePersona == pers.ID_PER).ToList();

                //int idINCRIP = db.Inscripcion.FirstOrDefault(m => m.IdPostulantePersona == pers.ID_PER).IdInscripcion;
                //´verifico si ya realizo el guardado de datos basicos.
                //si ya lo hizo bloqueo los input de las vistaparcial DatosBasicos
                //pers.YAguardado = (db.InscripcionEtapaEstado.Where(i=>i.IdInscripcionEtapaEstado==idINCRIP).Where(i=>i.IdSecuencia==7||i.IdSecuencia==21).ToList().Count() >0 ? true : false);
                return View(pers);
            }
            catch (Exception ex)
            {

                return View("Error", new System.Web.Mvc.HandleErrorInfo(ex, "Postulante", "Index"));
            }

        }

        public ActionResult Index2()
        {
            //error cdo existe uno registrado antes de los cambios de secuencia
            try
            {

                IDPersonaVM pers = new IDPersonaVM
                {
                    ID_PER = db.Persona.FirstOrDefault(m => m.Email == HttpContext.User.Identity.Name.ToString()).IdPersona,
                };
                pers.EtapaTabs = db.vPostulanteEtapaEstado.Where(id => id.IdPostulantePersona == pers.ID_PER).OrderBy(m => m.IdEtapa).DistinctBy(id => id.IdEtapa).Select(id => id.IdEtapa).ToList();
                pers.EtapaTabs.ForEach(m => pers.IDETAPA += m + ",");
                int idInscri = db.Inscripcion.FirstOrDefault(m => m.IdPostulantePersona == pers.ID_PER).IdInscripcion;

                var Secus = db.InscripcionEtapaEstado.OrderByDescending(m => m.Fecha).Where(m => m.IdInscripcionEtapaEstado == idInscri).Where(m => m.IdSecuencia == 11 || m.IdSecuencia == 12).ToList();
                pers.NoPostulado = (Secus.Count() > 0 && Secus[0].IdSecuencia == 12) ? true : false;//ver como mostrar esta pantalla de si fue postulado o no
                ViewBag.TextNoAsignado = db.Configuracion.FirstOrDefault(m => m.NombreDato == "MailCuerpo4NoPostulado1").ValorDato;
                if (db.InscripcionEtapaEstado.OrderByDescending(m => m.Fecha).Where(m => m.IdInscripcionEtapaEstado == idInscri).ToList()[0].IdSecuencia == 14)
                {
                    ViewBag.ValidacionEnCurso = true;
                }
                else
                {
                    ViewBag.ValidacionEnCurso = false;
                }

                //int idINCRIP = db.Inscripcion.FirstOrDefault(m => m.IdPostulantePersona == pers.ID_PER).IdInscripcion;
                //´verifico si ya realizo el guardado de datos basicos.
                //si ya lo hizo bloqueo los input de las vistaparcial DatosBasicos
                //pers.YAguardado = (db.InscripcionEtapaEstado.Where(i=>i.IdInscripcionEtapaEstado==idINCRIP).Where(i=>i.IdSecuencia==7||i.IdSecuencia==21).ToList().Count() >0 ? true : false);
                return View(pers);
            }
            catch (Exception ex)
            {
                return View("Error", new System.Web.Mvc.HandleErrorInfo(ex, "Postulante", "Index"));
            }

        }
        //----------------------------------DATOS BASICOS----------------------------------------------------------------------//


        //ver el tema de la fecha de casamiento
        //ver mejorar la seguridad 

        [AuthorizacionPermiso("ListarRP")]
        public ActionResult DatosBasicos(int ID_persona)
        {
            try
            {
                //se carga los datos basicos del usuario actual y los utilizados para los dropboxlist
                DatosBasicosVM datosba = new DatosBasicosVM()
                {
                    SexoVM = db.Sexo.OrderBy(m=>m.Descripcion).Where(m=>m.Descripcion!="Seleccione Sexo").ToList(),
                    vPeriodosInscripsVM = new List<vPeriodosInscrip>(),
                    OficinasYDelegacionesVM = db.OficinasYDelegaciones.ToList(),
                    vPersona_DatosBasicosVM = db.vPersona_DatosBasicos.FirstOrDefault(b => b.IdPersona == ID_persona),
                    ComoSeEnteroVM = db.ComoSeEntero.Where(n => n.IdComoSeEntero != 1).ToList()
                };
                //datosba.vPersona_DatosBasicosVM.Edad = 0;
                //datosba.vPersona_DatosBasicosVM.IdSexo = 0;
                //agrego la opcion de "Necesito Orientacion" en el combo Inctitucion 
                //datosba.vPeriodosInscripsVM.Add(new vPeriodosInscrip() { IdInstitucion= 1,NombreInst="Necesito Orientacion"});
                return PartialView(datosba);
            }
            catch (Exception ex)
            {
                //revisar como mostrar error en la vista
                return View("Error", new System.Web.Mvc.HandleErrorInfo(ex, "Postulante", "DatosBasicos"));
            }

        }


        //ACCION QUE GUARDA LOS DATOS INGRESADOS EN LA VISTA "DATOS BASICOS"
        [HttpPost]
        [AuthorizacionPermiso("CreaEditaDatosP")]

        public ActionResult DatosBasicos(DatosBasicosVM Datos)
        {
            if (Datos.vPersona_DatosBasicosVM.ComoSeEntero == null)
            {
                ModelState["vPersona_DatosBasicosVM.ComoSeEntero"].Errors.Clear();
            };

            if (ModelState.IsValid)
            {
                try
                {
                    //se guarda los datos de las persona devueltos
                    var p = Datos.vPersona_DatosBasicosVM;


                    var result = db.spDatosBasicosUpdate(p.Apellido, p.Nombres, p.IdSexo, p.DNI, p.Telefono, p.Celular, p.FechaNacimiento, p.Email, p.IdDelegacionOficinaIngresoInscribio, p.ComoSeEntero, p.IdComoSeEntero, p.IdPreferencia, p.IdPersona, p.IdPostulante);

                    return Json(new { success = true, msg = "Se guardaron los datos correctamente datos basicos", form = "datosbasicos" });

                }
                catch (Exception ex)
                {
                    //envio la error  a la vista
                    string msgerror = ex.Message + " " + ex.InnerException.Message;
                    return Json(new { success = false, msg = msgerror });
                }
            };
            return Json(new { success = false, msg = "Modelo no VALIDO" });
        }

       
        public JsonResult EdadInstituto(int? IdPOS, string? Fecha)
        {
            try
            {
                DateTime fechaNAC = DateTime.Parse(Fecha);
                var institutos = db.spRestriccionesParaEstePostulante(IdPOS, fechaNAC,null).DistinctBy(m => m.IdInstitucion).Select(m => new SelectListItem { Value = m.IdInstitucion.ToString(), Text = m.NombreInst }).ToList();
                institutos.Add(new SelectListItem() { Value = "1", Text = "Necesito Orientacion" });
                return Json(new { institucion = institutos }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }

        }

        /*--------------------------------------------------------------SOLICITUD DE ENTREVISTA------------------------------------------------------------------------------*/

        [AuthorizacionPermiso("ModificarSecuenciaP")]
        public async Task<JsonResult> SolicitudEntrevistaAsync(int ID_persona)
        {
            try
            {
                var p = db.vPersona_DatosBasicos.First(m => m.IdPersona == ID_persona);
                //ver esto al momento de poner datos basico valido o no valido, creo que deberia ser segun edad y si solo tinee la opcion "necesito orientacion"
                if (p.Edad <= 35 || p.Edad > 16)
                {
                    db.spProximaSecuenciaEtapaEstado(p.IdPersona, 0, false, 0, "DATOS BASICOS", "Validado");
                }
                else
                {
                    //Datos basicos - No Validado; ID= 21
                    db.spProximaSecuenciaEtapaEstado(p.IdPersona, 0, false, 0, "DATOS BASICOS", "No Validado");
                };

                //coloco el delay para que las secuencias sean insertadas en distintos tiempos
                await Task.Delay(1000);

                db.spProximaSecuenciaEtapaEstado(ID_persona, 0, false, 0, "ENTREVISTA", "A Asignar");

                //Envio de Mail para notificar a la delegacion correpondiente
                int ID_Delegacion = (int)db.Inscripcion.FirstOrDefault(m => m.IdPostulantePersona == ID_persona).IdDelegacionOficinaIngresoInscribio;
                int ID_INSCRIP = db.Inscripcion.FirstOrDefault(m => m.IdPostulantePersona == ID_persona).IdInscripcion;
                var grupoDelegacion = db.vUsuariosAdministrativos.Where(m => m.IdOficinasYDelegaciones == ID_Delegacion).ToList();
                SolicitudEntreCorreoPostulante datosMail = new SolicitudEntreCorreoPostulante();
                foreach (var dele in grupoDelegacion)
                {
                    var idAsp_delegacion = db.AspNetUsers.FirstOrDefault(m => m.Email == dele.Email).Id;
                    //armo modelo para armar el correo
                    datosMail.Apellido = dele.Apellido;
                    datosMail.Apellido_P = p.Apellido;
                    datosMail.Dni_P = p.DNI;
                    datosMail.IdInscripcion_P = ID_INSCRIP;
                    datosMail.Nombre_P = p.Nombres;
                    datosMail.url = Url.Action("EntrevistaAsignaFecha", "Delegacion", new { id = ID_persona }, protocol: Request.Url.Scheme);

                    Func.EnvioDeMail(datosMail, "PlantillaMailSolicitudEntrevista", idAsp_delegacion, null, "MailAsunto8");
                };

                return Json(new { success = true, msg = "La Solicitud de Entrevista fue exitosa, se le informara via CORREO la fecha ASIGNADA.", form = "solicitudentrevista" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = ex.InnerException.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        //----------------------------------ENTREVISTA----------------------------------------------------------------------//

        [AuthorizacionPermiso("ListarRP")]
        public ActionResult Entrevista(int ID_persona)
        {

            try
            {
                vEntrevistaLugarFecha entrevistafh = new vEntrevistaLugarFecha();
                entrevistafh = db.vEntrevistaLugarFecha.FirstOrDefault(m => m.IdPersona == ID_persona);
                var estado = db.InscripcionEtapaEstado.Where(m => m.IdInscripcionEtapaEstado == db.Inscripcion.FirstOrDefault(m => m.IdPostulantePersona == ID_persona).IdInscripcion).Where(m => m.IdSecuencia == 8 || m.IdSecuencia == 10 || m.IdSecuencia == 11).OrderBy(m => m.Fecha).ToList();
                int IDsecu = estado[estado.Count - 1].IdSecuencia;
                //coloco el estado de la entrevista
                ViewBag.EstadoEntre = (IDsecu == 11) ? "Concretada" : db.vSecuencia_EtapaEstado.FirstOrDefault(m => m.IdSecuencia == IDsecu).Estado;
                ViewBag.FechaAsisgnada = (entrevistafh.FechaEntrevista != null);
                //se carga los texto parametrizados desde la tabla configuracion
                string[] consideraciones = {
                    db.Configuracion.FirstOrDefault(m => m.NombreDato == "ConsideracionEntrevTitulo").ValorDato.ToString(),
                    db.Configuracion.FirstOrDefault(m => m.NombreDato == "ConsideracionEntrevTexto").ValorDato.ToString()
                };
                ViewBag.Considere = consideraciones;

                return PartialView(entrevistafh);
            }
            catch (Exception ex)
            {

                return PartialView(ex);
            }

        }

        //----------------------------------DATOS PERSONALES----------------------------------------------------------------------//


        [AuthorizacionPermiso("ListarRP")]
        public ActionResult DatosPersonales(int ID_persona)
        {
            try
            {
                int idInscripcion = db.Inscripcion.FirstOrDefault(m => m.IdPostulantePersona == ID_persona).IdInscripcion;

                DatosPersonalesVM datosba = new DatosPersonalesVM()
                {
                    vPersona_DatosPerVM = db.vPersona_DatosPer.FirstOrDefault(m => m.IdPersona == ID_persona),
                    TipoNacionalidadVM = db.TipoNacionalidad.Where(m => m.IdTipoNacionalidad != 4).ToList(),
                    vEstCivilVM = db.vEstCivil.ToList(),
                    vRELIGIONVM = db.vRELIGION.ToList(),
                    CarreraOficioVm = new List<spCarrerasParaEsteInscripto_Result2>(),
                    ModalidadVm = new List<ComboModalidad>()

                };
                datosba.vPersona_DatosPerVM.IdModalidad ??= "";
                var validosInscrip = db.spRestriccionesParaEstePostulante(ID_persona, datosba.vPersona_DatosPerVM.FechaNacimiento,null).ToList();
                foreach (var item in validosInscrip)
                {

                    //cargo modalidades
                    var modalidad = db.vConvocatoriaDetalles.FirstOrDefault(m => m.IdConvocatoria == item.IdConvocatoria);
                    if (datosba.ModalidadVm.FirstOrDefault(m => m.IdModalidad == modalidad.IdModalidad) == null)
                    {
                        datosba.ModalidadVm.Add(new ComboModalidad() { IdModalidad = modalidad.IdModalidad, Modalidad = modalidad.Modalidad, EstCivil = item.IdEstadoCivil });

                    }
                    //cargo carreras
                    var carrera = db.spCarrerasDelGrupo(modalidad.IdGrupoCarrOficio, "").ToList();
                    foreach (var item2 in carrera)
                    {
                        datosba.CarreraOficioVm.Add(new spCarrerasParaEsteInscripto_Result2 { IdCarreraOficio = item2.IdCarreraOficio, CarreraUoficio = item2.CarreraUoficio, IdModalidad = modalidad.IdModalidad });
                    }
                    //datosba.CarreraOficioVm.Add(carrera);
                }
                return PartialView(datosba);
            }
            catch (Exception ex)
            {
                //revisar como mostrar error en la vista
                return PartialView(ex);
            }
        }

        //ACCION QUE GUARDA LOS DATOS INGRESADOS EN LA VISTA "DATOS PERSONALES"
        [HttpPost]
        [AuthorizacionPermiso("CreaEditaDatosP")]
        public ActionResult DatosPersonales(DatosPersonalesVM Datos)
        {
            var fe = DateTime.Now;
            if (ModelState.IsValid)
            {
                try
                {
                    
                    var p = Datos.vPersona_DatosPerVM;
                    //Si el id religion en NULL le envio "", que corresponde a la religion NINGUNA
                    p.IdReligion ??= "";
                    //busco el nuevo id preferencia para la modalidad seleccionada
                    int IDpreNuevo = db.vConvocatoriaDetalles.FirstOrDefault(m => m.IdModalidad == p.IdModalidad).IdInstitucion;
                    var result = db.spDatosPersonalesUpdate(p.IdPersona, p.IdInscripcion, p.CUIL, p.FechaNacimiento, p.IdEstadoCivil, p.IdReligion, p.idTipoNacionalidad, p.IdModalidad, p.IdCarreraOficio,IDpreNuevo);
                   
                    return Json(new { success = true, msg = "se guardaron con exito los DATOS PERSONALES",form="DatosPersonales" });
                }
                catch (Exception ex)
                {
                    //envio la error  a la vista
                    string msgerror = ex.Message + " " + ex.InnerException.Message;
                    return Json(new { success = false, msg = msgerror });
                }
            }
            return Json(new { success = false, msg = "Modelo no VALIDO" });
        }
        //----------------------------------Antecedentes Penales----------------------------------------------------------------------//


        [AuthorizacionPermiso("ListarRP")]
        public ActionResult DocuPenal(int ID_persona)
        {
            try
            {
                DocuPenalVM d = new DocuPenalVM()
                {
                    IdPersona = ID_persona

                };

                string ubicacion = AppDomain.CurrentDomain.BaseDirectory;
                string CarpetaDeGuardado = $"{ubicacion}Documentacion\\ArchivosDocuPenal\\";
                string carpetaLink = "../Documentacion/ArchivosDocuPenal/";
                string archivo = ID_persona + "*";
                string[] archivos = Directory.GetFiles(CarpetaDeGuardado, archivo);
                foreach (var item in archivos)
                {
                    if (item.IndexOf("Anexo2") > 0) d.PathFormularioAanexo2 = carpetaLink + item.Substring(item.LastIndexOf("\\") + 1);
                    if (item.IndexOf("Certificado") > 0) d.PathConstanciaAntcPenales = carpetaLink + item.Substring(item.LastIndexOf("\\") + 1);
                }

                return PartialView(d);
            }
            catch (Exception ex)
            {

                throw;
            }

        }


        [HttpPost]
        [AuthorizacionPermiso("CreaEditaDatosP")]
        public JsonResult DocuPenal(DocuPenalVM data)
        {
            try
            {
                ViewBag.baase = AppDomain.CurrentDomain.BaseDirectory;
                string ubicacion = AppDomain.CurrentDomain.BaseDirectory??"no";
                string CarpetaDeGuardado = $"{ubicacion}Documentacion\\ArchivosDocuPenal\\";
                string anexo = "";
                string cert = "";
                string NombreArchivo, ExtencioArchivo,guarda;
                bool btanexo=false, btcert=false;
                int id = data.IdPersona;
                string[] archivos = Directory.GetFiles(CarpetaDeGuardado, id + "*");
                foreach (var item in archivos)
                {
                    if (item.IndexOf("Anexo2") > 0) anexo = item;
                    if (item.IndexOf("Certificado") > 0) cert = item;
                }

                if (data.FormularioAanexo2 != null)
                {
                    if (anexo != "") System.IO.File.Delete(anexo);
                    NombreArchivo = id + "&Anexo2";
                    ExtencioArchivo = Path.GetExtension(data.FormularioAanexo2.FileName);
                    guarda = CarpetaDeGuardado + NombreArchivo + "&" + data.FormularioAanexo2.FileName;
                    data.FormularioAanexo2.SaveAs(guarda);
                    btanexo = true;
                }
                if (data.ConstanciaAntcPenales != null)
                {
                    if (cert != "") System.IO.File.Delete(cert);
                    NombreArchivo = id + "&Certificado";
                    ExtencioArchivo = Path.GetExtension(data.ConstanciaAntcPenales.FileName);
                    guarda = CarpetaDeGuardado + NombreArchivo + "&" + data.ConstanciaAntcPenales.FileName;
                    data.ConstanciaAntcPenales.SaveAs(guarda);
                    btcert = true;
                }
                
                return Json(new { success = true, form = "DocuPenal", msg = "Se Guardaron correctamnete los archivos seleccionados." , anexo = btanexo,cert=btcert },JsonRequestBehavior.AllowGet) ;

            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = ex.InnerException.Message +"  "+ ViewBag.path },JsonRequestBehavior.AllowGet);
            }

        }


        public FileContentResult GetAnexo2(string? docu,int? Id)
        {
            string ubicacion = AppDomain.CurrentDomain.BaseDirectory;
            
            if (docu == null)
            {
                string UbicacionPDF = $"{ubicacion}Documentacion\\ANEXO 2 A LA SOLICITUD DE INGRESO.pdf";
                byte[] FileBytes = System.IO.File.ReadAllBytes(UbicacionPDF);
                //el tercer para obligar la descarga del archivo
                return File(FileBytes, "application/pdf","Anexo 2");
            }
            else
            {
                string Ubicacionfile = $"{ubicacion}Documentacion\\ArchivosDocuPenal\\";
                string[] archivos = Directory.GetFiles(Ubicacionfile, Id + "&" + docu + "*");
                byte[] FileBytes = System.IO.File.ReadAllBytes(archivos[0]);
                string app = "";
                switch (archivos[0].ToString().Substring(archivos[0].ToString().LastIndexOf('.')+1))
                {
                    case "jpg":
                        app= "image/jpeg";
                        break;
                    case "pdf":
                        app = "application/pdf";
                        break;
                    default:
                        break;
                };
                return File(FileBytes, app);
            }
        }

        //----------------------------------Domicilio----------------------------------------------------------------------//


        public ActionResult Domicio_API(int ID_persona)
        {

            Domiciolio_API domi = new Domiciolio_API()
            {
                vPersona_Domicilio_API = db.vPersona_Domicilio.FirstOrDefault(m => m.IdPersona == ID_persona),
                Pais_API = db.sp_vPaises("").Select(m => new SelectListItem { Text = m.DESCRIPCION, Value = m.CODIGO }).ToList(),
                Provincia_API = new List<SelectListItem>(),
                Localidad_API = new List<SelectListItem>()

            };
            return View(domi);
        }


        [AuthorizacionPermiso("ListarRP")]
        public ActionResult Domicilio(int ID_persona)
        {
            try
            {
                DomicilioVM datosdomilio = new DomicilioVM()
                {
                    vPersona_DomicilioVM = db.vPersona_Domicilio.FirstOrDefault(m => m.IdPersona == ID_persona),
                    sp_vPaises_ResultVM = db.sp_vPaises("").OrderBy(m => m.DESCRIPCION).ToList(),
                    provincias = db.vProvincia_Depto_Localidad.OrderBy(m=>m.Provincia).Select(m => new SelectListItem { Value = m.Provincia, Text = m.Provincia }).DistinctBy(m => m.Text).ToList()
                };

                if (datosdomilio.vPersona_DomicilioVM.IdPais != "AR")
                {
                    string[] domiextR = datosdomilio.vPersona_DomicilioVM.Prov_Loc_CP.Split('-');
                    datosdomilio.vPersona_DomicilioVM.TBoxProvincia = domiextR[0];
                    datosdomilio.vPersona_DomicilioVM.Localidad = domiextR[1];
                    datosdomilio.vPersona_DomicilioVM.CODIGO_POSTAL = domiextR[2];
                };

                if (datosdomilio.vPersona_DomicilioVM.EventualIdPais != "AR")
                {
                    string[] domiextE = datosdomilio.vPersona_DomicilioVM.EventualProv_Loc.Split('-');
                    datosdomilio.vPersona_DomicilioVM.TBoxEventualProvincia = domiextE[0];
                    datosdomilio.vPersona_DomicilioVM.EventualLocalidad = domiextE[1];
                    datosdomilio.vPersona_DomicilioVM.EventualCodigo_Postal = domiextE[2];
                };

                datosdomilio.vProvincia_Depto_LocalidadREALVM = (datosdomilio.vPersona_DomicilioVM.IdLocalidad != null) ?
                    db.vProvincia_Depto_Localidad.Where(m => m.Provincia == datosdomilio.vPersona_DomicilioVM.Provincia).ToList()
                  : new List<vProvincia_Depto_Localidad>();

                datosdomilio.vProvincia_Depto_LocalidadEVENTUALVM = (datosdomilio.vPersona_DomicilioVM.EventualIdLocalidad != null) ?
                    db.vProvincia_Depto_Localidad.Where(m => m.Provincia == datosdomilio.vPersona_DomicilioVM.EventualProvincia).ToList()
                  : new List<vProvincia_Depto_Localidad>();

                return PartialView(datosdomilio);
            }
            catch (Exception ex)
            {
                //revisar como mostrar error en la vista
                return PartialView(ex);
            }
        }


        //ACCION QUE GUARDA LOS DATOS INGRESADOS EN LA VISTA "DATOS PERSONALES"
        [HttpPost]
        [AuthorizacionPermiso("CreaEditaDatosP")]
        public JsonResult Domicilio(DomicilioVM Datos)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var p = Datos.vPersona_DomicilioVM;
                    if (p.IdPais != "AR")
                    {
                        p.IdLocalidad = null;
                        p.Prov_Loc_CP = p.TBoxProvincia + "-" + p.Localidad + "-" + p.CODIGO_POSTAL;
                    }
                    else
                    {
                        p.Prov_Loc_CP = null;
                    };

                    if (p.EventualIdPais != "AR")
                    {
                        p.EventualIdLocalidad = null;
                        p.EventualProv_Loc = p.TBoxEventualProvincia + "-" + p.EventualLocalidad + "-" + p.EventualCodigo_Postal;
                    }
                    else
                    {
                        p.EventualProv_Loc = null;
                    };

                    db.spDomiciliosU(
                        p.IdDomicilioDNI, p.Calle, p.Numero, p.Piso, p.Unidad, p.IdLocalidad, p.Prov_Loc_CP, p.IdPais,
                        p.IdDomicilioActual, p.EventualCalle, p.EventualNumero, p.EventualPiso, p.EventualUnidad, p.EventualIdLocalidad, p.EventualProv_Loc, p.EventualIdPais
                    );

                    return Json(new { success = true, msg = "Se guardaron con Exito datos de DOMICILIO" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    //envio la error  a la vista
                    string msgerror = ex.Message + " " + ex.InnerException.Message;
                    return Json(new { success = false, msg = msgerror }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new { success = false, msg = "Modelo no VALIDO - " /*+ errors*/ }, JsonRequestBehavior.AllowGet);
        }

        //Cargo el DropBoxList de localidad,segun Provincia Seleccionado o Cp segun Localidad Seleccionada
        public JsonResult DropEnCascadaDomicilio(string? Provincia, int? Localidad)
        {
            if (Provincia != null)
            {
                var localidades = db.vProvincia_Depto_Localidad
                                .Where(m => m.Provincia == Provincia)
                                .Select(m => new SelectListItem
                                {
                                    Value = m.IdLocalidad.ToString(),
                                    Text = m.Localidad
                                })
                                .OrderBy(m => m.Text)
                                .ToList();
                return Json(localidades, JsonRequestBehavior.AllowGet);
            }
            else if (Localidad != null)
            {
                string Value = db.vProvincia_Depto_Localidad.FirstOrDefault(m => m.IdLocalidad == Localidad).IdLocalidad.ToString();
                string Text = db.vProvincia_Depto_Localidad.FirstOrDefault(m => m.IdLocalidad == Localidad).CODIGO_POSTAL;

                return Json(new { Value, Text }, JsonRequestBehavior.AllowGet);
            };
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        //----------------------------------Estudios----------------------------------------------------------------------//

        [AuthorizacionPermiso("ListarRP")]
        public ActionResult Estudios(int ID_persona)
        {
            try
            {
                EstudiosVM estudio = new EstudiosVM
                {
                    vPersona_EstudioListVM = db.VPersona_Estudio.Where(m => m.IdPersona == ID_persona).ToList()
                };
                foreach (var item in estudio.vPersona_EstudioListVM)
                {
                    if (item.IdInstitutos == 0)
                    {
                        string[] paisyinst = item.NombreYPaisInstituto.Split('-');
                        item.Jurisdiccion = paisyinst[0];
                        item.Nombre = paisyinst[1];
                    }
                }
                return PartialView(estudio);
            }
            catch (Exception ex)
            {
                //revisar como mostrar error en la vista
                return PartialView(ex);
            }
        }

        [AuthorizacionPermiso("ListarRP")]
        public ActionResult EstudiosCUD(int? ID, int ID_persona)
        {
            try
            {
                EstudiosVM estudio = new EstudiosVM()
                {
                    NivelEstudioVM = db.NiveldEstudio.ToList(),
                    //cargo los datos para el combobox de provincia
                    Provincia = db.Institutos
                        .DistinctBy(m => m.Jurisdiccion)
                        .OrderBy(m => m.Jurisdiccion)
                        .Select(m => m.Jurisdiccion)
                        .ToList()
                };
                //verifico si envio un ID 
                //SI reciobio ID cargo los datos correspondiente
                //caso contrario envio un nuevo registro de Estudio
                if (ID != null)
                {
                    estudio.vPersona_EstudioIdVM = db.VPersona_Estudio.FirstOrDefault(m => m.IdEstudio == ID);

                    //Si IdInstituto existe se carga los datos relacionados con el, provincia localidad y nombre de institutos
                    //de NO existir se carga datos del campo "NombreYPaisInstituto"
                    if (estudio.vPersona_EstudioIdVM.IdInstitutos != 0)
                    {
                        estudio.Localidad = db.Institutos
                            .Where(m => m.Jurisdiccion == estudio.vPersona_EstudioIdVM.Jurisdiccion)
                            .DistinctBy(m => m.Localidad)
                            .OrderBy(m => m.Localidad)
                            .Select(m => m.Localidad)
                            .ToList();
                        estudio.InstitutoVM = db.Institutos
                            .Where(m => m.Localidad == estudio.vPersona_EstudioIdVM.Localidad)
                            .OrderBy(m => m.Nombre)
                            .Select(m => new SelectListItem
                            {
                                Value = m.Id.ToString(),
                                Text = m.Nombre
                            })
                            .ToList();
                        estudio.vPersona_EstudioIdVM.INST_EXT = false;
                        estudio.vPersona_EstudioIdVM.Nombre = "";
                    }
                    else
                    {
                        string[] paisinst = estudio.vPersona_EstudioIdVM.NombreYPaisInstituto.Split('-');
                        estudio.vPersona_EstudioIdVM.Jurisdiccion = paisinst[0];
                        estudio.vPersona_EstudioIdVM.Nombre = paisinst[1];
                        estudio.Localidad = new List<string>();
                        estudio.InstitutoVM = new List<SelectListItem>();
                        estudio.vPersona_EstudioIdVM.INST_EXT = true;
                    }
                }
                else
                {
                    VPersona_Estudio nuevoestu = new VPersona_Estudio()
                    {
                        IdPersona = ID_persona,
                        NombreYPaisInstituto = "-",
                        INST_EXT = false,
                        Completo = true,
                        CursandoUltimoAnio = true,
                        Localidad = "",
                        Jurisdiccion = ""

                    };

                    estudio.vPersona_EstudioIdVM = nuevoestu;
                    estudio.Localidad = new List<string>();
                    estudio.InstitutoVM = new List<SelectListItem>();
                };
                return PartialView(estudio);

            }
            catch (Exception ex)
            {
                //revisar como mostrar error en la vista
                return PartialView(ex);
            }
        }

        [HttpPost]
        [AuthorizacionPermiso("CreaEditaDatosP")]
        public ActionResult EstudiosCUD(EstudiosVM Datos)
        {
            if (Datos.vPersona_EstudioIdVM.IdInstitutos > 0)
            {
                ModelState["vPersona_EstudioIdVM.Jurisdiccion"].Errors.Clear();
            }
            else
            {
                ModelState["vPersona_EstudioIdVM.Localidad"].Errors.Clear();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var e = Datos.vPersona_EstudioIdVM;
                    if (e.IdInstitutos != 0)
                    {
                        e.NombreYPaisInstituto = null;
                    }
                    else
                    {
                        //e.IdInstitutos = 0;
                        e.NombreYPaisInstituto = e.Jurisdiccion + "-" + e.Nombre;
                    }
                    if (!e.Completo)
                    {
                        e.Promedio = null;
                        e.ultimoAnioCursado = null;
                    }
                    db.spEstudiosIU(e.IdEstudio, e.IdPersona, e.Titulo, e.Completo, e.IdNiveldEstudio, e.IdInstitutos, e.Promedio, e.CantidadMateriaAdeudadas, e.ultimoAnioCursado, e.NombreYPaisInstituto, e.CursandoUltimoAnio);

                    return Json(new { success = true, msg = "Se Inserto correctamente el  ESTUDIO" });
                }
                catch (Exception ex)
                {
                    //revisar como mostrar error en la vista
                    return Json(new { success = false, msg = ex.InnerException.Message });
                }
            }
            return Json(new { success = false, msg = "Error en el Modelo Recibido" });

        }

        [AuthorizacionPermiso("EliminarDatosP")]
        public JsonResult EliminaEST(int ID)
        {
            try
            {
                var estu = db.Estudio.Find(ID);
                if (estu != null)
                {
                    db.spEstudiosEliminar(ID);
                    //success: es true cundo la operacion es exitosa
                    //msg:mensjae que figurara en el modal
                    //form: se ejecutar un accion en un switch del script de la vista index
                    //url_Tabla: es el nombre de la accion y colocandole el subfijo NAV es el contenedor de la tabla actual
                    return Json(new { success = true, msg = "Se elimno correctamente el EStudio seleccionado", form = "Elimina", url_Tabla = "Estudios", url_Controller = "Postulante" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = false, msg = "Error: no existe el estudio con el id enviado", JsonRequestBehavior.AllowGet });
            }
            catch (Exception ex)
            {

                return Json(new { success = false, msg = ex.InnerException.Message, JsonRequestBehavior.AllowGet });
            }
        }


        public JsonResult DropCascadaEST(int opc, string val)
        {
            if (opc == 0)
            {
                var result = db.Institutos
                                 .Where(m => m.Jurisdiccion == val)
                                 .DistinctBy(m => m.Localidad)
                                 .Select(m => new SelectListItem
                                 {
                                     Value = m.Localidad,
                                     Text = m.Localidad
                                 })
                                 .OrderBy(m => m.Text)
                                 .ToList();


                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //recibo 2 parametroe en val uno pára privincia y otro es localidad
                string[] valpro = val.Split('-');
                string juri = valpro[0];
                string loca = valpro[1];
                var result = db.Institutos
                                 .Where(m => m.Jurisdiccion == juri)
                                 .Where(m => m.Localidad == loca)
                                 .Select(m => new SelectListItem
                                 {
                                     Value = m.Id.ToString(),
                                     Text = m.Nombre.Substring(0, 45)//ver para truncar esto aqui o en el cliente
                                 })
                                 .OrderBy(m => m.Text)
                                 .ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }

        }

        /*--------------------------------------------------------------IDIOMAS-------------------------------------------------------------------------------*/

        [AuthorizacionPermiso("ListarRP")]
        public ActionResult Idiomas(int ID_persona)
        {
            try
            {
                List<vPersona_Idioma> idioma;
                idioma = db.vPersona_Idioma.Where(m => m.IdPersona == ID_persona).ToList();

                return PartialView(idioma);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [AuthorizacionPermiso("ListarRP")]
        public ActionResult IdiomaCUD(int? ID, int? ID_persona)
        {
            try
            {
                IdiomasVM idioma = new IdiomasVM()
                {
                    NivelIdiomaVM = db.NivelIdioma.ToList(),
                    Sp_VIdiomas_VM = db.sp_vIdiomas("").ToList()
                };

                if (ID != null)
                {
                    idioma.VPersona_IdiomaIdVM = db.vPersona_Idioma.FirstOrDefault(m => m.IdPersonaIdioma == ID);
                }
                else
                {
                    idioma.VPersona_IdiomaIdVM = new vPersona_Idioma()
                    {
                        IdPersonaIdioma = 0,
                        IdPersona = ID_persona
                    };
                }

                return PartialView(idioma);

            }
            catch (Exception ex)
            {
                //revisar como mostrar error en la vista
                return PartialView(ex);
            }
        }


        [HttpPost]
        [AuthorizacionPermiso("CreaEditaDatosP")]
        public JsonResult IdiomaCUD(IdiomasVM datos)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    vPersona_Idioma i = datos.VPersona_IdiomaIdVM;
                    db.spIdiomasIU(i.IdPersonaIdioma, i.IdPersona, i.CodIdioma, i.Habla, i.Lee, i.Escribe);
                    return Json(new { success = true, msg = "Se Inserto correctamente el Idioma nuevo o se modifico IDIOMA" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {

                    return Json(new { success = true, msg = ex.InnerException.Message }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { success = true, msg = "El modelo recibido NO ES VALIDO" }, JsonRequestBehavior.AllowGet);
        }

        [AuthorizacionPermiso("EliminarDatosP")]
        public JsonResult EliminaIDIO(int ID)
        {
            try
            {
                var regidioma = db.PersonaIdioma.FirstOrDefault(m => m.IdPersonaIdioma == ID);
                db.PersonaIdioma.Remove(regidioma);
                db.SaveChanges();
                return Json(new { success = true, msg = "Se le elimino correctamente el idioma seleccionado", form = "Elimina", url_Tabla = "Idiomas", url_Controller = "Postulante" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(new { success = false, msg = ex.InnerException.Message });
            }
        }

        /*--------------------------------------------------------------ACTIVIDAD MILITAR-------------------------------------------------------------------------------*/

        [AuthorizacionPermiso("ListarRP")]
        public ActionResult ActMilitar(int ID_persona)
        {
            try
            {
                List<vPersona_ActividadMilitar> LISTactividad;
                LISTactividad = db.vPersona_ActividadMilitar.Where(m => m.IdPersona == ID_persona).ToList();

                return PartialView(LISTactividad);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [AuthorizacionPermiso("ListarRP")]
        public ActionResult ActMilitarCUD(int? ID, int ID_persona)
        {
            try
            {
                ActividadMIlitarVM actividad = new ActividadMIlitarVM()
                {
                    FuerzasVM = db.Fuerza.ToList(),
                    IDPErsona = ID_persona,
                    BajaVM = db.Baja.ToList(),
                    SituacionRevistaVM = db.SituacionRevista.ToList()
                };
                if (db.Postulante.FirstOrDefault(m => m.IdPersona == ID_persona) != null)
                {
                    actividad.SituacionRevistaVM.Remove(db.SituacionRevista.First(m => m.SituacionRevista1 == "Retirado"));
                };
                if (ID != null)
                {
                    actividad.ACTMilitarIDVM = db.ActividadMilitar.FirstOrDefault(m => m.IdActividadMilitar == ID);
                }
                else
                {
                    actividad.ACTMilitarIDVM = new ActividadMilitar()
                    {
                        IdActividadMilitar = 0,
                    };
                }
                return PartialView(actividad);
            }
            catch (Exception)
            {

                throw;
            }
        }


        [HttpPost]
        [AuthorizacionPermiso("CreaEditaDatosP")]
        public JsonResult ActMilitarCUD(ActividadMIlitarVM datos)
        {

            try
            {
                var a = datos.ACTMilitarIDVM;
                if (a.Ingreso == true)
                {
                    a.CausaMotivoNoingreso = null;
                }
                else
                {
                    a.FechaBaja = null;
                    a.FechaIngreso = null;
                    a.MotivoBaja = null;
                    a.Jerarquia = null;
                    a.Cargo = null;
                    a.Destino = null;
                    a.IdSituacionRevista = 0;
                    a.IdBaja = 0;
                }

                db.spActividadMilitarIU(a.IdActividadMilitar, datos.IDPErsona, a.Ingreso, a.FechaIngreso, a.FechaBaja, a.CausaMotivoNoingreso, a.MotivoBaja, a.Jerarquia, a.Cargo, a.Destino, a.IdSituacionRevista, a.IdFuerza, a.IdBaja);
                return Json(new { success = true, msg = "Se inserto o actualizo correctamente ACTMilitar" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = ex.InnerException.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        [AuthorizacionPermiso("EliminarDatosP")]
        public JsonResult EliminaACT(int ID)
        {
            try
            {
                db.spActividadMilitarEliminar(ID);
                return Json(new { success = false, msg = "se elimino correctamente el registro", form = "Elimina", url_Tabla = "ActMilitar", url_Controller = "Postulante" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = true, msg = ex.InnerException.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /*--------------------------------------------------------------SITUACION OCUPACIONAL-------------------------------------------------------------------------------*/


        [AuthorizacionPermiso("ListarRP")]
        public ActionResult SituOcupacional(int ID_persona)
        {
            try
            {

                SituacionOcupacionalVM SituOcu = new SituacionOcupacionalVM
                {
                    EstadoDescripcionVM = new SelectList(db.EstadoOcupacional.Where(m => m.Descripcion != "Jubilado" & m.Descripcion != "Retirado").ToList(), "Id", "Descripcion", "EstadoOcupacional1", 1)
                };

                List<string> InteresesSeleccionados = db.Persona.Find(ID_persona).Interes.Select(m => m.DescInteres).ToList();
                SituOcu.InteresesVM = db.Interes.Select(c => new SelectListItem { Text = c.DescInteres, Value = c.IdInteres.ToString(), Selected = InteresesSeleccionados.Contains(c.DescInteres) }).ToList();

                var situ = db.vPersona_SituacionOcupacional.FirstOrDefault(m => m.IdPersona == ID_persona);
                if (situ == null)
                {
                    SituOcu.VPersona_SituacionOcupacionalVM = new vPersona_SituacionOcupacional()
                    {
                        IdSituacionOcupacional = 0,
                        IdPersona = ID_persona,
                    };
                }
                else
                {
                    SituOcu.VPersona_SituacionOcupacionalVM = situ;
                };

                return PartialView(SituOcu);
            }
            catch (Exception ex)
            {
                return PartialView(ex);
            }
        }


        [HttpPost]
        [AuthorizacionPermiso("CreaEditaDatosP")]
        public ActionResult SituOcupacional(SituacionOcupacionalVM situ)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var s = situ.VPersona_SituacionOcupacionalVM;
                    db.spSituacionOcupacionalIU(s.IdSituacionOcupacional, s.IdPersona, s.IdEstadoOcupacional, s.OcupacionActual, s.Oficio, s.AniosTrabajados, s.DomicilioLaboral);

                    Interes ins = new Interes();
                    var per = db.Persona.Find(s.IdPersona).Interes;
                    per.Clear();
                    if (situ.IdInteres != null)
                    {
                        foreach (var item in situ.IdInteres)
                        {
                            ins = db.Interes.Find(Double.Parse(item));
                            per.Add(ins);
                        };
                    };
                    db.SaveChanges();
                    return Json(new { success = true, msg = "exito en guardar la situacion ocupacional" });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, msg = ex.InnerException.Message });
                }
            }
            return Json(new { success = false, msg = "Error en el MOdelo" });

        }

        /*--------------------------------------------------------------Antropometria------------------------------------------------------------------------------*/

        [AuthorizacionPermiso("ListarRP")]
        public ActionResult Antropometria(int ID_persona)
        {
            vPersona_Antropometria antropo = db.vPersona_Antropometria.FirstOrDefault(m => m.IdPersona == ID_persona);
            return PartialView(antropo);
        }


        [HttpPost]
        [AuthorizacionPermiso("CreaEditaDatosP")]
        public ActionResult Antropometria(vPersona_Antropometria a)
        {
            try
            {
                a.LargoFalda ??= 0;
                db.spAntropometriaIU(a.IdPersona, a.Altura, a.Peso, a.IMC, a.PerimCabeza, a.PerimTorax, a.PerimCintura, a.PerimCaderas, a.LargoPantalon, a.LargoEntrep, a.LargoFalda, a.Cuello, a.Calzado);
                return Json(new { success = true, msg = "Se guardaron los DATOS exitosamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = ex.InnerException.Message });
            }

        }

        public JsonResult VerificaAltIcm(int IdPostulante, string AltIcm, float num)
        {
            try
            {
                var p = db.Persona.FirstOrDefault(m => m.IdPersona == IdPostulante);
                var FechaNac = p.FechaNacimiento;
                object sexo = p.IdSexo;
                var inscrip = db.vInscripcionDetalle.FirstOrDefault(m => m.IdPersona == IdPostulante);
                string Carrera = inscrip.CarreraRelacionada;
                sexo = (Carrera == "Médicos") ? "Medico" : sexo;
                string PopUp = "";
                var Restric = db.spRestriccionesParaEstePostulante(IdPostulante, FechaNac, p.Postulante.Inscripcion.ToList()[0].IdPreferencia).ToList()[0];
                string Aplica = "";
                switch (AltIcm) 
                {
                    case "altura":
                        PopUp = db.Configuracion.First(m => m.NombreDato == "PopUpAltura").ValorDato;
                        switch (sexo)
                        {
                            case 1:
                                Aplica = (Restric.AlturaMinM > num) ? "NO" : "SI";
                                break;
                            case 4:
                                Aplica = (Restric.AlturaMinF > num) ? "NO" : "SI";
                                break;
                            case "Medico":
                                Aplica = "SI";
                                break;
                        }
                        break;
                    case "imc":
                        Aplica = (Restric.IMC_max < num || Restric.IMC_min > num) ? "NO" : "SI";
                        PopUp = db.Configuracion.First(m => m.NombreDato == "PopUpICM").ValorDato;
                        break;
                    default:
                        break;
                }
                return Json(new { APLICA = Aplica, POPUP = PopUp, ALTIMC = AltIcm }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {

                throw;
            }
        }

        /*--------------------------------------------------------------FAMILIA------------------------------------------------------------------------------*/


        [AuthorizacionPermiso("ListarRP")]
        public ActionResult Familia(int ID_persona)
        {
            try
            {
                //List<int> id_PER_FAMI = db.Familiares.Where(m => m.IdPostulantePersona == ID_persona).Select(m => m.IdPersona).ToList();
                List<sp_vPersona_Familiar_Result> FAMILIARES = db.sp_vPersona_Familiar(ID_persona).ToList();

                return PartialView(FAMILIARES);
            }
            catch (Exception)
            {
                return View();
            }
        }

        [AuthorizacionPermiso("ListarRP")]
        //recibo el idFamilia, si es 0 creo  una personaFamilia y su relacion.
        public ActionResult FamiliaCUD(int idPersonaFamilia, int idPostulante)
        {
            Session["FamiTable"] = true;
            //verificar que al crear un postulante llenar con los datos completos, si no es asi el familiar no se mostrara en vPersona_Familiar
            //viewmodel creado para la creacion de un familiar
            //cargo los datos necesarios para los combobox
            PersonaFamiliaVM pers = new PersonaFamiliaVM
            {
                vParentecoVM = db.vParentesco.Select(m => new SelectListItem { Value = m.idParentesco.ToString(), Text = m.Relacion }).ToList(),
                SexoVM = db.Sexo.Select(m => new SelectListItem { Value = m.IdSexo.ToString(), Text = m.Descripcion }).ToList(),
                vEstCivilVM = db.vEstCivil.Select(m => new SelectListItem { Value = m.Codigo_n, Text = m.Descripcion }).ToList(),
                ReligionVM = db.vRELIGION.Select(m => new SelectListItem { Value = m.CODIGO, Text = m.DESCRIPCION }).ToList(),
                TipoDeNacionalidadVm = db.TipoNacionalidad.Select(m => new SelectListItem { Value = m.IdTipoNacionalidad.ToString(), Text = m.Descripcion }).ToList()
            };
            if (idPersonaFamilia != 0)
            {
                string mailLogin = HttpContext.User.Identity.Name.ToString();
                var EsPostulante = db.Postulante.FirstOrDefault(m => m.IdAspNetUser == db.AspNetUsers.FirstOrDefault(m => m.UserName == mailLogin).Id);
                if (EsPostulante != null)
                {
                    var EtapaTabs = db.vPostulanteEtapaEstado.Where(id => id.IdPostulantePersona == idPostulante).OrderBy(m => m.IdEtapa).DistinctBy(id => id.IdEtapa).Select(id => id.IdEtapa).ToList();
                    EtapaTabs.ForEach(m => pers.IDETAPA += m + ",");
                    //le coloco 5 por si la pantalla esta cerrada
                    if (!(bool)db.spTildarPantallaParaPostulate(idPostulante).FirstOrDefault(m => m.IdPantalla == 9).Abierta) pers.IDETAPA += "5,";
                }
                else
                {
                    pers.IDETAPA = "5,0";
                };

                pers.ID_PER = idPersonaFamilia;
                pers.vPersona_FamiliarVM = db.vPersona_Familiar.FirstOrDefault(m => m.IdPersonaFamiliar == idPersonaFamilia);
                var p = db.Postulante.FirstOrDefault(m => m.IdPersona == pers.ID_PER);
                //verifico si la persona familiar es postulante
                if (p != null)
                {
                    //ver... verifico que sea un postulante que no se este en proceso de inscripcion en el año actual.
                    pers.postulante = (p.FechaRegistro.Date.Year == DateTime.Now.Year);
                }
                ViewBag.ValidacionEnCurso = db.InscripcionEtapaEstado.OrderByDescending(m => m.Fecha).Where(m => m.IdInscripcionEtapaEstado == db.Inscripcion.FirstOrDefault(m => m.IdPostulantePersona == idPostulante).IdInscripcion).Select(m => m.IdSecuencia).ToList()[0] == 14;
                //en caso de ser delegacion modifico el valor de ValdacionENcusrso a true para bloquear las vistas, si es postulante dejor el anterior valor
                ViewBag.ValidacionEnCurso = (db.Postulante.FirstOrDefault(m => m.IdAspNetUser == db.AspNetUsers.FirstOrDefault(n => n.UserName == HttpContext.User.Identity.Name).Id) != null) ?  ViewBag.ValidacionEnCurso :  true;
            }
            else
            {
                pers.IDETAPA = "0";
                pers.ID_PER = idPersonaFamilia;
                var IdAspUser = db.AspNetUsers.FirstOrDefault(e => e.Email == User.Identity.Name).Id;
                pers.vPersona_FamiliarVM = new vPersona_Familiar();
                pers.vPersona_FamiliarVM.IdPersonaPostulante = db.Postulante.FirstOrDefault(e => e.IdAspNetUser == IdAspUser).IdPersona;
                pers.vPersona_FamiliarVM.IdFamiliar = 0;
                pers.vPersona_FamiliarVM.IdPersonaFamiliar = 0;
            }

            return View(pers);
        }


        [HttpPost]
        [AuthorizacionPermiso("CreaEditaDatosP")]
        public JsonResult FamiliaCUD(SINU.ViewModels.PersonaFamiliaVM fami)
        {


            if (ModelState.IsValid)
            {
                var datos = fami.vPersona_FamiliarVM;
                int? idpersonafamiliar = 0;
                try
                {
                    var per = db.Persona.FirstOrDefault(d => d.DNI == datos.DNI);
                    var IDPOSTULANTE = datos.IdPersonaPostulante;
                    Familiares rela = null;
                    string msgs;

                    if (per != null)
                    {
                        rela = db.Persona.Find(IDPOSTULANTE).Postulante.Familiares.FirstOrDefault(m => m.IdPersona == per.IdPersona);
                        msgs = (rela != null) ? "Modificacion del Familiar Exitoso." : "Se agrego un familiar exitosamente. Refrescando Vista...";
                    }
                    else
                    {
                        msgs = "Creacion del Familiar Exitoso. Refrescando Vista...";
                    };

                    if ((per != null && rela != null) || per == null)
                    {
                        datos.IdReligion ??= "";
                        db.spPERSONAFamiliarIU(datos.IdPersonaFamiliar, datos.IdPersonaPostulante, datos.Mail, datos.Apellido, datos.Nombres, datos.IdSexo, datos.FechaNacimiento, datos.DNI, datos.CUIL,
                        datos.IdReligion, datos.IdEstadoCivil, datos.FechaCasamiento, datos.Telefono, datos.Celular, datos.Mail, datos.idTipoNacionalidad, 0, datos.idParentesco, datos.Vive, datos.ConVive);
                        idpersonafamiliar = (per == null) ? db.vPersona_Familiar.FirstOrDefault(d => d.DNI == datos.DNI).IdPersonaFamiliar : 0;

                    }
                    else if (rela == null)
                    {
                        db.spRelacionFamiliarIU(0, datos.IdPersonaPostulante, per.IdPersona, datos.idParentesco, datos.Vive, datos.ConVive);
                        datos.IdPersonaFamiliar = per.IdPersona;
                        idpersonafamiliar = db.vPersona_Familiar.FirstOrDefault(d => d.DNI == datos.DNI.ToString()).IdPersonaFamiliar;

                    }
                    return Json(new { success = true, msg = msgs, IDperFAMI = idpersonafamiliar, IDperPOST = IDPOSTULANTE }, JsonRequestBehavior.AllowGet);

                }
                catch (Exception ex)
                {
                    return Json(new { success = false, msg = ex.InnerException.Message }, JsonRequestBehavior.AllowGet);

                }
            }
            return Json(new { success = false, msg = "Modelo no valido!!!" }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult VerificarDNI(int DNI, int ID)
        {
            try
            {
                Persona per = db.Persona.FirstOrDefault(m => m.DNI == DNI.ToString());

                if (per != null)
                {
                    int Id_Persona = per.IdPersona;
                    Familiares rela = db.Persona.Find(ID).Postulante.Familiares.FirstOrDefault(m => m.IdPersona == Id_Persona);
                    //si ya tiene una relacion conel postulante la persona a agragr como familiar lo notifico
                    string msgs, resps;

                    msgs = (rela != null) ? string.Format("La persona con Dni: {0}, ya esta cargado como familiar. Redirigiendo...", DNI) : string.Format("La persona con Dni: {0} que desea agregar como familiar ya existe, ¿Desea agregarlo?", DNI);
                    resps = (rela != null) ? "son_familiares" : "existe";

                    return Json(new { resp = resps, msg = msgs, ID_PER = Id_Persona, ID_perPOST = ID }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { resp = "no_existe" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { error = false, msg = ex.InnerException.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [AuthorizacionPermiso("EliminarDatosP")]
        public JsonResult EliminaFAMI(int ID_per, int ID_fami)
        {
            try
            {

                int filas = db.Familiares.Where(p => p.IdPersona == ID_per).Count();
                bool postulante = db.Postulante.Find(ID_per) != null;
                if (postulante || filas > 1)
                {
                    db.spRelacionFamiliarEliminar(ID_fami);
                    return Json(new { success = true, msg = "Se elimino correctamente el Familiar", form = "Elimina", url_Tabla = "Familia", url_Controller = "Postulante" }, JsonRequestBehavior.AllowGet);
                }

                db.spFamiliarEliminar(ID_fami, ID_per);
                return Json(new { success = true, msg = "Se elimino correctamente el Familiar", form = "Elimina", url_Tabla = "Familia", url_Controller = "Postulante" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = ex.InnerException.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        /*-------------------------------------------------------------Documentacion------------------------------------------------------------------------------*/

        public ActionResult DocumentacionAnexo()
        {
            DocuAnexoVM asd = new DocuAnexoVM()
            {
                IdPersona = 1369
            };

            return PartialView("DocumentacionAnexo", asd);
        }


        /*--------------------------------------------------------------VALIDAR DATOS------------------------------------------------------------------------------*/
        [HttpPost]
        [AuthorizacionPermiso("ModificarSecuenciaP")]        //ver paso al postulante a la secuencia "Documentacion - A Validar"
        public JsonResult ValidarDatos(int ID_persona)
        {
            try
            {
                var persona = db.vPersona_DatosPer.FirstOrDefault(m => m.IdPersona == ID_persona);
                var antropo = db.Antropometria.FirstOrDefault(m => m.IdPostulantePersona == ID_persona);
                DataProblemaEncontrado problema = new DataProblemaEncontrado { IdPostulantePersona = persona.IdPersona };
                var problemasPostu = db.DataProblemaEncontrado.Where(m => m.IdPostulantePersona == ID_persona);
                if (antropo != null)
                {
                    //verificacion de la altura si valida o no en caso de no ser se genera un registro de error para ser revisado por la Delegacion
                    var APLICAAltura = VerificaAltIcm(ID_persona, "altura", antropo.Altura).Data.ToString().Split(',')[0].ToString().Split('=')[1].Trim();
                    if (APLICAAltura == "NO" && problemasPostu.FirstOrDefault(m => m.IdDataVerificacion == 48) == null)
                    {

                        problema.Comentario = db.DataVerificacion.First(m => m.IdDataVerificacion == 48).Descripcion;
                        problema.IdDataVerificacion = 48;
                          
                        db.DataProblemaEncontrado.Add(problema);

                    };
                    //verificacion de la altura si valida o no en caso de no ser se genera un registro de error para ser revisado por la Delegacion
                    var APLICAImc = VerificaAltIcm(ID_persona, "imc", (float)antropo.IMC).Data.ToString().Split(',')[0].ToString().Split('=')[1].Trim();
                    if (APLICAImc == "NO" && problemasPostu.FirstOrDefault(m => m.IdDataVerificacion == 49) == null)
                    {
                        problema.Comentario = db.DataVerificacion.First(m => m.IdDataVerificacion == 49).Descripcion;
                        problema.IdDataVerificacion = 49;
                         
                        db.DataProblemaEncontrado.Add(problema);
                    };
                };
                var restriccionesEstadoCivil = db.spRestriccionesParaEstePostulante(persona.IdPersona, persona.FechaNacimiento,db.Inscripcion.FirstOrDefault(m=>m.IdInscripcion==persona.IdInscripcion).IdPreferencia).ToList()[0];
                if (persona.IdModalidad!=null)
                {
                    //Verifico el estado civil y el tipo de nacionalidad
                    //verifico tipo de nacionalidad en caso de ser "Argentino por Opcion" y tenga modalidad distinta a "SMV", agrego un problema en DataProblemaEncontrado
                    if (persona.idTipoNacionalidad == 3 && persona.IdModalidad != "SMV" && problemasPostu.FirstOrDefault(m => m.IdDataVerificacion == 51) == null)
                    {
                        problema.IdDataVerificacion = 51;
                        problema.Comentario = "Verificar que al menos uno de los padres tenga tipo de nacionalidad NATIVO.";

                        db.DataProblemaEncontrado.Add(problema);
                    };
                    //verifico si cumple con la restrccion de Estado Civil para la modalidad que corresponde
                    if (restriccionesEstadoCivil.IdEstadoCivil != persona.IdEstadoCivil && restriccionesEstadoCivil.IdEstadoCivil != "" && problemasPostu.FirstOrDefault(m => m.IdDataVerificacion == 50) == null)
                    {
                        problema.IdDataVerificacion = 50;
                        problema.Comentario = "Restrccion que causa Interrupcion de Proceso de Inscripcion";

                        db.DataProblemaEncontrado.Add(problema);
                    };

                }
                db.SaveChanges();
                //Envio de Mail para notificar a la delegacion correpondiente
                int ID_Delegacion = (int)db.Inscripcion.FirstOrDefault(m => m.IdPostulantePersona == ID_persona).IdDelegacionOficinaIngresoInscribio;
                int ID_INSCRIP = db.Inscripcion.FirstOrDefault(m => m.IdPostulantePersona == ID_persona).IdInscripcion;
                var grupoDelegacion = db.vUsuariosAdministrativos.Where(m => m.IdOficinasYDelegaciones == ID_Delegacion).ToList();
                var per = db.Persona.FirstOrDefault(m => m.IdPersona == ID_persona);
                ValidoCorreoPostulante datosMail = new ValidoCorreoPostulante();
                foreach (var dele in grupoDelegacion)
                {
                    var idAsp_delegacion = db.AspNetUsers.FirstOrDefault(m => m.Email == dele.Email).Id;
                    //armo modelo para armar el correo
                    datosMail.Apellido = dele.Apellido;
                    datosMail.Apellido_P = per.Apellido;
                    datosMail.Dni_P = per.DNI;
                    datosMail.IdInscripcion_P = ID_INSCRIP;
                    datosMail.Nombre_P = per.Nombres;
                    datosMail.url = Url.Action("Documentacion", "Delegacion", new { id = ID_persona }, protocol: Request.Url.Scheme);

                    Func.EnvioDeMail(datosMail, "PlantillaInicioValidacionParaDelegacion", idAsp_delegacion, null, "MailAsunto7");
                };

                //ver esto solo disponible si se encuntra en la secuencia 13 "inicio De Carga/DOCUMENTACION"
                db.spProximaSecuenciaEtapaEstado(ID_persona, 0, false, 14, "", "");
                return Json(new { success = true, msg = "Operacion Exitosa", form = "ValidarDatos" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            { 
                return Json(new { success = false, msg = ex.InnerException.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ProblemasPantalla(int IDPostulante, int IdPantalla)
        {
            try
            {
                return PartialView(db.vDataProblemaEncontrado.Where(p => p.IdPostulantePersona == IDPostulante).Where(m => m.IdPantalla == IdPantalla).ToList());
            }
            catch (Exception)
            {

                throw;
            }
        }

        /*--------------------------------------------------------------PRESENTACION------------------------------------------------------------------------------*/
        [AuthorizacionPermiso("ListarRP")]
        public ActionResult Presentacion(int ID_persona)
        {
            var per = db.Persona.Find(ID_persona);
            Presentacion prese = new Presentacion
            {
                IdPersona = per.IdPersona,
                Apellido = per.Apellido,
                Nombre = per.Nombres

            };
            ViewBag.Asignado = true;
            var FechaPrese = db.Inscripcion.FirstOrDefault(m => m.IdPostulantePersona == ID_persona).FechaRindeExamen;
            if (FechaPrese == null)
            {
                ViewBag.Asignado = false;
            }
            else
            {
                prese.FechaPresentacion = (DateTime)FechaPrese;
                string url = HttpContext.Request.Url.Host  + Url.Action("Index","Postulante",new { ID_Postulante = ID_persona });
                ViewBag.QRCodeImage = generarQR(url);
                ViewBag.QRCodeImageLink = url;
            };
            return PartialView(prese);
        }


        private byte[] generarQR(string texto)
        {   //https://github.com/codebude/QRCoder ver documentacion ejemplo de usos ej logo en el qr entre muchas otras cosas
            //establesco la ubicacion del logo que aparecera em el codigo QR
            string ubicacion = AppDomain.CurrentDomain.BaseDirectory;
            string ubicacionImagen = $"{ubicacion}Imagenes\\AnclaQR.png";

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(texto, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20, ColorTranslator.FromHtml("#212429"), System.Drawing.Color.White, (Bitmap)Bitmap.FromFile(ubicacionImagen), 25, 10);
            using (MemoryStream stream = new MemoryStream())
            {
                qrCodeImage.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }

        }

    }
}
