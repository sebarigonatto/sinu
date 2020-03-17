//configuraciones por default de las DataTables
$.extend(true, $.fn.dataTable.defaults, {
    responsive:
    {
        details: false
    },
    "autoWidth": true,

    //ver si es necesario este fragmento
    select: 'single',
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

    //ver esto ya que en el navegador aparece nu aadvertencia relacionado si es asincronico o sincronico
    $.ajaxSetup({
        async: false
    });

    //para qu ela validacion de fecha reconosca el formato dd/MM/yyyy
    jQuery.validator.methods["date"] = function (value, element) { return true; };

    (function () {
        $("#TabGeneral li").each(function () {

            //    if ($(this).attr('id') > @ViewBag.secuenciaactual) {

            //    //$(this).addClass('Tabdesabilitado').css("background-color","red");
            //};
        });

        //ajusto la altura de los dropdownlist con la clase selectpicker
        $(".selectpicker").selectpicker({
            size: 7,
        });

    })();

    ///////////////////////////////////////////////////////////////////////////////// 

    //ver si  el modo de como ocultar el datepicker al seleccionar la fecha
    /*FUNCION DE LA VISTA DE DATOS PERSONALES */
    $("#datepicker").datepicker({
        format: "dd-mm-yyyy",
        language: "es",
        autoclose: true,
        startView: "years",
    });

    //cuando se selecciona una fecha se calcula la edad
    $('#datepicker').datepicker().on("changeDate", function (e) {
            var fechanac = $('#datepicker').datepicker('getDate');
            var fechahoy = new Date();
            var edad = fechahoy.getFullYear() - fechanac.getFullYear();
            //condicion que verefica si cumplio años, si no cumplio aun se le resta un año a edad
            if ((fechahoy.getMonth() <= fechanac.getMonth()) && (fechahoy.getDate() < fechanac.getDate())) {
                //alert("NO Cumplio AÑOS");
                edad--;
            };
            $("#edad").val(edad);
    });


    /////////////////////////////////////////////////////////////////////////////////
    /*FUNCION DE LA VISTA DE DOMICILIOS */

    //se aplicael selecpicker a alos conbo/s con autocomplete con la opcion de busqueda
    /* https://developer.snapappointments.com/bootstrap-select/ */
    $(".combobox").selectpicker({
        liveSearch: true,
        size: 7,
        liveSearchPlaceholder: "Ingrese su busqueda",
        liveSearchStyle: 'contains',//'startsWith'
        noneResultsText: 'No se Encuantran Resultados',
        noneSelectedText: 'Ninguna Opcion Seleccionada'
    });

    $("select").on('changed.bs.select', function (e, clickedIndex, isSelected, previousValue) {
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

        //los elementos html con la clase "Real,Eventual" corresponde a campos para los datos, de no ser de Argentina
        //Los que posean la clase "RealAR,EventualAR" en caso de pertenecer a la Argentina
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
        //alert(Combo + " " + ValP);
        var ComboLocalidad = (Combo == "ComboProvinciaR") ? "#ComboLocalidadR" : "#ComboLocalidadE";
        //limpio el combo de las localidades, para cargar las licalidades de la provincia seleccionada
        $(ComboLocalidad).empty();

        //llamo al JsonResult '/Postulante/DropEnCascadaDomicilio' y le envio la provincia seleccionada
        $.getJSON('/Postulante/DropEnCascadaDomicilio', {
            Provincia: ValP,
        },
            function (data) {
                $(ComboLocalidad).append('<option value=' + '' + '>' + "Seleccione una Localidad" + '</option>');
                //agrego al dropboxlist la etiqueta "option" con cada localidad que le corresponde a la provincia seleccionada
                $.each(data, function () {
                    $(ComboLocalidad).append('<option value=' + this.Value + '>' + this.Text + '</option>');
                });
                //para actualizar el combobox
                $(ComboLocalidad).selectpicker('refresh');
            });
    };

    //se actualiaz el codigo postal deacuerdo a la localidad seleccionada
    function LOCALIDAD(Combo) {
        var ComboCP = (Combo == "ComboLocalidadR") ? "#CPR" : "#CPE";
        //llamo al JsonResult '/Postulante/EnCascada' y le envio la localidad seleccionada
        var valCP = $("#" + comboid).val();
        $.getJSON('/Postulante/DropEnCascadaDomicilio', {
            Localidad: valCP,
        },
            function (data) {
                //agrego al dropboxlist la etiqueta option con cada localidad que le corresponde a la provincia seleccionada
                $(ComboCP).val(data.Text);
            });
        ////para actualizar el combobox
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
        Tabla.on('select.dt', function (e, dt, type, index) {
            var data = dt.rows(index).data();
            var id_registro = data[0][0];
            var id_Tabla = $(this).attr("id");
            //llamo a la funcion para mostrar el modal y le envio 2 paremtros
            ModalEIACUD(id_registro, id_Tabla);

            $("#ModalEIA").modal("show");
        });
    };

    //se llama al modal para cargar un nuevo registro dependiendo la tabla  a acualizar
    $(".Nuevo_REG").on("click", function () {
        var id_Tabla = $(this).attr("data-IdTabla");
        //alert(id_Tabla)
        ModalEIACUD(null, id_Tabla);
    });

    var url_Tabla ;
    var url_CUD ;
    var url_Elim;
    //armado el modal con la vista parcial correspondiente
    function ModalEIACUD(id_registro, id_Tabla) {

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
            data: { ID: id_registro },
            //si no surge error al redireccionar se reemplaza el contenido de la div
            success: function (response) {
                $('#ModalEIACuerpo').html(response);
                //con esto  funciona la validacion del lado del cliente con la vista parcial
                $('#ModalEstudioCuerpo').removeData("validator");
                $('#ModalEstudioCuerpo').removeData("unobtrusiveValidation");
                $.validator.unobtrusive.parse('#ModalEstudioCuerpo');

                                        
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
                //se aplica el selectpicker basico
                $(".selectpicker").selectpicker();


                ////////////////////////////ESTUDIOS///////////////////////////////////


                //evento que se desata cuando se selecciona un opcion de los combobox
                $("#ComboJuriEST,#ComboLocaliEST").on('changed.bs.select', function (e, clickedIndex, isSelected, previousValue) {
                    comboid = $(this).attr("id");
                    //no se realiza nada si el evento fue desatado por el combo de INSTITUTO
                    if (comboid != "ComboIdInstEST") {
                        //alert(comboid);
                        valcombo = $('#' + comboid + ' option:selected').html();
                        ComboCascada(comboid, valcombo);
                    }
                });
                                         

                //sie estudio en el exterior oculto los combobox de Institutos Argentinos
                if ($("#IdInstEST").val() != "" && id_registro != null) {
                    //alert($("#vPersona_EstudioIdVM_IdInstitutos").val());
                    $("#checkext").attr("checked", "checked");
                };

                //lamo la funcion INSTEXT y mando cero por que esla primera carga
                INSTEXT( 0);

                //cada vez que el checkbox de instituto en el exterior cambia de valor
                $("#checkext").on("change", function () {
                    INSTEXT(1);
                });


                /////////////////////////ACTIVIDAD MILITAR//////////////////////////////////


                IngreSINO();
                $("#IngresoSINO").on("change", function (e) {
                    IngreSINO()

                });


                /////////////////////////////////GUARDA////////////////////////////////////

                $("#Guardar_REG").on("click", function () {
                    //alert("se cerrara modal!!!");
                    $("#ModalEIA").modal("hide");
                });

                ///////////////////////////////ELIMINA///////////////////////////////////

                //ELIMINA EL ESTUDIO SELECCIONADO
                $(".Eliminar").on("click", function () {

                    $.getJSON('/Postulante/' + url_Elim,
                        { ID: id_registro },
                        function (response) {
                            alert(response.success + " - " + response.msg);
                        });
                    $("#ModalEIA").modal("hide");
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

    //se se actualiza la vista parcial de la tabla en el caso que se elimine, modifique o se agregue un registro
    $("#ModalEIA").on('hidden.bs.modal', function () {
        alert(url_Tabla);
        $("#" + url_Tabla).load('/Postulante/' + url_Tabla, function () {

            //se llama al modal para cargar un nuevo registrode la tabla actual
            $(".Nuevo_REG").on("click", function () {
                var id_Tabla = $(this).attr("data-IdTabla");
                //alert(id_Tabla)
                ModalEIACUD(null, id_Tabla);
            });

            //aplico datatable a la tabla de estudio
            TablasEIA();
        });
    });

  

    //funcion que carga el modal de estudio
    //recibe "NULL" en caso de agregar un nuevo estudio y distinto a "NULL" para la modificacion o eliminacion

    //muestra o ocualta los campos relacionado con los campos si el instituto pertenece al exterior o no
    function INSTEXT( pri) {

        if ($("#checkext").is(":checked")) {
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


    function ComboCascada(Combo, ValC) {
        var OPC;
        if (Combo == "ComboJuriEST") {
            OPC = 0;
            $("#ComboLocaliEST,#ComboIdInstEST").html("")
        } else {
            valprov = $("#ComboJuriEST").val();
            ValC = valprov + "-" + ValC;
            //alert(ValC);
            OPC = 1;
            $("#ComboIdInstEST").html("")
        }
        //alert("sdf");
        $.getJSON('/Postulante/DropCascadaEST', {
            opc: OPC, val: ValC
        },
            function (data) {
                //agrego al dropboxlist la etiqueta option con cada localidad que le corresponde a la juridiccion seleccionada
                var combocas = (OPC == 0) ? "#ComboLocaliEST" : "#ComboIdInstEST";
                //alert(combocas);
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

    //funcion que carga el modal de estudio
    //recibe "NULL" en caso de agregar un nuevo estudio y distinto a "NULL" para la modificacion o eliminacion

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

    $("#Estado").selectpicker({
        size: 6,
        width: 400,
        actionsBox: true,
        deselectAllText: 'Deseleccionar Todo',
        selectAllText: 'Seleccionar Todo',
        noneSelectedText: 'Ninguna Opcion Seleccionada'
    });



    /////////////////////////////////////////////////////////////////////////////
    /* FUNCION DE LA VISTA DE ANTROPOMETRIA */


    //verifico el sexo del postulante para olcultar o mostrar determinados input
    sexo($("#Sexo").val());

    sex.on("change", function () {
        sexo(sex.val());
    });

    function sexo(element) {
        //alert(element);
        if (element != "Mujer") {
            $("#mujer").hide();
            $("#mujer input").val(0);
        } else {
            $("#mujer").show();
        }
    };


    $("#altura,#peso").on("change", function () {

        var altura = $("#altura").val() / 100,
            peso = $("#peso").val().replace(",", ".");
        if (altura != 0 && peso != 0) {
            var imc = peso / (altura * altura);
            //alert(imc);
            $("#imc").attr("value", imc.toFixed(2).replace(".", ","));
        }

    });

    $("#antropo input").each(function () {
        if ($(this).val() == "") {
            $(this).val(0);
        }

    });
   


});