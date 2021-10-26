using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SINU.Models;
using SINU.Authorize;
using System.Data.Entity;
using Microsoft.Ajax.Utilities;
using static SINU.Models.AjaxDataTableModel;
using SINU.ViewModels;
using System.Text.Json;

namespace SINU.Controllers.Consultor
{
    /// <summary>Este Controlador CallConsulta depende de las Consultas Principales
    /// que se encuentran escritas en la tabla ConsultaProgramada.
    /// Cada método ACTION aquí tiene el nombre exacto que se carga
    /// en el campo ACTION de la tabla ConsultProgramada
    /// TODO registro en la tabla ConsultaProgramada debe tener un dato en ACTION
    /// que se encuentre en este Controller y si aun no se creo su ACTION
    /// se colocara el action por DEFUALT: FaltaCrearNuevoAction
    /// </summary>
    [Authorize]

    [AuthorizacionPermiso("ListaConsultaGeneral")]

    public class CallConsultaController : Controller
    {


        private SINUEntities db = new SINUEntities();
        /// <summary>Pantalla principal que genera el indice lateral de Consultas
        /// 
        /// </summary>
        /// <param name="id">parameto opcional que es el nro de id de consulta que deseo 
        /// este activada o seleccionada</param>
        /// <returns></returns>
        public ActionResult Index(int? id)
        {
            //Si no dan un id asume el que tenga id de consulta en 1, 
            //si no hay ninguno en id 1 simplemente no selecciona ninguno
            ViewBag.ActivarId = id ?? 9;

            IndexConsultorModel data = new IndexConsultorModel
            {
                ConsultaProgramadaVm = db.ConsultaProgramada.Where(m => m.IdConsulta != 9).OrderBy(m => m.OrdenConsulta).ToList(),
                EstadosEtapas = new SelectList(db.vSecuencia_EtapaEstado.Where(m => m.Estacional).ToList(), "Estado", "Estado", "Etapa", 1),// db.vSecuencia_EtapaEstado.Select(m=>new SelectListItem {Text= m.Estado, Value=m.Estado,Group= new SelectListGroup { Name=m.Etapa } }).ToList(),
                TablaVista = "vExportacionDatosBasicos",//"vInscripcionDetalleUltInsc",
                filtrosExtras = new List<filtroExtra>
                {
                    new filtroExtra{Columna="ESTADO" ,Valor= "ACTIVA"}
                },
                Columnas = new List<Column> {

                     ColumnaDTAjax("IdPostulantePersona",noPrint:true),
                     ColumnaDTAjax("IdConvocatoria",noPrint:true),
                     ColumnaDTAjax("IdOficinasYDelegaciones",noPrint:true),
                     ColumnaDTAjax("IdInscripcion", nombreDisplay:"Id"),
                     ColumnaDTAjax("Apellido",visible: true,searchable: true, orderable:true),
                     ColumnaDTAjax("Nombres", true,true,orderable:true),
                     ColumnaDTAjax("sexo",nombreDisplay:"Genero",noPrint:true),
                     ColumnaDTAjax("Deleg",nombreDisplay:"Delegación"),
                     ColumnaDTAjax("DNI",visible: true,searchable: true, orderable:true),
                     ColumnaDTAjax("fechanac",nombreDisplay:"Fecha de Nacimiento"),
                     ColumnaDTAjax("Telefono"),
                     ColumnaDTAjax("Email",visible: true,searchable: true, orderable:true),
                     ColumnaDTAjax("IdModalidad",true,true,"Modalidad", orderable:true),
                     ColumnaDTAjax("Etapa",true,true,"Etapa", orderable:true),
                     ColumnaDTAjax("EstadoEtapa",true,true,"Estado", orderable:true),
                     ColumnaDTAjax("celular_carac",nombreDisplay:"Caracteristica Celular",noPrint:true),
                     ColumnaDTAjax("Celular"),
                     ColumnaDTAjax("ESTADO",noPrint:true)
                }
            };
            //oculta las Consultas que no estan realizadas
            return View(data);
            //return View(db.ConsultaProgramada.OrderBy(m => m.OrdenConsulta).ToList());
        }

