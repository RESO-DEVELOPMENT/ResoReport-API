using System;
using System.Collections.Generic;

namespace ResoReportDataService.ViewModels
{
    public class BusinessInsightViewModel
    {
        // public string LastUpdatedTime { get; set; }
        public string TopPerformingStore { get; set; }
        public string TopSellingItem { get; set; }
        public List<DashboardInsight> Orders { get; set; }

        public TrendInsight TotalTransaction { get; set; }
        public TrendInsight GrossSales { get; set; }
        public TrendInsight NetSales { get; set; }
        public TrendInsight AvgTransactionAmount { get; set; }

        public TrendInsight TotalCustomers { get; set; }
    }

   
}