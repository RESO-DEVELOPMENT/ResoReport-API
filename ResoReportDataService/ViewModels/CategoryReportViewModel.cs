namespace ResoReportDataService.ViewModels
{
    public class CategoryReportViewModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int Quantity { get; set; }
        public double Percent { get; set; }
        public double TotalBeforeDiscount { get; set; }
        public double Discount { get; set; }
        public double TotalAfterDiscount { get; set; }
    }
}