        /// <summary>
        /// Exportacion de datos para las Escuelas
        /// </summary>   
        public ActionResult exportacionDatosPersonales()
        {
            var columnasTotal = db.Database.SqlQuery<tipoColumna>($"SELECT COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS where table_name ='vExportacion_Escuelas'").ToList();
            var columanVisibles = new List<Column>
            {
                ColumnaDTAjax("idaspirante"),
                ColumnaDTAjax("Nroinscripcion",true,true,orderable:true,nombreDisplay:"N° Inscripción"),
                ColumnaDTAjax("NRODDNN"),
                ColumnaDTAjax("NomDeleg",true,nombreDisplay:"Delegación"),
                ColumnaDTAjax("NROMOD"),
                ColumnaDTAjax("Modalidad",true,orderable:true),
                ColumnaDTAjax("ModalidadDescrip"),
                ColumnaDTAjax("nroinscripto"),
                ColumnaDTAjax("apellido",true,true,orderable:true, nombreDisplay:"Apellido"),                
                ColumnaDTAjax("nombre",true,true,orderable:true,nombreDisplay:"Nombre"),
                ColumnaDTAjax("Calle"),
                ColumnaDTAjax("Numero"),
                ColumnaDTAjax("Piso"),
                ColumnaDTAjax("Unidad"),
                ColumnaDTAjax("Localidad"),
                ColumnaDTAjax("idprovincia"),
                ColumnaDTAjax("Provincia"),
                ColumnaDTAjax("cpostal"),
                ColumnaDTAjax("PAIS"),
                ColumnaDTAjax("Loc_Prov_CP"),
                ColumnaDTAjax("caracteristica"),
                ColumnaDTAjax("telefono"),
                ColumnaDTAjax("Email",true,true,orderable:true,className:"truncate"),
                ColumnaDTAjax("opcioncuerpo1"),
                ColumnaDTAjax("opcioncuerpo2"),
                ColumnaDTAjax("sexo"),
                ColumnaDTAjax("iddelegacion"),
                ColumnaDTAjax("Deleg"),
                ColumnaDTAjax("dni",true,true,orderable:true,nombreDisplay:"DNI"),                
            };
            columanVisibles.AddRange(columnasTotal.Where(m=>!columanVisibles.Select(m => m.data).ToList().Contains(m.COLUMN_NAME)).Select(m=> ColumnaDTAjax(m.COLUMN_NAME)).ToList());

            ViewBag.Modalidad = new SelectList(db.Modalidad.ToList(), "IdModalidad", "IdModalidad");
            ViewBag.Delegacion = new SelectList(db.OficinasYDelegaciones.ToList(), "Nombre", "Nombre");

            DataTableVM data = new DataTableVM
            {
                TablaVista = "vExportacion_Escuelas",
                Columnas = columanVisibles
            };
            return View(data);          
        }


        public ActionResult exportacionDatosBasicos()
        {

            DataTableVM datosTabla = new DataTableVM
            {
                TablaVista = "vExportacionDatosBasicos",
                Columnas = new List<Column> {
                     ColumnaDTAjax("IdPostulantePersona",nombreDisplay:"N Postulante"),
                     ColumnaDTAjax("IdOficinasYDelegaciones",noPrint:true),
                     ColumnaDTAjax("IdCarreraOficio",noPrint:true),
                     ColumnaDTAjax("IdInscripcion", nombreDisplay:"N Inscricion"),
                     ColumnaDTAjax("Apellido", true,true, orderable:true),
                     ColumnaDTAjax("Nombres", true,true,orderable:true),
                     ColumnaDTAjax("sexo",nombreDisplay:"Genero"),          
                     ColumnaDTAjax("DNI",true, true, orderable:true),
                     ColumnaDTAjax("fechanac",nombreDisplay:"Fecha de Nacimiento"),
                     ColumnaDTAjax("Telefono"),
                     ColumnaDTAjax("Celular"),
                     ColumnaDTAjax("Email", true, true, orderable:true),
                     ColumnaDTAjax("IdConvocatoria",nombreDisplay:"N Convocatoria"),
                     ColumnaDTAjax("IdModalidad",true,true,"Modalidad", orderable:true),
                     ColumnaDTAjax("CarreraUoficio",true,true,"Carrera/Oficio", orderable:true),
                     ColumnaDTAjax("Deleg",true,true,"Delegacion", orderable:true),
                     ColumnaDTAjax("FechaInscripcion",nombreDisplay:"Fecha de Inscripcion"),
                     ColumnaDTAjax("Etapa"),
                     ColumnaDTAjax("EstadoEtapa"),
                     ColumnaDTAjax("ESTADO",noPrint:true)
   
                },
                 
            };
            ViewBag.Modalidad = new SelectList(db.Modalidad.ToList(), "IdModalidad", "IdModalidad");
            ViewBag.Delegacion = new SelectList(db.OficinasYDelegaciones.ToList(), "Nombre", "Nombre");
            return View(datosTabla);

        }
        /// <summary>FaltaCrearNuevoAction Es una rutina modelo 
        /// que se usa como plantilla para crear nuevas actions para nuevas CONSULTAS PRINCIPALES.
        /// Tiene asociada una View que solo emite el mensaje que falta desarrollar elementos.
        /// 
        /// Si agregamos a la tabla ConsultasProgramadas nuevos registros ,
        /// en el campo CONTROLLER se colocara CallConsulta y 
        /// en el  campo ACTION se colocara el nombre de esta rutina 
        /// hasta que esté finalizada la programación de la que le corresponda.
        /// </summary>
        /// <returns></returns>
        public ActionResult FaltaCrearNuevoAction()
        {
            return PartialView();
        }

