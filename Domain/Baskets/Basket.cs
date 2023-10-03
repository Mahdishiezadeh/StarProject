using Domain.Attributes;
using Domain.Catalogs;
using Domain.Discounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Baskets
{

    [Auditable]
    public class Basket
    {
        public int Id { get; set; }
        public string BuyerId { get; private set; }

        /// برای دسترسی از داخل کلاس ، که فقط ست کردن از داخل کلاس دارد
        private readonly List<BasketItem> _items = new List<BasketItem>();

        ///یک پراپرتی برای نگهداری مبلغ ی که بعد از زدن کد تخفیف از مبلغ کل ، کسر میشود
        ///مبلغ تخفیف 
        public int DiscountAmount { get; private set; } = 0;

        ///یک ریلیشن با خود دیسکانت که هر جا دیسکانت را بخواهیم باید اپلاید دیسکانت را فراخوانی کنیم
        public Discount AppliedDiscount { get; private set; }

        public int? AppliedDiscountId { get; private set; }


        /// ایجاد ریلیشن به بسکت آیتم و فقط به صورت گت برای دسترسی از بیرون
        public ICollection<BasketItem> Items => _items.AsReadOnly();
        public Basket(string buyerId)
        {
            this.BuyerId = buyerId;
        }
        ///ست کردن اطلاعات بسکت به آیتم که از کاربر اخذ میشود
        ///آیتم پرایویت را با کمک 
        /// public BasketItem(int unitPrice, int quantity, int catalogItemId)
        /// مقدار دهی میکنیم 
        /// با شرط آنکه آیتم پابلیک مقدار نداشته باشد
        public void AddItem(int catalogItemId, int quantity, int unitPrice)
        {
            ///اگر با کاتالوگ آیدی که از ورودی (کاربر) دریافت میکنیم
            ///با کاتالوگ آیدی اخذ شده از لیست بسکت آیتم یکسان نبود  
            if (!Items.Any(p => p.CatalogItemId == catalogItemId))
            {
                ///در آیتم پرایویت که امکان ست وجود دارد درج میکنیم
                _items.Add(new BasketItem(catalogItemId, quantity, unitPrice));
                return;
            }

            ///اگر وجود آیتمی که قصد اضافه کردن داشت تکراری بود یک عدد به تعداد آن آیتم می افزایم
            var existingItem = Items.FirstOrDefault(p => p.CatalogItemId == catalogItemId);

            ///چون نمیتوان با آیتم پابلیک به ست دسترسی داشت پس یک متد برای افزایش تعداد آیتم (کوانتیتی) مینویسیم
            existingItem.AddQuantity(quantity);
        }

        ///اخذ قیمت کل با کسر تخفیف سبد خرید
        public int TotalPrice()
        {
            ///با کمک آیتم به بسکت آیتم ها دسترسی داریم و قیمت کالاها را اخذ میکنیم
            int totalPrice = _items.Sum(p => p.UnitPrice * p.Quantity);
            ///اعمال تخفیف روی سبد خرید
            totalPrice -= AppliedDiscount.GetDiscountAmount(totalPrice);
            ///بازگشت قیمت نهایی 
            return totalPrice;
        }

        ///اخذ قیمت کل سبد خرید بدون تخفیف
        public int TotalPriceWithOutDiescount()
        {
            int totalPrice = _items.Sum(p => p.UnitPrice * p.Quantity);
            return totalPrice;
        }

        ///اگر بخواهند روی سبد خرید تخفیف اعمال کنند
        ///فقط کافی است این متد را فراخوانی کنند
        public void ApplyDiscountCode(Discount discount)
        {
            this.AppliedDiscount = discount;
            this.AppliedDiscountId = discount.Id;
            ///مبلغ سبد خرید را باید به متد پاس دهیم
            this.DiscountAmount = discount.GetDiscountAmount(TotalPriceWithOutDiescount());
        }

        ///پاک کردن تخفیف
        public void RemoveDescount()
        {
            AppliedDiscount = null;
            AppliedDiscountId = null;
            DiscountAmount = 0;
        }

    }

    [Auditable]
    public class BasketItem
    {
        public int Id { get; set; }
        public int UnitPrice { get; private set; }
        public int Quantity { get; private set; }
        public int CatalogItemId { get; private set; }
        public CatalogItem CatalogItem { get; private set; }
        public int BasketId { get; private set; }


        public BasketItem(int catalogItemId, int quantity, int unitPrice)
        {
            CatalogItemId = catalogItemId;
            UnitPrice = unitPrice;
            SetQuantity(quantity);
        }

        /// متد افزودن کوانتیتی 
        public void AddQuantity(int quantity)
        {
            Quantity += quantity;
        }

        /// متد ست کردن کوانتیتی
        public void SetQuantity(int quantity)
        {
            Quantity = quantity;
        }

    }
}
