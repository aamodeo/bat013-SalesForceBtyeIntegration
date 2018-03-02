using System;
using System.Data;
using System.Data.SqlClient;

namespace SalesForceClientDAL
{
    public class PartyDetailDL
    {
        CommonDAO objCommonDAO = null;
        SqlCommand ObjCmd = null;

        public DataTable GetPartyinfo(string FileDataID)
        {
            DataTable dt = null;
            objCommonDAO = new CommonDAO();

            try
            {
                ObjCmd = new SqlCommand("SELECT TOP 1 FileDataID,ContactNMLSID,CompanyNMLSID,BranchID,EMail FROM [BytePro].[dbo].[Party] WHERE CategoryID = 110 and FileDataID = '" + FileDataID + "'");
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

        public void UpdatePartyRecord(string strEmail, string userName, string filedataID)
        {            
            objCommonDAO = new CommonDAO();

            try
            {
                ObjCmd = new SqlCommand("UPDATE[FileData] SET OtherUserName = '" + userName + "' WHERE filedataid = '" + filedataID + "' UPDATE[Party] SET EMail = '" + strEmail + "' WHERE categoryid = 102 and filedataid = '" + filedataID + "'");
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
