using System;

namespace ResoReportDataService.ViewModels
{
    public class SummaryViewModel
    {
        public double NetSales { get; set; }
        public int TotalOrders { get; set; }
        public DateTime LastUpdatedTime { get; set; }
    }
}