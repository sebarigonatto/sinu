using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace SINU.Models
{

    public class parametro
    {
        public int filteredResultsCount { get; set; }
        public int totalResultsCount { get; set; }

        public List<object> result { get; set; }
    }

    public class DataTableVM
    {
        public List<ModelDataTable.Column> Columnas { get; set; }
        public string TablaVista { get; set; }

    } 
 
    public static class ExtensionClass
    {
        public static IOrderedQueryable<T> OrderByDT<T>(this IQueryable<T> query, string memberName, bool asc)
        {
            ParameterExpression[] typeParams = new ParameterExpression[] { Expression.Parameter(typeof(T), "") };
            System.Reflection.PropertyInfo pi = typeof(T).GetProperty(memberName);
            var re =  (IOrderedQueryable<T>)query.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable),
                    asc ? "OrderBy" : "OrderByDescending",
                    new Type[] { typeof(T), pi.PropertyType },
                    query.Expression,
                    Expression.Lambda(Expression.Property(typeParams[0], pi), typeParams))
                );
            return re;
        }

    }

    public class ModelDataTable
    {
    

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
            public string name { get; set; }
            public string title { get; set; }
            public bool searchable { get; set; } = true;
            public bool orderable { get; set; } = true;
            public bool visible { get; set; } = true;
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