using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Banners;
using Infrastructure.CacheHelpers;
using Infrastructure.ExternalApi.ImageServer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Distributed;

namespace Admin.EndPoint.Pages.Banners
{
    public class CreateModel : PageModel
    {
        private readonly IImageUploadService imageUploadService;
        private readonly IBannersService bannersService;
        private readonly IDistributedCache cache;

        public CreateModel(IImageUploadService imageUploadService, IBannersService bannersService, IDistributedCache cache)
        {
            this.imageUploadService = imageUploadService;
            this.bannersService = bannersService;
            this.cache = cache;
        }

        ///چون سرویس بنر یک بنر دی تی او دریافت میکند
        [BindProperty]
        public BannerDto Banner { get; set; }

        ///برای اخذ فایل عکس و استفاده از سرویس آپلود
        [BindProperty]
        public IFormFile BannerImage { get; set; }


        ///در این متد کاری انجام نمیدهیم چون قرار بر ایجاد و پست است
        public void OnGet()
        {

        }

        public IActionResult OnPost()
        {
            ///upload
            ///فقط یک تصویر
            var result = imageUploadService.Upload(new List<IFormFile> { BannerImage });

            ///اگر تعداد خروجی ریزالت بزرگتر از صفر بود یعنی با موفقیت آپلود شده 
            if (result.Count > 0)
            {
                ///اولین آدرس را درون دی تی او ذخیره میکنیم
                Banner.Image = result.FirstOrDefault();
                ///با کمک سرویس ادد میکنیم 
                bannersService.AddBanner(Banner);

                cache.Remove(CacheHelper.GenerateHomePageCacheKey());

            }
            return RedirectToPage("Index");
        }


    }
}
