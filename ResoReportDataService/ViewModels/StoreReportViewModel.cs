

using Reso.Sdk.Core.ViewModels;

namespace ResoReportDataService.ViewModels
{
    public class StoreReportViewModel : SortModel
    {
        public string StoreName { get; set; }
        public int? TotalOrderAtStore { get; set; }
        public int? TotalOrderTakeAway { get; set; }
        public int? TotalOrderDelivery { get; set; }
        public double? FinalAmountAtStore { get; set; }
        public double? FinalAmountTakeAway { get; set; }
        public double? FinalAmountDelivery { get; set; }
        public int? TotalBills { get; set; }
        public double? TotalSales { get; set; }
        public double? TotalDiscount { get; set; }
        public double? TotalSalesAfterDiscount { get; set; }
    }
}