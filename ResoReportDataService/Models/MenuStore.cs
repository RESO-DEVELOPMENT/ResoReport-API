using System;
using System.Collections.Generic;

#nullable disable

namespace ResoReportDataService.Models
{
    public partial class MenuStore
    {
        public Guid Id { get; set; }
        public Guid MenuId { get; set; }
        public Guid StoreId { get; set; }

        public virtual Menu Menu { get; set; }
        public virtual Store Store { get; set; }
    }
}
