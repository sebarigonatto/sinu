//recibo listado de la Clase "DataTableVM", presentados como un listado objetos
//cada objeto contienen : 
//"TablaVista": nombre de la tabla/vista
//"Columnas": array con las columnas para armar la tabla y correspondiente propiedades
//"filtrosIniciales": pre-filtros sobre los datos a mostrar en la Tabla
//https://datatables.net/

//funcion para aplicar el plug-in DataTabla a las tablas indicadas
function armadoDeTablas(tablas) {

    //recorro cada tabla
    $.each(tablas, function (index, tabla) {

        //agrego a cada tabla una columna para las opciones que pueda tener
        tabla.Columnas.push({ data: "Opciones", searchable: false, orderable: false, title: 'Opciones' });

        var input_filter_value;
        var input_filter_timeout = null;

        alert(tabla.TablaVista);          

    })
}

//columns.push({ data: "Opciones", searchable: false, orderable: false, title: 'Opciones' });

//var input_filter_value;
//var input_filter_timeout = null;
primerCarga = true;

//tabla
var table = $('table').DataTable({
    "serverSide": true,
    "processing": true,
    searchDelay: 900,
    "ajax": {
        url: "@Url.Action",//("CustomServerSideSearchActionAsync", "PostulanteEliminar")",
        type: "POST",
        //async: false,
        data: function (data) {
            if (primerCarga) {
                primerCarga = false;

                data.search.value = sessionStorage.searchTablaDelPost;
            };

            sessionStorage.searchTablaDelPost == undefined ? sessionStorage.searchTablaDelPost = "" : null;
            sessionStorage.dropDelegacionDelPost == undefined ? sessionStorage.dropDelegacionDelPost = "" : null;
            sessionStorage.dropModalidadDelPost == undefined ? sessionStorage.dropModalidadDelPost = "" : null;

            data.filtrosExtras = [
                {
                    Text: "Inscripto_En",
                    Value: sessionStorage.dropDelegacionDelPost
                },
                {
                    Text: "Modalidad",
                    Value: sessionStorage.dropModalidadDelPost
                }];
            //JSON.stringify({
            //    delegacion: sessionStorage.dropDelegacionDelPost,
            //    modalidad: 
            //});
            data.tablaVista = '@Model.TablaVista';

            data.columns.pop()
        },
        dataSrc: function (json) {

            $.each(json.data, function (index, item) {
                botonDetalle = `<a class='mr-1 btn btn-info fas fa-bars load-submit' href='/Postulante?ID_Postulante=${item.IdPersona}'  data-toggle='tooltip' data-placement='top' title='Detalles'></a>`;
                botonEliminar = "<button type='button' class='ml-1 btn btn-danger fas fa-trash-alt EliPost' data-toggle='tooltip' data-placement='top' title='Eliminar'></button>";
                item.Opciones = `<div class="d-flex">${botonDetalle} | ${botonEliminar}</div>`
            });
            return json.data;
        }
    },
    dom: '<"container row justify-content-center m-0"<"col-md-6 col-sm-12"l><"col-md-6 col-sm-12"f><"opcionesExtrasDT2 container  row">>r<"col-sm-12"t><"container row  d-flex"<"col-md-6 col-sm-12"i><"col-md-6 col-sm-12"p>>',

    "language":
    {
        "sProcessing": "<span class='fa-stack fa-lg'>\n\<i class='fa fa-spinner fa-spin fa-stack-2x fa-fw'></i>\n\</span>&nbsp;&nbsp;&nbsp;&nbsp;Cargando...",
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
        }
    },
    "columns": columns,
    "lengthMenu": [[10, 25, 50, -1], [10, 25, 50, "Todos"]],
    responsive: true,

}).on('processing.dt', function (e, settings, processing) {
    if (processing) {
        $("tbody").addClass("disabled");
    } else {
        $("tbody").removeClass("disabled");
    }

});

//function limpiar() {
//    sessionStorage.dropDelegacionDelPost = "";
//    sessionStorage.dropModalidadDelPost = "";
//    $("select.selectpicker").val("").selectpicker("refresh");
//    table.search("").ajax.reload();
//};


//$("div.dataTables_filter input").unbind();
//$("div.dataTables_filter input").keyup(function (e) {
//    input_filter_value = this.value;
//    clearTimeout(input_filter_timeout);
//    input_filter_timeout = setTimeout(function () {
//        table.search(input_filter_value).draw();
//    }, table.context[0].searchDelay);

//    // if (e.keyCode == 13) {
//    //  usertable.search( this.value ).draw();
//    // }
//});



//    $("table thead tr th").last().width(100);

//    $("button div div div.filter-option-inner-inner").css("color", "black");

//    $('#le-filters[data-toggle="tooltip"]').tooltip("enable")
//    jQuery("#opcionesExtrasDT1 div.mb-2")
//        .detach()
//        .appendTo('.opcionesExtrasDT2')

//    $("#le-filters_filter label")
//        .addClass("float-md-right")
//        .children("input")
//        .removeClass("form-control-sm").addClass("form-control")
//        .css("width", "225px")
//        .attr("placeholder", "dni, nombre, apellido, email")

//    $("[name='le-filters_length']").css({ "width": "180px", "font-size": "100%" }).removeClass("form-control-sm")


//    $("#dropDelegacionDelPost").val(sessionStorage.dropDelegacionDelPost).selectpicker("refresh");
//    $("#dropModalidadDelPost").val(sessionStorage.dropModalidadDelPost).selectpicker("refresh");
//    //se aplicael selecpicker a alos conbo/s con autocomplete con la opcion de busqueda


//    //establesco parametro de busqueda para al almacenamineto en el lado del cliente
//    table.on('search.dt', function () {
//        sessionStorage.searchTablaDelPost = table.search();
//    });

//    //esatblesco en el input la ultima busqueda realizada
//    table.on('draw', function () {

//        table.search(sessionStorage.searchTablaDelPost);
//    });

//    //guarda las seleciones de los combos
//    $("select").on("change", function () {
//        name = $(this).attr("name");
//        sessionStorage.setItem(name, $(this).val());
//        table.search(sessionStorage.searchTablaDelPost).ajax.reload();
//    });

    
