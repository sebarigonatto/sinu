using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;
using System.Web.Mvc;
using static SINU.Models.AjaxDataTableModel;
using System.Text;
using System.Collections.Generic;

namespace SINU.Models
{
    [Authorize]
    public class AjaxDataTableController : Controller
    {
        private SINUEntities db = new SINUEntities();

        private string tableClassNameSpace = "SINU.Models";

        public async Task<JsonResult> CustomServerSideSearchActionAsync(DataTableAjaxPostModel model)
        {
            try
            {

                var result = await YourCustomSearchFuncAsync(model);
                var jsonResult = Json(new
                {
                    // this is what datatables wants sending back
                    draw = model.draw,
                    recordsTotal = result.totalResultsCount,
                    recordsFiltered = result.filteredResultsCount,
                    data = result.result
                });
                jsonResult.MaxJsonLength = int.MaxValue;

                return jsonResult;
            }
            catch (Exception ex)
            {

                throw;
            }


        }



        public async Task<resultAjaxTable> YourCustomSearchFuncAsync(DataTableAjaxPostModel model)
        {
            try
            {

            

                var take = model.length;
                var skip = model.start;

                string sortBy = "";
                bool sortDir = true;

                if (model.order != null)
                {
                    // in this example we just default sort on the 1st column
                    sortBy = model.columns[model.order[0].column].data;
                    sortDir = model.order[0].dir.ToLower() == "asc";
                }

                var SWW = await createSWWAsync(model);
                // search the dbase taking into consideration table sorting and paging
                var result = await GetDataFromDbaseAsync(model.tablaVista,SWW, take, skip, sortBy, sortDir);
                //if (result == null)
                //{
                //    // empty collection...
                //    return new List<vInscripcionDetalleUltInsc>();
                //}
                return result;
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public async Task<List<dynamic>> createSWWAsync(DataTableAjaxPostModel model) {
            try
            {

                var searchBy = (model.search != null) ? model.search.value : null;

                //obtengo el tipo de cada columna
                var columnaTipo = db.Database.SqlQuery<tipoColumna>($"SELECT COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS where table_name ='{model.tablaVista}'").ToList();

                //armo un string con las columnas requeridad para relizar un select
                string selectColumn = "", select;

                int columnCount = 0;

                foreach (var columna in model.columns)
                {
                    select = "";
                    switch (columnaTipo.First(c => c.COLUMN_NAME == columna.data).DATA_TYPE)
                    {
                        case "varchar":
                        case "nvarchar":
                        case "bit":
                        case "nchar":
                            select = columna.data;
                            break;
                        case "int":
                        case "decimal":
                            select = $"(Int32({ columna.data})).ToString() as { columna.data}";
                            break;
                        case "date":
                        case "datetime":
                            select = $"(DateTime({columna.data})).ToString() as {columna.data}";
                            break;
                    }

                    columnCount++;

                    selectColumn += $"{select} {(columnCount < model.columns.Count() ? ',' : ' ')}";
                }



                //armo el where 
                string[] searchWhereDT = new string[] { };
                string whereDT = "";

                // where con la variable searchBy
                if (searchBy != null)
                {
                    searchWhereDT = searchBy.Split(' ').ToList().ConvertAll(m => m.ToLower()).ToArray();

                    int indexParametro = 0;

                    foreach (var param in searchWhereDT)
                    {
                        string wheres = "";
                        int indexColumna = 0;
                        foreach (var columna in model.columns.Where(m => m.searchable))
                        {
                            indexColumna++;
                            wheres += $"it.{columna.data}.Contains(@{indexParametro})" + (indexColumna < model.columns.Where(m => m.searchable).Count() ? " or " : "");
                        }
                        indexParametro++;

                        whereDT += $"({wheres}) {(indexParametro < searchWhereDT.Count() ? " and " : "")}";
                    }
                }
                else//trigo todos los elementos
                {
                    whereDT = "true";
                }


                //where si existen filtros extras, como de los dropboxs
                string[] searchWhereExtras = new string[] { };
                string whereExtras = "";
                var filtrosEx = model.filtrosExtras.Where(m => m.Valor != null).ToList();
                if (filtrosEx.Count() > 0)
                {
                    filtrosEx.Where(m => m.Valor == "null").ToList().ForEach(m => m.Valor = "");

                    searchWhereExtras = filtrosEx.Select(m => m.Valor).ToArray();
                    //clausula where
                    int indexExtras = 0;
                    string condicion = "";
                    foreach (var filtros in filtrosEx)
                    {
                        indexExtras++;

                        switch (columnaTipo.First(c => c.COLUMN_NAME == filtros.Columna).DATA_TYPE)
                        {
                            case "varchar":
                            case "nvarchar":
                            case "nchar":
                                condicion = $"{filtros.Columna}.Contains(@{indexExtras - 1})";
                                break;
                            case "int":
                                condicion = $"{filtros.Columna}{filtros.Condicion}@{indexExtras - 1}";
                                break;
                            case "bit":
                                condicion = $"{filtros.Columna}=={filtros.Valor}";
                                break;
                            case "datetime":
                            case "date":
                                condicion = $"{filtros.Columna}==@{indexExtras - 1}";
                                break;
                        }
                        whereExtras += condicion;
                        whereExtras += (indexExtras < filtrosEx.Count() ? " and " : "");
                    }
                }
                else
                {
                    whereExtras = "true";
                }

                return new List<dynamic> { selectColumn , whereDT, searchWhereDT, whereExtras, searchWhereExtras };
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<resultAjaxTable> GetDataFromDbaseAsync(string nombreTablaVista,List<dynamic> SWW, int take, int skip, string sortBy, bool sortDir)
        {
            try
            {
                resultAjaxTable result = new resultAjaxTable();

                string dirSort = sortDir ? "ASC" : "DESC";

                              
                
                db.Database.CommandTimeout = 36000;

                var totalResultsCount = getCountAsync(nombreTablaVista, SWW);
                var registrosWhere = db.Set(Type.GetType($"{tableClassNameSpace}.{nombreTablaVista}"))
                                       .Select($"new({(string)SWW[0]})")
                                       .Where((string)SWW[3], (string[])SWW[4])
                                       .Where((string)SWW[1], (string[])SWW[2])
                                       .OrderBy($"{sortBy} {dirSort}")
                                       .ToListAsync();

                Task.WaitAll( totalResultsCount, registrosWhere);

                result.totalResultsCount = totalResultsCount.Result;

                result.filteredResultsCount = registrosWhere.Result.Count();
                
                result.result = registrosWhere.Result.Skip(skip)
                                                     .Take(take > 0 ? take : result.totalResultsCount)
                                                     .ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }


        }


        public async Task<int> getCountAsync(string nombreTablaVista, List<dynamic> SWW)
        {
           return db.Set(Type.GetType($"{tableClassNameSpace}.{nombreTablaVista}"))
                                                                      .Select($"new({(string)SWW[0]})")
                                                                      .Where((string)SWW[3], (string[])SWW[4])
                                                                      .Count();
        }

        public async Task<JsonResult> DataTableToCSV(DataTableAjaxPostModel model,int recordsTotal, string titulo)
        {
            try
            {
                
                StringBuilder sb = new StringBuilder();
                var SWW = await createSWWAsync(model);
                string sortBy = "";
                bool sortDir = true;

                if (model.order != null)
                {
                    // in this example we just default sort on the 1st column
                    sortBy = model.columns[model.order[0].column].data;
                    sortDir = model.order[0].dir.ToLower() == "asc";
                }
                string dirSort = sortDir ? "ASC" : "DESC";

                var registrosWhere =await db.Set(Type.GetType($"{tableClassNameSpace}.{model.tablaVista}")).Select($"new({(string)SWW[0]})")
                                     .Where((string)SWW[3], (string[])SWW[4])
                                     .Where((string)SWW[1], (string[])SWW[2])
                                     .OrderBy($"{sortBy} {dirSort}")                                 
                                     .ToListAsync();
             
                for (int i = 0; i < model.columns.Count; i++)
                {
                    sb.Append($"\"{model.columns[i].data}\"");
                    if (i < model.columns.Count - 1)
                        sb.Append(';');
                }
                sb.AppendLine();
            
                foreach (DynamicClass dr in registrosWhere)
                {
                    
                    for (int i = 0; i < model.columns.Count; i++)
                    {
                        var value = $"\"{dr.GetType().GetProperty(model.columns[i].data).GetValue(dr)??""}\"";
                        sb.Append(value.ToString());

                        if (i < model.columns.Count - 1)
                           sb.Append(';');
                    } 
                    sb.AppendLine();
                }
                var jsonResult = Json(new { 
                                            content = sb.ToString(), 
                                            charset = "text/csv;charset=utf-8;", 
                                            nameFile = $"{titulo}.csv"
                });
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                throw;
            }
           
        }
    }



}