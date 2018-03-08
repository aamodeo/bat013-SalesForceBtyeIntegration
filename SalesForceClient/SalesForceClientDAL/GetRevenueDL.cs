using System;
using System.Data;
using System.Data.SqlClient;

namespace SalesForceClientDAL
{
    public class GetRevenueDL
    {
        CommonDAO objCommonDAO = null;
        SqlCommand ObjCmd = null;
        
        public string GetRevenueDetail(string FileDataID)
        {
            string strExpectedRevenue = string.Empty;             
            objCommonDAO = new CommonDAO();

            try
            {
                ObjCmd = new SqlCommand("select BaseLoan from Loan where FileDataID = '" + FileDataID + "'");

                ObjCmd.CommandType = CommandType.Text;
                ObjCmd.CommandTimeout = 600;
                ObjCmd.Connection = objCommonDAO.GetConnection();
                strExpectedRevenue = Convert.ToString(ObjCmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objCommonDAO.CloseConnection();
            }            
            return strExpectedRevenue;
        }
    }
}
