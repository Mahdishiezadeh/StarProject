using Application.Discounts.AddNewDiscountServices;
using MD.PersianDateTime.Standard;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.EndPoint.Binders
{

    ///برای ایجاد یک مدل بایندر اختصاصی باید از 
    ///IModelBinder
    ///ارث بری کرد و آن را ایمپلیمنت میکنیم 
    public class DiscountEntityBinder : IModelBinder
    {
        public DiscountEntityBinder()
        {

        }
        
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext==null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            ///فیلد نیم که در اینجا مدل است را اخذ میکنیم
            /// public AddNewDiscountDto model { get; set; }
            /// و به بایندینگ کانتکست اساین میکنیم 
            string FieldName = bindingContext.FieldName;

            ///این دی تی او را از کریت . سی اس  بایست در اینجا بایند کنیم 
            ///AddNewDiscountDto
            AddNewDiscountDto discountDto = new AddNewDiscountDto()
            {
                ///نام های استرینگ داخل کوتیشن از ویو 
                ///Create.cshtml
                ///اخذ میکنیم
                CouponCode = bindingContext.ValueProvider

                ///در این خط کد به عنوان نمونه با نام فیلد نیم
                ///model
                ///دات کوپن کد را با نیم آو .... به صورت استرینگ اخذ کردیم
                /// .CouponCode
                ///را از ویو کریت اخذ و تبدیل نمودیم
                .GetValue($"{FieldName}.{nameof(discountDto.CouponCode)}").Values.ToString(),

                DiscountAmount = int.Parse(bindingContext.ValueProvider
                .GetValue($"{FieldName}.{nameof(discountDto.DiscountAmount)}").Values.ToString()),

                DiscountLimitationId = int.Parse(bindingContext.ValueProvider
                .GetValue($"{FieldName}.{nameof(discountDto.DiscountLimitationId)}").Values.ToString()),


                DiscountPercentage = int.Parse(bindingContext.ValueProvider
                .GetValue($"{FieldName}.{nameof(discountDto.DiscountLimitationId)}").Values.ToString()),

                DiscountTypeId = int.Parse(bindingContext.ValueProvider
                .GetValue($"{FieldName}.{nameof(discountDto.DiscountTypeId)}").Values.ToString()),
                LimitationTimes = int.Parse(bindingContext.ValueProvider
                .GetValue($"{FieldName}.{nameof(discountDto.LimitationTimes)}").Values.ToString()),

                UsePercentage = bool.Parse(bindingContext.ValueProvider
                .GetValue($"{FieldName}.{nameof(discountDto.UsePercentage)}").FirstValue.ToString()),

                Name = bindingContext.ValueProvider
                .GetValue($"{FieldName}.{nameof(discountDto.Name)}").Values.ToString(),

                RequiresCouponCode = bool.Parse(bindingContext.ValueProvider
                .GetValue($"{FieldName}.{nameof(discountDto.RequiresCouponCode)}").FirstValue.ToString()),

                EndDate = PersianDateTime.Parse(bindingContext.ValueProvider
                .GetValue($"{FieldName}.{nameof(discountDto.EndDate)}").Values.ToString()),

                StartDate = PersianDateTime.Parse(bindingContext.ValueProvider
                .GetValue($"{FieldName}.{nameof(discountDto.StartDate)}").Values.ToString()),
            };

            ///ابتدا تمام آیدی های کاتالوگ ها را اخذ
            var appliedToCatalogItem = bindingContext.ValueProvider.GetValue("model.appliedToCatalogItem");

            ///بررسی میکنیم این استرینگ (آیدی کاتالوگ ها) نال نباشد 
            if (!string.IsNullOrEmpty(appliedToCatalogItem.Values))
            {
                discountDto.appliedToCatalogItem =
                bindingContext.ValueProvider
               .GetValue($"{FieldName}.{nameof(discountDto.appliedToCatalogItem)}")

               ///با کاما آیدی ها را جدا کن و تبدیل به اینت و لیست کن
               ///چون در ویو ریزور کریت ،کد اسکریپتی سلکت 2 گفته بودیم با کاما جدا شود
               .Values.ToString().Split(',').Select(x => Int32.Parse(x)).ToList();

            }
            bindingContext.Result = ModelBindingResult.Success(discountDto);
            return Task.CompletedTask;
        }
    }
}
