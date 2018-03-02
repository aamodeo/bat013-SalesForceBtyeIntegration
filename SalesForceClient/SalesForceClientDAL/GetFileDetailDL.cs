using System;
using System.Data;
using System.Data.SqlClient;

namespace SalesForceClientDAL
{
    public class GetFileDetailDL
    {
        CommonDAO objCommonDAO = null;
        SqlCommand ObjCmd = null;

        public DataTable GetFileDetail(string FileDataID)
        {
            DataTable dt = null;
            objCommonDAO = new CommonDAO();

            try
            {
                ObjCmd = new SqlCommand("SELECT FileName,CONVERT(date,(dateadd(day,3, DateCreated))) as CloseDate FROM FileData WHERE FileDataID = '" + FileDataID + "'");

                ObjCmd.CommandType = CommandType.Text;
                ObjCmd.CommandTimeout = 600;
                ObjCmd.Connection = objCommonDAO.GetConnection();
               
                SqlDataAdapter sda = new SqlDataAdapter(ObjCmd);
                dt = new DataTable();
                sda.Fill(dt);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objCommonDAO.CloseConnection();
            }

            return dt;
        }
    }
}
