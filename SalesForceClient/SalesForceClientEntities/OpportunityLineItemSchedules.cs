using System.Collections.Generic;

namespace SalesForceClientEntities.OpportunityLineItemSchedules
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
    }

    public class OpportunityLineItemSchedules
    {
        public int totalSize { get; set; }
        public bool done { get; set; }
        public List<Record> records { get; set; }
    }
}
