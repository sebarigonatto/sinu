﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using System.Web.Mvc;
using static SINU.Models.AjaxDataTableModel;

namespace SINU.Models
{

    public class resultAjaxTable
    {
        public int filteredResultsCount { get; set; }
        public int totalResultsCount { get; set; }

        public List<object> result { get; set; }
    }

    public class tipoColumna
    {
        public string COLUMN_NAME { get; set; }
        public string DATA_TYPE { get; set; }

    }

    public class DataTableVM
    {
        public List<AjaxDataTableModel.Column> Columnas { get; set; }
        public string TablaVista { get; set; }

        public List<SelectListItem> filtrosIniciales { get; set; } = new List<SelectListItem>() { new SelectListItem()};
    }

    public class AjaxDataTableModel
    {
      
        /// <summary>
        /// Metodo para crear las columnas de las Tablas a mostrar en la vistas.
        /// </summary>
        /// <param name="nombreColumna">Nombre del campo, correspondiente de la Tabla o Vista, en la Base de Datos</param>
        /// <param name="visible">Columna visible, por defecto es false</param>
        /// <param name="searchable">Columna buscable, por defecto es false</param>
        /// <param name="nombreDisplay">Nombre a mostrar en el encabezado de la Tabla, por defecto es igual a "nombreColumna"</param>
        /// <param name="noPrint">Indica si la columna sera exportada, ya sea en PDF, EXCEL o para imprimir</param>
        /// <param name="orderable">Columna ordenable, por defecto es false</param>
        /// <returns></returns>
        public static Column ColumnaDTAjax(string nombreColumna, bool visible = false, bool searchable = false, string nombreDisplay = null, bool noPrint = false, bool orderable = false)
        {
            return new Column
            {
                data = nombreColumna,
                title = nombreDisplay ?? nombreColumna,
                searchable = searchable,
                orderable = orderable,
                visible = visible,
                className = noPrint ? "noPrint" : ""
            };
        }

        public static string TablaDTAjax(string nombreTablaVista, List<Column> columnas, List<SelectListItem> filtrosIniciales)
        {
            DataTableVM tabla = new DataTableVM
            {
                Columnas = columnas,
                filtrosIniciales = filtrosIniciales,
                TablaVista = nombreTablaVista
            };
            return JsonSerializer.Serialize(tabla);
        }
             

        public class DataTableAjaxPostModel
        {
            // properties are not capital due to json mapping
            public int draw { get; set; }
            public int start { get; set; }
            public int length { get; set; }
            public List<Column> columns { get; set; }
            public Search search { get; set; }
            public List<Order> order { get; set; }
            public List<SelectListItem> filtrosExtras { get; set; }
            public string tablaVista { get; set; }
        }
        /// <summary>
        /// Clase para declarar las columnas de las distintas Tablas correspondiente a las vistas o tabs.
        /// </summary>
        /// <param name="data">Nombre del campo correspondiente de la Tablas o Vista a trabajar</param>
        /// <param name="name">Nombre del controlador donde ocurrio el Problema</param>
        /// <param name="defaultContent">Nombre de la Acción donde ocurrió el problema</param>
        /// <param name="searchable">Mensaje que deseo que aparezca principalmente</param>
        /// <param name="orderable">Nombre del controlador donde ocurrio el Problema</param>
        /// <param name="visible">Nombre de la Acción donde ocurrió el problema</param>
        /// <returns></returns>
        public class Column
        {
            public string data { get; set; }
            public string title { get; set; }
            public bool searchable { get; set; } 
            public bool orderable { get; set; }
            public string className { get; set; }
            public bool visible { get; set; }
            public Search search { get; set; }
        }

        public class Search
        {
            public string value { get; set; }
            public string regex { get; set; }
        }

        public class Order
        {
            public int column { get; set; }
            public string dir { get; set; }
        }

    }
}