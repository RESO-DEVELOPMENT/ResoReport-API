using System;
using System.Collections.Generic;

#nullable disable

namespace ResoReportDataService.Models
{
    public partial class GroupProduct
    {
        public GroupProduct()
        {
            ProductInGroups = new HashSet<ProductInGroup>();
        }

        public Guid Id { get; set; }
        public Guid? ComboProductId { get; set; }
        public string Name { get; set; }
        public string CombinationMode { get; set; }
        public int Priority { get; set; }
        public int Quantity { get; set; }
        public string Status { get; set; }

        public virtual Product ComboProduct { get; set; }
        public virtual ICollection<ProductInGroup> ProductInGroups { get; set; }
    }
}
