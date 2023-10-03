using Application.Visitors.VisitorOnline;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebSite.EndPoint.Hubs
{
    public class OnlineVisitorHub : Hub
    {
        private readonly IIVisitorOnlineService visitorOnlineService;
        public OnlineVisitorHub(IIVisitorOnlineService visitorOnlineService)
        {
            this.visitorOnlineService = visitorOnlineService;
        }
        public override Task OnConnectedAsync()
        {
            string visitorId = Context.GetHttpContext().Request.Cookies["VisitorId"];
            ///با کمک کانتکست کانکشن آیدی را به عنوان کلاینت آیدی پاس میدهیم
            visitorOnlineService.ConnectUser(visitorId);
            ///اخذ تعداد
            var coun = visitorOnlineService.GetCount();
            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception exception)
        {
            string visitorId = Context.GetHttpContext().Request.Cookies["VisitorId"];
            visitorOnlineService.DisConnectUser(visitorId);
            var coun = visitorOnlineService.GetCount();
            return base.OnDisconnectedAsync(exception);

        }
    }
}
