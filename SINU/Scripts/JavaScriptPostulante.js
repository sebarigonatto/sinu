//configuraciones por default de las DataTables
$.extend(true, $.fn.dataTable.defaults, {
    responsive:
    {
        details: false
    },
    "autoWidth": true,

    //ver si es necesario este fragmento
    select: false,//'single',
    "columnDefs": [{
        "searchable": false,
        "orderable": false,
        "targets": [0]
    }],
    "ordering": false,
    "dom": 'frt',
    "language":
    {
        "sProcessing": "Procesando...",
        "sLengthMenu": "Mostrar _MENU_ registros",
        "sZeroRecords": "No se encontraron resultados",
        "sEmptyTable": "Ningún dato disponible en esta tabla",
        "sInfo": "Mostrando registros del _START_ al _END_ de un total de _TOTAL_ registros",
        "sInfoEmpty": "Mostrando registros del 0 al 0 de un total de 0 registros",
        "sInfoFiltered": "(filtrado de un total de _MAX_ registros)",
        "sInfoPostFix": "",
        "sSearch": "Buscar:",
        "sUrl": "",
        "sInfoThousands": ",",
        "sLoadingRecords": "Cargando...",
        "oPaginate": {
            "sFirst": "Primero",
            "sLast": "Último",
            "sNext": "Siguiente",
            "sPrevious": "Anterior"
        },
        "oAria": {
            "sSortAscending": ": Activar para ordenar la columna de manera ascendente",
            "sSortDescending": ": Activar para ordenar la columna de manera descendente"
        },
    }

});


