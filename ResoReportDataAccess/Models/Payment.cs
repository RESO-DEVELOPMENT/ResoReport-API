using System;
using System.Collections.Generic;

#nullable disable

namespace ResoReportDataAccess.Models
{
    public partial class Payment
    {
        public int PaymentId { get; set; }
        public double Amount { get; set; }
        public int Status { get; set; }
        public int Type { get; set; }
        public int? CostId { get; set; }
        public int? CostType { get; set; }
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public string StoreCode { get; set; }
        public int OrderId { get; set; }
        public int OrderStatus { get; set; }
        public int OrderType { get; set; }
    }
}