        /// <summary>TotalesPorModalidadyGenero es una CONSULTA Principal 
        /// que adentro tiene links hacia otras subconsultas
        /// Tiene el conteo por GENERO con Totales y subtotales por modalidad
        /// </summary>
        /// <returns></returns>
        public ActionResult TotalesPorModalidadyGenero()
        {
            HttpContext.Server.ScriptTimeout = 120000;
            List<sp_ConsultaInscriptosModalidadGenero_Result> Datos = db.sp_ConsultaInscriptosModalidadGenero().ToList();

            if (Datos == null)
            {
                //return HttpNotFound();
                return View("Error", Func.ConstruyeError("Extraño que no haya registros devueltos por sp_ConsultaInscriptosModalidadGenero", "CallConsulta", "TotalesPorModalidadyGenero"));

            }
            return PartialView(Datos);
        }

        /// <summary>Esta rutina InscriptosPorModalidad es una SUBCONSULTA que es disparada desde un link  
        /// generado en la CONSULTA TotalesPorModalidadyGenero
        /// Si la ModalidadElegida es TODOS muestra todos los inscriptos sin discriminar
        /// Si la ModalidadElegida es otra cosa muestra los datos según la modalidad elegida
        /// </summary>
        /// <param name="ModalidadElegida">Este parametro es la modalidad por la cual desea ver los datos filtrados</param>
        /// <returns>Devuelve los inscriptos de la vista vConsultaInscripciones pero filtrado por modalidad dada</returns>
        public ActionResult InscriptosPorModalidad(string ModalidadElegida, string Genero)
        {
            //busco el id que le corresponde a la consulta original TotalesPorModalidadyGenero
            ViewBag.ActivarId = db.ConsultaProgramada.Where(m => m.Action == "TotalesPorModalidadyGenero").Select(m => m.IdConsulta).FirstOrDefault();
            //si la modalidad elegida es string, el signo ?? Verifica si esta nula dicha var, asignandole lo q sigue a ella, en este caso "" , de lo contrario queda con su valor original
            ModalidadElegida = ModalidadElegida ?? "";
            
            DataTableVM datosTabla = new DataTableVM
            {
                TablaVista = "vExportacionDatosBasicos",
                Columnas = new List<Column> {
                     ColumnaDTAjax("IdPostulantePersona",noPrint:true),
                     ColumnaDTAjax("IdConvocatoria",noPrint:true),
                     ColumnaDTAjax("IdOficinasYDelegaciones",noPrint:true),
                     ColumnaDTAjax("IdInscripcion", nombreDisplay:"Id"),
                     ColumnaDTAjax("Apellido",visible: true,searchable: true, orderable:true),
                     ColumnaDTAjax("Nombres", true,true,orderable:true),
                     ColumnaDTAjax("sexo",nombreDisplay:"Genero",noPrint:true),
                     ColumnaDTAjax("Deleg",nombreDisplay:"Delegación"),
                     ColumnaDTAjax("DNI",visible: true,searchable: true, orderable:true),
                     ColumnaDTAjax("fechanac",nombreDisplay:"Fecha de Nacimiento"),
                     ColumnaDTAjax("Telefono"),
                     ColumnaDTAjax("Email",visible: true,searchable: true, orderable:true),
                     ColumnaDTAjax("IdModalidad",true,true,"Modalidad", orderable:true),
                     ColumnaDTAjax("Etapa",true,true,"Etapa", orderable:true),
                     ColumnaDTAjax("EstadoEtapa",true,true,"Estado", orderable:true),
                     ColumnaDTAjax("celular_carac",nombreDisplay:"Caracteristica Celular",noPrint:true),
                     ColumnaDTAjax("Celular"),
                     ColumnaDTAjax("ESTADO",noPrint:true)
                }
            };
            if (ModalidadElegida == "TODOS")
            {
                datosTabla.filtrosExtras = new List<filtroExtra> { new filtroExtra { Columna = "ESTADO", Valor = "ACTIVA" } };
                ViewBag.ModalidadElegida = "Todas las Modalidades";
                ViewBag.CantReg = db.vExportacionDatosBasicos.Where(m => m.ESTADO == "ACTIVA").Count();
            }
            else
            {
                datosTabla.filtrosExtras = new List<filtroExtra> {
                    new filtroExtra { Columna = "sexo", Valor = Genero },
                    new filtroExtra { Columna="IdModalidad",Valor=ModalidadElegida }
                };
                ViewBag.modalidadElegida = ModalidadElegida + " - Genero: " + Genero;
                ViewBag.CantReg = db.vExportacionDatosBasicos.Where(m => m.sexo == Genero && m.IdModalidad == ModalidadElegida).Count();
            }
            return View(datosTabla);
        }


