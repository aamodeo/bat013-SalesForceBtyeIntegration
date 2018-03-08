using SalesForceClientDAL;
using SalesForceClientEntities;
using SalesForceClientEntities.Account;
using SalesForceClientEntities.Contact;
using SalesForceClientEntities.User;
using SFClientServiceLogger;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

using acc = SalesForceClientEntities.Account;
using con = SalesForceClientEntities.Contact;
using olis = SalesForceClientEntities.OpportunityLineItemSchedules;
using usr = SalesForceClientEntities.User;

namespace SalesForceClientBL
{
    public static class HttpClientExtensions
    {
        public static async Task<HttpResponseMessage> PatchAsync(this HttpClient httpClient, Uri requestUri, HttpContent httpContent)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();

            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(new HttpMethod("PATCH"), requestUri);
            httpRequestMessage.Content = httpContent;

            httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

            return httpResponseMessage;
        }
    }

    public class SalesForceBL
    {
        JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
        HttpResponseMessage httpResponseMessage = null;        
        PartyDetailDL partyDAL = new PartyDetailDL();
        GetQueueDataDL queuedataDAL = new GetQueueDataDL();
        GetFileDetailDL fileDetailDAL = new GetFileDetailDL();
        UpdateQueueDataDL UpdateQueueDataDL = new UpdateQueueDataDL();
        GetRevenueDL revenueDAL = new GetRevenueDL();

        public SalesForceConnectionData GetSalesForceConnectionData()
        {
            SalesForceConnectionData salesForceConnectionData = new SalesForceConnectionData();

            salesForceConnectionData.consumerKey = ConfigurationManager.AppSettings["consumerKey"];
            salesForceConnectionData.consumerSecret = ConfigurationManager.AppSettings["consumerSecret"];
            salesForceConnectionData.username = ConfigurationManager.AppSettings["username"];
            salesForceConnectionData.password = ConfigurationManager.AppSettings["password"];
            salesForceConnectionData.securityToken = ConfigurationManager.AppSettings["securityToken"];
            salesForceConnectionData.loginHost = ConfigurationManager.AppSettings["loginHost"];
            salesForceConnectionData.baseUrl = ConfigurationManager.AppSettings["baseUrl"];

            return salesForceConnectionData;
        }

        //LoanStatus = 4
        public DataTable GetLoanQueueData()
        {
            return queuedataDAL.GetLoanQueueData();            
        }

        ////Lost Loan Status = 9
        //public DataTable GetClosedLoanQueueData()
        //{
        //    return queuedataDAL.GetClosedLoanQueueData();
        //}

        ////Lost Loan Status = 10 or 59
        //public DataTable GetLostLoanQueueData()
        //{
        //    return queuedataDAL.GetLostLoanQueueData();
        //}

        ////Lost Loan Status = 12
        //public DataTable GetSuspendedLoanQueueData()
        //{
        //    return queuedataDAL.GetSuspendedLoanQueueData();
        //}

        

        //Status = PROCESSED
        public void UpdateQueueRecord(string status, string QueueID)
        {
            UpdateQueueDataDL.UpdateQueueRecord(status, QueueID);
        }

        //Status = PROCESSING / ERROR
        //msg = OppID / Error detail
        public void UpdateQueueRecord(string queueid, string status,string msg)
        {
            UpdateQueueDataDL.UpdateQueueRecord(queueid,status, msg);
        }

        public DataTable GetFileDetail(string FileDataID)
        {
            return fileDetailDAL.GetFileDetail(FileDataID);
        }

        public DataTable GetPartyinfo(string FileDataID)
        {
            return partyDAL.GetPartyinfo(FileDataID);
        }

        public void UpdatePartyRecord(string strEmail, string userName, string filedataID)
        {
            partyDAL.UpdatePartyRecord(strEmail, userName, filedataID);
        }

        public string GetRevenueinfo(string FileDataID)
        {
            return revenueDAL.GetRevenueDetail(FileDataID);
        }


        public async Task<string> CreateOpportunityAsync(SalesForceResponse salesForceResponse, SalesForceOpprtunity opp)
        {
            LoggerService.Debug("Entered CreateOpportunityAsync", "INFO");
            string opportunityId = string.Empty;
            try
            {
                CreateOpportunityFields createOpportunityFields = new CreateOpportunityFields();

                createOpportunityFields.Name = opp.OppurtunityName;
                createOpportunityFields.Contact__c = opp.ContactID;
                createOpportunityFields.CloseDate = opp.closedate;
                createOpportunityFields.StageName = opp.stage;                            
                createOpportunityFields.AccountId = opp.AccountID;                
                createOpportunityFields.Byte_FileDataID__c = opp.ByteFileDataID;
                
                LoggerService.Debug("Setting Opp data, and calling CreateOpportunityAsync()", "");
                opportunityId = await CreateOpportunityAsync(salesForceResponse, createOpportunityFields);

                bool isUpdated = false;
                if (opportunityId != null)
                    isUpdated = await UpdateOpportunityAsync(salesForceResponse, opp, opportunityId, "4"); //Creating New opp based on loanstatus code = 4
            }
            catch (Exception ex)
            {
                LoggerService.Error("", "SalesForceBL->CreateOpportunityAsync(): " + "\r\nError Message: " + ex.Message + "\r\nStackTrace: " + ex.StackTrace);
            }
            
            return opportunityId;
        }

        private async Task<string> CreateOpportunityAsync(SalesForceResponse salesForceResponse, CreateOpportunityFields createOpportunityFields)
        {
            string opportunityId = string.Empty;
            string createOpportunityUrl = salesForceResponse.instance_url + "/services/data/v26.0/sobjects/Opportunity/";

            LoggerService.Debug("Entered CreateOpportunityAsync()", "INFO");
            try
            {
                LoggerService.Debug("Inside CreateOpportunityAsync() 2", "");
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", "OAuth " + salesForceResponse.access_token);

                    httpResponseMessage = await httpClient.PostAsJsonAsync(createOpportunityUrl, createOpportunityFields);
                    LoggerService.Debug("CreateOpportunityAsync()- Response for httpResponseMessage:", httpResponseMessage);

                    var opportunityUserResult = await httpResponseMessage.Content.ReadAsStringAsync();
                    LoggerService.Debug("CreateOpportunityAsync() - Response for opportunityUserResult :", opportunityUserResult);

                    if (httpResponseMessage != null && httpResponseMessage.IsSuccessStatusCode)
                    {
                        LoggerService.Debug("httpResponseMessage = SuccessStatusCode ", "");
                        var opportunityUserResultObject = (IDictionary<string, object>)javaScriptSerializer.DeserializeObject(opportunityUserResult);
                        opportunityId = Convert.ToString(opportunityUserResultObject["id"]);
                        LoggerService.Debug("opportunityId:", opportunityId);
                    }
                    else
                        LoggerService.Debug("httpResponseMessage = Error ", httpResponseMessage);

                }
            }
            catch (Exception ex)
            {
                LoggerService.Error("", "SalesForceBL->CreateOpportunityAsync() 2: " + "\r\nError Message: " + ex.Message + "\r\nStackTrace: " + ex.StackTrace);
            }
            
            return opportunityId;
        }

        public async Task<OpportunityUpdateFields> GetOpportunityAsync(SalesForceResponse salesForceResponse, string filterCode)
        {
            string query = "Select op.Id,op.Name  From Opportunity op";
            string queryURL = salesForceResponse.instance_url + "/services/data/v24.0/query/?q=" + query;

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", "OAuth " + salesForceResponse.access_token);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                httpResponseMessage = await httpClient.GetAsync(queryURL);
            }

            if (httpResponseMessage != null && httpResponseMessage.IsSuccessStatusCode)
            {
                var OppContent = await httpResponseMessage.Content.ReadAsStringAsync();

                olis.OpportunityLineItemSchedules oppResults = (olis.OpportunityLineItemSchedules)javaScriptSerializer.Deserialize(OppContent, typeof(olis.OpportunityLineItemSchedules));               
            }
            return null;
        }

        //public async Task<bool> UpdateOpportunityAsync(SalesForceResponse salesForceResponse, SalesForceOpprtunity opp, string opportunityId)
        public async Task<bool> UpdateOpportunityAsync(SalesForceResponse salesForceResponse, SalesForceOpprtunity opp, string opportunityId, string loanStatusCode)
        {
            LoggerService.Debug("Entered UpdateOpportunityAsync", "INFO");
            bool isOpportunityUpdated = false;
            OpportunityUpdateFields ouf = new OpportunityUpdateFields();

            ouf.Name = opp.OppurtunityName;

            if (loanStatusCode == "4") //New Loan
            {
                LoggerService.Debug("loanStatusCode = 4", "INFO");
                ouf.StageName = opp.stage;
                ouf.CloseDate = opp.closedate;
                ouf.OwnerID = opp.OwnerID;
            }
            else if (loanStatusCode == "9" || loanStatusCode == "10" || loanStatusCode == "59" || loanStatusCode == "12")
            {
                LoggerService.Debug("loanStatusCode: ", loanStatusCode);                                
                ouf.StageName = opp.stage;
                ouf.OwnerID = opp.OwnerID;
                ouf.CloseDate = opp.closedate;
                ouf.Amount = opp.Amount;
            }

            string opportunityUpdateUrl = salesForceResponse.instance_url + "/services/data/v26.0/sobjects/Opportunity/" + opportunityId;

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", "OAuth " + salesForceResponse.access_token);
                HttpContent httpcontent = new StringContent(javaScriptSerializer.Serialize(ouf), Encoding.UTF8, "application/json");

                httpResponseMessage = await httpClient.PatchAsync(new Uri(opportunityUpdateUrl), httpcontent);
                LoggerService.Debug("httpResponseMessage :", httpResponseMessage);

                var opportunityUpdateResult = await httpResponseMessage.Content.ReadAsStringAsync();
                LoggerService.Debug("opportunityUpdateResult :", opportunityUpdateResult);


                if (httpResponseMessage != null && httpResponseMessage.IsSuccessStatusCode)
                {
                    isOpportunityUpdated = true;
                }
            }

            LoggerService.Debug("Leaving UpdateOpportunityAsync", "INFO");
            return isOpportunityUpdated;
        }

        public async Task<AccountFields> GetAccountAsync(SalesForceResponse salesForceResponse, string accountNMLSID)
        {
            LoggerService.Debug("Entered GetAccountAsync()", "INFO");

            string query = "Select acc.Id, acc.Name,Parent.Id From Account acc where acc.NMLS__c = '" + accountNMLSID + "'";
            string queryURL = salesForceResponse.instance_url + "/services/data/v24.0/query/?q=" + query;

            AccountFields accFields = new AccountFields();
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", "OAuth " + salesForceResponse.access_token);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                httpResponseMessage = await httpClient.GetAsync(queryURL);
            }

            if (httpResponseMessage != null && httpResponseMessage.IsSuccessStatusCode)
            {
                var AccContent = await httpResponseMessage.Content.ReadAsStringAsync();

                acc.AccountDetails accResult = ( acc.AccountDetails)javaScriptSerializer.Deserialize(AccContent, typeof(acc.AccountDetails));
                if (accResult != null && accResult.records.Count > 0)
                {
                    accFields.AccountId = accResult.records[0].Id;
                    accFields.ParentId = accResult.records[0].Parent;
                }
            }
            LoggerService.Debug("Leaving GetAccountAsync()", "INFO");
            return accFields;
        }

        public async Task<ContactFields> GetContactAsync(SalesForceResponse salesForceResponse, string contactNMLSID)
        {
            LoggerService.Debug("Entered GetContactAsync()", "INFO");
            string query = "Select conta.ID, conta.Owner.ID,conta.Account.ID From Contact conta where conta.NMLS__c = '" + contactNMLSID + "'";
            string queryURL = salesForceResponse.instance_url + "/services/data/v24.0/query/?q=" + query;
            ContactFields conFields = new ContactFields();

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", "OAuth " + salesForceResponse.access_token);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                httpResponseMessage = await httpClient.GetAsync(queryURL);
            }

            if (httpResponseMessage != null && httpResponseMessage.IsSuccessStatusCode)
            {
                var ContactContent = await httpResponseMessage.Content.ReadAsStringAsync();

                con.ContactDetails conResult = (con.ContactDetails)javaScriptSerializer.Deserialize(ContactContent, typeof(con.ContactDetails));

                if (conResult != null && conResult.records.Count > 0)
                {
                    conFields.ContactId = conResult.records[0].Id;
                    conFields.OwnerId = conResult.records[0].Owner.Id;
                    conFields.AccountId = conResult.records[0].account.Id;
                }
            }
            LoggerService.Debug("Leaving GetContactAsync()", "INFO");
            return conFields;
        }

        /// <summary>
        /// Get user details based on ownerid being passed.
        /// </summary>
        /// <param name="salesForceResponse"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<UserFields> GetUserAsync(SalesForceResponse salesForceResponse, string userId) //UserID = OwnerID
        {
            LoggerService.Debug("Entered GetUserAsync()", "INFO");
            string query = "Select usr.Email,usr.Username, usr.Name From User usr where usr.Id = '" + userId + "'";            
            string queryURL = salesForceResponse.instance_url + "/services/data/v24.0/query/?q=" + query;

            
            UserFields userFields = new UserFields();
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", "OAuth " + salesForceResponse.access_token);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                httpResponseMessage = await httpClient.GetAsync(queryURL);
            }

            if (httpResponseMessage != null && httpResponseMessage.IsSuccessStatusCode)
            {
                var AccContent = await httpResponseMessage.Content.ReadAsStringAsync();

                usr.UserDetails accResult = (usr.UserDetails)javaScriptSerializer.Deserialize(AccContent, typeof(usr.UserDetails));
                if (accResult != null && accResult.records.Count > 0)
                {                    
                    userFields.Name = accResult.records[0].Name;
                    userFields.Username = accResult.records[0].Username;
                    userFields.Email = accResult.records[0].Email;
                }
            }
            LoggerService.Debug("Leaving GetUserAsync()", "INFO");
            return userFields;
        }
    }
}
