using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResoReportDataService.ViewModels
{   
    public class OrderDetailReportViewModel
    {
        public string CheckInDate { get; set; }
        public string InvoiceId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public double UnitPrice { get; set; }
        public int Quantity { get; set; }
        public double TotalAmount { get; set; }
        public double DiscountOrderDetail { get; set; }
        public double Discount { get; set; }
        public double FinalAmount { get; set; }
        public string PaymentName { get; set; }
    }
}
