using System.Collections.Generic;

namespace SalesForceClientEntities.Account
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
        public string Name { get; set; }
        public string Parent { get; set; }
    }

    public class AccountDetails
    {
        public int totalSize { get; set; }
        public bool done { get; set; }
        public List<Record> records { get; set; }
    }

    public class AccountFields
    {
        public string AccountId { get; set; }
        public string ParentId { get; set; }
    }
}
