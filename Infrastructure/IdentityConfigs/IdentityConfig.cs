using Domain.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Infrastructure.IdentityConfigs
{
    /// <summary>
    /// برای استفاده در استارت آپ بایست استاتیک باشد
    /// </summary>
    public static class IdentityConfig
    {
        /// <summary>
        /// این متد استاتیک و بر روی آی سرویس کالکشن ایجاد میشود پس از 
        /// this 
        /// استفاده میکنیم
        /// </summary>
        /// <returns></returns>
        public static IServiceCollection AddIdentityService(this IServiceCollection services,IConfiguration configuration)
        {
            ///کپی شده از استارت آپ سی اس
            string connection = configuration["ConnectionString:SqlServer"];
            ///کات شده از استارت آپ سی اس 
            services.AddDbContext<IdentityDataBaseContext>(option => option.UseSqlServer(connection));
            /// موجودیت یوزر (کاستوم شده) داشتیم در اینجا پاس میدهیم اما چون رول کاستوم نداریم پس آیدنتی رول را پاس میدهم 
            services.AddIdentity<User, IdentityRole>()
                ///حال بایست دیتابیس کانتکست را معرفی کنیم 
                .AddEntityFrameworkStores<IdentityDataBaseContext>()
                .AddDefaultTokenProviders()
                .AddRoles<IdentityRole>()
                 ///برای فارسی کردن ارور های آیدنتی تی  
                .AddErrorDescriber<CustomIdentityError>();
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 8;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredUniqueChars = 1;
                options.Password.RequireNonAlphanumeric = false;
                options.User.RequireUniqueEmail = true;
                options.Lockout.MaxFailedAccessAttempts = 50;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
            });
            return services;
        }
    }
}
