using System;
using System.Collections.Generic;

#nullable disable

namespace ResoReportDataAccess.Models
{
    public partial class Brand
    {
        public Brand()
        {
            Stores = new HashSet<Store>();
        }

        public int Id { get; set; }
        public string BrandName { get; set; }
        public DateTime CreateDate { get; set; }
        public bool Active { get; set; }
        public string Description { get; set; }
        public string CompanyName { get; set; }
        public string ContactPerson { get; set; }
        public string PhoneNumber { get; set; }
        public string Fax { get; set; }
        public string Website { get; set; }
        public string Vatcode { get; set; }
        public int? Vattemplate { get; set; }
        public string Address { get; set; }
        public string ApiSmskey { get; set; }
        public string SecurityApiSmskey { get; set; }
        public int? Smstype { get; set; }
        public string BrandNameSms { get; set; }
        public string JsonConfigUrl { get; set; }
        public string BrandFeatureFilter { get; set; }
        public int? WiskyId { get; set; }
        public string DefaultDashBoard { get; set; }
        public string RsaprivateKey { get; set; }
        public string RsapublicKey { get; set; }
        public string Pgppassword { get; set; }
        public string PgpprivateKey { get; set; }
        public string PgppulblicKey { get; set; }
        public string DesKey { get; set; }
        public string DesVector { get; set; }
        public string AccessToken { get; set; }
        public string TaxCode { get; set; }

        public virtual ICollection<Store> Stores { get; set; }
    }
}
