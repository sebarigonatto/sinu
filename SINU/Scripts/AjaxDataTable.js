/// <reference path="jquery-3.3.1.js" />

//Recibo listado de la Clase "DataTableVM", presentados como un objeto
//Contienen: 
//"TablaVista": nombre de la tabla/vista
//"Columnas": array con las columnas para armar la tabla y correspondiente propiedades
//"filtrosIniciales": array con pre-filtros sobre los datos a mostrar en la Tabla
//https://datatables.net/

//declaro un array que contendra las distintas tablas armadas
var listadoDataTable = []

//verifico si existe sessionStorage['storageDataTables'], que almacena de forma local datos de la distintas tablas
sessionStorage['storageDataTables'] ??= '{}'
var jsonStoreageDataTable = JSON.parse(sessionStorage['storageDataTables'])

//funcion para aplicar el plug-in DataTabla a las tablas indicadas
/**
 * Armado de Tabla con DATATABsLES
 * @param {JSON} modeloTabla Json, Tabla o listado de las Tablas a armar, Ej: "@Html.tablaToJson(Model)"
 * @param {string} armadoBtn Botones Opcionales, string con las etiquetas "a" o funcion que devuelve las etiquetas 'a', en caso contrario enviar "null"
 * @param {[]} exportBtn Array de objetos, con los Botones a mostrar y si exporta el total de registro, botones disponibles "Copiar, Excel,Pdf,Imprimir", . Ej: "[{boton:'copy',todos:true},{boton:'Imprimir',todos:false}]". Por defecto muestra los botones predefinidos de "DataTables" que exportan
 * @param {string} selectOptions Id del div con los select para el filtro extras.

 */
function armadoDataTable(modeloTabla, armadoBtn, exportBtn, selectOptions) {
    //console.log(Array.isArray(modeloTabla))
    //veo si el objeto recibido es un array de Tablas 
    Array.isArray(modeloTabla) ? $.each(modeloTabla, async (index, tabla) => tablaArmado(tabla, armadoBtn, exportBtn, selectOptions)) : tablaArmado(modeloTabla, armadoBtn, exportBtn, selectOptions)
}

