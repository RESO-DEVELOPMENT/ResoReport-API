using System;
using System.Collections.Generic;

namespace ResoReportDataService.ViewModels
{
    public class SalesInsightViewModel
    {
        public TrendInsight GrossSales { get; set; }
        public TrendInsight NetSales { get; set; }
        public TrendInsight TotalOrders { get; set; }
        public TrendInsight AverageTransactionAmount { get; set; }
        public List<DashboardInsight> GrossSalesDashboard { get; set; }
        public List<DashboardInsight> NumberOfTransactionsDashboard { get; set; }
        public List<DashboardInsight> NetSalesDashboard { get; set; }
        public List<DashboardInsight> AvgTransactionAmountDashboard { get; set; }
    }

   
}