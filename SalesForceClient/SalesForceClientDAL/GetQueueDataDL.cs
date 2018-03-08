using System;
using System.Data;
using System.Data.SqlClient;

namespace SalesForceClientDAL
{
    public class GetQueueDataDL
    {
        CommonDAO objCommonDAO = null;
        SqlCommand ObjCmd = null;
        
        public DataTable GetLoanQueueData() //Lost Loan Status = 4
        {
            DataTable dt = null;
            objCommonDAO = new CommonDAO();

            try
            {
                ObjCmd = new SqlCommand("SELECT TOP 1 FileDataID, QueueID, LoanStatus, OppId FROM [ByteProSFQueue] WHERE status = 'QUEQUED' ORDER BY QueueDateTime ASC");
                ObjCmd.CommandType = CommandType.Text;                
                ObjCmd.CommandTimeout = 600;
                ObjCmd.Connection = objCommonDAO.GetConnection();
                ObjCmd.ExecuteNonQuery();

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
