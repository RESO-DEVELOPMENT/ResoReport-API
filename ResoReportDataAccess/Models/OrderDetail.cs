using System;
using System.Collections.Generic;

#nullable disable

namespace ResoReportDataAccess.Models
{
    public partial class OrderDetail
    {
        public int OrderDetailId { get; set; }
        public DateTime? OrderDate { get; set; }
        public int Quantity { get; set; }
        public double TotalAmount { get; set; }
        public double Discount { get; set; }
        public double? FinalAmount { get; set; }
        public double UnitPrice { get; set; }
        public int OrderId { get; set; }
        public int OrderStatus { get; set; }
        public int OrderType { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int ProductType { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public string StoreCode { get; set; }
    }
}
