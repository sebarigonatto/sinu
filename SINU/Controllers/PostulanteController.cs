using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SINU.Authorize;
using Microsoft.AspNet.Identity.EntityFramework;
using SINU.Models;
using SINU.ViewModels;
using Microsoft.Ajax.Utilities;

namespace SINU.Controllers
{
    [Authorize]
    public class PostulanteController : Controller
    {
        SINUEntities db = new SINUEntities();

        //ESTABLESCO LA VARIABLE MAIL DEL USUARIO QUE ESTA ACTUALMENTE LOGUEADO
        private string USUmail => HttpContext.User.Identity.Name.ToString();

//----------------------------------PAGINA PRINCIPAL----------------------------------------------------------------------//

        public ActionResult Index()
        {
            ViewBag.secuenciaactual = db.vInscripcionEtapaEstadoUltimoEstado.FirstOrDefault(m => m.Email == USUmail).IdSecuencia;
            return View();
        }

//----------------------------------DATOS BASICOS----------------------------------------------------------------------//

        public ActionResult DatosBasicos()
        {
            try
            {
                //guardo el mail del usuario que esta logueado
                DatosBasicosVM datosba = new DatosBasicosVM()
                {
                    SexoVM = db.Sexo.ToList(),
                    vPeriodosInscripsVM = db.vPeriodosInscrip.ToList(),
                    OficinasYDelegacionesVM = db.OficinasYDelegaciones.ToList()
                };

                //levanto el registro perteneciente al usuario logueado
                datosba.vPersona_DatosBasicosVM = db.vPersona_DatosBasicos.FirstOrDefault(b => b.Email == USUmail);
                return PartialView(datosba);
            }
            catch (Exception ex)
            {
                //revisar como mostrar error en la vista
                return View();
            }

        }

        //ACCION QUE GUARDA LOS DATOS INGRESADOS EN LA VISTA "DATOS BASICOS"
        [HttpPost]
        public ActionResult DatosBasicos(DatosBasicosVM Datos)
        {
            try
            {
                var p = Datos.vPersona_DatosBasicosVM;
                var result = db.spDatosBasicosUpdate(p.Apellido, p.Nombres, p.IdSexo, p.DNI, p.Telefono, p.Celular, p.Email, p.IdDelegacionOficinaIngresoInscribio, p.ComoSeEntero, p.IdPreferencia, p.IdPersona, p.IdPostulante);

                return Json(new { success = true, msg = "" });
            }
            catch (Exception ex)
            {
                //envio la error  a la vista
                string msgerror = ex.Message + " " + ex.InnerException.Message;
                return Json(new { success = false, msg = msgerror });
            }

        }

//----------------------------------ENTREVISTA----------------------------------------------------------------------//

        public ActionResult Entrevista()
        {
            vEntrevistaLugarFecha entrevistafh = new vEntrevistaLugarFecha();

            int idperson = db.vPersona_DatosBasicos.FirstOrDefault(m => m.Email == USUmail).IdPersona;
            entrevistafh = db.vEntrevistaLugarFecha.FirstOrDefault(m => m.IdPersona == idperson);
            //se carga los texto parametrizados desde la tabla configuracion
            string[] consideraciones = {
                db.Configuracion.FirstOrDefault(m => m.NombreDato == "ConsideracionEntrevTitulo").ValorDato.ToString(),
                db.Configuracion.FirstOrDefault(m => m.NombreDato == "ConsideracionEntrevTexto").ValorDato.ToString()
            };
            ViewBag.Considere = consideraciones;

            return PartialView(entrevistafh);
        }

//----------------------------------DATOS PERSONALES----------------------------------------------------------------------//

        public ActionResult DatosPersonales()
        {
            try
            {
                DatosPersonalesVM datosba = new DatosPersonalesVM()
                {
                    vPersona_DatosPerVM = db.vPersona_DatosPer.FirstOrDefault(m => m.Email == USUmail),
                    TipoNacionalidadVM = db.TipoNacionalidad.ToList(),
                    vEstCivilVM = db.vEstCivil.ToList(),
                    vRELIGIONVM = db.vRELIGION.ToList()
                };
                return PartialView(datosba);
            }
            catch (Exception ex)
            {
                //revisar como mostrar error en la vista
                return View(ex);
            }
        }


