using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.CacheHelpers
{
    public static class CacheHelper
    {
        //// زمان اکسپایر آن را اضافه میکنیم   
        public static readonly TimeSpan DefaultCacheDuration = TimeSpan.FromSeconds(60);

        private static readonly string _itemsKeyTemplate = "items-{0}-{1}-{2}";

        ///این متد برای کش کردن دیتاهای فیلتر ، پیجینیت شده و جستجو شده میباشد
        ///این کش برای پروداکت آیتم لیست تعبیه شده
        public static string GenerateCatalogItemCacheKey(int pageIndex, int itemsPage, int? typeId)
        {
            ///با استفاده از این کد پیج ایندکس ،آیتم پیج ، تایپ آیدی را جایگزین اعداد بالا صفر یک دو میکند
            return string.Format(_itemsKeyTemplate, pageIndex, itemsPage, typeId);
        }

        ///کلید کش های ایجاد شده
        public static string GenerateHomePageCacheKey()
        {
            return "HomePage";
        }
    }
}