        /// <summary>ConsultaTotalPostulantesEs una CONSULTA principal (agregada la subconsulta VerPostulanteElegido)
        /// que muestra un simple listado de los postulantes que se encuentra realmente 
        /// en la etapa de inscripcion más allá de la etapa 5 que equivale a todos aquellos
        /// que pasaron la VALIDACION DEL REGISTRO INCIAL y se encuentra en un estado
        /// de carga de DATOS BASICOS o posterior.
        /// </summary>
        /// <returns></returns>
        public ActionResult ConsultaTotalPostulantes()
        {
            //List<vInscripcionEtapaEstadoUltimoEstado> Todos;
            //Todos = db.vInscripcionEtapaEstadoUltimoEstado.Where( m => m.IdSecuencia >= 5).ToList();
            //return PartialView(Todos);
            List<vInscripcionEtapaEstadoUltimoEstado> Todos;
            Todos = db.vInscripcionEtapaEstadoUltimoEstado.Where(m => (bool)m.Activa).ToList();
            return PartialView(Todos);

        }

        public ActionResult ConsultaDelegacionPrincipal()
        {

            List<OficinasYDelegaciones> listoficinas = db.OficinasYDelegaciones.ToList();

            foreach (var item in listoficinas)
            {
                item.Count = db.vInscripcionEtapaEstadoUltimoEstado.Where(m => m.IdDelegacionOficinaIngresoInscribio == item.IdOficinasYDelegaciones && (bool)m.Activa).Count();
            }
            return PartialView(listoficinas);

        }

        ///<summary>Consulta por todas las Delegaciones o solamente una 
        //public ActionResult ConsultaPorDelegacion(string DelegacionSeleccionada)
        //{
        //    //busco el id que le corresponde a la consulta original Consulta por Delegacion
        //    ViewBag.ActivarId = db.ConsultaProgramada.Where(m => m.Action == "ConsultaDelegacionPrincipal").Select(m => m.IdConsulta).FirstOrDefault();
        //    DelegacionSeleccionada = DelegacionSeleccionada ?? "TODAS";
        //    List<vConsultaInscripciones> ListadoDelegaciones;

