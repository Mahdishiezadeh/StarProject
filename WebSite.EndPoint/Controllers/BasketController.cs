using Application.BasketsService;
using Application.Discounts;
using Application.Orders;
using Application.Payments;
using Application.Users;
using Domain.Order;
using Domain.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebSite.EndPoint.Models.ViewModels.Baskets;
using WebSite.EndPoint.Utilities;

namespace WebSite.EndPoint.Controllers
{
    [Authorize]
    public class BasketController : Controller
    {
        private readonly IBasketService basketService;
        private readonly SignInManager<User> signInManager;
        private readonly IUserAddressService userAddressService;
        private readonly IOrderService orderService;
        private readonly IPaymentService paymentService;
        private readonly IDiscountService discountService;
        private readonly UserManager<User> userManager;
        private string UserId = null;
        /// <summary>
        /// چون یوزر کاستوم شده 
        /// 
        /// </summary>
        /// <param name="basketService"></param>
        /// <param name="signInManager"></param>
        public BasketController(IBasketService basketService,SignInManager<User> signInManager,
            IUserAddressService userAddressService,IOrderService orderService, IPaymentService paymentService
            ,IDiscountService discountService,UserManager<User> userManager)
        {
            this.basketService = basketService;
            this.signInManager = signInManager;
            this.userAddressService = userAddressService;
            this.orderService = orderService;
            this.paymentService = paymentService;
            this.discountService = discountService;
            this.userManager = userManager;
        }
        [AllowAnonymous]
        public IActionResult Index()
        {
            ///در اینجا با کمک همان متد 
            ///GetOrSetBasket
            ///به کاربر لیست خرید را نمایش میدهیم
            var data = GetOrSetBasket();
            return View(data);
        }
        [AllowAnonymous]
        [HttpPost]
        ///چه تعداد و چه محصولی به سبد خرید ااضافه میشود 
        ///کوانتیتی مقدار پیش فرض یک دارد
        public IActionResult Index(int CatalogitemId, int quantity = 1)
        {
            ///بعد از ایجاد یا ست کردن بسکت اقدام به بازگشت به پیج ایندکس میکنیم
            var basket = GetOrSetBasket();
            ///با کمک این متد سرویس بسکت سرویس به بسکت ایجاد شده آیتم ادد میکنیم
            basketService.AddItemToBasket(basket.Id, CatalogitemId, quantity);

            return RedirectToAction(nameof(Index));
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult RemoveItemFromBasket(int ItemId)
        {
            basketService.RemoveItemFromBasket(ItemId);
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// چون باید با ایجکس کار کرد پس جیسون بازگت میدهیم
        /// </summary>
        /// <param name="basketItemId"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public IActionResult setQuantity(int basketItemId, int quantity)
        {
            return Json(basketService.SetQuantities(basketItemId, quantity));
        }

        ///با کمک آیدنتی تی منیجر میتوان فهمید کاربر لاگین شده یا خیر
        ///و اگر لاگین شده آیدی آن را اخذ کنیم
        ///اگر لاگین نشده یک آیدی ایجاد و بر روی کوکی ذخیره کنیم
        ///
        ///یک متد پرایویت برای اخذ بسکت کاربر ایجاد میکنیم
        ///چک میکنیم کاربری که درخواست داده لاگین شده است یا نه
        public IActionResult ShoppingPayment()
        {
            ShoppingPaymentViewModel model = new ShoppingPaymentViewModel();
            string userId = ClaimUtility.GetUserId(User);
            model.Basket = basketService.GetBasketForUser(userId);
            model.UserAddresses = userAddressService.GetAddress(userId);
            return View(model);
        }
        [HttpPost]
        ///ورودی متد دقیقا بر اساس نامی که برای رادیو باتن های ویو در نظر گرفتیم ست شده
        ///و نکته بعدی پیمنت متد چون اینام تعریف شده باید اینجکت شود
        public IActionResult ShoppingPayment(int Address,PaymentMethod PaymentMethod)
        {
            string userId = ClaimUtility.GetUserId(User);
            ///برای سبد خرید یک اردر باید ثبت کنیم و بسکت را فایند 
            var basket = basketService.GetBasketForUser(userId);
            int orderId = orderService.CreateOrder(basket.Id, Address, PaymentMethod);
            if (PaymentMethod==PaymentMethod.OnlinePaymnt)
            {
                ///ثبت پرداخت
                var payment = paymentService.PayForOrder(orderId);
                ///ارسال به درگاه پرداخت
                return RedirectToAction("Index", "Pay", new { PaymentId = payment.PaymentId });
            }
            else
            ///پرداخت در محل بود
            {
                //برو به صفحه سفارشات من
                return RedirectToAction("Index", "Orders", new { area = "customers" });
            }
        }

        public IActionResult Checkout()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult ApplyDiscount(string CouponCode, int BasketId)
        {
            var user = userManager.GetUserAsync(User).Result;
            var validsDiscount = discountService.IsDiscountValid(CouponCode,user);

            if (validsDiscount.IsSuccess)
            {
                discountService.ApplyDiscountInBasket(CouponCode, BasketId);
            }
            else
            {
            ///اگر ولید نبود با کمک دستور سلکت مسیج را سرویس دیسکانت سرویس متد ایز دیسکانت ولید اخذ و در قالب تمپ دیتا نمایش بده 
                TempData["InvalidMessage"] = String.Join(Environment.NewLine, validsDiscount.Message.Select(a => String.Join(", ", a)));
            }
            return RedirectToAction(nameof(Index));

        }

        [AllowAnonymous]
        public IActionResult RemoveDiscount(int Id)
        {
            discountService.RemoveDiscountFromBasket(Id);
            return RedirectToAction(nameof(Index));
        }



        private BasketDto GetOrSetBasket()
        {
            if (signInManager.IsSignedIn(User))
            {
                var userId = ClaimUtility.GetUserId(User);
                ///اگر کاربر لاگین بود با کمک سرویس مربوطه اقدام به ایجاد بسکت میکنیم 
                ///و بایر آیدی را از یوزر ، آیدنتی تی ، نیم اخذ میکنیم
                return basketService.GetOrCreateBasketForUser(userId);
            }
            ///در غیر این صورت برای کاربر بایست یک کوکی ایجاد کنیم 
            ///و برای آن کوکی بسکت را( با کمک متد پرایوت ست کوکی فور بسکت) ایجاد کنیم
            else
            {
                SetCookiesForBasket();
                return basketService.GetOrCreateBasketForUser(UserId);
                //return basketService.GetOrCreateBasketForUser(userId);
            }
        }
        ///متد ذخیره کوکی کاربر
        private void SetCookiesForBasket()
        {
            ///یک نام ثابت برای کوکی ذخیره شده روی مرورگر کاربران در نظر میگیریم
            string basketCookieName = "BasketId";
            ///اگر نام ذخیره شده روی مرورگر کاربر وجود داشت مقدار آن را اخذ کن 
            if (Request.Cookies.ContainsKey(basketCookieName))
            {
                UserId = Request.Cookies[basketCookieName];
            }
            ///اگر یوزر آیدی مقدار داشت دیگر ادامه نمیدهیم
            if (UserId != null) return;
            ///در غیر اینصورت که یوزر آیدی نال باشد اقدام به ایجاد جی یو آیدی برای آن میکنیم 
            UserId = Guid.NewGuid().ToString();
            ///یک کوکی آپشن اضافه میکنیم و مقدار پیش فرض آن را صحیح قرار میدهیم
            var cookieOptions = new CookieOptions { IsEssential = true };
            ///تاریخ انقضاء کوکی را نیز دوسال در نظر میگیریم
            cookieOptions.Expires = DateTime.Today.AddYears(2);
            ///اکنون روی ریسپانس کوکی را اپند میکنیم , در اصل جی یو آیدی را جایگزین بسکت کوکی نیم میکنیم
            ///و کوکی آپشن را نیز به کوکی میدهیم تا خواص کوکی (مثلا تایم انقضا ) را دریافت کند
            HttpContext.Response.Cookies.Append(basketCookieName,UserId,cookieOptions );
        }
    }
}
