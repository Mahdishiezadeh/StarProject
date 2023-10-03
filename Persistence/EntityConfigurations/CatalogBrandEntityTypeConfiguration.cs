using Domain.Catalogs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.EntityConfigurations
{
    /// <summary>
    /// برای کانفیگ  موجودیت بایست از اینترفیس 
    /// IEntityTypeConfiguration<Entity??>
    /// اینهریت شود
    /// </summary>
    public class CatalogBrandEntityTypeConfiguration : IEntityTypeConfiguration<CatalogBrand>
    {
        public void Configure(EntityTypeBuilder<CatalogBrand> builder)
        {
            ///تغییر نام تیبل در دیتابیس
            builder.ToTable("CatalogBrand");
            ///اجباری و میزان کاراکتر پراپرتی 
            builder.Property(cb => cb.Brand)
              .IsRequired()
               .HasMaxLength(100);
        }
    }
}
