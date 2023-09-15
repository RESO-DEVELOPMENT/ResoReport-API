﻿using System;
using System.Collections.Generic;

#nullable disable

namespace ResoReportDataService.Models
{
    public partial class Brand
    {
        public Brand()
        {
            BrandAccounts = new HashSet<BrandAccount>();
            Categories = new HashSet<Category>();
            Collections = new HashSet<Collection>();
            Menus = new HashSet<Menu>();
            OrderSources = new HashSet<OrderSource>();
            Products = new HashSet<Product>();
            Promotions = new HashSet<Promotion>();
            Stores = new HashSet<Store>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string PicUrl { get; set; }
        public string Status { get; set; }

        public virtual ICollection<BrandAccount> BrandAccounts { get; set; }
        public virtual ICollection<Category> Categories { get; set; }
        public virtual ICollection<Collection> Collections { get; set; }
        public virtual ICollection<Menu> Menus { get; set; }
        public virtual ICollection<OrderSource> OrderSources { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<Promotion> Promotions { get; set; }
        public virtual ICollection<Store> Stores { get; set; }
    }
}