        //    if (DelegacionSeleccionada == "TODAS")
        //    {
        //        ListadoDelegaciones = db.vConsultaInscripciones.ToList();
        //        ViewBag.DelegacionSeleccionada = "Todas las Delegaciones";
        //    }
        //    else
        //    {
        //        ListadoDelegaciones = db.vConsultaInscripciones.Where(d => d.Delegacion == DelegacionSeleccionada).ToList();
        //        ViewBag.delegacionSeleccionada = DelegacionSeleccionada;
        //    }
        //    return PartialView(ListadoDelegaciones);

        //}

        public ActionResult ConsultaPorDelegacion(string DelegacionSeleccionada)
        {
                int idDeleOfi = db.OficinasYDelegaciones.FirstOrDefault(m => m.Nombre == DelegacionSeleccionada).IdOficinasYDelegaciones;
            DataTableVM datosTabla = new DataTableVM
            {
                TablaVista = "vExportacionDatosBasicos",
                Columnas = new List<Column> {
                     ColumnaDTAjax("IdPostulantePersona",noPrint:true),
                     ColumnaDTAjax("IdConvocatoria",noPrint:true),
                     ColumnaDTAjax("IdOficinasYDelegaciones",noPrint:true),
                     ColumnaDTAjax("IdInscripcion", nombreDisplay:"Id"),
                     ColumnaDTAjax("Apellido",visible: true,searchable: true, orderable:true),
                     ColumnaDTAjax("Nombres", true,true,orderable:true),
                     ColumnaDTAjax("sexo",nombreDisplay:"Genero",noPrint:true),
                     ColumnaDTAjax("Deleg",nombreDisplay:"Delegación"),
                     ColumnaDTAjax("DNI",visible: true,searchable: true, orderable:true),
                     ColumnaDTAjax("fechanac",nombreDisplay:"Fecha de Nacimiento"),
                     ColumnaDTAjax("Telefono"),
                     ColumnaDTAjax("Email",visible: true,searchable: true, orderable:true),
                     ColumnaDTAjax("IdModalidad",true,true,"Modalidad", orderable:true),
                     ColumnaDTAjax("Etapa",true,true,"Etapa", orderable:true),
                     ColumnaDTAjax("EstadoEtapa",true,true,"Estado", orderable:true),
                     ColumnaDTAjax("celular_carac",nombreDisplay:"Caracteristica Celular",noPrint:true),
                     ColumnaDTAjax("Celular"),
                     ColumnaDTAjax("ESTADO",noPrint:true)
                },
                filtrosExtras = new List<filtroExtra>() {
                                   new filtroExtra {Columna="IdOficinasYDelegaciones", Valor=idDeleOfi.ToString() },
                                   new filtroExtra {Columna="ESTADO", Valor="ACTIVA" },
                }
            };
            //db.vConsultaInscripciones.Where(m => m.Fecha_Fin_Proceso >= DateTime.Today && m.Fecha_Inicio_Proceso <= DateTime.Today && m.Delegacion == DelegacionSeleccionada).ToList();
            ViewBag.CantReg = db.vExportacionDatosBasicos.Where(m => m.ESTADO == "ACTIVA" && m.IdOficinasYDelegaciones == idDeleOfi).Count();
            ViewBag.delegacionSeleccionada = DelegacionSeleccionada;
            ViewBag.ActivarId = db.ConsultaProgramada.Where(m => m.Action == "ConsultaDelegacionPrincipal").Select(m => m.IdConsulta).FirstOrDefault();
            ViewBag.EstadosEtapas = new SelectList(db.vSecuencia_EtapaEstado.Where(m => m.Estacional).ToList(), "Estado", "Estado", "Etapa", 1);

            return View(datosTabla);
        }

        /// <summary>Esta Action es llamada desde la consulta PRINCIPAL de TODOS LOS POSTULANTES :ConsultaTotalPostulantes
        /// Esta action es una SUBCONSULTA de ConsultaTotalPostulantes
        /// </summary>
        /// <param name="IdPostulantePersona"></param>
        /// <returns></returns>
        public ActionResult VerPostulanteElegido(int? IdPostulantePersona)
        {
            //el 1033 es por conveniencia de OTTINO-- eliminar 10033 y poner 0
            IdPostulantePersona = (IdPostulantePersona is null) ? 1033 : IdPostulantePersona;
            Postulante x = db.Postulante.FirstOrDefault(p => p.IdPersona == IdPostulantePersona);
            if (x is null)
            {
                return View("Error", Func.ConstruyeError("Extraño que no encuentre Postulante: " + IdPostulantePersona.ToString(), "CallConsulta", "VerPostulanteElegido"));
            }
            return RedirectToAction("Index", "Postulante", new { ID_Postulante = IdPostulantePersona });
        }


