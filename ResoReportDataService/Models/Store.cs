using System;
using System.Collections.Generic;

#nullable disable

namespace ResoReportDataService.Models
{
    public partial class Store
    {
        public Store()
        {
            MenuStores = new HashSet<MenuStore>();
            Sessions = new HashSet<Session>();
            StoreAccounts = new HashSet<StoreAccount>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Code { get; set; }
        public string Status { get; set; }
        public Guid BrandId { get; set; }
        public string Address { get; set; }

        public virtual Brand Brand { get; set; }
        public virtual ICollection<MenuStore> MenuStores { get; set; }
        public virtual ICollection<Session> Sessions { get; set; }
        public virtual ICollection<StoreAccount> StoreAccounts { get; set; }
    }
}
