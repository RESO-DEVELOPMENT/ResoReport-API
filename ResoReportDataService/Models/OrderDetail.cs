using System;
using System.Collections.Generic;

#nullable disable

namespace ResoReportDataService.Models
{
    public partial class OrderDetail
    {
        public OrderDetail()
        {
            InverseMasterOrderDetail = new HashSet<OrderDetail>();
        }

        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public int Quantity { get; set; }
        public double Discount { get; set; }
        public double TotalAmount { get; set; }
        public double FinalAmount { get; set; }
        public string Notes { get; set; }
        public Guid MenuProductId { get; set; }
        public double SellingPrice { get; set; }
        public Guid? MasterOrderDetailId { get; set; }

        public virtual OrderDetail MasterOrderDetail { get; set; }
        public virtual MenuProduct MenuProduct { get; set; }
        public virtual Order Order { get; set; }
        public virtual ICollection<OrderDetail> InverseMasterOrderDetail { get; set; }
    }
}
