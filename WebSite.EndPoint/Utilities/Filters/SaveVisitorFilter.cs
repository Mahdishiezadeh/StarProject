using Application.Visitors.SaveVisitorInfo;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UAParser;

namespace WebSite.EndPoint.Utilities.Filters
{
    public class SaveVisitorFilter : IActionFilter
    {
        private readonly ISaveVisitorInfoService _saveVisitorInfoService;
        public SaveVisitorFilter(ISaveVisitorInfoService saveVisitorInfoService)
        {
            _saveVisitorInfoService = saveVisitorInfoService;
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {

        }
        public void OnActionExecuting(ActionExecutingContext context)
        {
            ///این دستور  روی لوکال هاست آیپی به ما نمیدهد
            string ip = context.HttpContext.Request.HttpContext.Connection.RemoteIpAddress.ToString();
            ///این دستور برای اخد اکشن نیم است 
            var actionName = ((Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)context.ActionDescriptor).ActionName;
            ///اخذ نام کنترلر که به آن درخواست ارسال شده
            var controllerName = ((Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)context.ActionDescriptor).ControllerName;
            ///اطلاعات ورژن سیستم عامل و مرورگر از طریق مرورگر به یوزر ایجنت ارسال میشود
            ///واکشی اطلاعات یوزر ایجنت سخت است پس از پکیج
            ///UAParser
            ///استفاده میکنیم 
            var userAgent = context.HttpContext.Request.Headers["User-Agent"];
            ///بعد از نصب پکیج متد دیفالت پارسر را فراخوانی میکنیم
            var uaParser = Parser.GetDefault();
            ///اطلاعات کلاینت در متغییری از جنس اینفو ذخیره میشود
            ClientInfo clientInfo = uaParser.Parse(userAgent);
            ///لینکی که از طریق آن وارد وبسایت جاری شده
            var referer = context.HttpContext.Request.Headers["Referer"].ToString();
            ///یو آر ال جاری
            var currentUrl = context.HttpContext.Request.Path;
            ///برای اخذ متد و پروتکل نیاز به ریکوئست داریم
            var request = context.HttpContext.Request;
            ///کوکی کاربر را چک و ویزیتور آیدی را بررسی میکنیم 
            string visitorId = context.HttpContext.Request.Cookies["VisitorId"];
           

            _saveVisitorInfoService.Execute(new RequestSaveVisitorInfoDto
            {
                Browser=new VisitorVersionDto
                {
                    ///یو آ اطلاعات مرورگر است
                    Family=clientInfo.UA.Family,
                    ///سه نوع ورژن داریم که با نقطه از هم جدا میشوند
                    Version=$"{clientInfo.UA.Major}.{clientInfo.UA.Minor}.{clientInfo.UA.Patch}",
                },
                Device=new DeviceDto
                {
                    Family=clientInfo.Device.Family,
                    Brand=clientInfo.Device.Brand,
                    ///مشخص میکنه ربات یا بازدید کننده واقعی
                    IsSpider=clientInfo.Device.IsSpider,
                    Model=clientInfo.Device.Model,
                },
                OperationSystem=new VisitorVersionDto
                {
                    Family=clientInfo.OS.Family,
                    Version=$"{clientInfo.OS.Major}.{clientInfo.OS.Minor}.{clientInfo.OS.Patch}",
                },
                CurrentLink=currentUrl,
                Ip=ip,
                ReferrerLink=referer,
                Method=request.Method,
                Protocol=request.Protocol,
                ///همان آدرس کنترلر و اکشن متد است
                PhysicalPath=$"{controllerName}/{actionName}",
                VisitorId = visitorId,
                Time = DateTime.Now,
            });
        }
    }
}
