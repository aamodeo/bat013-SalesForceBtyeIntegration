using System.Collections.Generic;

namespace SalesForceClientEntities.User
{
    public class Attributes
    {
        public string type { get; set; }
        public string url { get; set; }
    }

    public class Record
    {
        public Attributes attributes { get; set; }        
        public string Name { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
    }

    public class UserDetails
    {
        public int totalSize { get; set; }
        public bool done { get; set; }
        public List<Record> records { get; set; }
    }

    public class UserFields
    {
        public string Name { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
    }
}
