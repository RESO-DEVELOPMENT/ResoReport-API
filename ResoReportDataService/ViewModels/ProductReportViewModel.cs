using System;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Reso.Sdk.Core.ViewModels;

namespace ResoReportDataService.ViewModels
{
    public class ProductReportViewModel : SortModel
    {
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string CateName { get; set; }
        public int? ProductId { get; set; }
        public int? Quantity { get; set; }
        public double? UnitPrice { get; set; }
        public double? UnitPriceNoVat { get; set; }

        public string Unit { get; set; }
        public double? TotalPriceBeforeVat { get; set; }
        public double? Vat { get; set; }
        public double? Discount { get; set; }
        public double? Percent { get; set; }
        public double? TotalBeforeDiscount { get; set; } //Total amount
        public double? TotalAfterDiscount { get; set; } //Final amount
    }
}