using Reso.Sdk.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResoReportDataService.ViewModels
{
    public class TopStoreRevenueViewModel : SortModel
    {
        public int? StoreId { get; set; }
        public string StoreName { get; set; }
        public string StoreCode { get; set; }
        public int? TotalProduct { get; set; }
        public int? TotalOrderSale { get; set; }
        public int? TotalOrderCard { get; set; }
        public double? TotalRevenueSale { get; set; }
        public double? TotalRevenueCard { get; set; }
        public double? TotalRevenueAll { get; set; }
        public double? TotalRevenueBeforeDiscount { get; set; }
        public double? TotalDiscount { get; set; }
        public double? AvgRevenueSale { get; set; }
        public int? Version { get; set; }
        public DateTime UpdateAt { get; set; }
    }
}
