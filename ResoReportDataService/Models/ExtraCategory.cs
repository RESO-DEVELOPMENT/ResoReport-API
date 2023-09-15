using System;
using System.Collections.Generic;

#nullable disable

namespace ResoReportDataService.Models
{
    public partial class ExtraCategory
    {
        public Guid Id { get; set; }
        public string Status { get; set; }
        public Guid ExtraCategoryId { get; set; }
        public Guid ProductCategoryId { get; set; }

        public virtual Category ExtraCategoryNavigation { get; set; }
        public virtual Category ProductCategory { get; set; }
    }
}
