using Application.Interfaces.Contexts;
using Domain.Attributes;
using Domain.Banners;
using Domain.Baskets;
using Domain.Catalogs;
using Domain.Discounts;
using Domain.Order;
using Domain.Payments;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Persistence.EntityConfigurations;
using Persistence.Seeds;
using System;
using System.Linq;

namespace Persistence.Contexts
{
    public class DataBaseContext : DbContext, IDataBaseContext
    {

        /// با کمک بیس کلاسی که از آن ارث بری کرده ایم متد های سازنده اش را فراخوانی میکند
        /// که آپشنی که از کلاس استارت آپ دریافت میکنیم را به کلاس پدر (دیتابیس کانتکست دراینجا)ارسال میکند
        public DataBaseContext(DbContextOptions<DataBaseContext> options) : base(options)
        {

        }
        public DbSet<CatalogBrand> CatalogBrands { get; set; }
        public DbSet<CatalogType> CatalogTypes { get; set; }
        public DbSet<CatalogItem> CatalogItems { get; set; }
        public DbSet<Basket> Baskets { get; set; }
        public DbSet<BasketItem> BasketItems { get; set; }
        public DbSet<UserAddress> UserAddresses { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<DiscountUsageHistory> DiscountUsageHistories { get; set; }
        public DbSet<CatalogItemFavourite> CatalogItemFavourites { get; set; }
        public DbSet<Banner>  Banners { get; set; }
        public DbSet<CatalogItemComment> CatalogItemComments { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            ///ایجاد اتربیوت خاص برای موجودیت ها به روش شدو پراپرتی
            //builder.Entity<User>().Property<DateTime?>("InsertTime");
            //builder.Entity<User>().Property<DateTime?>("UpdateTime");
            ///عملیات 
            ///Audit روی تمام ان تی تی ها انجام میشود
            builder.HasDefaultSchema("dbo");
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                if (entityType.ClrType.GetCustomAttributes(typeof(AuditableAttribute), true).Length > 0)
                {
                    builder.Entity(entityType.Name).Property<DateTime>("InsertTime").HasDefaultValue(DateTime.Now);
                    builder.Entity(entityType.Name).Property<DateTime?>("UpdateTime");
                    builder.Entity(entityType.Name).Property<DateTime?>("RemoveTime");
                    builder.Entity(entityType.Name).Property<bool>("IsRemoved").HasDefaultValue(false);
                }
            }
            builder.Entity<CatalogType>()
                .HasQueryFilter(m => EF.Property<bool>(m, "IsRemoved") == false);
            builder.Entity<BasketItem>()
                .HasQueryFilter(m => EF.Property<bool>(m, "IsRemoved") == false);
            builder.Entity<Basket>()
                .HasQueryFilter(m => EF.Property<bool>(m, "IsRemoved") == false);


            builder.ApplyConfiguration(new CatalogBrandEntityTypeConfiguration());
            builder.ApplyConfiguration(new CatalogTypeEntityTypeConfiguration());

            DataBaseContextSeed.CatalogSeed(builder);

            builder.Entity<Order>().OwnsOne(p => p.Address);

            base.OnModelCreating(builder);
        }

        /// هر گاه بعد از فراخوانی متد سیو چنج روی موجودیت ها استفاده شود تمام
        /// تغییرات زیر روی آن اعمال  میشود
        public override int SaveChanges()
        {
            ///اگر هریک از تغییرات داخل پرانتز روی موجودیت ها انجام شد در غالب ترکر برگشت بده
            var modifiedEntries = ChangeTracker.Entries()
                .Where(p => p.State == EntityState.Modified ||
                p.State == EntityState.Added ||
                p.State == EntityState.Deleted
                );

            ///اگر چندین موجودیت همزمان تغییر کرد
            ///مقادیر شدو پراپرتی همه موارد را تغییر میدهیم
            foreach (var item in modifiedEntries)
            {
                ///موجودیت مربوطه به دست آمد
                var entityType = item.Context.Model.FindEntityType(item.Entity.GetType());

                if (entityType!=null)
                {
                    var inserted = entityType.FindProperty("InsertTime");
                    var updated = entityType.FindProperty("UpdateTime");
                    var RemoveTime = entityType.FindProperty("RemoveTime");
                    var IsRemoved = entityType.FindProperty("IsRemoved");

                    ///چک میکنیم چه استیتی برای پراپرتی تنظیم شده است
                    if (item.State == EntityState.Added && inserted != null)
                    {
                        item.Property("InsertTime").CurrentValue = DateTime.Now;
                    }

                    if (item.State == EntityState.Modified && updated != null)
                    {
                        item.Property("UpdateTime").CurrentValue = DateTime.Now;
                    }

                    if (item.State == EntityState.Deleted && RemoveTime != null && IsRemoved != null)
                    {
                        item.Property("RemoveTime").CurrentValue = DateTime.Now;
                        item.Property("IsRemoved").CurrentValue = true;
                        item.State = EntityState.Modified;
                    }
                }
            }
            return base.SaveChanges();
        }
    }
}
