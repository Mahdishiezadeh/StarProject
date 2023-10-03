using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Admin.EndPoint.Binders;
using Application.Discounts.AddNewDiscountServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Admin.EndPoint.Pages.Discounts
{
    public class CreateModel : PageModel
    {
        private readonly IAddNewDiscountService addNewDiscountService;
        ///یک پراپرتی از دی تی او سرویس ایجاد میکنیم
        ///تا آن را از کلاینت دریافت و به کد بک اند (سرویس) ارسال کنیم
        

        [ModelBinder(BinderType = typeof(DiscountEntityBinder))]
        [BindProperty]
        ///کلاینت با نام مدل برای ما دیتا ارسال میکند
        public AddNewDiscountDto model { get; set; }
        public CreateModel(IAddNewDiscountService addNewDiscountService )
        {
            this.addNewDiscountService = addNewDiscountService;
        }

        public void OnGet()
        {

        }
        public void OnPost()
        {
            addNewDiscountService.Execute(model);
        }
    }
}
