using Domain.Attributes;
using Domain.Discounts;
using Domain.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Catalogs
{
    [Auditable]
    public class CatalogItem
    {
        ///برای ست کردن پراپرتی ها
        private int _price = 0;
        private int? _oldPrice = null;


        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }


        ///تغییراتی اعمال شود تا در صورت ارائه تخفیف این قیمت ها تغییر کند
        public int Price
        {
            ///برای گت کردن متد مینویسیم
            get
            {
                return GetPrice();
            }
            set
            {
                Price = _price;
            }
        }
        public int? OldPrice
        {
            get
            {
                return _oldPrice;
            }
            set
            {
                OldPrice = _oldPrice;
            }
        }
        public int? PercentDiscount { get; set; }

        public int CatalogTypeId { get; set; }
        public CatalogType CatalogType { get; set; }
        public int CatalogBrandId { get; set; }
        public CatalogBrand CatalogBrand { get; set; }

        /// موجودی انبار
        public int AvailableStock { get; set; }

        /// اگر موجودی انبار به این عدد رسید به مدیر سایت برای تهیه این کالا هشدار داده شود
        public int RestockThreshold { get; set; }

        /// حداکثر تعداد کالایی که میتوان سفارش داد و در انبار ذخیره کرد.
        public int MaxStockThreshold { get; set; }

        ///تعداد بازدید محصول 
        public int VisitCount { get; set; }

        ///اسلاگ برای سئو جایگزین آیدی میشود
        public string Slug { get; set; }

        ///رلیشن ها
        public ICollection<CatalogItemFeature> CatalogItemFeatures { get; set; }
        public ICollection<CatalogItemImage> CatalogItemImages { get; set; }

        ///ارتباط چند به چند با موجودیت دیسکانت
        public ICollection<Discount> Discounts { get; set; }
        public ICollection<CatalogItemFavourite> CatalogItemFavourites { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }

        private int GetPrice()
        {
            ///بیشترین تخفیف را اخذ
            var dis = GetPreferredDiscount(Discounts, _price);
            if (dis != null)
            {
                ///مقدار  تخفیف را محاسبه 
                var discountAmount = dis.GetDiscountAmount(_price);
                ///قیمت قبل را از مقدار بیشترین تخفیف کسر 
                int newPrice = _price - discountAmount;
                ///مقدار قبل منتقل به الد پرایس
                _oldPrice = _price;

                ///مقدار تخفیف ضرب در صد تقسیم بر قیمت 
                PercentDiscount = (discountAmount * 100) / _price;
                
                return newPrice;
            }
            return _price;
        }

        ///برای یک محصول ممکن است چند تخفیف ثبت شده باشد
        ///بایست بیشترین آنها شناسایی و اعمال شود
        private Discount GetPreferredDiscount(ICollection<Discount> discounts, int price)
        {
            Discount preferredDiscount = null;
            decimal? maximumDiscountValue = null;

            if (discounts!=null)
            {
                foreach (var discount in discounts)
                {
                    var currentDiscountValue = discount.GetDiscountAmount(price);
                    if (currentDiscountValue != decimal.Zero)
                    {
                        if (!maximumDiscountValue.HasValue || currentDiscountValue > maximumDiscountValue)
                        {
                            maximumDiscountValue = currentDiscountValue;
                            preferredDiscount = discount;
                        }
                    }
                }
            }
            return preferredDiscount;
        }

    }
}
