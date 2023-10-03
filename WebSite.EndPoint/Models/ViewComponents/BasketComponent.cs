using Application.BasketsService;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebSite.EndPoint.Utilities;

namespace WebSite.EndPoint.Models.ViewComponents
{
    public class BasketComponent: ViewComponent
    {
        private readonly IBasketService basketService;

        public BasketComponent(IBasketService basketService)
        {
            this.basketService = basketService;
        }
        /// <summary>
        /// دیتای این کلیم را از ویو کانتکست اخذ و آیدی یوزر با این کد به دست می آید
        /// </summary>
        private ClaimsPrincipal userClaimsPrincipal => ViewContext?.HttpContext?.User;
        public IViewComponentResult Invoke()
        {
            ///برخی یوزر های ما سبد خریدی ندارند و بایست به طور پیش فرض نال باشد 
            BasketDto basket = null;
            ///باید چک کنیم یوزر لاگین هست یا خیر ؟ و به این منظور از آیدنتیتی کمک میگیریم
            ///از متد GetOrCreateBasketForUser
            ///نمیتوان استفاده کرد چون به ازاء هر بازدید کننده یک سبد خرید در دیتابیس ایجاد میشود
            if (User.Identity.IsAuthenticated)
            {
                basket = basketService.GetBasketForUser(ClaimUtility.GetUserId(userClaimsPrincipal));
            }
            else
            {
                ///اگر لاگین نشده باید ببینیم کوکی نیم ما را درون مرورگر دارد یا نه ؟
                string basketCookieName = "BasketId";
                if (HttpContext.Request.Cookies.ContainsKey(basketCookieName))
                {
                    var buyerId = Request.Cookies[basketCookieName];
                    basket = basketService.GetBasketForUser(buyerId);
                }

            }
            return View(viewName: "BasketComponent", model: basket);
        }
    }
}
