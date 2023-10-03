using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Catalogs.CatalogItems.AddNewCatalogItem;
using Application.Catalogs.CatalogItems.CatalogItemServices;
using Application.Dtos;
using Infrastructure.ExternalApi.ImageServer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Admin.EndPoint.Pages.CatalogItems
{
    public class CreateModel : PageModel
    {
        ///سرویس ادد کردن یک کالا
        private readonly IAddNewCatalogItemService addNewCatalogItemService;
  
        /// سرویس اخذ مشخصات کاتالوگ (کالا)ست
        private readonly ICatalogItemService catalogItemService;

        /// سرویس آپلود
        private readonly IImageUploadService imageUploadService;

        public CreateModel(IAddNewCatalogItemService addNewCatalogItemService
            , ICatalogItemService catalogItemService, IImageUploadService imageUploadService )
        {
            this.addNewCatalogItemService = addNewCatalogItemService;
            this.catalogItemService = catalogItemService;
            this.imageUploadService = imageUploadService;
        }

        /// این دو پراپرتی برای دراپ داون ها میباشد
       /// با کمک سرویس های اینجکت شده دراپ داون ها را مقداردهی میکنیم
        public SelectList Categories { get; set; }
        public SelectList Brands { get; set; }

       

        ///سرویس اصلی 
        ///AddNewCatalogItem
        ///هست که باید در ورودی یک دی تی او دریافت کند
        
        [BindProperty] ///این اتربیوت برای ارسال دیتا از ویو به بک اند است
        public AddNewCatalogItemDto Data { get; set; }

       
        /// برای ارسال ایمیج در متد 
        /// OnPost
        public  List<IFormFile> Files { get; set; }

        ///دراپ داو لیست ها مقداردهی میشود
        public void OnGet()
        {
            ///کتگوری و برند برای استفاده در ویو اخذ میشود
            Categories = new SelectList(catalogItemService.GetCatalogType(), "Id", "Type");
            Brands = new SelectList(catalogItemService.GetBrand(), "Id", "Brand");
        }

        ///متد پست اخذ دیتا از ویو
        public JsonResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                ///اخذ تمام ارور ها اگر مدل استیت ولید نبود
                var allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new JsonResult(new BaseDto<int>(false, allErrors.Select(p => p.ErrorMessage).ToList(), 0));
            }

            ///حلقه فور برای فایل هایی که به همراه ریکوئست زده شدند 
            for (int i = 0; i < Request.Form.Files.Count; i++)
            {
                ///تک تک فابل ها را استخراج کرده 
                var file = Request.Form.Files[i];

                ///بعد از استخراج عکس ها ادد میکنیم به پراپرتی فایلز
                Files.Add(file);
            }

            ///یک نمونه از دی تی او ایمیج ایجاد میکنیم
            List<AddNewCatalogItemImage_Dto> images = new List<AddNewCatalogItemImage_Dto>();
            if (Files.Count > 0)
            {
                //Upload 
                var result = imageUploadService.Upload(Files);
                foreach (var item in result)
                {
                    ///آدرس ها در دیتابیس به ازا تصاویر آپلود شده ذخیره شود
                    images.Add(new AddNewCatalogItemImage_Dto { Src = item });
                }
            }

            ///ایمیج های دی تی او ایمیج را به دی تی او اصلی اضافه میکنیم
            Data.Images = images;

            var resultService = addNewCatalogItemService.Execute(Data);
            return new JsonResult(resultService);
        }
    }
}
