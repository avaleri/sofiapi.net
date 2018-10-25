using sofiapi.net.Interfaces;
using sofiapi.net.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace sofiapi.net.Services
{
    public class DB : IDB
    {
        private string connectionString;

        public DB()
        {

        }

        public DB(string _connectionString)
        {
            connectionString = _connectionString;
        }

        private DataSet GetRouteByPathDS(string path)
        {
            DataSet ds = new DataSet();
            using(SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlDataAdapter sda = new SqlDataAdapter("usp_Routes_SelByPath", conn))
                {
                    sda.SelectCommand.CommandType = CommandType.StoredProcedure;

                    sda.SelectCommand.Parameters.Add("@RoutePath", SqlDbType.NVarChar).Value = path;

                    conn.Open();
                    sda.Fill(ds);
                    return ds;
                }
            }
        }

        private ApiRoute ConvertRowToRoute(DataRow dr)
        {
            ApiRoute r = new ApiRoute();
            r.RouteID = Convert.ToInt32(dr["RouteID"]);
            r.AppName = dr["AppName"].ToString();
            r.RoutePath = dr["RoutePath"].ToString();
            r.RouteCommand = dr["RouteCommand"].ToString();
            r.AllowNoParameters = Convert.ToBoolean(dr["AllowNoParameters"]);
            r.PublicRoute = Convert.ToBoolean(dr["PublicRoute"]);
            r.PermissionList = dr["PermissionList"] == DBNull.Value ? "" : dr["PermissionList"].ToString();
            r.CreateDt = Convert.ToDateTime(dr["CreateDt"].ToString());
            r.CreatedBy = dr["CreatedBy"].ToString();
            r.ModifiedBy = dr["ModifiedBy"] == DBNull.Value ? "" : dr["ModifiedBy"].ToString();
            r.ModifiedDt = dr["ModifiedDt"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dr["ModifiedDt"].ToString());

            return r;
        }

        private ProcedureParameter ConvertRowToProcedureParameter(DataRow dr)
        {
            ProcedureParameter result = new ProcedureParameter();
            result.ROUTINE_NAME = dr["ROUTINE_NAME"].ToString();
            result.ORDINAL_POSITION = Convert.ToInt32(dr["ORDINAL_POSITION"].ToString());
            result.PARAMETER_MODE = dr["PARAMETER_MODE"].ToString();
            result.PARAMETER_NAME = dr["PARAMETER_NAME"].ToString();
            result.DATA_TYPE = dr["DATA_TYPE"].ToString();
            return result;
        }

        public ApiRoute GetRouteByPath(string path)
        {
            var result = new ApiRoute();
            DataSet ds = GetRouteByPathDS(path);
            if(ds != null && ds.Tables != null && ds.Tables.Count == 2 && ds.Tables[0].Rows.Count >0 && ds.Tables[1].Rows.Count > 0)
            {
                DataRow routeRow = ds.Tables[0].Rows[0];
                result = ConvertRowToRoute(routeRow);
                result.parameters = new List<ProcedureParameter>();
                foreach(DataRow row in ds.Tables[1].Rows)
                {
                    ProcedureParameter param = ConvertRowToProcedureParameter(row);
                    result.parameters.Add(param);
                }
            }
            return result;
        }

        /// <summary>
        /// Takes a Dataset and converts to JSON.
        /// Modified from stackoverflow post https://stackoverflow.com/questions/4166202/sql-dataset-to-json (Scott Kramer)
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public static string DStoJSON(DataSet ds)
        {
            StringBuilder json = new StringBuilder();
            int t = 0;
            int tableCount = ds.Tables.Count;
            if (tableCount > 1)
            {
                json.Append("[");
            }
            foreach (DataTable table in ds.Tables)
            {
                json.Append("[");
                int r = 0;
                int rowCount = table.Rows.Count;
                foreach (DataRow dr in table.Rows)
                {
                    json.Append("{");

                    int i = 0;
                    int colcount = dr.Table.Columns.Count;

                    foreach (DataColumn dc in dr.Table.Columns)
                    {
                        json.Append("\"");
                        json.Append(dc.ColumnName);
                        json.Append("\":\"");
                        json.Append(dr[dc]);
                        json.Append("\"");

                        i++;
                        if (i < colcount) json.Append(",");

                    }
                    json.Append("}");
                    r++;
                    if (r < rowCount)
                    {
                        json.Append(",");
                    }
                }
                json.Append("]");
                t++;
                if (t < tableCount)
                {
                    json.Append(",");
                }
            }
            if (tableCount > 1)
            {
                json.Append("]");
            }
            return json.ToString();
        }

        public string ExecuteRoute(ApiRoute route)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlDataAdapter sda = new SqlDataAdapter(route.RouteCommand, conn))
                {
                    sda.SelectCommand.CommandType = CommandType.StoredProcedure;

                    foreach(var p in route.parameters)
                    {
                        sda.SelectCommand.Parameters.Add(p.PARAMETER_NAME, SqlDbType.NVarChar);
                        sda.SelectCommand.Parameters[p.PARAMETER_NAME].Value = p.VALUE;
                    }

                    conn.Open();
                    sda.Fill(ds);

                    string json = DStoJSON(ds);
                    return json;
                }
            }
        }
    }
}