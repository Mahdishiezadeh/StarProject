using Application.HomePageService;
using Infrastructure.CacheHelpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WebSite.EndPoint.Models;
using WebSite.EndPoint.Utilities.Filters;

namespace WebSite.EndPoint.Controllers
{
    [ServiceFilter(typeof(SaveVisitorFilter))]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHomePageService homePageService;
        private readonly IDistributedCache cache;

        public HomeController(ILogger<HomeController> logger, IHomePageService homePageService, IDistributedCache distributedCache)
        {
            _logger = logger;
            this.homePageService = homePageService;
            cache = distributedCache;
        }

        public IActionResult Index()
        {
            ///یک نمونه از دی تی او سرویس هوم ایجاد میکنیم 
            HomePageDto homePageData = new HomePageDto();

            ///ایجاد کش 
            var homePageCache = cache.GetAsync(CacheHelper.GenerateHomePageCacheKey()).Result;

            ///اگر دیتا کش نال نبود
            if (homePageCache!=null)
            {
                ///دیتا را از کش دریافت میکنیم
                ///چون کش (استریم از جنس فایل) تبدیل به دی تی او (آبجکت) میشود از دیسرالایز استفاده میکنیم
                homePageData = JsonSerializer.Deserialize<HomePageDto>(homePageCache);
            }
            else
            {
                ///دیتای هوم پیج را مستقیم از سرویس میگیریم
                homePageData= homePageService.GetData();

                ///****برای استفاده دیتا در دفعات بعدی کش میکنیم*****
                
                ///اول دیتا جی سان شود
                string jsonData = JsonSerializer.Serialize(homePageData);
                ///گام بعدی به بایت تبدیل شود
                byte[] encodedJson = Encoding.UTF8.GetBytes(jsonData);

                ///آپشن های کش را از لایه اینفراستراکشن دریافت میکنیم
                var options = new DistributedCacheEntryOptions()
                   .SetSlidingExpiration(CacheHelper.DefaultCacheDuration);

                ///برای ست کردن کش کلید (را با کمک متد هلپر) و دیتای بایت شده ، و آپشن های کش 
                cache.SetAsync(CacheHelper.GenerateHomePageCacheKey(), encodedJson, options);

            }

            return View(homePageData);
        }

        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