        //ACCION QUE GUARDA LOS DATOS INGRESADOS EN LA VISTA "DATOS PERSONALES"
        [HttpPost]
        public ActionResult DatosPersonales(DatosPersonalesVM Datos)
            {
            try
            {
                var p = Datos.vPersona_DatosPerVM;
                var result = db.spDatosPersonalesUpdate(p.IdPersona, p.CUIL, p.FechaNacimiento, p.IdEstadoCivil, p.IdReligion, p.idTipoNacionalidad);
                return Json(new { success = true, msg = "" });
            }
            catch (Exception ex)
            {
                //envio la error  a la vista
                string msgerror = ex.Message + " " + ex.InnerException.Message;
                return Json(new { success = false, msg = msgerror });
            }
        }

//----------------------------------Domicilio----------------------------------------------------------------------//

        public ActionResult Domicilio()
        {
            try
            {
                DomicilioVM datosdomilio = new DomicilioVM()
                {
                    vPersona_DomicilioVM = db.vPersona_Domicilio.FirstOrDefault(m => m.Email == USUmail),
                    sp_vPaises_ResultVM = db.sp_vPaises("").ToList()
                };
                    
                datosdomilio.provincias = (datosdomilio.vPersona_DomicilioVM.IdPais.ToString() == "AR") ? 
                    db.vProvincia_Depto_Localidad.Select(m => m.Provincia).Distinct().ToList()
                  : db.vProvincia_Depto_Localidad.Where(m => m.Provincia == "").Select(m => m.Provincia).Distinct().ToList();

                datosdomilio.vProvincia_Depto_LocalidadsVM = (datosdomilio.vPersona_DomicilioVM.IdLocalidad != null) ?
                    db.vProvincia_Depto_Localidad.Where(m => m.Provincia == datosdomilio.vPersona_DomicilioVM.Provincia).ToList()
                  : db.vProvincia_Depto_Localidad.Where(m => m.Provincia == "").ToList();

                return PartialView(datosdomilio);
            }
            catch (Exception ex)
            {
                //revisar como mostrar error en la vista
                return View(ex);
            }
        }
        //ACCION QUE GUARDA LOS DATOS INGRESADOS EN LA VISTA "DATOS PERSONALES"
        [HttpPost]
        public ActionResult Domicilio(DomicilioVM Datos)
        {
            try
            {
                var p = Datos.vPersona_DomicilioVM;
                
                p.Prov_Loc_CP = p.Provincia + " " + p.Localidad + " " + p.CODIGO_POSTAL;
                p.EventualProv_Loc = p.EventualProvincia + " " + p.EventualLocalidad + " " + p.EventualCodigo_Postal;


                int result = db.spDomiciliosU(p.IdDomicilioDNI, p.Calle, p.Numero, p.Piso, p.Unidad, p.IdLocalidad, p.Prov_Loc_CP, p.IdPais, 
                                              p.IdDomicilioActual, p.EventualCalle, p.EventualNumero, p.EventualPiso, p.EventualUnidad, p.EventualIdLocalidad, p.EventualProv_Loc, p.EventualIdPais);
                //var result = db.spDatosPersonalesUpdate(p.IdPersona, p.CUIL, p.FechaNacimiento, p.IdEstadoCivil, p.IdReligion, p.idTipoNacionalidad);
                return Json(new { success = true, msg = "" });
            }
            catch (Exception ex)
            {
                //envio la error  a la vista
                string msgerror = ex.Message + " " + ex.InnerException.Message;
                return Json(new { success = false, msg = msgerror });
            }
        }



        //Cargo el DropBoxList de localidad,segun Provincia Seleccionado o Cp segun Localidad Seleccionada
        public JsonResult DropEnCascada(string? Provincia,int? Localidad)
        {
            if (Provincia != null) { 

                var result = db.vProvincia_Depto_Localidad
                                .Where(m => m.Provincia == Provincia)
                                .Select(m => new SelectListItem
                                {
                                    Value = m.IdLocalidad.ToString(),
                                    Text = m.Localidad
                                })
                                .OrderBy(m => m.Text)
                                .ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string Value = db.vProvincia_Depto_Localidad.FirstOrDefault(m => m.IdLocalidad == Localidad).IdLocalidad.ToString();
                string Text = db.vProvincia_Depto_Localidad.FirstOrDefault(m => m.IdLocalidad == Localidad).CODIGO_POSTAL.ToString();

                return Json(new {  Value ,  Text }, JsonRequestBehavior.AllowGet);
            }
        }
//----------------------------------Idioma----------------------------------------------------------------------//
        //public ActionResult Idioma()
        //{
        //    try
        //    {
        //        DomicilioVM datosdomilio = new DomicilioVM()
        //        {
        //            vPersona_DomicilioVM = db.vPersona_Domicilio.FirstOrDefault(m => m.Email == USUmail),
        //            sp_vPaises_ResultVM = db.sp_vPaises("").ToList()
        //        };

