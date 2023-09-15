using System;
using System.Collections.Generic;

#nullable disable

namespace ResoReportDataService.Models
{
    public partial class ProductInGroup
    {
        public Guid Id { get; set; }
        public Guid GroupProductId { get; set; }
        public Guid ProductId { get; set; }
        public int Priority { get; set; }
        public double AdditionalPrice { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }
        public int Quantity { get; set; }
        public string Status { get; set; }

        public virtual GroupProduct GroupProduct { get; set; }
        public virtual Product Product { get; set; }
    }
}
