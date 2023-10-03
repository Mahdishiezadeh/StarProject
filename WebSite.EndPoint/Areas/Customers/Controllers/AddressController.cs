using Application.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebSite.EndPoint.Utilities;

namespace WebSite.EndPoint.Areas.Customers.Controllers
{
    /// <summary>
    /// آتورایز را هم قرار میدهیم تا فقط کسانی که لاگین کردند 
    /// بتوانند از این کنترلر استفاده کنند
    /// </summary>
    [Authorize]
    [Area("Customers")]
    public class AddressController : Controller
    {
        private readonly IUserAddressService userAddressService;

        public AddressController(IUserAddressService userAddressService)
        {
            this.userAddressService = userAddressService;
        }
        public IActionResult Index()
        {
            
            var address = userAddressService.GetAddress(ClaimUtility.GetUserId(User));
            
            return View(address);
        }

       
        public IActionResult AddNewAddress()
        {
            return View(new AddUserAddressDto());
        }
        [HttpPost]
        public IActionResult AddNewAddress(AddUserAddressDto addressDto)
        {
            string userId = ClaimUtility.GetUserId(User);
            addressDto.UserId = userId;
            userAddressService.AddnewAddress(addressDto);
            return RedirectToAction(nameof(Index));
        }
        public IActionResult EditAddress()
        {
            return View(new EditUserAddressDto());
        }
        [HttpPost]
        public IActionResult EditAddress(EditUserAddressDto addressDto)
        {
            ///آیدی یوزر را اخذ میکنیم 
            string userId = ClaimUtility.GetUserId(User);
            addressDto.UserId = userId;
            userAddressService.EditAddress(addressDto);
            return RedirectToAction(nameof(Index));
        }
    }
}
