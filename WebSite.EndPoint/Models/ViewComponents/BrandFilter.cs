using Application.Catalogs.CatalogItems.CatalogItemServices;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebSite.EndPoint.Models.ViewComponents
{
    public class BrandFilter : ViewComponent
    {
        private readonly ICatalogItemService catalogItemService;

        public BrandFilter(ICatalogItemService catalogItemService)
        {
            this.catalogItemService = catalogItemService;
        }

        ///یک متد برای اخذ دیتا و ارسال به ویو
        public IViewComponentResult Invoke()
        {
            var brands = catalogItemService.GetBrand();
            return View(viewName: "BrandFilter", model: brands);
        }
    }
}
