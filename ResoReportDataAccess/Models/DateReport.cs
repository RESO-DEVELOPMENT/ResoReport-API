using System;
using System.Collections.Generic;

#nullable disable

namespace ResoReportDataAccess.Models
{
    public partial class DateReport
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string CreateBy { get; set; }
        public int Status { get; set; }
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public double? TotalAmount { get; set; }
        public double? FinalAmount { get; set; }
        public double? Discount { get; set; }
        public double? DiscountOrderDetail { get; set; }
        public double? TotalCash { get; set; }
        public int TotalOrder { get; set; }
        public int TotalOrderAtStore { get; set; }
        public int TotalOrderTakeAway { get; set; }
        public int TotalOrderDelivery { get; set; }
        public double TotalOrderDetail { get; set; }
        public double TotalOrderFeeItem { get; set; }
        public int TotalOrderCard { get; set; }
        public int? TotalOrderCanceled { get; set; }
        public int? TotalOrderPreCanceled { get; set; }
        public double? FinalAmountAtStore { get; set; }
        public double? FinalAmountTakeAway { get; set; }
        public double? FinalAmountDelivery { get; set; }
        public double? FinalAmountCard { get; set; }
        public double? FinalAmountCanceled { get; set; }
        public double? FinalAmountPreCanceled { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int Version { get; set; }
        public bool? Active { get; set; }

        public virtual Store Store { get; set; }
    }
}
