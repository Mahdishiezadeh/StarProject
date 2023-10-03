using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Catalogs.CatalogItems.CatalogItemServices;
using Application.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Admin.EndPoint.Pages.CatalogItems
{
    public class IndexModel : PageModel
    {
        private readonly ICatalogItemService catalogItemService;

        public IndexModel(ICatalogItemService catalogItemService)
        {
            this.catalogItemService = catalogItemService;
        }
        /// <summary>
        /// از خروجی سرویس اینجکت شده بایست یک پراپرتی از نوع پابلیک ایجاد کنیم 
        /// </summary>
        public PaginatedItemsDto<CatalogItemListItemDto> CatalogItems { get; set; }
        public void OnGet(int page=1,int pageSize=100)
        {
           CatalogItems= catalogItemService.GetCatalogList(page, pageSize);
        }
    }
}
