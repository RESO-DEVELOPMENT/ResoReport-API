using System;
using System.Collections.Generic;

#nullable disable

namespace ResoReportDataService.Models
{
    public partial class CollectionProduct
    {
        public Guid Id { get; set; }
        public Guid CollectionId { get; set; }
        public Guid ProductId { get; set; }
        public string Status { get; set; }

        public virtual Collection Collection { get; set; }
        public virtual Product Product { get; set; }
    }
}
