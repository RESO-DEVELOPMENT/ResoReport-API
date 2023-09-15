﻿using System;
using System.Collections.Generic;

#nullable disable

namespace ResoReportDataService.Models
{
    public partial class PromotionProductMapping
    {
        public Guid Id { get; set; }
        public Guid PromotionId { get; set; }
        public Guid ProductId { get; set; }
        public string Status { get; set; }
        public int? Quantity { get; set; }

        public virtual Product Product { get; set; }
        public virtual Promotion Promotion { get; set; }
    }
}