        //Consulta la Convocatoria que tiene personal de inscriptos
        //Es una Consulta Principal Consulta la Convocatoria. Tendrá subconsultas
        //public ActionResult TotalizarPorConvocatoria()
        //{

        //    List<vInscriptosCantYTODASConvocatorias> ListadoConvocatorias;

        //    //ListadoConvocatorias = db.vInscriptosCantYTODASConvocatorias.Where(m => m.CantInscriptos > 0).ToList();
        //    ListadoConvocatorias = db.vInscriptosCantYTODASConvocatorias.ToList();

        //    return PartialView(ListadoConvocatorias);

        //}


        public ActionResult TotalizarPorConvocatoria()
        {
            List<vInscriptosCantYTODASConvocatorias> ListadoConvocatorias;
            ListadoConvocatorias = db.vInscriptosCantYTODASConvocatorias.ToList();
            ViewBag.Activas = 0;
            ViewBag.ActivarId = db.ConsultaProgramada.Where(m => m.Action == "TotalizarPorConvocatoria").Select(m => m.IdConsulta).FirstOrDefault();

            return PartialView(ListadoConvocatorias);
        }

        public ActionResult TotalizarPorConvocatoriaActivas()
        {
            List<vInscriptosCantYTODASConvocatorias> ListadoConvocatorias;
            //ListadoConvocatorias = db.vInscriptosCantYTODASConvocatorias.Where(m => m.Fecha_Fin_Proceso > DateTime.Today).ToList();
            ListadoConvocatorias = db.vInscriptosCantYTODASConvocatorias.Where(m => m.Fecha_Fin_Proceso >= DateTime.Today && m.Fecha_Inicio_Proceso <= DateTime.Today).ToList();
            ViewBag.Activas = 1;
            ViewBag.ActivarId = db.ConsultaProgramada.Where(m => m.Action == "TotalizarPorConvocatoria").Select(m => m.IdConsulta).FirstOrDefault();

            return View("TotalizarPorConvocatoria", ListadoConvocatorias);
        }


