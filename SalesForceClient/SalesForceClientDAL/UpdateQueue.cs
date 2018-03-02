using System;
using System.Data;
using System.Data.SqlClient;

namespace SalesForceClientDAL
{
    public class UpdateQueueDataDL
    {
        CommonDAO objCommonDAO = null;
        SqlCommand ObjCmd = null;

        public void UpdateQueueRecord(string status, string QueueID)
        {
            string FileDataID = string.Empty;
            objCommonDAO = new CommonDAO();

            try
            {               
                ObjCmd = new SqlCommand("UPDATE [ByteProSFQueue] SET Status='" + status + "' WHERE QueueID='"+ QueueID +"'");
                ObjCmd.CommandType = CommandType.Text;                
                ObjCmd.CommandTimeout = 600;
                ObjCmd.Connection = objCommonDAO.GetConnection();
                ObjCmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objCommonDAO.CloseConnection();
            }
        }

        public void UpdateQueueRecord(string queueid, string status, string msg)
        {
            string FileDataID = string.Empty;
            objCommonDAO = new CommonDAO();

            try
            {
                if (status == "PROCESSED")
                    ObjCmd = new SqlCommand("UPDATE[ByteProSFQueue] SET Status = 'PROCESSED', ProcessedDateTime = GETDATE(), OppId = '" + msg + "' WHERE QueueID = '" + queueid + "'");
                else if (status == "ERROR")
                    ObjCmd = new SqlCommand("UPDATE [ByteProSFQueue] SET Status='ERROR',ProcessedDateTime=GETDATE(),Error='" + msg + "' WHERE QueueID = '" + queueid + "'");

                ObjCmd.CommandType = CommandType.Text;
                ObjCmd.CommandTimeout = 600;
                ObjCmd.Connection = objCommonDAO.GetConnection();
                ObjCmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objCommonDAO.CloseConnection();
            }
        }
    }    
}
