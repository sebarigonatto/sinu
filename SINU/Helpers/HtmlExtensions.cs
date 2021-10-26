using SINU.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using System.Web;
using System.Web.Mvc;


namespace SINU.Helpers
{
    public static class HtmlExtensions
    {
        /// <summary>
        /// HtmlHelper personalizado para el armado de un tabla
        /// </summary>
        /// <param name="idTablaVista">IdTabla, si es null colocar TablaVista</param>
        /// <param name="themeHead">Tema para el encabezadp de la tabla, por defecto "primary". Opciones: primary, secondary, success,dark etc.</param>
        /// <returns></returns>
        public static MvcHtmlString TablaHelper<TModel>(this HtmlHelper<TModel> htmlHelper, string idTablaVista, string themeHead= "primary")
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
            Td.Attributes.Add("colspan", "100");
               Tr.InnerHtml = Td.ToString();
            Tbody.InnerHtml = Tr.ToString();

            //TABLA
            var table = new TagBuilder("table");
            table.AddCssClass("table table-bordered table-hover nowrap w-100");
            table.Attributes.Add("id",$"dataTable-{idTablaVista}" );
            table.InnerHtml = Thead.ToString()+Tbody.ToString();
            
            return MvcHtmlString.Create(table.ToString());
        }

        public static MvcHtmlString tablaToJson<TModel>(this HtmlHelper<TModel> htmlHelper, dynamic tabla)
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(tabla, Newtonsoft.Json.Formatting.Indented);
            return MvcHtmlString.Create(json);
        }

    }
}