        //Subconsulta de TotalizarPorConvocatoria.
        //Habiendo elegido una convocatoria en TotalizarPorConvocatoria 
        //esto muestra el detalle de los postulantes y si cumplen o no las restricciones
        public ActionResult TotalesConvocatoriaDetalle(int? IdConvocatoria)
        {
            try
            {
                IdConvocatoria ??= db.vConvocatoriaDetalles.First().IdConvocatoria;
                ConsultaPorConvocatoria data = new ConsultaPorConvocatoria
                {
                    //siempre es conveniente fijarse si me pasa null pues no se ir al where con null en estos casos
                    idConvocatoria = (IdConvocatoria is null) ? 0 : (int)IdConvocatoria,
                    infoConvocatoria = db.vConvocatoriaDetalles.First(m => m.IdConvocatoria == IdConvocatoria),
                    restriccionesConvocatoria = db.sp_Totales_FullRestriccion("", (int)IdConvocatoria).ToList(),
                    tablaModel = new DataTableVM
                    {
                        TablaVista = "vExportacionDatosBasicos",
                        Columnas = new List<Column> {
                             ColumnaDTAjax("IdPostulantePersona",noPrint:true),
                             ColumnaDTAjax("IdCarreraOficio",noPrint:true),
                             ColumnaDTAjax("IdConvocatoria",noPrint:true),
                             ColumnaDTAjax("IdOficinasYDelegaciones",noPrint:true),
                             ColumnaDTAjax("IdInscripcion", nombreDisplay:"Id"),
                             ColumnaDTAjax("Apellido",visible: true,searchable: true, orderable:true),
                             ColumnaDTAjax("Nombres", true,true,orderable:true),
                             ColumnaDTAjax("sexo",nombreDisplay:"Genero",noPrint:true),
                             ColumnaDTAjax("Deleg",nombreDisplay:"Delegación"),
                             ColumnaDTAjax("DNI",visible: true,searchable: true, orderable:true),
                             ColumnaDTAjax("fechanac",nombreDisplay:"Fecha de Nacimiento"),
                             ColumnaDTAjax("Telefono"),
                             ColumnaDTAjax("Email",visible: true,searchable: true, orderable:true,className:"truncate"),
                             ColumnaDTAjax("IdModalidad",true,true,"Modalidad", orderable:true),
                             ColumnaDTAjax("CarreraUoficio",true,true, nombreDisplay:"Carrera/Oficio",noPrint:true),
                             ColumnaDTAjax("Etapa",true,true,"Etapa", orderable:true),
                             ColumnaDTAjax("EstadoEtapa",true,true,"Estado", orderable:true),
                             ColumnaDTAjax("celular_carac",nombreDisplay:"Caracteristica Celular",noPrint:true),
                             ColumnaDTAjax("Celular"),
                             ColumnaDTAjax("ESTADO",noPrint:true)
                        },
                        filtrosExtras = new List<filtroExtra>() {
                                    new filtroExtra {Columna="IdConvocatoria", Valor=IdConvocatoria.ToString() },
                                    new filtroExtra {Columna="IdCarreraOficio", Condicion="!=", Valor="null" }
                        }
                    }
                };
                ViewBag.poseeInscriptos = data.restriccionesConvocatoria.Count() > 1;
                //busco el id que le corresponde a la consulta original TotalizarPorConvocatoria
                ViewBag.ActivarId = db.ConsultaProgramada.Where(m => m.Action == "TotalizarPorConvocatoria").Select(m => m.IdConsulta).FirstOrDefault();

                return View(data);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        public JsonResult CheckPostulante(int? idPostulante)
        {

            var check = db.vInscriptosYRestriccionesCheck.FirstOrDefault(m => m.IdPostulantePersona == idPostulante);
            return Json(new { check = check }, JsonRequestBehavior.AllowGet);
        }
        //Subconsulta de TotalizarPorConvocatoria.
        //Habiendo elegido una convocatoria en TotalizarPorConvocatoria 
        //fue a la pantallla de TotalesConvocatoriaDetalle y ahi existe un boton que llama a esta pantalla
        //o si va por el menu principal me pasa un idconvocatoria en null
        public ActionResult TotalesConvocatoriaTitulos(int? IdConvocatoria)
        {
            List<vInscriptosconTitulosProblemas> InscriptosconTitulosProblemas;

            if (IdConvocatoria is null)
            {
                //considero que se hizo click en una opcion del menu ppal de consultas
                //no se filtra por convocatoria pero solo se muestran las activas
                InscriptosconTitulosProblemas = db.vInscriptosconTitulosProblemas.Where(m => m.Fecha_Fin_Proceso > DateTime.Now)
                                                                                 .ToList();
                ViewBag.vInscriptosConTitulosProblemasCount = db.vInscriptosConTitulosProblemasCount.Where(m => m.Fecha_Fin_Proceso > DateTime.Now).ToList();

                //Para Probar sin pedir las convocatorias ACTIVAS
                //InscriptosconTitulosProblemas = db.vInscriptosconTitulosProblemas
                //                                                                 .ToList();
                //ViewBag.vInscriptosConTitulosProblemasCount = db.vInscriptosConTitulosProblemasCount
                //                                                                   .ToList();
                ViewBag.CantToPers = InscriptosconTitulosProblemas.Select(m => m.IdInscripcion).Distinct().Count();
                return PartialView(InscriptosconTitulosProblemas);
            }
            else
            {
                //busco el id que le corresponde 
                ViewBag.ActivarId = IdConvocatoria;
                InscriptosconTitulosProblemas = db.vInscriptosconTitulosProblemas.Where(m => m.IdConvocatoria == IdConvocatoria).ToList();
                ViewBag.CantToPers = InscriptosconTitulosProblemas.Select(m => m.IdInscripcion).Distinct().Count();
                ViewBag.vInscriptosConTitulosProblemasCount = db.vInscriptosConTitulosProblemasCount.Where(m => m.Idconvocatoria == IdConvocatoria).ToList();
                return View(InscriptosconTitulosProblemas);
            }


        }




    }
}
