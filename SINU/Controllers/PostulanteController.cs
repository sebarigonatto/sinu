using Microsoft.Ajax.Utilities;
using SINU.Models;
using SINU.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SINU.Controllers
{
    [Authorize]
    public class PostulanteController : Controller
    {
        SINUEntities db = new SINUEntities();

        //ESTABLESCO LA VARIABLE MAIL DEL USUARIO QUE ESTA ACTUALMENTE LOGUEADO
        //ESTA CARIABLE ES UTILIZADA PARA BUSCAR EN LAS DISTINTAS VISTAS, UTILIZADAS EN EL CONTROLADOR, PARA  BUSCAR LOS REGISTROS DEL USUARIO LOGUEADO
        private int ID_per => db.Persona.FirstOrDefault(m=>m.Email == HttpContext.User.Identity.Name.ToString()).IdPersona;
        private int IDFAMI;
 //----------------------------------PAGINA PRINCIPAL----------------------------------------------------------------------//

        public ActionResult Index()
        {//error cdo existe uno registrado antes de los cambios de secuencia

            ViewBag.secuenciaactual = db.vInscripcionEtapaEstadoUltimoEstado.FirstOrDefault(m => m.IdPersona == ID_per).IdSecuencia;
            return View();
        }

//----------------------------------DATOS BASICOS----------------------------------------------------------------------//

        public ActionResult DatosBasicos()
        {
            try
            {
                //se carga los datos basicos del usuario actual y los utilizados para los dropboxlist
                DatosBasicosVM datosba = new DatosBasicosVM() 
                { 
                    SexoVM = db.Sexo.ToList(),
                    vPeriodosInscripsVM = db.vPeriodosInscrip.ToList(),
                    OficinasYDelegacionesVM = db.OficinasYDelegaciones.ToList(),
                    vPersona_DatosBasicosVM = db.vPersona_DatosBasicos.FirstOrDefault(b => b.IdPersona == ID_per)
                };

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
            
           
            if (ModelState.IsValid)
            {
                try
                {
                    //se guarda los datos de las persona devueltos
                    var p = Datos.vPersona_DatosBasicosVM;
                    //se llama el "spDatosBasicosUpdate" para guadar los datos ingresados en la base de datos
                    var result = db.spDatosBasicosUpdate(p.Apellido, p.Nombres, p.IdSexo, p.DNI, p.Telefono, p.Celular, p.Email, p.IdDelegacionOficinaIngresoInscribio, p.ComoSeEntero, p.IdPreferencia, p.IdPersona, p.IdPostulante);

                    return Json(new { success = true, msg = "se guardoron los datos correctamente" });
                }
                catch (Exception ex)
                {
                    //envio la error  a la vista
                    string msgerror = ex.Message + " " + ex.InnerException.Message;
                    return Json(new { success = false, msg = msgerror });
                }
            };
            return Json(new { success = false, msg= "Modelo no VALIDO" });
         

        }

//----------------------------------ENTREVISTA----------------------------------------------------------------------//

        public ActionResult Entrevista()
        {
            vEntrevistaLugarFecha entrevistafh = new vEntrevistaLugarFecha();
            try
            {
                entrevistafh = db.vEntrevistaLugarFecha.FirstOrDefault(m => m.IdPersona == ID_per);
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

        public ActionResult DatosPersonales()
        {
            try
            {
                DatosPersonalesVM datosba = new DatosPersonalesVM()
                {
                    vPersona_DatosPerVM = db.vPersona_DatosPer.FirstOrDefault(m => m.IdPersona == ID_per),
                    TipoNacionalidadVM = db.TipoNacionalidad.ToList(),
                    vEstCivilVM = db.vEstCivil.ToList(),
                    vRELIGIONVM = db.vRELIGION.ToList()
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
                        var result = db.spDatosPersonalesUpdate(p.IdPersona, p.CUIL, p.FechaNacimiento, p.IdEstadoCivil, p.IdReligion, p.idTipoNacionalidad);
                        return Json(new { success = true, msg = "se guadaron con exito los DATOS PERSONALES" });
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
//----------------------------------Domicilio----------------------------------------------------------------------//

        public ActionResult Domicilio()
        {
            try
            {
                DomicilioVM datosdomilio = new DomicilioVM()
                {
                    vPersona_DomicilioVM = db.vPersona_Domicilio.FirstOrDefault(m => m.IdPersona == ID_per),
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

                    int result = db.spDomiciliosU(
                        p.IdDomicilioDNI, p.Calle, p.Numero, p.Piso, p.Unidad, p.IdLocalidad, p.Prov_Loc_CP, p.IdPais,
                        p.IdDomicilioActual, p.EventualCalle, p.EventualNumero, p.EventualPiso, p.EventualUnidad, p.EventualIdLocalidad, p.EventualProv_Loc, p.EventualIdPais
                    );

                    return Json(new { success = true, msg = "Se guardaron con Exito datos de domicilio" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    //envio la error  a la vista
                    string msgerror = ex.Message + " " + ex.InnerException.Message;
                    return Json(new { success = false, msg = msgerror }, JsonRequestBehavior.AllowGet);
                }
            }

            //ver como saber que capos causan el error
            //string errors = "";
            //foreach (var i in ModelState)
            //{
            //    errors = errors + " -- " + i.Value.Errors;
            //};
           
            return Json(new { success = false, msg = "Modelo no VALIDO - " /*+ errors*/ }, JsonRequestBehavior.AllowGet);
        }



        //Cargo el DropBoxList de localidad,segun Provincia Seleccionado o Cp segun Localidad Seleccionada
        public JsonResult DropEnCascadaDomicilio(string? Provincia, int? Localidad)
        {

            if (Provincia!=null)
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
            else
            {
                string Value = db.vProvincia_Depto_Localidad.FirstOrDefault(m => m.IdLocalidad == Localidad).IdLocalidad.ToString();
                string Text = db.vProvincia_Depto_Localidad.FirstOrDefault(m => m.IdLocalidad == Localidad).CODIGO_POSTAL.ToString();

                return Json(new { Value, Text }, JsonRequestBehavior.AllowGet);
            }
        }

//----------------------------------Estudios----------------------------------------------------------------------//
        public ActionResult Estudios()
        {
            try
            {
                EstudiosVM estudio = new EstudiosVM();
                estudio.vPersona_EstudioListVM = db.VPersona_Estudio.Where(m => m.IdPersona == ID_per).ToList();
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

        public ActionResult EstudiosCUD(int? ID)
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
                            estudio.vPersona_EstudioIdVM.Nombre = "";
                    }
                    else
                    {
                            string[] paisinst = estudio.vPersona_EstudioIdVM.NombreYPaisInstituto.Split('-');
                            estudio.vPersona_EstudioIdVM.Jurisdiccion = paisinst[0];
                            estudio.vPersona_EstudioIdVM.Nombre = paisinst[1];
                            estudio.Localidad = new List<string>();
                            estudio.InstitutoVM = new List<SelectListItem>();
                    }
                }
                else
                {
                    VPersona_Estudio nuevoestu = new VPersona_Estudio()
                    {
                        IdPersona = ID_per,
                        IdInstitutos = 0,
                        IdEstudio = 0,
                        NombreYPaisInstituto = "-"
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

                    db.spEstudiosIU(e.IdEstudio, e.IdPersona, e.Titulo, e.Completo, e.IdNiveldEstudio, e.IdInstitutos, e.Promedio, e.CantidadMateriaAdeudadas, e.ultimoAnioCursado, e.NombreYPaisInstituto);

                    return Json(new { success = true, msg = "Se Inserto correctamente el estudio nuevo" });
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
                    return Json(new { success = true, msg = "Se elimno correctamente el EStudio seleccionado" }, JsonRequestBehavior.AllowGet);
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
        public ActionResult Idiomas()
        {
            try
            {
                List<vPersona_Idioma> idioma;
                idioma = db.vPersona_Idioma.Where(m=>m.IdPersona==ID_per).ToList();

                return PartialView(idioma);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public ActionResult IdiomaCUD(int? ID)
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
                        IdPersona = ID_per
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
                    return Json(new { success = true, msg = "Se Inserto correctamente el Idioma nuevo o s emodifico" }, JsonRequestBehavior.AllowGet);
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
                return Json(new { success = true, msg = "Se le elimino correctamente el idioma seleccionado" });
            }
            catch (Exception ex)
            { 

                return Json(new { success = false, msg =  ex.InnerException.Message});
            }
        }

/*--------------------------------------------------------------ACTIVIDAD MILITAR-------------------------------------------------------------------------------*/
        public ActionResult ActMilitar()
        {
            try
            {
                List<vPersona_ActividadMilitar> LISTactividad;
                LISTactividad = db.vPersona_ActividadMilitar.Where(m => m.IdPersona == ID_per).ToList();

                return PartialView(LISTactividad);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public ActionResult ActMilitarCUD(int? ID)
        {
            try
            {
                ActividadMIlitarVM actividad = new ActividadMIlitarVM() {
                    FuerzasVM = db.Fuerza.ToList(),
                    IDPErsona = ID_per,
                    BajaVM = db.Baja.ToList(),
                    SituacionRevistaVM = db.SituacionRevista.ToList()
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
                return Json(new { success = true  , msg = "Se inserto o actulaizo correctamente" }, JsonRequestBehavior.AllowGet);
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
                return Json(new { success = false, msg = "se elimino correctamente el registro"}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = true, msg = ex.InnerException.Message }, JsonRequestBehavior.AllowGet);
            }
        }

 /*--------------------------------------------------------------SITUACION OCUPACIONAL-------------------------------------------------------------------------------*/
        public ActionResult SituOcupacional()
        {
            try
            {
               
                SituacionOcupacionalVM SituOcu = new SituacionOcupacionalVM();
         
                SituOcu.EstadoDescripcionVM = new SelectList(db.EstadoOcupacional.ToList(), "Id", "Descripcion", "EstadoOcupacional1", 1);

                List<string> InteresesSeleccionados = db.Persona.Find(ID_per).Interes.Select(m => m.DescInteres).ToList();
                SituOcu.InteresesVM = db.Interes.Select(c => new SelectListItem { Text = c.DescInteres, Value = c.IdInteres.ToString(), Selected = InteresesSeleccionados.Contains(c.DescInteres) ? true : false }).ToList();

                var situ = db.vPersona_SituacionOcupacional.FirstOrDefault(m => m.IdPersona == ID_per);
                if (situ == null)
                {
                    SituOcu.VPersona_SituacionOcupacionalVM = new vPersona_SituacionOcupacional()
                    {
                        IdSituacionOcupacional = 0,
                        IdPersona = ID_per,   
                    };
                }
                else
                {
                    SituOcu.VPersona_SituacionOcupacionalVM = situ;
                };

                return PartialView(SituOcu);
            }
            catch (Exception)
            {
                throw;
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
                var per = db.Persona.Find(ID_per).Interes;
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
                //var asd = db.Persona.Find(ID_per).Interes.ToList();
                return Json(new { success = true, msg = "exito en guardar la situacion ocupacional" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = ex.InnerException.Message });
            }
        }
            /*--------------------------------------------------------------Antropometria------------------------------------------------------------------------------*/
        public ActionResult Antropometria()
        {
            vPersona_Antropometria antropo = db.vPersona_Antropometria.FirstOrDefault(m=>m.IdPersona==ID_per);
            return PartialView(antropo);
        }
        [HttpPost]
        public ActionResult Antropometria(vPersona_Antropometria a)
        {
            try
            {
                db.spAntropometriaIU(a.IdPersona, a.Altura, a.Peso, a.IMC, a.PerimCabeza, a.PerimTorax, a.PerimCintura, a.PerimCaderas, a.LargoPantalon, a.LargoEntrep, a.LargoFalda, a.Cuello, a.Calzado);
                return Json(new { success = true, msg = "exito en guardar la situacion ocupacional" });
            }
            catch (Exception ex )
            {
                return Json(new { success = false, msg = ex.InnerException.Message});
            }

        }
    }
}
