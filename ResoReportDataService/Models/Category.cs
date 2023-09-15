using System;
using System.Collections.Generic;

#nullable disable

namespace ResoReportDataService.Models
{
    public partial class Category
    {
        public Category()
        {
            ExtraCategoryExtraCategoryNavigations = new HashSet<ExtraCategory>();
            ExtraCategoryProductCategories = new HashSet<ExtraCategory>();
            Products = new HashSet<Product>();
        }

        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int DisplayOrder { get; set; }
        public string Description { get; set; }
        public string PicUrl { get; set; }
        public string Status { get; set; }
        public Guid? BrandId { get; set; }

        public virtual Brand Brand { get; set; }
        public virtual ICollection<ExtraCategory> ExtraCategoryExtraCategoryNavigations { get; set; }
        public virtual ICollection<ExtraCategory> ExtraCategoryProductCategories { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}
