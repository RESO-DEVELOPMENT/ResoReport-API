using Reso.Sdk.Core.ViewModels;

namespace ResoReportDataService.ViewModels
{
    public class PaymentReportViewModel : SortModel
    {
        public string StoreName { get; set; }
        public double? Cash { get; set; }
        public double? CreditCard { get; set; }
        public double? CreditCardUse { get; set; }
        public double? Bank { get; set; }
        public double? Momo { get; set; }
        public double? GrabPay { get; set; }
        public double? GrabFood { get; set; }
        public double? VnPay { get; set; }
        public double? Baemin { get; set; }
        public double? ShopeePay { get; set; }
        public double? ZaloPay { get; set; }
    }
}