        //        datosdomilio.provincias = (datosdomilio.vPersona_DomicilioVM.IdPais.ToString() == "AR") ?
        //            db.vProvincia_Depto_Localidad.Select(m => m.Provincia).Distinct().ToList()
        //          : db.vProvincia_Depto_Localidad.Where(m => m.Provincia == "").Select(m => m.Provincia).Distinct().ToList();

        //        datosdomilio.vProvincia_Depto_LocalidadsVM = (datosdomilio.vPersona_DomicilioVM.IdLocalidad != null) ?
        //            db.vProvincia_Depto_Localidad.Where(m => m.Provincia == datosdomilio.vPersona_DomicilioVM.Provincia).ToList()
        //          : db.vProvincia_Depto_Localidad.Where(m => m.Provincia == "").ToList();

        //        return PartialView(datosdomilio);
        //    }
        //    catch (Exception ex)
        //    {
        //        //revisar como mostrar error en la vista
        //        return View(ex);
        //    }
        //}
        ////ACCION QUE GUARDA LOS DATOS INGRESADOS EN LA VISTA "DATOS PERSONALES"
        //[HttpPost]
        //public ActionResult Idioma(DomicilioVM Datos)
        //{
        //    try
        //    {
        //        var p = Datos.vPersona_DomicilioVM;

        //        p.Prov_Loc_CP = p.Provincia + " " + p.Localidad + " " + p.CODIGO_POSTAL;
        //        p.EventualProv_Loc = p.EventualProvincia + " " + p.EventualLocalidad + " " + p.EventualCodigo_Postal;


        //        int result = db.spDomiciliosU(p.IdDomicilioDNI, p.Calle, p.Numero, p.Piso, p.Unidad, p.IdLocalidad, p.Prov_Loc_CP, p.IdPais,
        //                                      p.IdDomicilioActual, p.EventualCalle, p.EventualNumero, p.EventualPiso, p.EventualUnidad, p.EventualIdLocalidad, p.EventualProv_Loc, p.EventualIdPais);
        //        //var result = db.spDatosPersonalesUpdate(p.IdPersona, p.CUIL, p.FechaNacimiento, p.IdEstadoCivil, p.IdReligion, p.idTipoNacionalidad);
        //        return Json(new { success = true, msg = "" });
        //    }
        //    catch (Exception ex)
        //    {
        //        //envio la error  a la vista
        //        string msgerror = ex.Message + " " + ex.InnerException.Message;
        //        return Json(new { success = false, msg = msgerror });
        //    }
        //}



        ////Cargo el DropBoxList de localidad,segun Provincia Seleccionado o Cp segun Localidad Seleccionada
        //public JsonResult DropEnCascada(string? Provincia, int? Localidad)
        //{
        //    if (Provincia != null)
        //    {

        //        var result = db.vProvincia_Depto_Localidad
        //                        .Where(m => m.Provincia == Provincia)
        //                        .Select(m => new SelectListItem
        //                        {
        //                            Value = m.IdLocalidad.ToString(),
        //                            Text = m.Localidad
        //                        })
        //                        .OrderBy(m => m.Text)
        //                        .ToList();
        //        return Json(result, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        string Value = db.vProvincia_Depto_Localidad.FirstOrDefault(m => m.IdLocalidad == Localidad).IdLocalidad.ToString();
        //        string Text = db.vProvincia_Depto_Localidad.FirstOrDefault(m => m.IdLocalidad == Localidad).CODIGO_POSTAL.ToString();

        //        return Json(new { Value, Text }, JsonRequestBehavior.AllowGet);
        //    }
        //}











        // POST: Postulante/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Postulante/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Postulante/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Postulante/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Postulante/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
