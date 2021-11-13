using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Zmap.App_Start;

namespace Zmap
{
    public class MvcApplication : System.Web.HttpApplication
    {

        public static bool isIn = false;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_BeginRequest()
        {
            if (isIn == true)
                return;

            try
            {
                string connection = "data source=198.71.227.2;initial catalog=Zmap;User Id = zmapuser;Password = Zm@p2021";
                var con = new SqlConnectionStringBuilder(connection);
                var sqlCon = new SqlConnection(con.ConnectionString);
                sqlCon.Open();
                sqlCon.Close();
            }
            catch (Exception)
            {
                isIn = true;
                Response.Redirect("~/Home/TechnicalSupport/?Error=DBCL");
            }
        }

        //protected void Application_Error(object sender, EventArgs e)
        //{
        //    Exception exception = Server.GetLastError();
        //    Response.Clear();

        //    HttpException httpException = exception as HttpException;

        //    if(httpException != null)
        //    {
        //        Server.ClearError();
        //        Response.Redirect(string.Format("~/Home/TechnicalSupport/?Error={0}", exception.Message.ToString()));
        //    }

        //}

    }
}
