using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace WebApplication1
{
    /// <summary>
    /// Summary description for paint
    /// </summary>
    public class paint : IHttpHandler
    {
        List<Profile> ListData;
        JavaScriptSerializer js;

        public void ProcessRequest(HttpContext context)
        {
            ListData = new List<Profile>();
            js = new JavaScriptSerializer();

            SqlConnection conn = new SqlConnection();
            string strConnection = "Server=tcp:wgdmr4vkzo.database.windows.net,1433;Database=paintie;User ID=avenir@wgdmr4vkzo;Password=1q2w3e4r.;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;";
            conn.ConnectionString = strConnection;
            conn.Open();

            string sql = "SELECT * FROM userdb";
            SqlCommand com = new SqlCommand(sql,conn);
            SqlDataReader dr = com.ExecuteReader();
            DataTable dt = new DataTable();
            if (dr.HasRows)
            {
                dt.Load(dr);
            }

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Profile p = new Profile()
                {
                    Id = (int)dt.Rows[i]["Id"],
                    Name = (string)dt.Rows[i]["Name"],
                    Surname = (string)dt.Rows[i]["Surname"],
                    Age = (int)dt.Rows[i]["Age"]
                };
                ListData.Add(p);
            }

            string str = js.Serialize(ListData);

            context.Response.ContentType = "text/plain";
            context.Response.Write(str);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}