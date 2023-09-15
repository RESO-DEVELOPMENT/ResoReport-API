using System;
using System.Collections.Generic;

#nullable disable

namespace ResoReportDataService.Models
{
    public partial class StoreAccount
    {
        public Guid Id { get; set; }
        public Guid StoreId { get; set; }
        public Guid AccountId { get; set; }

        public virtual Account Account { get; set; }
        public virtual Store Store { get; set; }
    }
}
