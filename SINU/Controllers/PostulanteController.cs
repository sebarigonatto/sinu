using Microsoft.Ajax.Utilities;
using QRCoder;
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

namespace SINU.Controllers
{
    [Authorize]
    public class PostulanteController : Controller
    {
        SINUEntities db = new SINUEntities();

        //ESTABLESCO LA VARIABLE MAIL DEL USUARIO QUE ESTA ACTUALMENTE LOGUEADO
        //ESTA CARIABLE ES UTILIZADA PARA BUSCAR EN LAS DISTINTAS VISTAS, UTILIZADAS EN EL CONTROLADOR, PARA  BUSCAR LOS REGISTROS DEL USUARIO LOGUEADO
        //private int ID_persona;
        //=> db.Persona.FirstOrDefault(m => m.Email == HttpContext.User.Identity.Name.ToString()).IdPersona;
        //private int ID_fami;
        //----------------------------------PAGINA PRINCIPAL----------------------------------------------------------------------//

        public ActionResult Index()
        {//error cdo existe uno registrado antes de los cambios de secuencia
            try
            {
                IDPersonaVM pers = new IDPersonaVM
                {
                    ID_PER = db.Persona.FirstOrDefault(m => m.Email == HttpContext.User.Identity.Name.ToString()).IdPersona,
                };
                pers.EtapaTabs = db.vPostulanteEtapaEstado.Where(id => id.IdPostulantePersona == pers.ID_PER).OrderBy(m=>m.IdEtapa).DistinctBy(id => id.IdEtapa).Select(id => id.IdEtapa).ToList();
                pers.EtapaTabs.ForEach(m => pers.IDETAPA += m + ",");
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
        //mejorar la seguridad 
        public ActionResult DatosBasicos(int ID_persona)
        {
            try
            {
                //se carga los datos basicos del usuario actual y los utilizados para los dropboxlist
                DatosBasicosVM datosba = new DatosBasicosVM()
                {
                    SexoVM = db.Sexo.Where(m=>m.IdSexo!=4).ToList(),
                    vPeriodosInscripsVM = db.vPeriodosInscrip.ToList(),
                    OficinasYDelegacionesVM = db.OficinasYDelegaciones.ToList(),
                    vPersona_DatosBasicosVM = db.vPersona_DatosBasicos.FirstOrDefault(b => b.IdPersona == ID_persona),
                    ComoSeEnteroVM = db.ComoSeEntero.Where(n=>n.IdComoSeEntero!=1).ToList()
                };
                datosba.vPersona_DatosBasicosVM.IdComoSeEntero = 0;
                //var Com = new[] { new SelectListItem { Value = "1", Text="Familiar en la Institucion" },
                //                 new SelectListItem { Value = "2", Text="En tu escuela, por parte de personal de la Armada" },
                //                 new SelectListItem { Value = "3", Text="TV, ¿Cual?" },
                //                 new SelectListItem { Value = "4", Text="Radio, ¿Cual?" },
                //                 new SelectListItem { Value = "5", Text="Periodicos / Revistas, ¿Cual?" },
                //                 new SelectListItem { Value = "6", Text="Redes Sociales, ¿Cual?" },
                //                 new SelectListItem { Value = "7", Text="Otros" },
                //};
                // Com.ForEach(m => datosba.ComoSeEntero.Add(m));

                return PartialView(datosba);
            }
            catch (Exception ex)
            {
                //revisar como mostrar error en la vista
                return PartialView(ex);
            }

        }

        //ACCION QUE GUARDA LOS DATOS INGRESADOS EN LA VISTA "DATOS BASICOS"
        [HttpPost]
        public ActionResult DatosBasicos(DatosBasicosVM Datos)
        {
            if (Datos.vPersona_DatosBasicosVM.ComoSeEntero== null)
            {
                    ModelState["vPersona_DatosBasicosVM.ComoSeEntero"].Errors.Clear();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    //se guarda los datos de las persona devueltos
                    var p = Datos.vPersona_DatosBasicosVM;
                   

                    var result = db.spDatosBasicosUpdate(p.Apellido, p.Nombres, p.IdSexo, p.DNI, p.Telefono, p.Celular,p.FechaNacimiento, p.Email, p.IdDelegacionOficinaIngresoInscribio, p.ComoSeEntero,p.IdComoSeEntero, p.IdPreferencia, p.IdPersona, p.IdPostulante);
                    
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

        //DEVUELVE TRUE SI LA EDAD ES COHERENTE Y FALSE SI NO.
        public JsonResult EdadInstituto(int? IDinst ,int? edad ) {

            if (IDinst == 9 & edad > 22)
            {
                return Json(new { coherencia = false },JsonRequestBehavior.AllowGet);
            } else if (IDinst == 10 & edad > 24) {
                return Json(new { coherencia = false }, JsonRequestBehavior.AllowGet);
            };

            return Json(new { coherencia = true }, JsonRequestBehavior.AllowGet);
        }

        /*--------------------------------------------------------------SOLICITUD DE ENTREVISTA------------------------------------------------------------------------------*/

        public async Task<JsonResult> SolicitudEntrevistaAsync(int ID_persona)
        {
            try
            {
                var p = db.vPersona_DatosBasicos.First(m=>m.IdPersona == ID_persona);
                //llamo a la JsonResult para ferificar la restriccion de edad de acuerdo con el instituto
                JsonResult GRUPO = new PostulanteController().EdadInstituto(p.IdPreferencia, p.Edad);
                dynamic data = GRUPO.Data;
                if (data.coherencia)
                {
                    //Datos basicos - Validado; ID= 7
                    db.spProximaSecuenciaEtapaEstado(p.IdPersona, 0, false, 0, "DATOS BASICOS", "Validado");
                }
                else
                {
                    //Datos basicos - No Validado; ID= 21
                    db.spProximaSecuenciaEtapaEstado(p.IdPersona, 0, false, 0, "DATOS BASICOS", "No Validado");
                };
                await Task.Delay(1000);

                db.spProximaSecuenciaEtapaEstado(ID_persona, 0, false, 0, "ENTREVISTA", "A Asignar");

                return Json(new { success = true, msg = "La Solicitud de Entrevista fue exitosa, se le informara via CORREO la fecha ASIGNADA.", form = "solicitudentrevista" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = ex.InnerException.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        //----------------------------------ENTREVISTA----------------------------------------------------------------------//

        public ActionResult Entrevista(int ID_persona)
        {
            
            try
            {
                vEntrevistaLugarFecha entrevistafh = new vEntrevistaLugarFecha();
                entrevistafh = db.vEntrevistaLugarFecha.FirstOrDefault(m => m.IdPersona == ID_persona);
                if (entrevistafh.FechaEntrevista == null)
                {
                    ViewBag.NoAsignado = true;
                }
                else {
                    entrevistafh.FechaEntrevista = entrevistafh.FechaEntrevista;
                    ViewBag.NoAsignado = false; }
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

        public ActionResult DatosPersonales(int ID_persona)
            {
            try
            {
                int idInscripcion = db.Inscripcion.FirstOrDefault(m => m.IdPostulantePersona == ID_persona).IdInscripcion;

                DatosPersonalesVM datosba = new DatosPersonalesVM()
                {
                    vPersona_DatosPerVM = db.vPersona_DatosPer.FirstOrDefault(m => m.IdPersona == ID_persona),
                    TipoNacionalidadVM = db.TipoNacionalidad.Where(m=>m.IdTipoNacionalidad !=4 ).ToList(),
                    vEstCivilVM = db.vEstCivil.ToList(),
                    vRELIGIONVM = db.vRELIGION.ToList(),
                    CarreraOficioVm = db.spCarrerasParaEsteInscripto(idInscripcion).Select(m=>new  SelectListItem() { Text = m.CarreraUoficio, Value= m.IdCarreraOficio.ToString()}).ToList(),
                    ModalidadVm = db.spModalidadParaEsteInscripto(idInscripcion).Select(m => new SelectListItem() { Text = m.Descripcion, Value = m.IdModalidad }).ToList()
                };
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
                    var result = db.spDatosPersonalesUpdate(p.IdPersona,p.IdInscripcion, p.CUIL, p.FechaNacimiento, p.IdEstadoCivil, p.IdReligion, p.idTipoNacionalidad,p.IdModalidad,p.IdCarreraOficio);
                    return Json(new { success = true, msg = "se guardaron con exito los DATOS PERSONALES" });
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
        public class MemoryPostedFile : HttpPostedFileBase
        {
            private readonly byte[] fileBytes;

            public MemoryPostedFile(byte[] fileBytes, string fileName = null)
            {
                this.fileBytes = fileBytes;
                this.FileName = fileName;
                this.InputStream = new MemoryStream(fileBytes);
            }

            public override int ContentLength => fileBytes.Length;

            public override string FileName { get; }

            public override Stream InputStream { get; }
        }
        public ActionResult DocuPenal(int ID_persona)
        {
            try
            {
                DocuPenalVM d = new DocuPenalVM() {
                    IdPersona=ID_persona

                };
           
                return PartialView(d);
            }
            catch (Exception ex)
            {

                throw;
            }
           
        }
        [HttpPost]
        public JsonResult DocuPenal(DocuPenalVM data)
        {
            try
            {
                if (ModelState.IsValid)
                {  
                    string ubicacion = AppDomain.CurrentDomain.BaseDirectory;
                    string CarpetaDeGuardado = $"{ubicacion}Documentacion\\ArchivosDocuPenal\\";

                    //guardio certificado
                    string NombreArchivo = data.IdPersona + "_Certificado"; //Path.GetFileNameWithoutExtension(data.ConstanciaAntcPenales.FileName);
                    string ExtencioArchivo = Path.GetExtension(data.ConstanciaAntcPenales.FileName);
                    string guarda = CarpetaDeGuardado + NombreArchivo + ExtencioArchivo; 
                    data.ConstanciaAntcPenales.SaveAs(guarda);

                    //guardio anexo2
                    NombreArchivo = data.IdPersona + "_Anexo2"; //Path.GetFileNameWithoutExtension(data.ConstanciaAntcPenales.FileName);
                    ExtencioArchivo = Path.GetExtension(data.FormularioAanexo2.FileName);
                    guarda = CarpetaDeGuardado + NombreArchivo + ExtencioArchivo;
                    data.ConstanciaAntcPenales.SaveAs(guarda);

                    
              


                    return Json(new { });
                }
                return Json(new { });
            }
            catch (Exception ex)
            {

                return Json(new { });
            }

        }
        public FileResult GetAnexo2()
        {

            string ubicacion = AppDomain.CurrentDomain.BaseDirectory;
            string UbicacionPDF = $"{ubicacion}Documentacion\\ANEXO 2 A LA SOLICITUD DE INGRESO.pdf";
            byte[] FileBytes = System.IO.File.ReadAllBytes(UbicacionPDF);
            //el tercer para obligar la descarga del archivo
            return File(FileBytes, "application/pdf", "ANEXO 2 A LA SOLICITUD DE INGRESO.pdf");

        }



        //----------------------------------Domicilio----------------------------------------------------------------------//


        public ActionResult Domicio_API(int ID_persona)
        {

            Domiciolio_API domi = new Domiciolio_API()
            {
                vPersona_Domicilio_API = db.vPersona_Domicilio.FirstOrDefault(m => m.IdPersona == ID_persona),
                Pais_API = db.sp_vPaises("").Select(m => new SelectListItem { Text = m.DESCRIPCION, Value = m.CODIGO }).ToList(),
                Provincia_API = new List<SelectListItem>(),
                Localidad_API= new List<SelectListItem>()

            };
            return View(domi);
        }




        public ActionResult Domicilio(int ID_persona)
        {
            try
            {
                DomicilioVM datosdomilio = new DomicilioVM()
                {
                    vPersona_DomicilioVM = db.vPersona_Domicilio.FirstOrDefault(m => m.IdPersona == ID_persona),
                    sp_vPaises_ResultVM = db.sp_vPaises("").OrderBy(m => m.DESCRIPCION).ToList(),
                    provincias = db.vProvincia_Depto_Localidad.Select(m => m.Provincia).Distinct().ToList()
                };

                if (datosdomilio.vPersona_DomicilioVM.IdPais != "AR")
                {
                    string[] domiextR = datosdomilio.vPersona_DomicilioVM.Prov_Loc_CP.Split('-');
                    datosdomilio.vPersona_DomicilioVM.Provincia = domiextR[0];
                    datosdomilio.vPersona_DomicilioVM.Localidad = domiextR[1];
                    datosdomilio.vPersona_DomicilioVM.CODIGO_POSTAL = domiextR[2];
                };

                if (datosdomilio.vPersona_DomicilioVM.EventualIdPais != "AR")
                {
                    string[] domiextE = datosdomilio.vPersona_DomicilioVM.EventualProv_Loc.Split('-');
                    datosdomilio.vPersona_DomicilioVM.EventualProvincia = domiextE[0];
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
                        p.Prov_Loc_CP = p.Provincia + "-" + p.Localidad + "-" + p.CODIGO_POSTAL;
                    }
                    else
                    {
                        p.Prov_Loc_CP = null;
                    };

                    if (p.EventualIdPais != "AR")
                    {
                        p.EventualIdLocalidad = null;
                        p.EventualProv_Loc = p.EventualProvincia + "-" + p.EventualLocalidad + "-" + p.EventualCodigo_Postal;
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
                else if (Localidad!= null)
                {
                    string Value = db.vProvincia_Depto_Localidad.FirstOrDefault(m => m.IdLocalidad == Localidad).IdLocalidad.ToString();
                    string Text = db.vProvincia_Depto_Localidad.FirstOrDefault(m => m.IdLocalidad == Localidad).CODIGO_POSTAL.ToString();

                    return Json(new { Value, Text }, JsonRequestBehavior.AllowGet);
                };
            return Json(new {}, JsonRequestBehavior.AllowGet);
        }

//----------------------------------Estudios----------------------------------------------------------------------//
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

        public ActionResult EstudiosCUD(int? ID,int ID_persona )
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
                            estudio.INST_EXT = false;
                            estudio.vPersona_EstudioIdVM.Nombre = "";
                    }
                    else
                    {
                            string[] paisinst = estudio.vPersona_EstudioIdVM.NombreYPaisInstituto.Split('-');
                            estudio.vPersona_EstudioIdVM.Jurisdiccion = paisinst[0];
                            estudio.vPersona_EstudioIdVM.Nombre = paisinst[1];
                            estudio.Localidad = new List<string>();
                            estudio.InstitutoVM = new List<SelectListItem>();
                            estudio.INST_EXT = true;
                    }
                }
                else
                {
                    VPersona_Estudio nuevoestu = new VPersona_Estudio()
                    {
                        
                        IdInstitutos = 0,
                        IdEstudio = 0,
                        NombreYPaisInstituto = "-",
                        Completo= true,
                        
                    };
                    estudio.INST_EXT = false;
                    nuevoestu.IdPersona = ID_persona;
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
        public ActionResult EstudiosCUD(EstudiosVM Datos)
        {
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
                    db.spEstudiosIU(e.IdEstudio, e.IdPersona, e.Titulo, e.Completo, e.IdNiveldEstudio, e.IdInstitutos, e.Promedio, e.CantidadMateriaAdeudadas, e.ultimoAnioCursado, e.NombreYPaisInstituto);

                    return Json(new { success = true, msg = "Se Inserto correctamente el  ESTUDIOS" });
                }
                catch (Exception ex)
                {
                    //revisar como mostrar error en la vista
                    return Json(new { success = false, msg = ex.InnerException.Message });
                }
            }
            return Json(new { success = false, msg = "Error en el Modelo Recibido" });

        }


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
                    return Json(new { success = true, msg = "Se elimno correctamente el EStudio seleccionado",form="Elimina", url_Tabla= "Estudios" }, JsonRequestBehavior.AllowGet);
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
                                     Text = m.Nombre
                                 })
                                 .OrderBy(m => m.Text)
                                 .ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }

        }

/*--------------------------------------------------------------IDIOMAS-------------------------------------------------------------------------------*/
        public ActionResult Idiomas(int ID_persona)
        {
            try
            {
                List<vPersona_Idioma> idioma;
                idioma = db.vPersona_Idioma.Where(m=>m.IdPersona== ID_persona).ToList();

                return PartialView(idioma);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public ActionResult IdiomaCUD(int? ID,int? ID_persona)
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
        public JsonResult IdiomaCUD(IdiomasVM datos)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    vPersona_Idioma i = datos.VPersona_IdiomaIdVM;
                    db.spIdiomasIU(i.IdPersonaIdioma, i.IdPersona, i.CodIdioma, i.Habla, i.Lee, i.Escribe);
                    return Json(new { success = true, msg = "Se Inserto correctamente el Idioma nuevo o s emodifico IDIOMA" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {

                    return Json(new { success = true, msg = ex.InnerException.Message }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { success = true, msg = "El modelo recibido NO ES VALIDO"}, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EliminaIDIO(int ID)
        {
            try
            {
                var regidioma = db.PersonaIdioma.FirstOrDefault(m=>m.IdPersonaIdioma==ID);
                db.PersonaIdioma.Remove(regidioma);
                db.SaveChanges();
                return Json(new { success = true, msg = "Se le elimino correctamente el idioma seleccionado",form = "Elimina", url_Tabla = "Idiomas" },JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            { 

                return Json(new { success = false, msg =  ex.InnerException.Message});
            }
        }

/*--------------------------------------------------------------ACTIVIDAD MILITAR-------------------------------------------------------------------------------*/
        public ActionResult ActMilitar(int ID_persona)
        {
            try
            {
                List<vPersona_ActividadMilitar> LISTactividad;
                LISTactividad = db.vPersona_ActividadMilitar.Where(m => m.IdPersona == ID_persona).ToList();

                return PartialView(LISTactividad);
            }
            catch (Exception )
            {
                throw;
            }
        }

        public ActionResult ActMilitarCUD(int? ID,int ID_persona)
        {
            try
            {
                ActividadMIlitarVM actividad = new ActividadMIlitarVM() {
                    FuerzasVM = db.Fuerza.ToList(),
                    IDPErsona = ID_persona,
                    BajaVM = db.Baja.ToList(),
                    SituacionRevistaVM = db.SituacionRevista.ToList()
                 };
                if (db.Postulante.First(m => m.IdPersona == ID_persona).AspNetUsers != null)
                {
                    actividad.SituacionRevistaVM.Remove(db.SituacionRevista.First(m => m.SituacionRevista1 == "Retirado"));  
                };
                if (ID!= null)
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
        public JsonResult ActMilitarCUD(ActividadMIlitarVM datos)
        {

            try
            {
                var a = datos.ACTMilitarIDVM;
                if (a.Ingreso == true)
                {
                    a.CausaMotivoNoingreso=null;
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
                return Json(new { success = true  , msg = "Se inserto o actualizo correctamente ACTMilitar" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = ex.InnerException.Message }, JsonRequestBehavior.AllowGet);
            }
            
        }

        public JsonResult EliminaACT(int ID)
        {
            try
            {
                db.spActividadMilitarEliminar(ID);
                return Json(new { success = false, msg = "se elimino correctamente el registro", form = "Elimina", url_Tabla = "ActMilitar" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = true, msg = ex.InnerException.Message }, JsonRequestBehavior.AllowGet);
            }
        }

 /*--------------------------------------------------------------SITUACION OCUPACIONAL-------------------------------------------------------------------------------*/
        public ActionResult SituOcupacional(int ID_persona)
        {
            try
            {

                SituacionOcupacionalVM SituOcu = new SituacionOcupacionalVM
                {
                    EstadoDescripcionVM = new SelectList(db.EstadoOcupacional.Where(m=>m.Descripcion != "Jubilado" & m.Descripcion != "Retirado").ToList(), "Id", "Descripcion", "EstadoOcupacional1", 1)
                };

                List<string> InteresesSeleccionados = db.Persona.Find(ID_persona).Interes.Select(m => m.DescInteres).ToList();
                SituOcu.InteresesVM = db.Interes.Select(c => new SelectListItem { Text = c.DescInteres, Value = c.IdInteres.ToString(), Selected = InteresesSeleccionados.Contains(c.DescInteres) ? true : false }).ToList();

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
        public ActionResult SituOcupacional(SituacionOcupacionalVM situ)
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

        /*--------------------------------------------------------------Antropometria------------------------------------------------------------------------------*/
        public ActionResult Antropometria(int ID_persona)
        {
            vPersona_Antropometria antropo = db.vPersona_Antropometria.FirstOrDefault(m=>m.IdPersona == ID_persona);
            return PartialView(antropo);
        }
        [HttpPost]
        public ActionResult Antropometria(vPersona_Antropometria a)
        {
            try
            {
                db.spAntropometriaIU(a.IdPersona, a.Altura, a.Peso, a.IMC, a.PerimCabeza, a.PerimTorax, a.PerimCintura, a.PerimCaderas, a.LargoPantalon, a.LargoEntrep, a.LargoFalda, a.Cuello, a.Calzado);
                return Json(new { success = true, msg = "Se guardaron los DATOS exitosamente." });
            }
            catch (Exception ex )
            {
                return Json(new { success = false, msg = ex.InnerException.Message});
            }

        }

        /*--------------------------------------------------------------FAMILIA------------------------------------------------------------------------------*/
        public ActionResult Familia(int ID_persona)
        {
            try
            {
                //List<int> id_PER_FAMI = db.Familiares.Where(m => m.IdPostulantePersona == ID_persona).Select(m => m.IdPersona).ToList();
                var FAMI = db.Familiares.Where(id => id.IdPostulantePersona == ID_persona).ToList();
                List<vPersona_Familiar> FAMILIARES = new List<vPersona_Familiar>();
                FAMI.ForEach(f=>
                
                    FAMILIARES.Add(
                            new vPersona_Familiar
                            {
                                IdFamiliar = f.IdFamiliar,
                                Relacion = db.vParentesco.FirstOrDefault(m=>m.idParentesco==f.idParentesco).Relacion,
                                IdPersonaPostulante = f.IdPostulantePersona,
                                IdPersonaFamiliar = f.IdPersona,
                                Apellido = db.Persona.FirstOrDefault(m => m.IdPersona == f.IdPersona).Apellido,
                                Nombres = db.Persona.FirstOrDefault(m => m.IdPersona == f.IdPersona).Nombres,
                            }
                        )
                );
                return PartialView(FAMILIARES);
            }
            catch (Exception )
            {
                return View();
            }
        }

        //recibo el idFamilia, si es 0 creo  una personaFamilia y su relacion.
        public ActionResult FamiliaCUD(int idPersonaFamilia)
        { 
            //verificar que al crear un postulante llenar con los datos completos, si no es asi el familiar no se mostrara en vPersona_Familiar
            //viewmodel creado para la creacion de un familiar
            //cargo los datos necesarios para los combobox
            PersonaFamiliaVM pers = new PersonaFamiliaVM
            {
                vParentecoVM = db.vParentesco.Select(m => new SelectListItem { Value = m.idParentesco.ToString(), Text = m.Relacion }).ToList(),
                SexoVM = db.Sexo.Select(m => new SelectListItem { Value = m.IdSexo.ToString(), Text = m.Descripcion }).Where(m => m.Value != "4").ToList(),
                vEstCivilVM = db.vEstCivil.Select(m => new SelectListItem { Value = m.Codigo_n, Text = m.Descripcion }).ToList(),
                ReligionVM = db.vRELIGION.Select(m => new SelectListItem { Value = m.CODIGO, Text = m.DESCRIPCION }).ToList(),
                TipoDeNacionalidadVm = db.TipoNacionalidad.Select(m => new SelectListItem { Value = m.IdTipoNacionalidad.ToString(), Text = m.Descripcion }).ToList()
            };
            if (idPersonaFamilia != 0  )
            {
                pers.ID_PER = idPersonaFamilia;
                pers.vPersona_FamiliarVM = db.vPersona_Familiar.FirstOrDefault(m => m.IdPersonaFamiliar == idPersonaFamilia);
                pers.postulante = (db.Postulante.FirstOrDefault(m => m.IdPersona == pers.ID_PER)!=null) ?true: false;

            }
            else
            {
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
                    Familiares rela=null;
                    string msgs;

                    if (per!=null)
                    {
                        rela = db.Persona.Find(IDPOSTULANTE).Postulante.Familiares.FirstOrDefault(m => m.IdPersona == per.IdPersona);
                        msgs = (rela!=null)?"Modificacion del Familiar Exitoso.": "Se agrego un familiar exitoxamente. Rediriginedo...";
                    }
                    else
                    {
                        msgs = "Creacion del Familiar Exitoso. Redirigiendo...";
                    };

                    if ((per != null && rela != null) || per == null)
                    {
                            datos.IdReligion ??= "";
                            db.spPERSONAFamiliarIU(datos.IdPersonaFamiliar, datos.IdPersonaPostulante, datos.Mail, datos.Apellido, datos.Nombres, datos.IdSexo, datos.FechaNacimiento, datos.DNI, datos.CUIL,
                            datos.IdReligion, datos.IdEstadoCivil, datos.FechaCasamiento, datos.Telefono, datos.Celular, datos.Mail, datos.idTipoNacionalidad, 0, datos.idParentesco, datos.Vive, datos.ConVive);
                            idpersonafamiliar = (per == null) ? db.vPersona_Familiar.FirstOrDefault(d => d.DNI == datos.DNI).IdPersonaFamiliar : 0;

                    }
                    else if(rela==null) { 
                        db.spRelacionFamiliarIU(0, datos.IdPersonaPostulante, per.IdPersona, datos.idParentesco, datos.Vive, datos.ConVive);
                        datos.IdPersonaFamiliar = per.IdPersona;
                        idpersonafamiliar = db.vPersona_Familiar.FirstOrDefault(d => d.DNI == datos.DNI).IdPersonaFamiliar;

                    }
                    return Json(new { success = true, msg = msgs,accion= idpersonafamiliar }, JsonRequestBehavior.AllowGet);

                }
                catch (Exception ex)
                {
                    return Json(new {success= false, msg = ex.InnerException.Message}, JsonRequestBehavior.AllowGet);
                    
                }
            }
            return Json(new {success= false, msg= "Modelo no valido" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult VerificarDNI(int DNI , int ID)
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
                    resps = (rela != null) ?"son_familiares": "existe";
                  
                    return Json(new { resp =resps, msg =msgs, ID_PER = Id_Persona }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { resp = "no_existe" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { error = false, msg=  ex.InnerException.Message}, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult EliminaFAMI(int ID_per,int ID_fami)
        {
            try
            {
               
                int filas = db.Familiares.Where(p => p.IdPersona == ID_per).Count();
                bool postulante = db.Postulante.Find(ID_per) != null;
                if (postulante || filas > 1)
                {
                    db.spRelacionFamiliarEliminar(ID_fami);
                    return Json(new { success = true, msg = "Se elimino correctamente el Familiar", form = "Elimina", url_Tabla = "Familia" }, JsonRequestBehavior.AllowGet);
                }

                db.spFamiliarEliminar(ID_fami, ID_per);
                return Json(new { success = true, msg = "Se elimino correctamente el Familiar", form = "Elimina", url_Tabla = "Familia" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = ex.InnerException.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /*--------------------------------------------------------------VALIDAR DATOS------------------------------------------------------------------------------*/

        //paso al postulante a la secuencia "Documentacion - A Validar"
        public JsonResult ValidarDatos(int ID_persona)
        {
            try
            {
                db.spProximaSecuenciaEtapaEstado(ID_persona, 0, false, 14, "", "");
                return Json(new { success = true, msg = "Operacion Exitosa" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(new { success = false, msg = ex.InnerException.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /*--------------------------------------------------------------PRESENTACION------------------------------------------------------------------------------*/


        public ActionResult Presentacion(int ID_persona)            
        {
            var per = db.Persona.Find(ID_persona);
            Presentacion prese = new Presentacion {
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
            else {
                prese.FechaPresentacion = (DateTime)FechaPrese;
                string url = "Link donde estaran los datos del postulante.";
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
            Bitmap qrCodeImage = qrCode.GetGraphic(20, ColorTranslator.FromHtml("#212429"), Color.White, (Bitmap)Bitmap.FromFile(ubicacionImagen),25,10);
            using (MemoryStream stream = new MemoryStream())
            {
                qrCodeImage.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }

        }



    }
}
