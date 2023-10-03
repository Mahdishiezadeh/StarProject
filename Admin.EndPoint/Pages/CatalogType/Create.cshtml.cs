using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Admin.EndPoint.ViewModels.Catalogs;
using Application.Catalogs.CatalogTypes.CrudService;
using Application.Dtos;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Admin.EndPoint.Pages.CatalogType
{
    public class CreateModel : PageModel
    {
        private readonly ICatalogTypeService catalogTypeService;
        private readonly IMapper mapper;

        public CreateModel(ICatalogTypeService catalogTypeService, IMapper mapper)
        {
            this.catalogTypeService = catalogTypeService;
            this.mapper = mapper;
        }

        [BindProperty]
        public CatalogTypeViewModel CatalogType { get; set; } = new CatalogTypeViewModel { };
        public List<String> Message { get; set; } = new List<string>();


        public void OnGet(int? parentId)
        {
            CatalogType.ParentCatalogTypeId = parentId;
        }
        /// <summary>
        /// دیتا رو بگیرم مپ کنم به دی تی او
        /// </summary>
        /// <returns></returns>
        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            ///دیتای دریافتی مپ میشه یه دی تی او لایه سرویس 
            var model = mapper.Map<CatalogTypeDto>(CatalogType);
            ///ادد میکنیم
            var result = catalogTypeService.Add(model);
            if (result.IsSuccess)
            {
                ///بازگشت به ایندکس و پرنت آیدی رو هم ارسال کن 
                return RedirectToPage("index", new { parentid = CatalogType.ParentCatalogTypeId });
            }
            ///اگر موفقیت آمیز نبود پیام بده
            Message = result.Message;
            return Page();
        }
    }
}
