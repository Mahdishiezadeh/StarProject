using Domain.Attributes;
using Domain.Catalogs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Discounts
{
    [Auditable]
    public class Discount
    {
        public int Id { get; set; }
        public string Name { get; set; }
        ///آیا تخفیف درصدی دارد ؟
        public bool UsePercentage { get; set; }
        ///درصد برای تخفیف
        public int DiscountPercentage { get; set; }
        ///مبلغ برای تخفیف
        public int DiscountAmount { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        ///آیا کد تخفیف دارد ؟
        public bool RequiresCouponCode { get; set; }
        public string CouponCode { get; set; }

        ///با ایجاد نات مپ ، دیسکانت آیدی و دیسکانت تایپ اینام کاربر ادمین با ارسال آیدی به تخفیف مورد نظر میرسد
        ///نات مپ یعنی نمیخواهیم به عنوان یک فیلد (پراپرتی)در دیتابیس ثبت شود
        [NotMapped]
        public DiscountType DiscountType
        {
            get => (DiscountType)this.DiscountTypeId;
            set => this.DiscountTypeId = (int)value;
        }

        public int DiscountTypeId { get; set; }
        ///یک ارتباط چند به چند با کاتالوگ آیتم (پروداکت)موجودیت
        public ICollection<CatalogItem> CatalogItems { get; set; }

        public int LimitationTimes { get; set; }
        [NotMapped]
        public DiscountLimitationType DiscountLimitation
        {
            get => (DiscountLimitationType)this.DiscountLimitationId;
            set => this.DiscountLimitationId = (int)value;
        }
        public int DiscountLimitationId { get; set; }


        ///با این متد مقدار تخفیف اگر درصدی باشد ، یا اگر دقیقا عدد باشد به دست می آید
        ///مبلغ سبد خرید ورودی است
        public int GetDiscountAmount(int amount)
        {
            var result = 0;

            if (UsePercentage)
            {
                ///اگر درصد بود ، درصد اون مبلغ را محاسبه کن 
                result = (((amount) * (DiscountPercentage)) / 100);
            }
            else
            {
                ///اگر مبلغ انتخاب شد عینا خود مبلغ را برگردان
                result = DiscountAmount;
            }

            return result;
        }

    }

    public enum DiscountType
    {
        [Display(Name = "تخفیف برای محصولات")]
        AssignedProduct = 1,
        [Display(Name = "تخفیف برای دسته بندی")]
        AssignedToCategories = 2,
        [Display(Name = "تخفیف برای مشتری")]
        AssignedToUser = 3,
        [Display(Name = "تخفیف برای برند")]
        AssignedToBrand = 3,
    }

    ///  محدودیت تعداد استفاده
    public enum DiscountLimitationType
    {
        /// بدونه محدودیت تعداد
        [Display(Name = "بدونه محدودیت تعداد")]
        Unlimited = 0,
        /// فقط N بار
        [Display(Name = "فقط N بار")]
        NTimesOnly = 1,
        /// N بار به ازای هر مشتری
        [Display(Name = "N بار به ازای هر مشتری")]
        NTimesPerCustomer = 2,
    }
}
