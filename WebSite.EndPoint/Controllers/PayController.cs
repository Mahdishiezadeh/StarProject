using Application.Payments;
using Dto.Payment;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebSite.EndPoint.Utilities;
using ZarinPal.Class;

namespace WebSite.EndPoint.Controllers
{
    public class PayController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly IPaymentService paymentService;
        ///یک استرینگ برای اخد کد مرچند آیدی (کد درگاه زیرن پال)
        ///داخل فایل اپ ستینگ دولوپمنت ایجاد میکنیم
        private readonly string merchendId;
        /// <summary>
        /// کلاس های پیش فرض زرین پال 
        private readonly Payment _payment;
        private readonly Authority _authority;
        private readonly Transactions _transactions;

        public PayController(IConfiguration configuration, IPaymentService paymentService)
        {
            this.configuration = configuration;
            this.paymentService = paymentService;
            ///مقداردهی مرچند آیدی
            merchendId = configuration["ZarinpalMerchendId"];
            ///زرین پال
            var expose = new Expose();
            _payment = expose.CreatePayment();
            _authority = expose.CreateAuthority();
            _transactions = expose.CreateTransactions();
        }
        ///چون پیمنت آیدی را به درگاه پرداخت میخواهیم ارسال کنیم 
        public async Task<IActionResult> Index(Guid PaymentId)
        {
            var payment = paymentService.GetPayment(PaymentId);
            if (payment==null)
            {
                return NotFound();
            }
            ///برای امنیت بیشتر بعد از پرداخت موفق یوزر آیدی کاربر را با آیدی پرداخت آن مطابقت میدهیم
            string userId = ClaimUtility.GetUserId(User);

            if (userId !=payment.Userid)
            {
                return BadRequest();
            }
            ///بعد از پرداخت موفق با کمک کال بک یو آر ال کاربر را به یکی از صفحات دلخواه بر میگردانیم
            ///وقتی پروتکل را با ریکوئست . اسکیم برابر قرار میدهیم نیازی نیست آدرس سایت را کامل بنویسیم
            ///پیمنت آدی برای دانستن پرداخت برای کدام سبد و کاربر است ارسال میشود
            string callbackUrl = Url.Action(nameof(Verify), "pay", new { payment.Id }, protocol: Request.Scheme);
            ///حال یک ریکوئست به زرین پال میدهیم و دیتی او . پیمینت را نیز یوزینگ میکنیم
            var resultZarinpalRequest = await _payment.Request(new DtoRequest()
            {
                Amount = payment.Amount,
                CallbackUrl = callbackUrl,
                Description = payment.Description,
                Email = payment.Email,
                MerchantId = merchendId,
                Mobile = payment.PhoneNumber,
            }, Payment.Mode.sandbox
              );
            ///بعد از پرداخت باید با آدرس درگاه زرین پال ،آتوریتی ریزالت زیرن پال را در یو آر ال ارسال کنیم
            return Redirect($"https://zarinpal.com/pg/StartPay/{resultZarinpalRequest.Authority}");
        }
        ///بعد از پرداخت موفق یا ناموفق بایست خرید کاربر را وریفای کنیم
        ///با کمک کوئری استرینگ هایی نظیر آتوریتی میتوانیم دیتای پرداخت را آنالیز کنیم
        ///یک جی یو آیدی دریافت میکنیم تا مشخص شود این پرداخت برای کدام پیمنت آیدی است
        public IActionResult Verify(Guid Id,string Authority)
        {
            ///در ورودی متد هم میشد دریافت کرد از کوئری ریکوئست هم میتوان
            string Status = HttpContext.Request.Query["Status"];
            ///استاتوس اگر اکی باشد قطعا خالی نیست و آتوریتری هم نباید خالی باشد
            if (Status!="" && Status.ToString().ToLower()=="ok" && Authority!="")
            {
                var payment = paymentService.GetPayment(Id);
                if (payment == null)
                {
                    return NotFound();
                }
                 #region verify
                ///چون این احتمال وجود دارد کاربر حرفه ای جی یو آیدی خودش را به دست آورده
                ///، آتوریتی و استاتوس را نیز با پستمن ارسال کند باید جلوی این هک را گرفت
                //var verification = _payment.Verification(new DtoVerification
                //{
                //    Amount = payment.Amount,
                //    Authority = Authority,
                //    MerchantId = merchendId,
                //}, Payment.Mode.sandbox).Result;
                #endregion
                ///این آدرس پیمنت وریفیکیشن است و از داکیومنت زرین پال گرفته شده
                var client = new RestClient("https://www.zarinpal.com/pg/rest/WebGate/PaymentVerification.json");
                client.Timeout = -1;
                ///یک ریکوئست ایجاد کردیم
                var request = new RestRequest(Method.POST);
                ///هدر ریکوئست
                request.AddHeader("Content-Type", "application/json");
                ///بادی ریکوئست
                request.AddParameter("application/json",
                    $"{{\"MerchantID\" :\"{merchendId}\",\"Authority\":\"{Authority}\",\"Amount\":\"{payment.Amount}\"}}",ParameterType.RequestBody);
                ///پاسخ که از یک اینت استاتوس و یک لانگ رف آیدی تشکیل شده
                var response = client.Execute(request);
                ///ریسپانس را بایست به یک دی تی او تبدیل کنیم
                VerificationPayResultDto verification = JsonConvert.DeserializeObject<VerificationPayResultDto>(response.Content);
                ///با استفاده از رست شارپ 
                if (verification.Status == 100)
                {       
                    bool verifyResult = paymentService.VerifyPayment(Id, Authority, verification.RefID);
                    if (verifyResult)
                    {
                        ///Area Customers
                        return Redirect("/customers/orders");
                    }
                    else
                    {
                        TempData["message"] = "پرداخت انجام شد اما ثبت نشد";
                        return RedirectToAction("checkout", "basket");
                    }
                }///اگر استاتوس برابر 100 نبود 
                else
                {
                    TempData["message"] = "پرداخت شما ناموفق بوده است ." +
                        " لطفا مجددا تلاش نمایید یا در صورت بروز مشکل با مدیریت سایت تماس بگیرید .";
                    ///به صفحه چک اوت فرستاده شود
                    return RedirectToAction("checkout", "basket");
                }
            }
            TempData["message"] = "پرداخت شما ناموفق بوده است .";
            return RedirectToAction("checkout", "basket");
        }
        public class VerificationPayResultDto
        {
            public int Status { get; set; }
            public long RefID { get; set; }
        }

    }
}
