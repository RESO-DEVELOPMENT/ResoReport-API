namespace ResoReportDataService.ViewModels
{
    public class OverviewDashboard
    {
        public double TotalRevenue { get; set; }
        public double TotalRevenueWithoutCard { get; set; }
        public double TotalRevenueWithDiscount { get; set; }
        public double TotalDiscount { get; set; }
        public double TotalDiscount100 { get; set; }
        public double TotalRevenueWithoutDiscountAndCard { get; set; }
        public double TotalRevenueCard { get; set; }
        public double TotalRevenuePreCancel { get; set; }
        public double TotalRevenueAfterCancel { get; set; }
        public int TotalOrder { get; set; }
        public int TotalOrderAtStore { get; set; }
        public long TotalRevenueAtStore { get; set; }
        public long TotalOrderTakeAway { get; set; }
        public long TotalRevenueTakeAway { get; set; }
        public long TotalOrderDelivery { get; set; }
        public long TotalRevenueDelivery { get; set; }
        public long TotalOrderCard { get; set; }
        public long TotalRevenueOrderCard { get; set; }
        public long TotalOrderPreCancel { get; set; }
        public long TotalOrderAfterCancel { get; set; }
        public long TotalOrderCancel { get; set; }

        public double AvgRevenueOrder { get; set; }
        public double AvgRevenueOrderAtStore { get; set; }
        public double AvgRevenueOrderTakeAway { get; set; }
        public double AvgRevenueOrderDelivery { get; set; }
        public double AvgProductOrder { get; set; }
        public double AvgProductOrderTakeAway { get; set; }
        public double AvgProductOrderAtStore { get; set; }
        public double AvgProductOrderDelivery { get; set; }

        public double AvgOrderTakeAway { get; set; }
        public double AvgOrderAtStore { get; set; }
        public double AvgOrderDelivery { get; set; }


    }
}