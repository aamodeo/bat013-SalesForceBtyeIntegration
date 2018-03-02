using log4net;
using SalesForceClientBL;
using SalesForceClientEntities;
using SFClientServiceLogger;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Http;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SFClientService
{
    public partial class SFService : ServiceBase
    {
        SalesForceBL objSalesForceBL = new SalesForceClientBL.SalesForceBL();
        SalesForceResponse salesForceResponse = null;
        JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
        HttpResponseMessage httpResponseMessage = null;
        
        public ILog log;

        public SFService()
        {
            InitializeComponent();
            log4net.GlobalContext.Properties["LogPath"] = String.Concat("", ".log");
            log4net.GlobalContext.Properties["NDC"] = "========================================================================================================";
            log4net.Config.XmlConfigurator.Configure();
            this.log = LogManager.GetLogger("UserId");
        }

        public string BuildLogFolderStructure()
        {
            string strFolderName = DateTime.Today.Year.ToString() + @"\";
            try
            {
                strFolderName += DateTime.Now.ToString("MMMM") + @"\";
                string CurrDirectoryPath = strFolderName;
                if (!File.Exists(CurrDirectoryPath))
                {
                    Directory.CreateDirectory(CurrDirectoryPath);
                }
                strFolderName += DateTime.Today.ToShortDateString();
                strFolderName = strFolderName.Replace(@"/", @"-");
            }
            catch (Exception ex)
            {
                LoggerService.Error("", "SFService-BuildLogFolderStructure: " + "\r\nError Message: " + ex.Message + "\r\nStackTrace: " + ex.StackTrace);
            }
            return strFolderName;
        }

        internal void DebugStartAndStop(string[] args)
        {
            OnStart(args);
            Console.ReadLine();
            OnStop();
        }

        public  void Start(string[] args)
        {
            OnStart(args);
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                LoggerService.Debug("SFService started", "");                
                SalesForceCRUD().Wait();
            }
            catch (Exception ex)
            {
                LoggerService.Error("", "SFService->OnStart(): " + "\r\nError Message: " + ex.Message + "\r\nStackTrace: " + ex.StackTrace);
            }
        }

        protected override void OnStop()
        {
            try
            {
                LoggerService.Debug("SFService stopped", "");
            }
            catch (Exception ex)
            {
                LoggerService.Error("", "SFService->OnStop(): " + "\r\nError Message: " + ex.Message + "\r\nStackTrace: " + ex.StackTrace);
            }
        }        

        async Task SalesForceCRUD()
        {
            string sfAuthenticationResult = string.Empty;
            bool processData = true;
            string strFileDataID = null;
            string queueid = "-1";

            try
            {
                DateTime start = DateTime.Now;                

                LoggerService.Debug("Salesforce authentication started at ", start);
                
                SalesForceConnectionData sfd = objSalesForceBL.GetSalesForceConnectionData();

                using (HttpClient httpClient = new HttpClient())
                {
                    FormUrlEncodedContent formUrlEncodedContent = new FormUrlEncodedContent(new[]
                    {
                       new KeyValuePair<string, string>("grant_type", "password"),
                       new KeyValuePair<string, string>("username",  sfd.username),
                       new KeyValuePair<string, string>("password", sfd.password+sfd.securityToken),
                       new KeyValuePair<string, string>("client_id", sfd.consumerKey),
                       new KeyValuePair<string, string>("client_secret", sfd.consumerSecret)
                   });

                    //The line below enables TLS1.1 and TLS1.2
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    httpResponseMessage = await httpClient.PostAsync(sfd.baseUrl, formUrlEncodedContent);
                    sfAuthenticationResult = await httpResponseMessage.Content.ReadAsStringAsync();
                }

                if (httpResponseMessage != null && httpResponseMessage.IsSuccessStatusCode)
                {
                    salesForceResponse = (SalesForceResponse)jsonSerializer.Deserialize(sfAuthenticationResult, typeof(SalesForceResponse));

                    sfd.restServiceUrl = salesForceResponse.instance_url + ConfigurationManager.AppSettings["restServiceUrlPostfix"];

                    LoggerService.Debug("Logged in as " + sfd.username + " in environment " + sfd.loginHost + "\n", "");
                    LoggerService.Debug("postback_url:", sfd.baseUrl);

                    LoggerService.Debug("response:", jsonSerializer.Serialize(salesForceResponse));

                    LoggerService.Debug("access_token:", salesForceResponse.access_token);

                    LoggerService.Debug("instance_url:", salesForceResponse.instance_url);

                    LoggerService.Debug("restServiceUrl:", sfd.restServiceUrl);

                    LoggerService.Debug("It took " + (DateTime.Now - start).Hours + " Hour(s) " + (DateTime.Now - start).Minutes + " Minute(s) " + (DateTime.Now - start).Seconds + " Second(s) in salesforce authentication", "");

                    /*DateTime hashRecordsFetchStart = DateTime.Now;
                    LoggerService.Debug("Hash records fetching started from db at ", hashRecordsFetchStart);
                    DateTime recordsFetchStart = DateTime.Now;
                    LoggerService.Debug("Account records fetching started from db at ", recordsFetchStart);*/
                }
                else
                {
                    LoggerService.Error("", "SFService->SalesForceCRUD(): " + "\r\nError Message: " + httpResponseMessage.StatusCode);
                }
                
                while (processData)
                {
                    try
                    {
                        //Get top 1 record to process [Query 1]                        
                        DataTable dtQueueDetail = objSalesForceBL.GetQueueData(); //Based on LoanStatus=4

                        if (dtQueueDetail.Rows.Count > 0)
                        {
                            strFileDataID = dtQueueDetail.Rows[0]["FileDataID"].ToString(); //Get FileDataID
                            LoggerService.Debug("FiledataID:", strFileDataID);

                            queueid = dtQueueDetail.Rows[0]["QueueID"].ToString(); //Get QueueID                                                
                            LoggerService.Debug("QueueID: ", queueid);

                            //if you get record to process then continue...else processData = false;
                            if (strFileDataID.Length <= 0)
                            {
                                processData = false;
                                LoggerService.Debug("No data found in queue table.", "INFO");
                            }
                            else
                            {
                                //UPDATE[ByteProSFQueue] SET Status = 'PROCESSING'
                                objSalesForceBL.UpdateQueueRecord("PROCESSING", queueid);
                                LoggerService.Debug("UpdateQueueRecord:", "PROCESSING");

                                // Get party Email, CategoryId,ContactNMLSID,BranchId etc from Party based on filedataid  and CategoryId= 110 = Broker                        
                                string ContactNMLSID = string.Empty;
                                string CompanyNMLSID = string.Empty;
                                string BranchID = string.Empty;
                                string EMail = string.Empty;
                                DataTable dtParty = new DataTable();
                                dtParty = objSalesForceBL.GetPartyinfo(strFileDataID);

                                if (dtParty.Rows.Count > 0)
                                {
                                    ContactNMLSID = dtParty.Rows[0]["ContactNMLSID"].ToString(); //Get ContactNMLSID
                                    LoggerService.Debug("ContactNMLSID:", ContactNMLSID);

                                    //CompanyNMLSID = "144549";//To be removed, other smaple data "8473959844" for debugging purpose.
                                    CompanyNMLSID = dtParty.Rows[0]["CompanyNMLSID"].ToString(); //Get CompanyNMLSID                                
                                    LoggerService.Debug("CompanyNMLSID:", CompanyNMLSID);

                                    BranchID = dtParty.Rows[0]["BranchID"].ToString(); //Get BranchID
                                    LoggerService.Debug("BranchID:", BranchID);

                                    EMail = dtParty.Rows[0]["EMail"].ToString(); //Get Email
                                    LoggerService.Debug("EMail:", EMail);
                                }
                                else
                                {
                                    LoggerService.Debug("No data found in Party Table", "ERROR");
                                }


                                //Get filename and Closingdate from FileData table
                                string strFileName = string.Empty;
                                DateTime closingDate = DateTime.MinValue;
                                DataTable dtFileDetail = objSalesForceBL.GetFileDetail(strFileDataID);
                                if (dtFileDetail.Rows.Count > 0)
                                {
                                    strFileName = dtFileDetail.Rows[0]["FileName"].ToString(); //Get FileName
                                    LoggerService.Debug("FileName:", strFileName);

                                    closingDate = (DateTime)(dtFileDetail.Rows[0]["CloseDate"]); //Get ClosingDate
                                    LoggerService.Debug("ClosingDate:", closingDate);
                                }
                                else
                                {
                                    LoggerService.Debug("No data found in FileData Table", "ERROR");
                                }

                                //await objSalesForceBL.GetOpportunityAsync(salesForceResponse, "test");
                                // retreive salesforce accountid and user id using those methods
                                //SalesForceClientEntities.Account.AccountFields accFields = await objSalesForceBL.GetAccountAsync(salesForceResponse, CompanyNMLSID);

                                SalesForceClientEntities.Contact.ContactFields conFileds = await objSalesForceBL.GetContactAsync(salesForceResponse, ContactNMLSID);

                                //Set entity with data retrieved

                                LoggerService.Debug("API 1.0 start", "INFO");
                                //create opportunity
                                SalesForceOpprtunity opp = new SalesForceOpprtunity();
                                opp.OppurtunityName = strFileName; //FileName from dbo.FileData based on FileDataID
                                opp.stage = "Qualification";
                                opp.closedate = closingDate.ToString("yyyy-MM-dd");
                                opp.ContactID = conFileds.ContactId;
                                opp.AccountID = conFileds.AccountId;
                                opp.OwnerID = conFileds.OwnerId;
                                opp.ByteFileDataID = strFileDataID;

                                LoggerService.Debug("Calling CreateOpportunityAsync()", "INFO");                                
                                string opportunityId = await objSalesForceBL.CreateOpportunityAsync(salesForceResponse, opp);
                                if (opportunityId != null)
                                    LoggerService.Debug("Opportunity created with opportunityId:", opportunityId);
                                else
                                    LoggerService.Debug("Opportunity creation failed", "ERROR!!!");


                                //Update queue table with PROCESSED and oppid
                                objSalesForceBL.UpdateQueueRecord(queueid, "PROCESSED", opportunityId);
                                LoggerService.Debug("UpdateQueueRecord: PROCESSED", "INFO");

                                LoggerService.Debug("API 1.5 Starts", "INFO");                                
                                SalesForceClientEntities.User.UserFields usrFileds = await objSalesForceBL.GetUserAsync(salesForceResponse, conFileds.OwnerId);                                
                                string strName = usrFileds.Name;
                                string strUserName = usrFileds.Username;
                                string strUsrEmail = usrFileds.Email;

                                //Update dbo.Party Table
                                objSalesForceBL.UpdatePartyRecord(strUsrEmail, strUserName, strFileDataID);
                                LoggerService.Debug("Update dbo.Party Table", "INFO");
                            }
                        }
                        else
                        {
                            processData = false;
                            LoggerService.Debug("No queued data found. Exit loop.", "ERROR");
                        }
                    }
                    catch(Exception ex)
                    {
                        // update queue table with error and status 
                        objSalesForceBL.UpdateQueueRecord(queueid, "ERROR", ex.Message.ToString());
                        LoggerService.Error("", "SFService->SalesForceCRUD(): " + "\r\nError Message: " + ex.Message + "\r\nStackTrace: " + ex.StackTrace);
                    }
                }               
            }
            catch (Exception ex)
            {
                LoggerService.Error("", "SFService->SalesForceCRUD(): " + "\r\nError Message: " + ex.Message + "\r\nStackTrace: " + ex.StackTrace);
            }
        }       
    }
}