function tablaArmado(tabla, armadoBtn, exportBtn, selectOptions=null) {


    //Armado de la Columna "Opcion"
    if (armadoBtn!=null) {
        const opcionColumna = {
            data: "Opciones", searchable: false, orderable: false, title: 'Opciones', className: 'noPrint', render:(data, type, row, meta) => {

                let btnString = typeof (armadoBtn) == "function" ? armadoBtn(row) : armadoBtn;
                let div = $.parseHTML('<div class="d-flex justify-content-around"></d>')
                //reemplazo de columna por dato
                let btnList = btnString.split('<a')
                btnList.shift()
                let paramsConpuesto = true
                for (let btn of btnList) {
                    btn = $.parseHTML(`<a ${btn.toString()}`);
                    let href = $(btn).attr('href')
                    if (href.indexOf('?') > 0) {
                        paramsConpuesto = true
                        href = href.split('?')
                        var params = new URLSearchParams(href.pop())
                        //console.log(href.pop())
                    } else {
                        paramsConpuesto = false
                        href = href.split('/',)
                        var params = new URLSearchParams(href.pop())
                        //console.log(href.pop())
                    }
                    let paramsNew = new URLSearchParams()
                    for (let param of params) {
                        //console.log(param[paramsConpuesto ? 1 : 0])
                        let indexItem = paramsConpuesto ? 1 : 0
                        param[indexItem] = row[param[indexItem]]
                        paramsNew.append(param[0], param[1])
                        href = paramsConpuesto ? `${href.toString()}?${paramsNew.toString()}` : `${href.join('/')}/${param[indexItem]}`
                        //console.log(href)
                    }
                    $(btn).attr('href', href)
                    $(div).append(btn)
                }
                //console.log($(div).prop('outerHTML'))
                return $(div).prop('outerHTML');
            }
        }
        //agrego al objeto "Columnas" la columna "Opciones"
        tabla.Columnas.push(opcionColumna)
    }

    //armado de los botones para la exportacion de los registros
    //id de la etiqueta table
    const idTabla = tabla.IdTabla ?? tabla.TablaVista;

    //titulo que aparecera en la exportaciones
    const titleTable = $(`#dataTable-${idTabla}`).prevAll(":header").first().html();

    //verifico si la tabla a armar posee un elemento con sus datos dentro del objeto "jsonStoreageDataTable", caso contrario agrego al objeto "jsonStoreageDataTable" el elemento correspondiente a la tabla a armar
    //console.log(jsonStoreageDataTable[idTabla])
    jsonStoreageDataTable[idTabla] ??= {
        search: '',
        data: {},
        filtrosSelect: {},
        titulo: titleTable
    }

    //function para la exportacion de datos en CSV
    function getCvsServer(e, dt, button, config) {
        $(`#dataTable-${idTabla}_processing`).css('display','block')
        var data = jsonStoreageDataTable[idTabla].data;
        var titulo = jsonStoreageDataTable[idTabla].titulo
        oldLength = data.length
        data.length = -1
        recordsTotal=dt.page.info().recordsTotal
        $.ajax({
            type: 'POST',
            url: '/AjaxDataTable/DataTableToCSV',
            dataType: 'json',
            data: { model: data, recordsTotal: recordsTotal, titulo: titulo},
            success: function (data) {
                              
                var blob = new Blob([`\uFEFF${data.content}`], {
                    encoding: "UTF-8",
                    type: "text/csv;charset=utf-8"
                });
                var blobUrl = window.URL.createObjectURL(blob);

                //console.log(blob);
                //console.log(blobUrl);

                //ie (naturally) does things differently
                var csvLink = document.createElement("a");
                if (!window.navigator.msSaveOrOpenBlob) {
                    csvLink.href = blobUrl;
                    csvLink.download = data.nameFile;
                }               
                csvLink.click()
                $(`#dataTable-${idTabla}_processing`).css('display', 'none')
            },
            error: function (ex) {
                alert('ERROR, actualice la pagina.')
            }
        })
        jsonStoreageDataTable[idTabla].data.length = oldLength
        
    }

    //funcion para exportar todos los registros sin necesidad de mostrar las totalidad de los mismos
    function newexportaction(e, dt, button, config) {

        var self = this;
        var oldStart = dt.settings()[0]._iDisplayStart;
        dt.one('preXhr', function (e, s, data) {
            // Just this once, load all data from the server...
            data.start = 0;
            data.length = -1;

            dt.one('preDraw', function (e, settings) {

                // Call the original action function
                if (button[0].className.indexOf('buttons-copy') >= 0) {
                    $.fn.dataTable.ext.buttons.copyHtml5.action.call(self, e, dt, button, config);
                } else if (button[0].className.indexOf('buttons-excel') >= 0) {
                    $.fn.dataTable.ext.buttons.excelHtml5.available(dt, config) ?
                        $.fn.dataTable.ext.buttons.excelHtml5.action.call(self, e, dt, button, config) :
                        $.fn.dataTable.ext.buttons.excelFlash.action.call(self, e, dt, button, config);
                } else if (button[0].className.indexOf('buttons-csv') >= 0) {
                    $.fn.dataTable.ext.buttons.csvHtml5.available(dt, config) ?
                        $.fn.dataTable.ext.buttons.csvHtml5.action.call(self, e, dt, button, config) :
                        $.fn.dataTable.ext.buttons.csvFlash.action.call(self, e, dt, button, config);
                } else if (button[0].className.indexOf('buttons-pdf') >= 0) {
                    $.fn.dataTable.ext.buttons.pdfHtml5.available(dt, config) ?
                        $.fn.dataTable.ext.buttons.pdfHtml5.action.call(self, e, dt, button, config) :
                        $.fn.dataTable.ext.buttons.pdfFlash.action.call(self, e, dt, button, config);
                } else if (button[0].className.indexOf('buttons-print') >= 0) {
                    $.fn.dataTable.ext.buttons.print.action(e, dt, button, config);
                }
                dt.one('preXhr', function (e, s, data) {
                    // DataTables thinks the first item displayed is index 0, but we're not drawing that.
                    // Set the property to what it was before exporting.
                    settings._iDisplayStart = oldStart;
                    data.start = oldStart;
                });
                // Reload the grid with the original page. Otherwise, API functions like table.cell(this) don't work properly.
                setTimeout(dt.ajax.reload, 0);
                // Prevent rendering of the full data to the DOM
                return false;
            });
        });
        // Requery the server with the new one-time export settings
        dt.ajax.reload();
    };

    //Array con los botones de exportacion de defecto, EXCEL, PDF, COPIAR, IMPRIMIR
    const botonesPorDefecto = [
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
            text: 'Excel',
            exportOptions: {
                columns: ':not(.noPrint)'
            },
            title: titleTable
        },
        {
            extend: 'pdfHtml5',
            text: 'Pdf',
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
        }
    ]

    function armadoBtnExport(exportBtn) {
        let btns= new Array()
        $.each(exportBtn, function (index, btn) {
            let boton = {}
            boton.exportOptions= {
                columns: ':not(.noPrint)',
              
            }
            boton.charset = 'UTF-8'
            boton.bom=true
            boton.text = btn.todos ? `${btn.boton} - Total`: btn.boton
            switch (btn.boton) {
                case 'Copiar':
                    boton.extend ='copyHtml5'
                    break;
                case 'Excel':
                    boton.extend = 'excelHtml5'
                    break;
                case 'Pdf':
                    boton.extend = 'pdfHtml5'
                    break;
                case 'Imprimir':
                    boton.extend = 'print'
                    break;
                case 'Csv':
                    boton.extend = 'csv'
                    boton.fieldSeparator = ';'
                    break;
            };
            btn.todos ? boton.action = btn.boton == "Csv" ? getCvsServer : newexportaction : null
            boton.title= titleTable
            btns.push(boton)
        })
        return btns
    }

    let Botones = !exportBtn ? botonesPorDefecto : armadoBtnExport(exportBtn)

    //preparacion de los select para realizar el fltrado en los DataTable
    var selectList
    if (selectOptions) {       
        selectList = $(`div #${selectOptions} select`)
        $.each(selectList, (index, select) => {
            $(select).attr("tabla", idTabla)
            if (!jsonStoreageDataTable[idTabla].filtrosSelect[`${$(select).attr('id')}`]) {
                //alert("select nuevo")
                jsonStoreageDataTable[idTabla].filtrosSelect[`${$(select).attr('id')}`] = {
                    Columna: $(select).attr("selectOption"),
                    Valor: "",
                    Condicion: "=="
                }
            }
                             
        })
    }

    //aplico a la tabla actual "DataTable"
    listadoDataTable[`dataTable-${idTabla}`] = $(`#dataTable-${idTabla}`).DataTable({
        "serverSide": true,
        "processing": true,
        searchDelay: 900,
        "ajax": {
            url: "/AjaxDataTable/CustomServerSideSearchActionAsync",
            type: "POST",
            data: (data) => {

                data.search.value = jsonStoreageDataTable[idTabla].search

                data.filtrosExtras = [];
                //agrego filtros extras por defecto de la tabla
                $.each(tabla.filtrosExtras, (index, item) => {
                    data.filtrosExtras.push(item);
                })
                //agrego filtros de os selects
                $.each($(`select[tabla='${idTabla}']`), function (index,selectFiltro) {
                    data.filtrosExtras.push(jsonStoreageDataTable[idTabla].filtrosSelect[$(selectFiltro).attr('id')])
                })

                data.tablaVista = tabla.TablaVista;
                if (armadoBtn != null) { data.columns.pop(); }
                //guardo la variable data, para poder aceder desde otra funcion
                jsonStoreageDataTable[idTabla].data = data
                //alert(JSON.stringify(jsonStoreageDataTable))
                sessionStorage['storageDataTables'] = JSON.stringify(jsonStoreageDataTable)

            },
            dataSrc: function (json) {
                return json.data;
            }
        },
        dom: '<"container row m-0 col-12  d-flex justify-content-center justify-content-sm-between"lBf<"container row m-0 col-12  d-flex justify-content-center justify-content-sm-between divContentSelect">>r<"col-sm-12"t><"container row  d-flex"<"col-md-6 col-sm-12 blockquote"i><"col-md-6 col-sm-12"p>>',

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
        buttons: Botones,
        "columns": tabla.Columnas,
        "lengthMenu": [[10, 25, 50, -1], [10, 25, 50, "Todos"]],
        rowReorder: {
            selector: 'td:nth-child(2)'
        },
        responsive: true,

    }).on('processing.dt', (e, settings, processing) => {
        //desahnilito la tabla cundo se reacliza una actualizacion de la tabla
        if (processing) {
            $(`.dataTables${idTabla}_wrapper`).addClass("disabled");
        } else {
            $(`.dataTables${idTabla}_wrapper`).removeClass("disabled");
        }
    }).on('search.dt', async () => {
        //guardo la busqueda realizada en el json de la tabla
        jsonStoreageDataTable[idTabla].search = listadoDataTable[`dataTable-${idTabla}`].search();
    }).on('draw', () => {
        //coloco en el input de busqueda la busuqeda guardada
        listadoDataTable[`dataTable-${idTabla}`].search(jsonStoreageDataTable[idTabla].search);
    })

    //si existen select a agregar a la tabla
    if (selectOptions) {
        let divPri= $('<div></div>')
        $.each($(`#${selectOptions} select`), function (index,select) {
            let div = $('<div class="col-md-4 col-sm-12 mb-2 p-0"><div class="col-md-12"></div></div>')
            div.append($(select))
            $(select).val(jsonStoreageDataTable[idTabla].filtrosSelect[`${$(select).attr('id')}`].Valor)
            $(divPri).append(div)
        })        
        $(`#dataTable-${idTabla}_wrapper .divContentSelect`).append(divPri.html());
        $.each($(`#dataTable-${idTabla}_wrapper .divContentSelect select`), function (index,select) {
            $(select).val(jsonStoreageDataTable[idTabla].filtrosSelect[$(select).attr('id')].Valor).selectpicker('refresh')
        })
        $(`#dataTable-${idTabla}_wrapper .divContentSelect`).append(`<div class="col-md-4 col-sm-12 mb-2 p-0"><div class="col-md-12"><button class="btn btn-dark form-control" onclick="limpiar('${idTabla}')">Limpiar filtros</button></div></div>`)
    }        

    //al detectar un cambio de los select de una tabla actualizo los registros mostrados
    $(`select[tabla='${idTabla}']`).on("change", function () {
        //alert('cambio select')
        jsonStoreageDataTable[idTabla].filtrosSelect[$(this).attr('id')].Valor = $(this).val()
        listadoDataTable[`dataTable-${idTabla}`].search(jsonStoreageDataTable[idTabla].search).draw()
    })
}

function limpiar(idtabla) { 
    $(`#dataTable-${idtabla}_wrapper .divContentSelect select`).val("").change()
    $(`#dataTable-${idtabla}_wrapper .divContentSelect select`).selectpicker('refresh')
    $(`#dataTable-${idtabla}_wrapper input[type='search']`).val("")
    listadoDataTable[`dataTable-${idtabla}`].search('').draw()
}

//guardo la busqueda en 
$("div.dataTables_filter input").unbind();
$("div.dataTables_filter input").keyup( (e)=> {
    input_filter_value = this.value;
    clearTimeout(input_filter_timeout);
    input_filter_timeout = setTimeout( ()=> {
        table.search(input_filter_value).draw();
    }, table.context[0].searchDelay);
});

$(document).ready( ()=> {

    $("table thead tr th").last().width(100);

    $("button div div div.filter-option-inner-inner").css("color", "black");
       
    $("#le-filters_filter label")
        .addClass("float-md-right")
        .children("input")
        .removeClass("form-control-sm").addClass("form-control")
        .css("width", "225px")
        .attr("placeholder", "dni, nombre, apellido, email")
    $("[name='le-filters_length']").css({ "width": "180px", "font-size": "100%" }).removeClass("form-control-sm")

});
