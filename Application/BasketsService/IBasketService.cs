using Application.Catalogs.CatalogItems.UriComposer;
using Application.Interfaces.Contexts;
using Domain.Baskets;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.BasketsService
{
    public interface IBasketService
    {
        ///سرویس برای اخذ سبد خرید کاربر
        ///دو حالت دارد یا سبد خریدی داریم یا آن را ایجاد میکنیم
        BasketDto GetOrCreateBasketForUser(string BuyerId);
        void AddItemToBasket(int basketId, int CtalogItemId, int quantity = 1);

        bool RemoveItemFromBasket(int ItemId);
        bool SetQuantities(int itemId, int quantity);
        ///این متد برای ویوکامپوننت بسکت ایجاد شده
        BasketDto GetBasketForUser(string UserId);
        ///این متد برای انتقال سبد کاربر لاگین نکرده به سبد همان کاربر بعد از لاگین است
        void TransferBasket(string anonymousId, string UserId);
    }
    public class BasketService : IBasketService
    {
        private readonly IDataBaseContext context;
        private readonly IUriComposerService uriComposerService;

        public BasketService(IDataBaseContext context, IUriComposerService uriComposerService)
        {
            this.context = context;
            this.uriComposerService = uriComposerService;
        }

        public void AddItemToBasket(int basketId, int CatalogItemId, int quantity = 1)
        {
            var basket = context.Baskets.FirstOrDefault(p => p.Id == basketId);
            if (basket == null)
                ///بسکت آیدی نباید نال باشد زمان ادد کردن محصول
                throw new Exception("");
            ///با فایند کردن کاتالوگ پرایس آن را اخذ میکنیم
            var catalog = context.CatalogItems.Find(CatalogItemId);
            ///چون بسکت را به صورت ریچ انتیتی ایجاد کردیم 
            ///در اینجا امکان ادد نیست و بایست از متدهای پرایویت خود 
            ///ریچ انتیتی بسکت برای ادد کردن کمک بگیریم
            basket.AddItem(CatalogItemId, quantity, catalog.Price);
            context.SaveChanges();
        }

        public BasketDto GetBasketForUser(string UserId)
        {
            var basket = context.Baskets
          .Include(p => p.Items)
          .ThenInclude(p => p.CatalogItem)
          .ThenInclude(p => p.CatalogItemImages)
          .SingleOrDefault(p => p.BuyerId == UserId);
            if (basket == null)
            {
                ///برای ویو کامپوننت قصد نداریم به ازاء هر کاربر بازدید کننده
                ///از سایت یک سبد ایجاد کنیم پس نال برگشت میدهیم 
                return null;
            }
            return new BasketDto
            {
                BuyerId = basket.BuyerId,
                Id = basket.Id,
                Items = basket.Items.Select(item => new BasketItemDto
                {
                    CatalogItemid = item.CatalogItemId,
                    Id = item.Id,
                    CatalogName = item.CatalogItem.Name,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    ///برای آنکه اگر نال بود خطا ندهد از علامت سوال استفاده میکنیم 
                    ImageUrl = uriComposerService.ComposeImageUri(item?.CatalogItem?
                     ///اولین تصویر را اخذ میکنیم و آدرس آن را بگیر 
                     ///و سپس دوتا علامت سوال که اگر نال بود آدرس یک تصویر دیگر را قرار بده
                     .CatalogItemImages?.FirstOrDefault()?.Src ?? ""),

                }).ToList()
            };
        }

        public BasketDto GetOrCreateBasketForUser(string BuyerId)
        {
            var basket = context.Baskets
                .Include(p=>p.Items)
                .ThenInclude(p=>p.CatalogItem)
                .ThenInclude(p=>p.CatalogItemImages)

                .Include(p => p.Items)
                .ThenInclude(p => p.CatalogItem)
                .ThenInclude(p => p.Discounts)
                .SingleOrDefault(p => p.BuyerId == BuyerId);
            if (basket==null)
            {
                ///سبد خرید را ایجاد میکنیم با کمک متد پرایویت
               return CreateBasketForUser(BuyerId);
            }
            return new BasketDto
            {
                BuyerId = basket.BuyerId,
                Id = basket.Id,
                DiscountAmount=basket.DiscountAmount,
                Items = basket.Items.Select(item => new BasketItemDto
                {
                    CatalogItemid = item.CatalogItemId,
                    Id = item.Id,
                    CatalogName = item.CatalogItem.Name,
                    Quantity = item.Quantity,
                    UnitPrice = item.CatalogItem.Price,
                    ///برای آنکه اگر نال بود خطا ندهد از علامت سوال استفاده میکنیم 
                    ImageUrl = uriComposerService.ComposeImageUri(item?.CatalogItem?
                     ///اولین تصویر را اخذ میکنیم و آدرس آن را بگیر 
                     ///و سپس دوتا علامت سوال که اگر نال بود آدرس یک تصویر دیگر را قرار بده
                     .CatalogItemImages?.FirstOrDefault()?.Src ?? ""),

                }).ToList()
            };
            
        }

        public bool RemoveItemFromBasket(int ItemId)
        {
            var item = context.BasketItems.SingleOrDefault(p => p.Id == ItemId);
            context.BasketItems.Remove(item);
            context.SaveChanges();
            return true;
        }

        /// <summary>
        /// باید به ریچ Entity
        /// مراجعه و متد SetQuantity
        ///  را اضافه کنیم
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public bool SetQuantities(int itemId, int quantity)
        {
            var item = context.BasketItems.FirstOrDefault(p => p.Id == itemId);
            item.SetQuantity(quantity);
            context.SaveChanges();
            return true;
        }


        /// <param name="anonymousId">همان آیدی که به عنوان بایر آیدی روی مرورگر کاربر است</param>
        /// <param name="UserId">آیدی یوزری است که سبد به آن متقل میشود</param>
        public void TransferBasket(string anonymousId, string UserId)
        {
            var anonymousBasket = context.Baskets
                .Include(p=>p.Items)
                .Include(p=>p.AppliedDiscount)
                .SingleOrDefault(p => p.BuyerId == anonymousId);
            ///اگر سبد خریدی برای این کاربر وجود نداشت ریترن میکنیم که ادامه انجام نشود
            if (anonymousBasket == null) return;
            ///بایست سبد خرید یوزر را پیدا کنیم و این موارد را به آن اضافه کنیم 
            ///و یا اگر برای یوزر سبد خریدی وجود نداشت آن را ایجاد کنیم
            var userBasket = context.Baskets.SingleOrDefault(p => p.BuyerId == UserId);
            if(userBasket==null)
            {
                userBasket = new Basket(UserId);
                context.Baskets.Add(userBasket);
            }
            ///ادد کردن در بسکت لاگین شده
            foreach (var item in anonymousBasket.Items)
            {
                userBasket.AddItem(item.CatalogItemId, item.Quantity, item.UnitPrice);
            }
            ///سبد خرید ناشناس تخفیفی دارد ؟
            if (anonymousBasket.AppliedDiscount != null)
            {
                ///اعمال تخفیف آنونیموس (بسکتی که لاگین نشده) به یوزر بسکت (بسکتی که لاگین) شده
                userBasket.ApplyDiscountCode(anonymousBasket.AppliedDiscount);
            }
            ///در این مرحله بسکت کاربر ناشناس باید ریمو شود
            context.Baskets.Remove(anonymousBasket);
            context.SaveChanges();
        }

        ///داخل همین متد با یک متد پرایوت اقدام به ایجاد بسکت در صورت خالی بودن میکنیم
        ///از موجودیت بسکت که بالا ایجاد کردیم یک نمونه ایجاد میکنیم 
        ///علت عدم مقدار دهی آیتم در این متد این است که بسکت ایجاد شده و باید خالی از کالا باشد
        private BasketDto CreateBasketForUser(string BuyerId)
        {
            ///چون پروسه ادد را با کمک ریچ ان تی تی ایجاد کردیم با
            ///ایجاد یک نمونه از موجودیت اقدام به ادد کردن میکنیم
            Basket basket = new Basket(BuyerId);
            ///به کانتکست ادد میکنیم نمونه ایجاد شده را
            context.Baskets.Add(basket);
            context.SaveChanges();
            return new BasketDto
            {
                BuyerId = basket.BuyerId,
                Id = basket.Id,
            };
        }
    }

    public class BasketDto
    {
        public int Id { get; set; }
        public string BuyerId { get; set; }
        public List<BasketItemDto> Items { get; set; } = new List<BasketItemDto>();
        ///مبلغ تخفیف
        public int DiscountAmount { get; set; }

        public int Total()
        {
            if (Items.Count>0)
            {
                ///قیمت کل را به دست و منهای مبلغ تخفیف میکنیم
                int total= Items.Sum(p => p.UnitPrice * p.Quantity);
                total -= DiscountAmount;
                return total;
            }
            return 0;
        }
        public int TotalWithOutDiescount()
        {
            if (Items.Count > 0)
            {
                int total = Items.Sum(p => p.UnitPrice * p.Quantity);
                return total;
            }
            return 0;
        }
    }

    /// در این دی تی او کاتالوگ آیتم قرار دارد (پروداکت) د
    public class BasketItemDto
    {
        public int Id { get; set; }
        public int CatalogItemid { get; set; }
        public string CatalogName { get; set; }
        public int UnitPrice { get; set; }
        public int Quantity { get; set; }
        public string ImageUrl { get; set; }
    }
}
