using System;
using System.Collections.Generic;

#nullable disable

namespace ResoReportDataService.Models
{
    public partial class Account
    {
        public Account()
        {
            Orders = new HashSet<Order>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Status { get; set; }
        public Guid RoleId { get; set; }
        public string Username { get; set; }

        public virtual Role Role { get; set; }
        public virtual BrandAccount BrandAccount { get; set; }
        public virtual StoreAccount StoreAccount { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