$(document).ready(function () {

    $.arrayEtapas ;
    
    //ver esto ya que en el navegador aparece UNA aadvertencia relacionado si es asincronico o sincronico
    $.ajaxSetup({
        async: false
    });

    //cargo en "id_persona" el id de la persona que se esta llenando los datos
    var id_persona
    (function () {
        id_persona = $("#vPersona_DatosBasicosVM_IdPersona").val();
        $("#fechacumpleaños").attr("data-date-end-date", "0d");
        //alert(id_persona);
    })();


    $(document).on('change', '.custom-file-input', function (event) {
        $(this).next('.custom-file-label').html(event.target.files[0].name);
    })

    //ver si  el modo de como ocultar el datepicker al seleccionar la fecha
    /*FUNCION DE LA VISTA DE DATOS PERSONALES */
    $(".datepicker").datepicker({
        format: "dd-mm-yyyy",
        language: "es",
        autoclose: true,
        startView: "years",

    });

    //se aplicael selecpicker a alos conbo/s con autocomplete con la opcion de busqueda
    //https://developer.snapappointments.com/bootstrap-select/
    $(".combobox").selectpicker({
        liveSearch: true,
        size: 7,
        liveSearchPlaceholder: "Ingrese su busqueda...",
        liveSearchStyle: 'contains',//'startsWith'
        noneResultsText: 'No se Encuantran Resultados',
        noneSelectedText: 'Seleccione una Opcion',
        mobile:true
        //styleBase: 'form-control',
        //style: 'btn btn-white'
    });
    $(".selectpicker").selectpicker({
        size: 7,
        noneSelectedText: 'Seleccione una Opcion',
        //styleBase:'btn',
        //style: 'btn-white'

    });


    ///////////////////////////////////////////////////////////////////////////////  DATOS BASICOS  /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    if ($("#edad").val() > 35 || $("#edad").val() < 17 && $("#edad").val() != 0) {
        alert($("#edad").val());
        edadMAXMIN($("#edad").val());
    }

    if ($("#edad").val() > 0) {
        ActualizarINStDatosBasicos();
    }

    //cuando se selecciona una fecha se calcula la edad, la misma se muestra en el campo de EDAD
    $('#fechacumpleaños').datepicker().on("changeDate", function (e) {
        var fechanac = $('#fechacumpleaños').datepicker('getDate');
        var fechahoy = new Date();
        var edad = fechahoy.getFullYear() - fechanac.getFullYear();
        //condicion que verefica si cumplio años, si no cumplio aun se le resta un año a edad
        if (fechahoy.getMonth() <= fechanac.getMonth()) {
            if (!(fechahoy.getDate() >= fechanac.getDate() && fechahoy.getMonth() == fechanac.getMonth())) {
                edad--;
            };
        };
        $("#edad").val(edad);
        //si la edad supera los 35 muestro el modal advirtiendole
        edadMAXMIN(edad);
        // CARGO EL COMBO DE INSTITUCIONES SEGUN LA FECHA DE CUMPLEAÑOS 
        ActualizarINStDatosBasicos();

    });

    function edadMAXMIN(edad) {
        if (edad > 35 && $("#idetapaactual").val() == 2) {
            $("#BTNModal").html("Cerrar");
            $("#GuardarDTF").css("display", "none");
            $("#ModalCenterTitle").html("SINU:");
            $("#TextModal").html("Su edad supera las edades maximas permitidas de los distintos Institutos.");
            $("#ModalAnuncios").modal();
        } else if (edad != 0 && edad < 17 && $("#idetapaactual").val() == 2) {
            $("#BTNModal").html("Cerrar");
            $("#GuardarDTF").css("display", "none");
            $("#ModalCenterTitle").html("SINU:");
            $("#TextModal").html("Su edad es menor a las edades minimas permitidas de los distintos Institutos.");
            $("#ModalAnuncios").modal();
        }
    }
    function ActualizarINStDatosBasicos() {
        $.get("/Postulante/EdadInstituto",
            {
                IdPOS: $("#vPersona_DatosBasicosVM_IdPersona").val(),
                Fecha: $("#fechacumpleaños").val().toString()

            },
            function (data) {

                var idselect = $("#InstitutoPref").val();
                $("#InstitutoPref").empty();
                $("#InstitutoPref").append('<option value="">' + 'Seleccione una Opcion' + '</option>');

                $.each(data.institucion, function (index, row) {
                    if (row.Value == idselect) {
                        $("#InstitutoPref").append("<option selected = 'selected' value='" + row.Value + "'>" + row.Text + "</option>")
                    } else {
                        $("#InstitutoPref").append("<option value='" + row.Value + "'>" + row.Text + "</option>")
                    }


                });

                $("#InstitutoPref").removeAttr("disabled");
                $("#InstitutoPref").selectpicker('refresh');



            })
    }

    ComoSeEntero()
    $('#DROPComoEntero').on('changed.bs.select', function (e, clickedIndex, isSelected, previousValue) {
        ComoSeEntero();

    });
    function ComoSeEntero() {
        if ($("#DROPComoEntero").val() > 2 && $("#DROPComoEntero").val() != 8) {
            $("#IdComentario").show()
        } else {
            $("#IdComentario").hide();
            $("#IdComentario input").val("");
        }
    }

    ////////////////////////////DATOS PERSONALES///////////////////////////////////

    $("#vPersona_DatosPerVM_IdCarreraOficio").selectpicker();

    ActualizaCarrerasCivil($("#vPersona_DatosPerVM_IdModalidad").val());
    $("#vPersona_DatosPerVM_IdModalidad").on('changed.bs.select', function () {
        //var idInscr = $("#vPersona_DatosPerVM_IdInscripcion").val();
        var modalidad = $(this).val();
        //alert(modalidad); 
        $("#vPersona_DatosPerVM_IdCarreraOficio").val("");
        ActualizaCarrerasCivil(modalidad)

    });

    function ActualizaCarrerasCivil(modalidad) {
        $("#vPersona_DatosPerVM_IdCarreraOficio option").each(function (index, element) {
            if ($(element).attr("modali") == modalidad && $(element).val() != "") {
                $(element).removeAttr("hidden");
            } else if ($(element).val() != "") {
                $(element).attr("hidden", true);
            }
            if ($("#vPersona_DatosPerVM_edad").val() > 30) {

            }
        })

        $("#vPersona_DatosPerVM_IdCarreraOficio").selectpicker('refresh');

        var estCivil = $("#vPersona_DatosPerVM_IdModalidad option:selected").attr("civil");
        //alert(estCivil);
        $("#vPersona_DatosPerVM_IdEstadoCivil option").each(function (index, element) {
            if (estCivil!="") {
                if ($(element).val() == estCivil && $(element).val() != "") {
                    $(element).removeAttr("hidden");
                } else if ($(element).val() != "") {
                    $(element).attr("hidden", true);
                }
            } else {
                $(element).removeAttr("hidden");
            }


        })
        $("#vPersona_DatosPerVM_IdEstadoCivil").selectpicker('refresh');
    }

    $("#vPersona_DatosPerVM_idTipoNacionalidad").on("changed.bs.select", function () {

        var modali = $("#vPersona_DatosPerVM_IdModalidad").val();
        if (modali != "SMV" && $(this).val() == 3) {

            $("#BTNModal").html("Aceptar");
            $("#GuardarDTF").css("display", "none");
            $("#ModalCenterTitle").html("SINU:");
            $("#TextModal").html("Al menos uno de sus padres debe ser argentino nativo y haber formalizado tramite ante el Ministerio del Interior. Comunicarse con Delegacion Naval para acreditar documentacion.");
            $("#ModalAnuncios").modal();

        };

    });


    /////////////////////////////////////////////////////////////////////////////////
    /*FUNCION DE LA VISTA DE DOMICILIOS */

    //evento que se desata al elegirse un pais, provincia y localidad
    $("#BeginFormDomicilio select").on('changed.bs.select', function (e, clickedIndex, isSelected, previousValue) {
        comboid = $(this).attr("id");
        //alert(comboid);
        switch (comboid) {
            case "ListaPaisR":
            case "ListaPaisE":
                PAIS(comboid, 1)
                break;
            case "ComboProvinciaR":
            case "ComboProvinciaE":
                PROVINCIA(comboid);
                break;
            case "ComboLocalidadR":
            case "ComboLocalidadE":
                LOCALIDAD(comboid);
                break;
            default:
        }
    });

    //al crgar la pagina se verifica si el pais del domiciolio REAL es "AR"
    PAIS("ListaPaisR", 0);
    PAIS("ListaPaisE", 0);



    ////si se recibe 0 es carga inicial de la pagina y nose se limpian los campos, si es 1 se limpia los campos
    function PAIS(Combo, PRI) {

        //condicion donde selecciono un pais, se cargan los campos de domiciolio real o eventual segun corresponda
        var Comboelemt = (Combo == "ListaPaisR") ?
            [".Real", ".RealAR", "#CPR"]
            : [".Eventual", ".EventualAR", "#CPE"];

        //alert($("#" + Combo).val());
        if ($("#" + Combo).val() != "AR") {
            $(Comboelemt[0]).show();
            $(Comboelemt[1]).hide();
            //se inhabilita la edicion del campo para el codigo postal
            $(Comboelemt[2]).attr("readonly", false);
        } else {
            $(Comboelemt[0]).hide();
            $(Comboelemt[1]).show();
            //se habilita la edicion del campo para el codigo postal
            $(Comboelemt[2]).attr("readonly", true);
        };

        //limpio los campos de "provincia,localidad,codigopostal" si seselecciona otro pais
        if (PRI == 1) {

            $(Comboelemt[0] + "," + Comboelemt[2]).val("");
            $(Comboelemt[1]).selectpicker('val', '');
        }
    };

    //Funcion de Provincia que actualiza los combobox de localidad
    function PROVINCIA(Combo) {

        var ValP = $("#" + Combo).val();
        if (ValP != "") {
            //alert(Combo + " " + ValP);
            var ComboLocalidad = (Combo == "ComboProvinciaR") ? "#ComboLocalidadR" : "#ComboLocalidadE";
            //limpio el combo de las localidades, para cargar las licalidades de la provincia seleccionada
            $(ComboLocalidad).empty();

            //llamo al JsonResult '/Postulante/DropEnCascadaDomicilio' y le envio la provincia seleccionada
            $.getJSON('/Postulante/DropEnCascadaDomicilio', {
                Provincia: ValP,
            },
                function (data) {
                    //agrego al dropboxlist la etiqueta "option" con cada localidad que le corresponde a la provincia seleccionada
                    $(ComboLocalidad).append('<option value="">' + 'Seleccione una Localidad...' + '</option>');
                    $.each(data, function () {
                        $(ComboLocalidad).append('<option value=' + this.Value + '>' + this.Text + '</option>');
                    });
                    //para actualizar el combobox
                    $(ComboLocalidad).selectpicker('refresh');
                });
        }

    };

    //se actualiaz el codigo postal deacuerdo a la localidad seleccionada
    function LOCALIDAD(Combo) {
        var ComboCP = (Combo == "ComboLocalidadR") ? "#CPR" : "#CPE";
        //llamo al JsonResult '/Postulante/EnCascada' y le envio la localidad seleccionada
        var valCP = $("#" + comboid).val();
        if (valCP != "") {
            $.getJSON('/Postulante/DropEnCascadaDomicilio', {
                Localidad: valCP,
            },
                function (data) {
                    //agrego al dropboxlist la etiqueta option con cada localidad que le corresponde a la provincia seleccionada
                    $(ComboCP).val(data.Text);
                });
            ////para actualizar el combobox
        }
    };

    //////////////////////////////////////////////////////////////////////////////
    /* FUNCION DE LA VISTA DE ESTUDIOS */

    //aplico DATATABLES a las tablas de ESTUDIO, IDIOMA Y ACTIVIDAD MILITAR
    TablasEIA()

    //funcion para aplicar datatable a la tabla estudio en la primera carga y actualizar la vista parcial de estudio
    function TablasEIA() {
        var Tabla = $('table').DataTable();
        //al seleccionar una fila
        //guardo el index de la fila seleccionada
        //se llama al modal y se le envia la id de estudio correspondiete

        $(".Agrega").on("click", function () {
            id_registro = $(this).attr("data-ID");
            id_tabla = id = $(this).closest("table").attr("ID");
            //alert(id_registro+ "  " +id_tabla); 
            ModalEIACUD(id_registro, id_persona, id_tabla);
            $("#ModalEIA").modal("show");
        });


        //Tabla.on('select.dt', function (e, dt, type, index) {
        //    var id_Tabla = $(this).attr("id");
        //    //solo utilizo modal para las que no tengan el id TABLAFAMILIA
        //    if (id_Tabla != "TablaFamilia") {
        //        var data = dt.rows(index).data();
        //        var id_registro = data[0][0];
        //        //llamo a la funcion para mostrar el modal y le envio 2 paremtros
        //        ModalEIACUD(id_registro, id_persona, id_Tabla);
        //        $("#ModalEIA").modal("show");
        //    };
        //});
    };

    //se llama al modal para cargar un nuevo registro dependiendo la tabla  a acualizar
    $(".Nuevo_REG").on("click", function () {
        var id_Tabla = $(this).attr("data-IdTabla");
        ModalEIACUD(null, id_persona, id_Tabla);
    });

    //VARIABLES PARA LAS DIRECCIONES DE LA VISTA PARCIAL, PARA ELIMINAR O ENVIAR LA MODIFICACION
    var url_Tabla;
    var url_CUD;
    var url_Elim;

    //armado el modal con la vista parcial correspondiente
    //recibe 2 parametros 
    //id_registro: id del registro a modificar o NULL en caso de agregar un nuevo registro
    //id_Tabla: id de la tabla con lo datos que se va trabajar
    function ModalEIACUD(id_registro, id_persona, id_Tabla) {

        //elimino el contenido html del modal
        $('#ModalEIACuerpo').html("");

        //cargo la url que se utilizara para el armado del MODAL
        //estos datos esta como atributos de las distintas tablas
        url_Tabla = $("#" + id_Tabla).attr("data-URL");
        url_CUD = $("#" + id_Tabla).attr("data-CUD");
        url_Elim = $("#" + id_Tabla).attr("data-ELI");
        

        $.ajax({
            cache: false,
            asyn: false,
            type: "GET",
            url: '/Postulante/' + url_CUD,
            data: { ID: id_registro, ID_persona: id_persona },
            //si no surge error al redireccionar se reemplaza el contenido de la div
            success: function (response) {
                $('#ModalEIACuerpo').html(response);

                //con esto  funciona la validacion del lado del cliente con la vista parcial
                $('#ModalEIACuerpo').removeData("validator");
                $('#ModalEIACuerpo').removeData("unobtrusiveValidation");
                $.validator.unobtrusive.parse('#ModalEIACuerpo');

                //se aplicael selecpicker a alos conbo/s con autocomplete Y busqueda
                /* https://developer.snapappointments.com/bootstrap-select/ */
                $(".combobox").selectpicker({
                    liveSearch: true,
                    size: 7,
                    liveSearchPlaceholder: "Ingrese su busqueda",
                    liveSearchStyle: 'contains',//'startsWith'
                    noneResultsText: 'No se Encuantran Resultados',
                    noneSelectedText: 'Ninguna Opcion Seleccionada'
                });
                //se aplica datepicker a los campos que requieran el ingreso de fecha
                $(".datepicker").datepicker({
                    format: "dd-mm-yyyy",
                    language: "es",
                    autoclose: true,
                    startView: "years",
                });
                //se aplica el selectpicker basico
                $(".selectpicker").selectpicker({ size: 7 });

                //ver remuevo el boton de guardado
                
                if ($.arrayEtapas) {
                    //alert("estapa 5 321")
                    $(".BTAcciones").html("");
                    $("#ModalEIACuerpo :input,#TabDocumentacion input").attr("disabled", "true");
                    $(".BTMuestraTable :input").removeAttr("disabled");
                }
                ////////////////////////////ESTUDIOS///////////////////////////////////

                //evento que se desata cuando se selecciona un opcion de los combobox
                $("#ComboJuriEST,#ComboLocaliEST").on('changed.bs.select', function (e, clickedIndex, isSelected, previousValue) {
                    comboid = $(this).attr("id");
                    //no se realiza nada si el evento fue desatado por el combo de INSTITUTO
                    if (comboid != "ComboIdInstEST") {
                        //alert(comboid);
                        valcombo = $('#' + comboid + ' option:selected').html();
                        ComboCascada(comboid, valcombo);
                    };
                });

                //lamo la funcion INSTEXT y mando cero por que esla primera carga
                INST_EXT(0);

                //sie estudio en el exterior oculto los combobox de Institutos Argentinos
                if ($("#IdInstEST").val() != "" && id_registro != null) {
                    $("#DropdownEXT option[value='true']").attr("selected", true);
                } else {
                    $("#DropdownEXT option[value='false']").attr("selected", true);
                };

                //si  instituto en el exterior cambia de los campos de instituto
                $("#DropdownEXT").on("change.bs.select", function () {
                    INST_EXT(1);
                });
                //verifico si egreso o no para mostrar/ocultar ciertos campos del formulario
                EgresoSINO();
                $("#TerminoEST").on("change", function () {
                    EgresoSINO();
                });

                //consulto si esta cursando el ultimo año
                UltimoAñoSINO();
                $("#UltimoAño").on("changed.bs.select", function () {
                    UltimoAñoSINO();
                });

                function UltimoAñoSINO() {
                    //alert("asda");
                    if ($("#UltimoAño").val() == "true") {
                        $("#CANTMATERIA, #ULT_AÑO").hide()
                        $("#CANTMATERIA input, #ULT_AÑO input").val("");
                    } else {
                        $("#CANTMATERIA, #ULT_AÑO").show();
                    }
                }
                /////////////////////////ACTIVIDAD MILITAR//////////////////////////////////

                IngreSINO();
                $("#IngresoSINO").on("change", function (e) {
                    IngreSINO()
                });

                /////////////////////////////////GUARDA////////////////////////////////////
                //al guardar los registros de un formulario de una vista parcial confirmo que la validacion de los campos
                //si es .valid() es falso, muestro los errores y no cierro el modal
                $(".Guardar_REG").on("click", function () {
                    var form_actual = "#" + this.getAttribute("data-form");
                    //alert(form_actual);
                    var valido = $(form_actual).valid();
                    $(form_actual).submit()
                    if (valido) {
                        $("#ModalEIA").modal("hide");
                        ActualizaTabla();
                    };
                });


            },

            //si ocurre un error de no aurtorizacion redirige ala pagina de error del mismo
            statusCode: {
                500:
                    function (context) {
                        $('#ModalEstudioCuerpo').html(context.responseText);
                    }
            }
        });
    };

    ///////////////////////////////ELIMINA///////////////////////////////////

    //ELIMINA EL ESTUDIO SELECCIONADO
    $.ActualizaTabla = function (tabla) {
        url_Tabla = tabla;
        //oculta el modal
        ActualizaTabla();
    };

    //se se actualiza la vista parcial de la tabla en el caso que se elimine, modifique o se agregue un registro
    function ActualizaTabla() {
        //alert(id_persona + url_Tabla );
        $("#" + url_Tabla + "NAV").load('/Postulante/' + url_Tabla, { ID_persona: id_persona }, function () {
            //alert("se recargo la vista de la tabla actual...")
            //aplico datatable a la tabla de estudio
            TablasEIA();

            //se llama al modal para cargar un nuevo registrode la tabla actual
            $(".Nuevo_REG").on("click", function () {
                var id_Tabla = $(this).attr("data-IdTabla");
                //alert(id_Tabla)
                ModalEIACUD(null, id_persona, id_Tabla);
            });

        });
    };




    //muestra o ocualta los campos relacionado con los campos si el instituto pertenece al exterior o no
    function INST_EXT(pri) {
        if ($("#DropdownEXT").val() == "true") {
            $("#JuriEST,#IdInstEST").show();
            $("label[for='Provincia']").text("Pais");
            $(".INSAR").hide();
            $("#ComboIdInstEST").val(0);
            $(".COM_ESTUAR").selectpicker("val", "");
        } else {
            $("#JuriEST,#IdInstEST").hide().val("");
            $("label[for='Provincia']").text("Provincia/Juridiccion");
            $(".INSAR").show();
        };
        if (pri != 0) {
            $("#JuriEST,#IdInstEST").val("");
        }
    };

    //funcion de si egreso o no para mostrar campos de promedio y ultimo año cursado
    function EgresoSINO() {
        if ($("#TerminoEST").val() == "true") {
            $("#PROMEDIO").show();
            $("#CurUltAño").hide();
        } else {
            $("#CurUltAño").show();
            $("#PROMEDIO").hide();
            $("#PROMEDIO input").val("");
        };
    };





    //funcion que arma los combos en cascada de la vista parcial Estudios
    function ComboCascada(Combo, ValC) {
        var OPC;
        //en caso de que el combobox es de la provincia
        if (Combo == "ComboJuriEST") {
            OPC = 0;
            $("#ComboLocaliEST,#ComboIdInstEST").html("")
            //cuando el combobox seleccionado es de la localidad
        } else {
            valprov = $("#ComboJuriEST").val();
            ValC = valprov + "-" + ValC;
            OPC = 1;
            $("#ComboIdInstEST").html("")
        }
        $.getJSON('/Postulante/DropCascadaEST', {
            opc: OPC, val: ValC
        },
            function (data) {
                //agrego al dropboxlist la etiqueta option con cada localidad que le corresponde a la juridiccion seleccionada
                var combocas = (OPC == 0) ? "#ComboLocaliEST" : "#ComboIdInstEST";
                //alert(combocas);
                $(combocas).append("<option>Seleccione una Opcion</option>");
                $.each(data, function () {
                    $(combocas).append("<option value=" + this.Value + " >" + this.Text + "</option>");
                });
                //refresco los combobox con los datos nuevos
                $(combocas).selectpicker('refresh');

            });
    };

    //////////////////////////////////////////////////////////////////////////////
    /* FUNCION DE LA VISTA DE AACTIVIDAD MILITAR */

    function IngreSINO() {
        if ($("#IngresoSINO").val() == "true") {
            //alert("si");
            $(".no input").val("");
            $(".si").show();
            $(".no").hide();

        } else {
            //alert("no");
            $(".si input,.si select").val("");
            $(".no").show();
            $(".si").hide();
        };
    };

    //////////////////////////////////////////////////////////////////////////////
    /* FUNCION DE LA VISTA DE SITUACION OCUPACIONAL */

    //se aplicael selecpicker a alos conbo/s con autocomplete
    /* https://developer.snapappointments.com/bootstrap-select/ */
    SITUOCUPA();

    $("#inaoact").on('changed.bs.select', function (e, clickedIndex, isSelected, previousValue) {
        SITUOCUPA();
    });

    function SITUOCUPA() {
        var group = $("#inaoact option:selected").closest('optgroup').attr('label');
        var text = $("#inaoact option:selected").html();
        if (group == "Inactivo" || text == "Desocupado") {
            $("#SI").hide();
            $("#SI input").val("");
        } else {
            $("#SI").show();

        }
    };

    //le aplico al selectpicker las opciones de "sleccionar todo y deseleccionar todo"
    $("#Estado").selectpicker({
        size: 6,
        width: 400,
        actionsBox: true,
        deselectAllText: 'Deseleccionar Todo',
        selectAllText: 'Seleccionar Todo',
        noneSelectedText: 'Ninguna Opcion Seleccionada',
        header: 'Cerrrar'

    });

    /////////////////////////////////////////////////////////////////////////////
    /* FUNCION DE LA VISTA DE ANTROPOMETRIA */

    //verifico el sexo del postulante para olcultar o mostrar determinados input
    sexo($("#Sexo").val());

    function sexo(sexo) {
        //alert(element);
        if (sexo != "Mujer") {
            $("#mujer").hide();
            $("#mujer input").val(0);
        } else {
            $("#mujer").show();
        }
    };

    //calculo de la IMC cuando los campos de altura y peso con cargados
    $("#altura,#peso").on("change", function () {

        var altura = $("#altura").val() / 100,
            peso = $("#peso").val().replace(",", ".");
        if (altura != 0 && peso != 0) {
            var imc = peso / (altura * altura);
            //alert(imc);
            $("#imc").val(imc.toFixed(2).replace(".", ","));
        }

    });

    //cargo todo los campos con 0, para eviarlos al POST de Antropometria
    $("#antropo input").each(function () {
        if ($(this).val() == "") {
            $(this).val(0);
        }
    });

    $("#EstadoCivil").on("change", function () {
        var estci = $(this).val();
        if (estci.indexOf("C") > -1) {
            $("#FechaCasamiento").removeClass("d-none");
        } else {
            $("#FechaCasamiento").addClass("d-none");
            $("#FechaCasamiento input").val("");

        };
    });

    /////////////////////////////////////////////////////////////////////////////
    /* FUNCION DE LA VISTA DE FAMILIA */
    $("#TablaFamilia").on('select.dt', function (e, dt, type, index) {
        e.preventDefault;
        e.stopImmediatePropagation();
        //var data = dt.rows(index).data();
        //var idPersonaFamilia = data[0][0];


        ////alert(IdFamilia);
        ////redirijo la pagina hacia la vista FamiliaCUD enviandole como parametro el IdPersona correspondiente al familiar Seleccionado
        //var url = "/Postulante/FamiliaCUD?idPersonaFamilia=" + idPersonaFamilia;
        //window.location.href = url;

    });




    //funcion para contraer todos los TAb que esten abiertos al abrir uno nuevo 
    $(".TABMovil .navbar-toggler").on("click", function (e) {
        var idBT = $(this).attr("id");
        var idTAB;
        $(".TABMovil .navbar-toggler ").each(function (e, i) {
            if ($(this).attr("id") != idBT) {
                $(this).addClass("collapsed");
                idTAB = $(this).attr("data-target");
                $(idTAB).removeClass("show");
            }

        });
    });

    //////////////////////////////////////////////  DATOS BASICOS CONTROL DE EDAD //////////////////////////////////////////////////////////////////////////
    //$("#DatosBasicosBTGuarda").on("click", function () {
    //    var valido = $("#BeginFormDatosBasicos").valid();
    //    //alert(valido);
    //    if (!valido) {
    //        $("#BeginFormDatosBasicos").submit();
    //    } else {
    //        alert($("#vPersona_DatosBasicosVM_IdPostulante").val() + "   " + $("#edad").val());
    //        $.getJSON('/Postulante/EdadInstituto', {
    //            IdPOS: $("#vPersona_DatosBasicosVM_IdPostulante").val(),
    //            edad: $("#edad").val(),
    //            Fecha: $("#fechacumpleaños").val(),
    //            grupoCarreraOficio: $("#vPersona_DatosBasicosVM_IdGrupoCarreraOficio").val()
    //        },
    //            function (response) {
    //                //alert(response.coherencia);
    //                if (response.coherencia) {
    //                    $("#BeginFormDatosBasicos").submit();
    //                } else {

    //                    //Datos Basicos control de edad si es valido para la inscripcion a la que quiere inscribirse
    //                    $("#BTNModal").html("Cancelar");
    //                    $("#ModalCenterTitle").html("Advertencia");
    //                    $("#GuardarDTF").css("display", "block");
    //                    $("#TextModal").html("La EDAD ingresado supera la permitida para el Instituto a Inscribirse Seleccionado.");
    //                    $("#ModalAnuncios").modal();
    //                };
    //            });
    //    };
    //});


    //$("#GuardarDTF").on("click", function (e) {
    //    e.preventDefault;
    //    e.stopImmediatePropagation();
    //    $("#BeginFormDatosBasicos").submit();
    //});


    //si la vista Index es llamada por la vista FamiliaCUD se ase visible la tabla Familia
    var paganterior = document.referrer.toString().indexOf("FamiliaCUD");
    if (paganterior > 1) {
        //alert("vino de familia");
        $("#5 a").tab("show");
        $("#FamiTAB a").tab("show");
        $("#BTFamiliaNAV").trigger("click");
        //$("html,body").animate({
        //    scrollTop: 250
        //}, 1500);
    }
    //////////////////////////////////////////////  SULICITUD DE ENTREVISTA  //////////////////////////////////////////////////////////////////////////

    //habilito el boto solicitar entrevista una vez haya realizado el guardado de datos
    //ver esto de donde tomar el dato que ya  realizo un guardado de datos.
    if ($("#BeginFormDatosBasicos .fechacumple").val() != "") {
        $("#BTentrevista").removeClass("disabled");
    };






});