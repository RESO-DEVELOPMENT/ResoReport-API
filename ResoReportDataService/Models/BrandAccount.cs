using System;
using System.Collections.Generic;

#nullable disable

namespace ResoReportDataService.Models
{
    public partial class BrandAccount
    {
        public Guid Id { get; set; }
        public Guid BrandId { get; set; }
        public Guid AccountId { get; set; }

        public virtual Account Account { get; set; }
        public virtual Brand Brand { get; set; }
    }
}
