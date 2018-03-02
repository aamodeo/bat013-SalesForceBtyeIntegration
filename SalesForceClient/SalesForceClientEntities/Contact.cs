using System.Collections.Generic;

namespace SalesForceClientEntities.Contact
{
    public class Attributes
    {
        public string type { get; set; }
        public string url { get; set; }
    }

    public class Record
    {
        public Attributes attributes { get; set; }
        public string Id { get; set; }
        public Owner Owner { get; set; }
        public Account account { get; set; }        
    }

    public class Owner
    {
        public Attributes attributes { get; set; }
        public string Id { get; set; }        
    }

    public class Account
    {
        public Attributes attributes { get; set; }
        public string Id { get; set; }
    }

    public class ContactDetails
    {
        public int totalSize { get; set; }
        public bool done { get; set; }
        public List<Record> records { get; set; }
    }

    public class ContactFields
    {
        public string ContactId { get; set; }
        public string OwnerId { get; set; }
        public string AccountId { get; set; }
    }
}
