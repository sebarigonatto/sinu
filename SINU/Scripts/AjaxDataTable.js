/// <reference path="jquery-3.3.1.js" />
//Recibo listado de la Clase "DataTableVM", presentados como un objeto
//Contienen: 
//"TablaVista": nombre de la tabla/vista
//"Columnas": array con las columnas para armar la tabla y correspondiente propiedades
//"filtrosIniciales": array con pre-filtros sobre los datos a mostrar en la Tabla
//https://datatables.net/

//funcion para aplicar el plug-in DataTabla a las tablas indicadas
/**
 * Armado de Tabla con DATATABLES
 * @param {any} modeloTabla Tabla o listado de las Tablas a armar, Ej: "@Model.tabla"
 */

function armadoDataTable(modeloTabla) {
    //alert(Array.isArray(modeloTabla))
    //veo si el objeto recibido es un array de Tablas 
    if (Array.isArray(modeloTabla)) {

        $.each(modeloTabla, (index, item) => tablaArmado(item))

    } else {

        tablaArmado(modeloTabla)
    }        
}

listadoDataTable = []

//obeto que contiene la columna de opciones que se agregara a la tabla
const opcionColumna = { data: "Opciones", searchable: false, orderable: false, title: 'Opciones', className: 'noPrint' }

function tablaArmado(tabla) {

    var idTabla = tabla.IdTabla ?? tabla.TablaVista;

    titleTable = $(`#dataTable-${idTabla}`).prev(':header').html();

    tabla.Columnas.push(opcionColumna)
    listadoDataTable[`dataTable-${idTabla}`] = $(`#dataTable-${idTabla}`).DataTable({
        "serverSide": true,
        "processing": true,
        searchDelay: 900,
        "ajax": {
            url: "/AjaxDataTable/CustomServerSideSearchActionAsync",
            type: "POST",
            //async: false,
            data: function (data) {
                //if (primerCarga) {
                //    primerCarga = false;

                //    data.search.value = sessionStorage.searchTexts;

                //};
               
                sessionStorage.searchText == undefined ? sessionStorage.searchText = "" : null;
                data.filtrosExtras = [];
                $.each(tabla.filtrosExtras, function (index, item) {
                    data.filtrosExtras.push(item);
                })

                data.tablaVista = tabla.TablaVista;
                data.columns.pop();
            },
            dataSrc: function (json) {
               
                $.each(json.data, function (index, item) {

                    botonDetalle = `<a class='mr-1 btn btn-info fas fa-bars load-submit' href='/Postulante?ID_Postulante=${item[Object.keys(item)[0]]}'  data-toggle='tooltip' data-placement='top' title='Detalles'></a>`;

                    item.Opciones = `<div class="d-flex justify-content-around">${botonDetalle}</div>`
                });

                return json.data;
            }
        },
        dom: '<"container row justify-content-center m-0"<"col-md-6 col-sm-12"lB><"col-md-6 col-sm-12"f><"container  row mt-1">>r<"col-sm-12"t><"container row  d-flex"<"col-md-6 col-sm-12"i><"col-md-6 col-sm-12"p>>',
        
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
        buttons: [
            {
                extend: 'copyHtml5',
                text: 'Copiar',
                exportOptions: {
                    columns: ':not(.noPrint)'
                },
                title: titleTable
            },
            {
                extend: 'excelHtml5',
                exportOptions: {
                    columns: ':not(.noPrint)'
                },
                title: titleTable
            },
            {
                extend: 'pdfHtml5',
                exportOptions: {
                    columns: ':not(.noPrint)'
                },
                title: titleTable
            },
            {
                extend: 'print',
                text: 'Imprimir',
                exportOptions: {
                    columns: ':not(.noPrint)'
                },
                title: titleTable
            },
        ],
        "columns": tabla.Columnas,
        "lengthMenu": [[10, 25, 50, -1], [10, 25, 50, "Todos"]],
        responsive: true,

    }).on('processing.dt', function (e, settings, processing) {
        if (processing) {
            $(".dataTables_wrapper").addClass("disabled");
        } else {
            $(".dataTables_wrapper").removeClass("disabled");
        }

    });
      
}


//guardo la busqueda en 
$("div.dataTables_filter input").unbind();
$("div.dataTables_filter input").keyup(function (e) {
    input_filter_value = this.value;
    clearTimeout(input_filter_timeout);
    input_filter_timeout = setTimeout(function () {
        table.search(input_filter_value).draw();
    }, table.context[0].searchDelay);

});