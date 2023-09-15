using System;
using System.Collections.Generic;

#nullable disable

namespace ResoReportDataAccess.Models
{
    public partial class Order
    {
        public int OrderId { get; set; }
        public string OrderCode { get; set; }
        public DateTime? CheckInDate { get; set; }
        public double TotalAmount { get; set; }
        public double Discount { get; set; }
        public double DiscountOrderDetail { get; set; }
        public double FinalAmount { get; set; }
        public int OrderStatus { get; set; }
        public int OrderType { get; set; }
        public string Notes { get; set; }
        public string ServedPerson { get; set; }
        public int SourceId { get; set; }
        public string DeliveryAddress { get; set; }
        public int OrderDetailsTotalQuantity { get; set; }
        public string Att1 { get; set; }
        public string Att2 { get; set; }
        public string Att3 { get; set; }
        public string Att4 { get; set; }
        public string Att5 { get; set; }
        public int? PaymentStatus { get; set; }
        public int? DeliveryStatus { get; set; }
        public int? DeliveryType { get; set; }
        public int? DeliveryPayment { get; set; }
        public string DeliveryReceiver { get; set; }
        public string DeliveryPhone { get; set; }
        public double? MemberPoint { get; set; }
        public int StoreId { get; set; }
        public string StoreCode { get; set; }
        public string StoreName { get; set; }
        public int? CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public int BrandId { get; set; }
    }
}
