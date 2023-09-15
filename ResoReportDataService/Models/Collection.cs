using System;
using System.Collections.Generic;

#nullable disable

namespace ResoReportDataService.Models
{
    public partial class Collection
    {
        public Collection()
        {
            CollectionProducts = new HashSet<CollectionProduct>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public string PicUrl { get; set; }
        public Guid BrandId { get; set; }

        public virtual Brand Brand { get; set; }
        public virtual ICollection<CollectionProduct> CollectionProducts { get; set; }
    }
}
