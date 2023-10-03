using Application.Catalogs.CatalogItems.UriComposer;
using Application.Dtos;
using Application.Interfaces.Contexts;
using Common;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Application.Catalogs.CatalogItems.GetCatalogIItemPLP
{
    public interface IGetCatalogIItemPLPService
    {
        //PaginatedItemsDto<CatalogPLPDto> Execute(int page,int pageSize);
        PaginatedItemsDto<CatalogPLPDto> Execute(CatlogPLPRequestDto requestDto);
    }
    public class GetCatalogIItemPLPService : IGetCatalogIItemPLPService
    {
        private readonly IDataBaseContext context;
        private readonly IUriComposerService uriComposerService;

        public GetCatalogIItemPLPService(IDataBaseContext context, IUriComposerService uriComposerService)
        {
            this.context = context;
            this.uriComposerService = uriComposerService;
        }

        ///اول کوئری میزنیم 
        ///دوم فیلتر میکنیم 
        ///سوم پیج بندی میکنیم 
        public PaginatedItemsDto<CatalogPLPDto> Execute(CatlogPLPRequestDto requestDto)
        {
            int rowCount = 0;
            var query = context.CatalogItems
                .Include(p => p.Discounts)
                .Include(p => p.CatalogItemImages)
                .OrderByDescending(p => p.Id)
                .AsQueryable();


            if (requestDto.brandId!=null)
            {
                ///چون برند آیدی ها در قالب آرایه ذخیره میشود 
                ///از انی برای پیمایش (پیدا کردن یکی)از آنها کمک میگیریم
                query = query.Where(p => requestDto.brandId.Any(b => b == p.CatalogBrandId));
            }

            if (requestDto.CatalogTypeId!=null)
            {
                query = query.Where(p => p.CatalogTypeId == requestDto.CatalogTypeId);
            }

            ///از الاستیک سرچ استفاده شود از این کد بسیار بهینه تر است
            if (!string.IsNullOrEmpty(requestDto.SearchKey))
            {
                query = query.Where(p => p.Name.Contains(requestDto.SearchKey) || p.Description.Contains(requestDto.SearchKey));
            }

            ///فقط موارد موجود را نمایش دهد
            if (requestDto.AvailableStock==true)
            {
                query = query.Where(p => p.AvailableStock > 0);
            }

            ///پرفروش ترین
            if (requestDto.SortType==SortType.Bestselling)
            {
                query = query.Include(p => p.OrderItems)
                    .OrderByDescending(p => p.OrderItems.Count());

            }

            ///محبوب ترین ها
            if (requestDto.SortType==SortType.MostPopular)
            {
                query = query.Include(p => p.CatalogItemFavourites)
                    .OrderByDescending(p => p.CatalogItemFavourites.Count());
            }

            ///پربازدیدترین محصول
            if (requestDto.SortType==SortType.MostVisited)
            {
                query = query.OrderByDescending(p => p.VisitCount);
            }

            ///جدیدترین محصول
            if (requestDto.SortType==SortType.newest)
            {
                query = query.OrderByDescending(p => p.Id);
            }

            ///ارزانترین
            if (requestDto.SortType==SortType.cheapest)
            {
                query = query
                    ///دیسکانت باید فراخوانی شود تا تخفیف ها نیز روی قیمت اعمال شود
                    .Include(p=>p.Discounts)
                    .OrderBy(p => p.Price);
            }

            ///گرانترین 
            if (requestDto.SortType==SortType.mostExpensive)
            {
                query = query
                    .Include(p => p.Discounts)
                    .OrderByDescending(p => p.Price);
            }

            var data = query.PagedResult(requestDto.page, requestDto.pageSize, out rowCount)
                .ToList()
                .Select(p => new CatalogPLPDto
                {
                    Id = p.Id,
                    Slug=p.Slug,
                    Name = p.Name,
                    Price = p.Price,
                    Rate = 4,
                    ///ایمیج روی یک دامنه دیگر قرار دارد 
                    ///پس یک سرویس برای اخذ آدرس ایمیج ها نیاز داریم
                    Image = uriComposerService
                    .ComposeImageUri(p.CatalogItemImages.FirstOrDefault().Src),
                    AvailableStock=p.AvailableStock,

                }).ToList();
            return new PaginatedItemsDto<CatalogPLPDto>(requestDto.page, requestDto.pageSize, rowCount, data);
        }
    }

    public class CatlogPLPRequestDto
    {
        public int page { get; set; } = 1;
        public int pageSize { get; set; } = 10;
        /// مشخص شدن یک نوع از محصولات 
        public int? CatalogTypeId { get; set; }
        ///دیدن صرفا محصولات چند برند 
        public int[] brandId { get; set; }
        ///کالاهای موجود
        public bool AvailableStock { get; set; }
        ///سرچ یک کلمه خاص
        public string SearchKey { get; set; }
        ///تعیین نوع طبقه بندی دیتا
        public SortType SortType { get; set; }
    }

    public enum SortType
    {
        ///بدونه مرتب سازی
        None = 0,

        ///پربازدیدترین
        MostVisited = 1,

        ///پرفروش‌ترین
        Bestselling = 2,

        ///محبوب‌ترین
        MostPopular = 3,

        ///  ‌جدیدترین
        newest = 4,

        /// ارزان‌ترین
        cheapest = 5,

        /// گران‌ترین
        mostExpensive = 6,
    }


    public class CatalogPLPDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public int Price { get; set; }
        public string Image { get; set; }
        public byte Rate { get; set; }
        public int AvailableStock { get; set; }

    }
}
