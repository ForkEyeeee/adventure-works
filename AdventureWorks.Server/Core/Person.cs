using System.ComponentModel.DataAnnotations;

namespace AdventureWorks.Server.Core
{
    public class BusinessEntity
    {
        public int BusinessEntityID { get; set; }
        public Guid rowguid { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
