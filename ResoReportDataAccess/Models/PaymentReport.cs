using System;
using System.Collections.Generic;

#nullable disable

namespace ResoReportDataAccess.Models
{
    public partial class PaymentReport
    {
        public int Id { get; set; }
        public string CreateBy { get; set; }
        public int Status { get; set; }
        public DateTime Date { get; set; }
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public int PaymentType { get; set; }
        public string PaymentTypeName { get; set; }
        public int? TotalTransaction { get; set; }
        public double Amount { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int Version { get; set; }
        public bool? Active { get; set; }

        public virtual Store Store { get; set; }
    }
}
