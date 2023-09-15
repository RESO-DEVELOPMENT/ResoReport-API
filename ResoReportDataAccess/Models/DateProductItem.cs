using System;
using System.Collections.Generic;

#nullable disable

namespace ResoReportDataAccess.Models
{
    public partial class DateProductItem
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int ProductItemId { get; set; }
        public string ProductItemName { get; set; }
        public double Quantity { get; set; }
        public string Unit { get; set; }
        public int? StoreId { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int Version { get; set; }

        public virtual Store Store { get; set; }
    }
}
