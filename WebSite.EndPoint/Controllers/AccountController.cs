using Application.BasketsService;
using Domain.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebSite.EndPoint.Models.ViewModels.User;

namespace WebSite.EndPoint.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IBasketService basketService;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IBasketService basketService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            this.basketService = basketService;
        }
       
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            ///ویومدل را به یک نمونه از موجودیت یوزر اساین میکنیم
            User newUser = new User()
            {
                Email = model.Email,
                ///در سایت ما یوزرنیم همان ایمیل است 
                UserName = model.Email,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
            };


            ////حال با کمک یوزر منیجر اقدام به ریجستر میکنیم
            ///متد کریت یک یوزر ، و یک پسورد دریافت میکند
            var result = _userManager.CreateAsync(newUser, model.Password).Result;
            if (result.Succeeded)
            {
                ///یوزر را بر اساس نام یا ایمیل میتوان فایند کرد
                var user = _userManager.FindByEmailAsync(newUser.Email).Result;
                ///از متد پرایویت برای انتقال بسکت استفاده 
                TransferBasketForuser(user.Id);
                ///در این مرحله اقدام ساین این میکنیم
                _signInManager.SignInAsync(user, true).Wait();
                ///اگر اکانت ریجستر شد به پروفایل منتقل شود
                return RedirectToAction(nameof(Profile));
            }
            ///با کمک فوریچ ایرادات را به مدل استیت پاس میدهیم
            foreach (var item in result.Errors)
            {
                ModelState.AddModelError(item.Code,item.Description);
            }
            return View(model);
        }

        public IActionResult Profile()
        {
            return View();
        }
        /// <summary>
        /// پیش فرض وارد ریشه سایت میشه
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        public IActionResult Login(string returnUrl ="/")
        {
            return View(new LoginViewModel {
            ReturnUrl=returnUrl,

            });
        }
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //یا این کد میشود استفاده کرد چون نام کاربری همان ایمیل است
            //_userManager.FindByNameAsync(model.Email).Result;
            var user = _userManager.FindByEmailAsync(model.Email).Result;

            if (user==null)
            {
                ModelState.AddModelError("", "کاربر یافت نشد");
                return View(model);
            }
            ////اگر قبلا ساین این کرده خارج شود
            _signInManager.SignOutAsync();

            var result = _signInManager.PasswordSignInAsync(user, model.Password, model.IsPersistent, true).Result;
            if (result.Succeeded)
            {
                ///اگر لاگین موفقیت آمیز بود بسکت را نیز انتقال میدهیم
                TransferBasketForuser(user.Id);
                return Redirect(model?.ReturnUrl ?? "/");
            }
            if (result.RequiresTwoFactor)
            {
                ///
            }
            return View(model);

        }

        public IActionResult LogOut()
        {
            _signInManager.SignOutAsync();
            return RedirectToAction("Index","Home");
        }


        private void TransferBasketForuser(string userId)
        {
            ///در تمام بخش های برنامه برای درک آنکه کوکی را
            ///ما روی مرورگر کاربر ایجاد کردیم از همین نام استفاده شده است
            string cookieName = "BasketId";
            ///اگر این کوکی وجود داشت یعنی سبد خریدی موجوده
            if (Request.Cookies.ContainsKey(cookieName))
            {
                ///در این مرحله یوزر آیدی و آنینموس آیدی را داریم
                var anonymousId = Request.Cookies[cookieName];
                ///با کمک متد ترنسفر سبد را به یوزر لاگین کرده انتقال میدهیم 
                basketService.TransferBasket(anonymousId, userId);
                /// و حتما کوکی را پاک میکنیم که دو سبد برای کاربر موجود نباشد
                Response.Cookies.Delete(cookieName);
            }
        }
    }
}
