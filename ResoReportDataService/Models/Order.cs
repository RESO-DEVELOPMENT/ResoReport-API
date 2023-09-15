using System;
using System.Collections.Generic;

#nullable disable

namespace ResoReportDataService.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
            PromotionOrderMappings = new HashSet<PromotionOrderMapping>();
        }

        public Guid Id { get; set; }
        public Guid CheckInPerson { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public string InvoiceId { get; set; }
        public double TotalAmount { get; set; }
        public double Discount { get; set; }
        public double FinalAmount { get; set; }
        public double Vat { get; set; }
        public double Vatamount { get; set; }
        public string OrderType { get; set; }
        public int? NumberOfGuest { get; set; }
        public string Status { get; set; }
        public Guid? OrderSourceId { get; set; }
        public string Note { get; set; }
        public double? FeeAmount { get; set; }
        public string FeeDescription { get; set; }
        public Guid SessionId { get; set; }
        public string PaymentType { get; set; }

        public virtual Account CheckInPersonNavigation { get; set; }
        public virtual OrderSource OrderSource { get; set; }
        public virtual Session Session { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<PromotionOrderMapping> PromotionOrderMappings { get; set; }
    }
}
