using Application.Catalogs.CatalogItems.UriComposer;
using Application.Interfaces.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Catalogs.CatalogItems.GetCatalogItemPDP
{
    public interface IGetCatalogItemPDPService
    {
        CatalogItemPDPDto Execute(string Slug);
    }
    public class GetCatalogItemPDPService : IGetCatalogItemPDPService
    {
        private readonly IDataBaseContext context;
        private readonly IUriComposerService uriComposerService;

        public GetCatalogItemPDPService(IDataBaseContext context, IUriComposerService uriComposerService)
        {
            this.context = context;
            this.uriComposerService = uriComposerService;
        }

        public CatalogItemPDPDto Execute(string Slug)
        {
            ///کاتالوگ آیتم و فیچرهایش را بایست به دست بیاوریم
            ///bugeto-asp-dot-net-core
            var catalogitem = context.CatalogItems
                 .Include(p => p.CatalogItemFeatures)
                 .Include(p => p.CatalogItemImages)
                 .Include(p => p.CatalogType)
                 .Include(p => p.CatalogBrand)
                 .Include(p => p.Discounts)
                 .SingleOrDefault(p => p.Slug == Slug);
            catalogitem.VisitCount += 1;
            context.SaveChanges();


            ///حال روی فیچرهای کاتالوگ آیتم گروپ بای میزنیم تا دیتای مورد نظر را به دست آوریم
            ///تایپ فیچر همان خروجی دی تی او آن است
            var feature = catalogitem.CatalogItemFeatures
                .Select(p => new PDPFeaturesDto
                {
                    Group = p.Group,
                    Key = p.Key,
                    Value = p.Value
                }).ToList()
                .GroupBy(p => p.Group);

            ///برای پیدا کردن مشابهات تایپ آیدی ها برابر باشد
            var similarCatalogItems = context
               .CatalogItems
               .Include(p => p.CatalogItemImages)
               .Where(p => p.CatalogTypeId == catalogitem.CatalogTypeId)
                ///ده عدد پیدا کن
                .Take(10)
               ///مپ کن به دی تی او 
               .Select(p => new SimilarCatalogItemDto
               {
                   Id = p.Id,
                   ///داخل ایمیج با کمک فرست اور دیفالت به آدرس میرسیم
                   ///و حتما از uriComposerService
                   ///برای ارسال آدرس عکس کمک بایست بگیریم
                   Images = uriComposerService.ComposeImageUri(p.CatalogItemImages.FirstOrDefault().Src),
                   Price = p.Price,
                   Name = p.Name
               }).ToList();

           
            return new CatalogItemPDPDto
            {
                Features = feature,
                SimilarCatalogs = similarCatalogItems,
                Id = catalogitem.Id,
                Name = catalogitem.Name,
                Brand = catalogitem.CatalogBrand.Brand,
                Type = catalogitem.CatalogType.Type,
                Price = catalogitem.Price,
                Description = catalogitem.Description,
                ///یک لیست استرینگی از ایمیج ها
                ///و با کمک سرویس 
                ///uriComposerService
                ///آدرس آن را تبدیل میکنیم 
                Images = catalogitem.CatalogItemImages.Select(p => uriComposerService.ComposeImageUri(p.Src)).ToList(),
                OldPrice = catalogitem.OldPrice,
                PercentDiscount = catalogitem.PercentDiscount,
                AvailableStock=catalogitem.AvailableStock,
            };
          
        }
    }

    public class CatalogItemPDPDto
    {
        /// برای بحث سئو سایت باید اسلاگ رو هم در این سرویس ارسال کنیم
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Brand { get; set; }
        public int Price { get; set; }

        ///بعد از ادیت موجودیت کاتالوگ آیتم و افزودن دیسکانت اضافه شدند 
        ///OldPrice & PercentDiscount
        public int? OldPrice { get; set; }
        public int? PercentDiscount { get; set; }

        public List<string> Images { get; set; }
        public string Description { get; set; }
        public IEnumerable<IGrouping<string, PDPFeaturesDto>> Features { get; set; }
        public List<SimilarCatalogItemDto> SimilarCatalogs { get; set; }
        public int AvailableStock { get; set; }

    }

    /// نمایش جزئیات محصول
    public class PDPFeaturesDto
    {
        public string Group { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }

    /// نمایش محصولات مشابه
    public class SimilarCatalogItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public string Images { get; set; }
    }

}
