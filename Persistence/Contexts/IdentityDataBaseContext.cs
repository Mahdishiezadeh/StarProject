using Application.Interfaces.Contexts;
using Domain.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Contexts
{
    /// <summary>
    /// یوزر را در بخش ارث بری قرار دادیم تا فیلد های این موجودیت را در دیتابیس اضافه کند
    /// </summary>
    public class IdentityDataBaseContext : IdentityDbContext<User>, IIdentityDataBaseContext
    {
        public IdentityDataBaseContext(DbContextOptions<IdentityDataBaseContext> options) : base(options)
        {

        }
        /// <summary>
        /// این متد زمانی ایجاد میشود که قصد داریم تغییراتی
        /// در نام پیش فرض جداول یا اسکیما آن ایجاد کنیم
        /// اگر این متد ایجاد شود سه تا از جداول آیدنتیتی شامل 
        /// یوزر لاگین و یوزر رل و یوزر توکن کلید اصلی ارور خواهد داد
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            ///در این خط کد استرینگ جنس کلید اصلی ایدنتی تی یوزر است
            ///و یوزرز نام تیبل و آیدنتی تی نام اسکیما است
            builder.Entity<IdentityUser<string>>().ToTable("Users", "identity");
            builder.Entity<IdentityRole<string>>().ToTable("Roles", "identity");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims", "identity");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims", "identity");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins", "identity");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles", "identity");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens", "identity");

            builder.Entity<IdentityUserLogin<string>>()
                .HasKey(p => new { p.LoginProvider, p.ProviderKey });
            builder.Entity<IdentityUserRole<string>>()
                .HasKey(p => new { p.RoleId, p.UserId });
            builder.Entity<IdentityUserToken<string>>()
                .HasKey(p => new { p.UserId, p.LoginProvider, p.Name });

            ///چون با ارور مواجه شدیم این کد را کامنت کردیم
            //base.OnModelCreating(builder);
            
        }
    }
}
