using System;
using System.Collections.Generic;

#nullable disable

namespace ResoReportDataService.Models
{
    public partial class PromotionOrderMapping
    {
        public Guid Id { get; set; }
        public Guid PromotionId { get; set; }
        public Guid OrderId { get; set; }
        public double? DiscountAmount { get; set; }
        public int? Quantity { get; set; }

        public virtual Order Order { get; set; }
        public virtual Promotion Promotion { get; set; }
    }
}
