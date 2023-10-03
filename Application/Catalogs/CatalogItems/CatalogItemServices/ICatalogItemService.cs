using Application.Catalogs.CatalogItems.UriComposer;
using Application.Dtos;

using Application.Interfaces.Contexts;
using AutoMapper;
using Common;
using Domain.Catalogs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Catalogs.CatalogItems.CatalogItemServices
{
    public interface ICatalogItemService
    {
        List<CatalogBrandDto> GetBrand();
        List<ListCatalogTypeDto> GetCatalogType();
        PaginatedItemsDto<CatalogItemListItemDto> GetCatalogList(int page, int pageSize);
        void AddToMyFavourite(string UserId, int CatalogItemId);

        ///ارسال یک لیست از کالاهای مورد علاقه به خروجی
        PaginatedItemsDto<FavouriteCatalogItemDto> GetMyFavourite(string UserId,int page=1,int pageSize=20);
    }

    public class CatalogItemService : ICatalogItemService
    {
        private readonly IDataBaseContext context;
        private readonly IMapper mapper;
        private readonly IUriComposerService uriComposerService;

        public CatalogItemService(IDataBaseContext context, IMapper mapper, IUriComposerService uriComposerService)
        {
            this.context = context;
            this.mapper = mapper;
            this.uriComposerService = uriComposerService;
        }

        public List<CatalogBrandDto> GetBrand()
        {
            var brands = context.CatalogBrands
           .OrderBy(p => p.Brand).Take(500).ToList();

            var data = mapper.Map<List<CatalogBrandDto>>(brands);
            return data;
        }

        public List<ListCatalogTypeDto> GetCatalogType()
        {
            ///چون تا سه سطح پرنت میتوان داشت دو بار در این کد پرنت را اخذ میکنیم
            var types = context.CatalogTypes
                .Include(p => p.ParentCatalogType)
                .Include(p => p.ParentCatalogType)
                .ThenInclude(p => p.ParentCatalogType.ParentCatalogType)
                ///ساب تایپ ها (فرزندان) را نیز اخذ میکنیم 
                .Include(p => p.SubType)
                ///این شروط برای یه دست آوردن پایین ترین سطح است
                .Where(p => p.ParentCatalogTypeId != null)
                .Where(p => p.SubType.Count == 0)
                ///سلکت برای واکشی دیتای مورد نظر
                .Select(p => new { p.Id, p.Type, p.ParentCatalogType, p.SubType })
                                .ToList()
                ///به علت پیچیدگی کد دستی با کمک سلکت مپ کردیم
                .Select(p => new ListCatalogTypeDto
                {
                    Id = p.Id,
                    ///علامت های سوال برای آن است که اگر فرزند نداشت خطا ندهد
                    ////فرزند سوم                   فرزند دوم                          فرزند اول                    
                    Type = $"{p?.Type ?? ""} - {p?.ParentCatalogType?.Type ?? ""} - {p?.ParentCatalogType?.ParentCatalogType?.Type ?? ""}"
                }).ToList();
            return types;
        }


        public PaginatedItemsDto<CatalogItemListItemDto> GetCatalogList(int page, int pageSize)
        {
            int rowCount = 0;
            var data = context.CatalogItems
                .Include(p => p.CatalogType)
                .Include(p => p.CatalogBrand)
                .ToPaged(page, pageSize, out rowCount)
                ///باعث میشه آخرین کاتالوگ آیتم در اول لییست به ما نمایش داده بشه
                .OrderByDescending(p => p.Id)
                .Select(p => new CatalogItemListItemDto
                {
                    Id = p.Id,
                    AvailableStock = p.AvailableStock,
                    Brand = p.CatalogBrand.Brand,
                    MaxStockThreshold = p.MaxStockThreshold,
                    Name = p.Name,
                    Price = p.Price,
                    RestockThreshold = p.RestockThreshold,
                    Type = p.CatalogType.Type,
                }).ToList();

            return new PaginatedItemsDto<CatalogItemListItemDto>(page, pageSize, rowCount, data);
        }

        public void AddToMyFavourite(string UserId, int CatalogItemId)
        {
            ///کالا را فایند
            var catalogItem = context.CatalogItems.Find(CatalogItemId);
            ///چون دسترسی برای فایند یوزر نداریم پس از طریف اند پوینت یوزرآیدی را به سرویس میفرستیم
            CatalogItemFavourite catalogItemFavourite = new CatalogItemFavourite
            {
                CatalogItem=catalogItem,
                UserId= UserId,
            };
            context.CatalogItemFavourites.Add(catalogItemFavourite);
            context.SaveChanges();
        }

        public PaginatedItemsDto<FavouriteCatalogItemDto> GetMyFavourite(string UserId, int page = 1, int pageSize = 20)
        {
            var model = context.CatalogItems
                .Include(p => p.CatalogItemImages)
                .Include(p => p.Discounts)
                .Include(p => p.CatalogItemFavourites)
                 ///فقط کاتالوگ آیتم های مورد علاقه ای را لود کن که یک عدد (انی) از آنها 
                 ///برابر با یوزر آیدی ورودی باشد
                 .Where(p => p.CatalogItemFavourites.Any(f => f.UserId == UserId))
               .OrderByDescending(p => p.Id)
               .AsQueryable();
            int rowCount = 0;

            var data = model.PagedResult(page, pageSize, out rowCount)
           .ToList()
           .Select(p => new FavouriteCatalogItemDto
           {
               Id = p.Id,
               Name = p.Name,
               Price = p.Price,
               Rate = 4,
               AvailableStock = p.AvailableStock,
               Image = uriComposerService
               .ComposeImageUri(p.CatalogItemImages.FirstOrDefault().Src),
           }).ToList();
            return new PaginatedItemsDto<FavouriteCatalogItemDto>(page, pageSize, rowCount, data);
        }
    }

    public class CatalogBrandDto
    {
        public int Id { get; set; }
        public string Brand { get; set; }
    }
    public class ListCatalogTypeDto
    {
        public int Id { get; set; }
        public string Type { get; set; }
    }

    public class CatalogItemListItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public string Type { get; set; }
        public string Brand { get; set; }
        public int AvailableStock { get; set; }
        public int RestockThreshold { get; set; }
        public int MaxStockThreshold { get; set; }
    }
    public class FavouriteCatalogItemDto
    {
        public int Id { get; set; }
        public int Price { get; set; }
        public int Rate { get; set; }
        public int AvailableStock { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
    }



}
