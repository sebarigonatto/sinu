using System;
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

    public class opcionDTAjax
    {
        public string action { get; set; }
        public string constroller { get; set; }
        public string columna { get; set; }
        public string className { get; set; }
    }

    public class filtroExtra
    {
        public string Columna { get; set; }
        public string Valor { get; set; }
        public string Condicion { get; set; } = "==";
    }

    public class DataTableVM
    {
        public List<Column> Columnas { get; set; }
        public string TablaVista { get; set; }
        public string IdTabla { get; set; } = null;
        public List<filtroExtra> filtrosExtras { get; set; } = new List<filtroExtra>() { new filtroExtra()};
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
        public static Column ColumnaDTAjax(string nombreColumna, bool visible = false, bool searchable = false, string nombreDisplay = null, bool noPrint = false, bool orderable = false, string className="")
        {
            return new Column
            {
                data = nombreColumna,
                title = nombreDisplay ?? nombreColumna,
                searchable = searchable,
                orderable = orderable,
                visible = visible,
                className = noPrint ? "noPrint "+className : className
            };
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
            public List<filtroExtra> filtrosExtras { get; set; }
            public string tablaVista { get; set; }
        }
       
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