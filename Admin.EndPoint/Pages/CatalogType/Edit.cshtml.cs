using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Admin.EndPoint.ViewModels.Catalogs;
using Application.Catalogs.CatalogTypes.CrudService;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Admin.EndPoint.Pages.CatalogType
{
    public class EditModel : PageModel
    {
        private readonly ICatalogTypeService catalogTypeService;
        private readonly IMapper mapper;

        public EditModel(ICatalogTypeService catalogTypeService,IMapper mapper)
        {
            this.catalogTypeService = catalogTypeService;
            this.mapper = mapper;
        }
        /// <summary>
        /// معادل سورس در مپینگ است و ما ویو مدل از کاربر اخذ میکنیم 
        /// </summary>
        [BindProperty]
        public CatalogTypeViewModel CatalogType { get; set; } = new CatalogTypeViewModel { };
        public List<String> Message { get; set; } = new List<string>();

        public void OnGet(int Id)
        {
            var model= catalogTypeService.FindById(Id);
            if (model.IsSuccess)
                ///مدلی که پیدا شد از جنس ویو مدل مپ شود
                ///به نمونه ویو مدل ایجاد شده توسط پراپرتی بالا 
                CatalogType= mapper.Map<CatalogTypeViewModel>(model.Data);
            Message = model.Message;
        }
        /// <summary>
        /// دیتا زمانی که پست میشود از نوع کاتالوگ تایپ ویو مدل است
        /// </summary>
        public IActionResult OnPost()
        {
            ///ما پراپرتی کاتالوگ تایپ را که ایجاد کردیم 
            ///با دی تی او اقدام به ارسال به سرویس اصلی میکنیم
            var model = mapper.Map<CatalogTypeDto>(CatalogType);
            var result = catalogTypeService.Edit(model);
            Message = result.Message;
            ///حال مجدد بایست مپینگ به ویو مدل انجام شود تا دیتا به کاربر نمایش داده شود
            CatalogType = mapper.Map<CatalogTypeViewModel>(result.Data);
            return Page();
        }
    }
}
