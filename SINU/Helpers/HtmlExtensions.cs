﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;


namespace SINU.Helpers
{
    public static class HtmlExtensions
    {
        /// <summary>
        /// HtmlHelper personalizado para el armado de un tabla
        /// </summary>
        /// <param name="idTabla">ID de la tabla para su identificacion durante las busqueda y actualizacion de datos</param>
        /// <param name="themeHead">Tema para el encabezadp de la tabla. Opciones: primary, secondary, success, etc.</param>
        /// <returns></returns>
        public static MvcHtmlString TablaHelper<TModel>(this HtmlHelper<TModel> htmlHelper, string idTabla, string themeHead)
        {
            //etiquetas para el armado de la tabla            
            //TR TD TH
            var Tr = new TagBuilder("tr");
            var Td = new TagBuilder("td");

            //THEAD
            var Thead = new TagBuilder("thead");
            Thead.AddCssClass($"thead-{themeHead}");
            Thead.InnerHtml=Tr.ToString();

            //TBODY
            var Tbody = new TagBuilder("tbody");
            Td.InnerHtml = "Ningún dato disponible en esta tabla";
            Td.AddCssClass("text-center");
            Tr.InnerHtml = Td.ToString();
            Tbody.InnerHtml = Tr.ToString();

            //TABLA
            var table = new TagBuilder("table");
            table.AddCssClass("table table-filters table-bordered table-light table-hover");
            table.Attributes.Add("id", idTabla);
            table.InnerHtml = Thead.ToString()+Tbody.ToString();
            
            return MvcHtmlString.Create(table.ToString());
        }
    }
}