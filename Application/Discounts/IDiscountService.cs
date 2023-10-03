using Application.Dtos;
using Application.Interfaces.Contexts;
using Domain.Discounts;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Discounts
{
    public interface IDiscountService
    {
        List<CatlogItemDto> GetCatalogItems(string searchKey);
        bool ApplyDiscountInBasket(string CoponCode, int BasketId);
        bool RemoveDiscountFromBasket(int BasketId);
        BaseDto IsDiscountValid(string couponCode, User user);
    }
    public class DiscountService : IDiscountService
    {
        private readonly IDataBaseContext context;
        private readonly IDiscountHistoryService discountHistoryService;

        public DiscountService(IDataBaseContext context,IDiscountHistoryService discountHistoryService)
        {
            this.context = context;
            this.discountHistoryService = discountHistoryService;
        }

        ///سرچ کی باید مقدار داشته باشد یا یک مقدار دیفالت خودملن بدهیم
        public List<CatlogItemDto> GetCatalogItems(string searchKey)
        {
            if (!string.IsNullOrEmpty(searchKey))
            {
                var data = context.CatalogItems
                      .Where(p => p.Name.Contains(searchKey))
                      .Select(p => new CatlogItemDto
                      {
                          Id = p.Id,
                          Name = p.Name,
                      }).ToList();
                return data;
            }
            else
            {
                var data = context.CatalogItems
                    ///برای اینکه همیشه آخری رو نمایش بده
                    .OrderByDescending(p => p.Id)
                    .Take(10)
                    .Select(p => new CatlogItemDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                    }).ToList();
                return data;
            }
        }

        public bool ApplyDiscountInBasket(string CoponCode, int BasketId)
        {
            var basket = context.Baskets
                ///آیتم های بسکت
                .Include(p => p.Items)
                ///هر جا نیاز به کوئری به دیسکانت باشد باید اپلاید دیسکانت را فراخوانی کنیم 
                ///در موجودیت بسکت رلیشن با دیسکانت را اپلاید دیسکانت نام گذاری نمودیم
                .Include(p => p.AppliedDiscount)
                .FirstOrDefault(p => p.Id == BasketId);

            ///حال دیسکانت را نیز بر اساس کوپن کد پیدا میکنیم
            var discount = context.Discounts
               .Where(p => p.CouponCode == CoponCode).FirstOrDefault();

            ///برای اپلاید کردن از متد اپلاید دیسکانت کد موجودیت بسکت که فایند کردیم کمک میگیریم
            basket.ApplyDiscountCode(discount);
            context.SaveChanges();
            return true;
        }

        public bool RemoveDiscountFromBasket(int BasketId)
        {
            var basket = context.Baskets.Find(BasketId);
            basket.RemoveDescount();
            context.SaveChanges();
            return true;
        }

        public BaseDto IsDiscountValid(string couponCode, User user)
        {
            var discount = context.Discounts
                .Where(p => p.CouponCode.Equals(couponCode)).FirstOrDefault();

            ///کد تخفیفی وجود دارد؟
            if (discount==null)
            {
                return new BaseDto(IsSuccess: false, Message: new List<string> { $"کد تخفیف معتبر نمیباشد ." });
            }

            ///زمان کد تخفیف راستی آزمایی شود 
            var now = DateTime.UtcNow;
            if (discount.StartDate.HasValue)
            {
                var startDate = DateTime.SpecifyKind(discount.StartDate.Value, DateTimeKind.Utc);
                if (startDate.CompareTo(now) > 0)
                    return new BaseDto(false, new List<string>
                    { "هنوز زمان استفاده از این کد تخفیف فرا نرسیده است" });
            }
            if (discount.EndDate.HasValue)
            {
                var endDate = DateTime.SpecifyKind(discount.EndDate.Value, DateTimeKind.Utc);
                if (endDate.CompareTo(now) < 0)
                    return new BaseDto(false, new List<string> { "کد تخفیف منقضی شده است" });
            }

            ///از متد پایین کمک میگیریم
            var checkLimit = CheckDiscountLimitations(discount, user);

            if (checkLimit.IsSuccess == false)
                return checkLimit;
            return new BaseDto(true, null);
        }

        ///بررسی تعداد استفاده از کد تخفیف 
        private BaseDto CheckDiscountLimitations(Discount discount, User user)
        {
            switch (discount.DiscountLimitation)
            {
                     ///کد تخفیف نا محدود باشد
                case DiscountLimitationType.Unlimited:
                    {
                        return new BaseDto(true, null);
                    }
                    ///ان بار از کد استفاده شود
                    ///از سرویس دیسکانت هیستوری در متد سازنده باید کمک بگیریم
                case DiscountLimitationType.NTimesOnly:
                    var totalUsage = discountHistoryService.GetAllDiscountUsageHistory(discount.Id, null, 0, 1).Data.Count();
                    if (totalUsage < discount.LimitationTimes)
                    {
                        return new BaseDto(true, null);

                    }
                    else
                    {
                        return new BaseDto(false, new List<string> { "ظرفیت استفاده از این کد تخفیف تکمیل شده است" });

                    }
                ///هر کد تخفیف چند بار به ازای یک کاربر استفاده شود
                case DiscountLimitationType.NTimesPerCustomer:
                    {
                        if (user != null)
                        {
                            var TotalUsage = discountHistoryService.GetAllDiscountUsageHistory(discount.Id, user.Id, 0, 1).Data.Count();
                            if (TotalUsage < discount.LimitationTimes)
                            {
                                return new BaseDto(true, null);
                            }
                            else
                            {
                                return new BaseDto(false, new List<string> { "تعدادی که شما مجاز به استفاده از این تخفیف بوده اید به پایان رسیده است" });
                            }
                        }
                        else
                        {
                            return new BaseDto(true, null);
                        }
                    }
                default:
                    break;
            }
            return new BaseDto(true, null);
        }
    }

    public class CatlogItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

}
