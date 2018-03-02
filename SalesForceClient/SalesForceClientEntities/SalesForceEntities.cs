using System;
using System.Collections.Generic;

namespace SalesForceClientEntities
{

    public class SalesForceOpprtunity
    {
        public string closedate { get; set; }
        public string OppurtunityName { get; set; }        
        public string stage { get; set; }
        public string ContactID { get; set; }
        public string AccountID { get; set; }
        public string OwnerID { get; set; }
        public string ByteFileDataID { get; set; }
    }

    public class SalesForceResponse
    {
        public string access_token { get; set; }
        public string instance_url { get; set; }
        public string id { get; set; }
        public string token_type { get; set; }
        public string issued_at { get; set; }
        public string signature { get; set; }
    }

    public class SalesForceConnectionData
    {
        public string consumerKey { get; set; }
        public string consumerSecret { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string securityToken { get; set; }
        public string loginHost { get; set; }
        public string baseUrl { get; set; }
        public string restServiceUrl { get; set; }
    }

    public class Result
    {
        public int totalSize { get; set; }
        public bool done { get; set; }
        public List<Records> records { get; set; }
    }

    public class Records
    {
        public Attributes attributes { get; set; }
        public string Id { get; set; }        
    }
    
    public class Attributes
    {
        public string type { get; set; }
        public string url { get; set; }
    }     

    public class CreateOpportunityFields
    {
        public string Name { get; set; }
        public string AccountId { get; set; }        
        public string StageName { get; set; }
        public string CloseDate { get; set; }
        public string Contact__c { get; set; }
        public string Byte_FileDataID__c { get; set; }
    }

    public class OpportunityIds
    {
        public string PlanCodeOpportunityId { get; set; }
        public string CreditOpportunityId { get; set; }

        public static implicit operator string(OpportunityIds v)
        {
            throw new NotImplementedException();
        }
    }

    public class OpportunityUpdateFields
    {
        public string Name { get; set; }        
        public string StageName { get; set; }
        public string OwnerID { get; set; }        
    }    
}
