using System;
using System.Collections.Generic;

#nullable disable

namespace ResoReportDataService.Models
{
    public partial class OrderSource
    {
        public OrderSource()
        {
            Orders = new HashSet<Order>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string PicUrl { get; set; }
        public Guid BrandId { get; set; }

        public virtual Brand Brand { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
