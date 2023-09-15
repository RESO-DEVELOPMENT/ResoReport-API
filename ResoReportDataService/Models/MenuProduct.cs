using System;
using System.Collections.Generic;

#nullable disable

namespace ResoReportDataService.Models
{
    public partial class MenuProduct
    {
        public MenuProduct()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public Guid Id { get; set; }
        public string Status { get; set; }
        public double SellingPrice { get; set; }
        public double DiscountPrice { get; set; }
        public double HistoricalPrice { get; set; }
        public Guid MenuId { get; set; }
        public Guid ProductId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual Menu Menu { get; set; }
        public virtual Product Product { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
