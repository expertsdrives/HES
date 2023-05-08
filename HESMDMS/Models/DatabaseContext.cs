using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace HESMDMS.Models
{
    public class DatabaseContext
    {
        public SqlConnectionStringBuilder Sbu = new SqlConnectionStringBuilder();
        public static SqlConnection GlobalConnection = new SqlConnection();

        public static SqlConnection GlobalConnection_Web = new SqlConnection();

        public SqlConnectionStringBuilder Sbu_Web = new SqlConnectionStringBuilder();
        public bool OpenConnection_Web()
        {
            lock (this)
            {
                try
                {
                    if (GlobalConnection_Web.State.ToString().ToUpper() == "OPEN")
                    {
                        GlobalConnection_Web.Close();
                    }

                    Sbu_Web.DataSource = @"52.140.120.20,20349";
                    Sbu_Web.InitialCatalog = "SmartMeter";
                    Sbu_Web.IntegratedSecurity = false;
                    //Sbu_Web.IntegratedSecurity = true;
                    //Sbu_Web.PersistSecurityInfo = false;
                    Sbu_Web.MultipleActiveResultSets = true;
                    //Sbu_Web.PersistSecurityInfo = false;
                    Sbu_Web.ConnectTimeout = 0;
                    Sbu_Web.UserID = "azureadmin";
                    Sbu_Web.Password = "Itouch@2015";

                    GlobalConnection_Web.ConnectionString = Sbu_Web.ConnectionString;
                    GlobalConnection_Web.Open();

                    return true;
                }
                catch (Exception Ex)
                {
                    return false;
                }
            }
        }

        public DataSet RunDynamicSP(string spname, string strConnType)
        {
            lock (this)
            {
                SqlDataAdapter da = null;
                DataSet ds = null;

                SqlCommand SelCmd = new SqlCommand();

                try
                {
                    if (strConnType.Trim().ToUpper() == "LOCAL")
                    {

                        OpenConnection_Web();
                        SelCmd.Connection = GlobalConnection;
                    }
                    else if (strConnType.Trim().ToUpper() == "WEB")
                    {

                        OpenConnection_Web();
                        SelCmd.Connection = GlobalConnection_Web;
                    }



                    SelCmd.CommandText = spname;
                    SelCmd.CommandType = CommandType.StoredProcedure;
                    da = new SqlDataAdapter(SelCmd);
                    ds = new DataSet();
                    da.Fill(ds);
                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }

                    return ds;

                }
                catch (Exception Ex)
                {
                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }



                    return ds;
                }
                finally
                {
                    if (SelCmd != null)
                    {
                        SelCmd.Dispose();
                    }


                }
            }
        }

    }
}