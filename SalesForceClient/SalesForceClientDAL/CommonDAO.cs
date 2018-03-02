using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace SalesForceClientDAL
{
    public class CommonDAO
    {
        private SqlConnection objConn = null;

        public string GetConnectionString()
        {
            string strValue;
            try
            {
                strValue = ConfigurationManager.AppSettings["SQLConnectionString"];
            }
            catch (Exception ex)
            {
                strValue = string.Empty;
                throw ex;
            }

            return strValue;
        }

        public SqlConnection GetConnection()
        {
            if (objConn != null && objConn.State == ConnectionState.Open)
            {
                return objConn;
            }
            else if (objConn != null && objConn.State == ConnectionState.Closed)
            {
                objConn.Open();
                return objConn;
            }
            else
            {
                objConn = new SqlConnection();
                objConn.ConnectionString = GetConnectionString();
                objConn.Open();
                return objConn;
            }
        }

        public void CloseConnection()
        {
            if (objConn != null)
            {
                if (objConn.State == ConnectionState.Open)
                {
                    objConn.Close();
                }
            }
        }
    }
}
