using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Admin.EndPoint.ViewModels.Catalogs;
using Application.Catalogs.CatalogItems.CatalogItemServices;
using Application.Catalogs.CatalogItems.RudService;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Admin.EndPoint.Pages.CatalogItems
{
    public class DeleteModel : PageModel
    {
        private readonly IRudCatalogItemService rudCatalogItemService;
        private readonly IMapper mapper;

        public DeleteModel(IRudCatalogItemService rudCatalogItemService, IMapper mapper)
        {
            this.rudCatalogItemService = rudCatalogItemService;
            this.mapper = mapper;
        }
        [BindProperty]
        public CatalogItemViewModel catalogItemViewModel { get; set; } = new CatalogItemViewModel { };
        public List<String> Message { get; set; } = new List<string>();
        public void OnGet(int Id)
        {
            var model = rudCatalogItemService.FindById(Id);
            var temp = model.Data;
            if (model.IsSuccess)
                catalogItemViewModel = mapper.Map<CatalogItemViewModel>(model.Data);
            Message = model.Message;
        }
        public IActionResult OnPost()
        {
            var result = rudCatalogItemService.Remove(catalogItemViewModel.Id);
            Message = result.Message;

            if (result.IsSuccess)
            {
                return RedirectToPage("Index");
            }
            ///اگر موفقیت آمیز نیود همین پیج را مجدد بازگشت میدهیم
            return Page();
        }
    }
}
