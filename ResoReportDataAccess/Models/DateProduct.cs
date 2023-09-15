using System;
using System.Collections.Generic;

#nullable disable

namespace ResoReportDataAccess.Models
{
    public partial class DateProduct
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public int? ProductType { get; set; }
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public int Quantity { get; set; }
        public double TotalAmount { get; set; }
        public double Discount { get; set; }
        public double FinalAmount { get; set; }
        public double? UnitPrice { get; set; }
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int? Time0Quantity { get; set; }
        public int? Time1Quantity { get; set; }
        public int? Time2Quantity { get; set; }
        public int? Time3Quantity { get; set; }
        public int? Time4Quantity { get; set; }
        public int? Time5Quantity { get; set; }
        public int? Time6Quantity { get; set; }
        public int? Time7Quantity { get; set; }
        public int? Time8Quantity { get; set; }
        public int? Time9Quantity { get; set; }
        public int? Time10Quantity { get; set; }
        public int? Time11Quantity { get; set; }
        public int? Time12Quantity { get; set; }
        public int? Time13Quantity { get; set; }
        public int? Time14Quantity { get; set; }
        public int? Time15Quantity { get; set; }
        public int? Time16Quantity { get; set; }
        public int? Time17Quantity { get; set; }
        public int? Time18Quantity { get; set; }
        public int? Time19Quantity { get; set; }
        public int? Time20Quantity { get; set; }
        public int? Time21Quantity { get; set; }
        public int? Time22Quantity { get; set; }
        public int? Time23Quantity { get; set; }
        public int? OrderQuantity { get; set; }
        public int? QuantityAtStore { get; set; }
        public int? QuantityTakeAway { get; set; }
        public int? QuantityDelivery { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int Version { get; set; }
        public bool? Active { get; set; }

        public virtual Store Store { get; set; }
    }
}
