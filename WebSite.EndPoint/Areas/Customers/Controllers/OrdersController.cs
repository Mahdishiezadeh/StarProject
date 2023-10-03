using Application.Orders.CustomerOrdersServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebSite.EndPoint.Utilities;

namespace WebSite.EndPoint.Areas.Customers.Controllers
{
    [Authorize]
    [Area("Customers")]
    public class OrdersController : Controller
    {
        private readonly ICustomerOrdersService customerOrdersService;

        public OrdersController(ICustomerOrdersService customerOrdersService)
        {
            this.customerOrdersService = customerOrdersService;
        }

        public IActionResult Index()
        {
            //var user = userManager.GetUserAsync(User).Result;
            //var orders = customerOrdersService.GetMyOrder(user.Id);
            //return View(orders);
            string userId=  ClaimUtility.GetUserId(User);
            var orders = customerOrdersService.GetMyOrder(userId);
            return View(orders);
        }
    }
}
