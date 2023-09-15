using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace ResoReportDataAccess.Models
{
    public partial class DataWareHouseReportingContext : DbContext
    {
        public DataWareHouseReportingContext()
        {
        }

        public DataWareHouseReportingContext(DbContextOptions<DataWareHouseReportingContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Brand> Brands { get; set; }
        public virtual DbSet<DateProduct> DateProducts { get; set; }
        public virtual DbSet<DateProductItem> DateProductItems { get; set; }
        public virtual DbSet<DateReport> DateReports { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<PaymentReport> PaymentReports { get; set; }
        public virtual DbSet<Store> Stores { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                // optionsBuilder.UseSqlServer("Server=15.164.244.155;Database=DataWareHouseReporting;User Id=report-dev-team;Password=zaQ@1234");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Brand>(entity =>
            {
                entity.ToTable("Brand");

                entity.Property(e => e.Address).HasMaxLength(100);

                entity.Property(e => e.ApiSmskey)
                    .HasMaxLength(255)
                    .HasColumnName("ApiSMSKey");

                entity.Property(e => e.BrandFeatureFilter).HasMaxLength(255);

                entity.Property(e => e.BrandName).HasMaxLength(255);

                entity.Property(e => e.BrandNameSms)
                    .HasMaxLength(255)
                    .HasColumnName("BrandNameSMS");

                entity.Property(e => e.CompanyName).HasMaxLength(255);

                entity.Property(e => e.ContactPerson).HasMaxLength(255);

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.DefaultDashBoard).HasMaxLength(250);

                entity.Property(e => e.Description).HasMaxLength(255);

                entity.Property(e => e.Fax).HasMaxLength(255);

                entity.Property(e => e.Pgppassword).HasColumnName("PGPPassword");

                entity.Property(e => e.PgpprivateKey).HasColumnName("PGPPrivateKey");

                entity.Property(e => e.PgppulblicKey).HasColumnName("PGPPulblicKey");

                entity.Property(e => e.PhoneNumber).HasMaxLength(255);

                entity.Property(e => e.RsaprivateKey).HasColumnName("RSAPrivateKey");

                entity.Property(e => e.RsapublicKey).HasColumnName("RSAPublicKey");

                entity.Property(e => e.SecurityApiSmskey)
                    .HasMaxLength(255)
                    .HasColumnName("SecurityApiSMSKey");

                entity.Property(e => e.Smstype).HasColumnName("SMSType");

                entity.Property(e => e.Vatcode)
                    .HasMaxLength(13)
                    .HasColumnName("VATCode");

                entity.Property(e => e.Vattemplate).HasColumnName("VATTemplate");

                entity.Property(e => e.Website).HasMaxLength(255);
            });

            modelBuilder.Entity<DateProduct>(entity =>
            {
                entity.ToTable("DateProduct");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CategoryName).HasMaxLength(100);

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.ProductCode).HasMaxLength(100);

                entity.Property(e => e.ProductName).HasMaxLength(100);

                entity.Property(e => e.StoreId).HasColumnName("StoreID");

                entity.Property(e => e.StoreName).HasMaxLength(100);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.Property(e => e.Version).HasDefaultValueSql("((1))");

                entity.HasOne(d => d.Store)
                    .WithMany(p => p.DateProducts)
                    .HasForeignKey(d => d.StoreId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DateProduct_Store");
            });

            modelBuilder.Entity<DateProductItem>(entity =>
            {
                entity.ToTable("DateProductItem");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.ProductItemId).HasColumnName("ProductItemID");

                entity.Property(e => e.ProductItemName).HasMaxLength(50);

                entity.Property(e => e.Unit).HasMaxLength(20);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.Property(e => e.Version).HasDefaultValueSql("((1))");

                entity.HasOne(d => d.Store)
                    .WithMany(p => p.DateProductItems)
                    .HasForeignKey(d => d.StoreId)
                    .HasConstraintName("FK_DateProductItem_Store");
            });

            modelBuilder.Entity<DateReport>(entity =>
            {
                entity.ToTable("DateReport");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreateBy).HasMaxLength(256);

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.StoreId).HasColumnName("StoreID");

                entity.Property(e => e.StoreName).HasMaxLength(100);

                entity.Property(e => e.TotalCash).HasDefaultValueSql("((0))");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.Property(e => e.Version).HasDefaultValueSql("((1))");

                entity.HasOne(d => d.Store)
                    .WithMany(p => p.DateReports)
                    .HasForeignKey(d => d.StoreId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DateReport_Store");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Order");

                entity.HasIndex(e => e.OrderCode, "Order_OrderCode_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.OrderId, "Order_OrderID_index");

                entity.Property(e => e.OrderId).HasColumnName("OrderID");

                entity.Property(e => e.Att1).HasMaxLength(250);

                entity.Property(e => e.Att2).HasMaxLength(250);

                entity.Property(e => e.Att3).HasMaxLength(250);

                entity.Property(e => e.Att4).HasMaxLength(250);

                entity.Property(e => e.Att5).HasMaxLength(250);

                entity.Property(e => e.BrandId).HasColumnName("BrandID");

                entity.Property(e => e.CheckInDate).HasColumnType("datetime");

                entity.Property(e => e.CustomerId).HasColumnName("CustomerID");

                entity.Property(e => e.CustomerName).HasMaxLength(250);

                entity.Property(e => e.CustomerPhone).HasMaxLength(250);

                entity.Property(e => e.DeliveryAddress).HasMaxLength(500);

                entity.Property(e => e.DeliveryPhone).HasMaxLength(250);

                entity.Property(e => e.DeliveryReceiver).HasMaxLength(250);

                entity.Property(e => e.OrderCode)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.ServedPerson).HasMaxLength(250);

                entity.Property(e => e.StoreCode).HasMaxLength(250);

                entity.Property(e => e.StoreId).HasColumnName("StoreID");

                entity.Property(e => e.StoreName).HasMaxLength(250);
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.ToTable("OrderDetail");

                entity.HasIndex(e => e.OrderDetailId, "OrderDetail_OrderDetailID_index");

                entity.HasIndex(e => e.OrderDetailId, "OrderDetail_pk")
                    .IsUnique();

                entity.Property(e => e.OrderDetailId)
                    .ValueGeneratedNever()
                    .HasColumnName("OrderDetailID");

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.CategoryName).HasMaxLength(250);

                entity.Property(e => e.OrderDate).HasColumnType("datetime");

                entity.Property(e => e.OrderId).HasColumnName("OrderID");

                entity.Property(e => e.ProductId).HasColumnName("ProductID");

                entity.Property(e => e.ProductName).HasMaxLength(250);

                entity.Property(e => e.StoreCode).HasMaxLength(250);

                entity.Property(e => e.StoreId).HasColumnName("StoreID");

                entity.Property(e => e.StoreName).HasMaxLength(250);
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.ToTable("Payment");

                entity.HasIndex(e => e.PaymentId, "Payment_PaymentID_index");

                entity.Property(e => e.PaymentId)
                    .ValueGeneratedNever()
                    .HasColumnName("PaymentID");

                entity.Property(e => e.CostId).HasColumnName("CostID");

                entity.Property(e => e.OrderId).HasColumnName("OrderID");

                entity.Property(e => e.StoreCode)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.StoreId).HasColumnName("StoreID");

                entity.Property(e => e.StoreName)
                    .IsRequired()
                    .HasMaxLength(250);
            });

            modelBuilder.Entity<PaymentReport>(entity =>
            {
                entity.ToTable("PaymentReport");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreateBy)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.PaymentTypeName).HasMaxLength(500);

                entity.Property(e => e.StoreId).HasColumnName("StoreID");

                entity.Property(e => e.StoreName).HasMaxLength(100);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.Property(e => e.Version).HasDefaultValueSql("((1))");

                entity.HasOne(d => d.Store)
                    .WithMany(p => p.PaymentReports)
                    .HasForeignKey(d => d.StoreId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PaymentReport_Store");
            });

            modelBuilder.Entity<Store>(entity =>
            {
                entity.ToTable("Store");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.CloseTime).HasColumnType("datetime");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.DefaultAdminPassword).HasMaxLength(20);

                entity.Property(e => e.DefaultDashBoard).HasMaxLength(250);

                entity.Property(e => e.District).HasMaxLength(50);

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.Fax).HasMaxLength(50);

                entity.Property(e => e.IsAvailable).HasColumnName("isAvailable");

                entity.Property(e => e.Lat).HasMaxLength(256);

                entity.Property(e => e.Lon).HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.OpenTime).HasColumnType("datetime");

                entity.Property(e => e.Phone).HasMaxLength(50);

                entity.Property(e => e.Province).HasMaxLength(50);

                entity.Property(e => e.ReportDate).HasColumnType("datetime");

                entity.Property(e => e.ShortName).HasMaxLength(100);

                entity.Property(e => e.StoreCode)
                    .HasMaxLength(10)
                    .IsFixedLength(true);

                entity.Property(e => e.StoreConfig).HasColumnType("ntext");

                entity.Property(e => e.StoreFeatureFilter).HasMaxLength(255);

                entity.Property(e => e.Ward).HasMaxLength(50);

                entity.HasOne(d => d.Brand)
                    .WithMany(p => p.Stores)
                    .HasForeignKey(d => d.BrandId)
                    .HasConstraintName("FK_Store_Brand");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
