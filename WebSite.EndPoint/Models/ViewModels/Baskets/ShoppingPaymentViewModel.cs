using Application.BasketsService;
using Application.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebSite.EndPoint.Models.ViewModels.Baskets
{
    /// <summary>
    /// اطلاعات بسکت و آدرس ها رو بایست بفرستیم
    /// </summary>
    public class ShoppingPaymentViewModel
    {
        public BasketDto Basket {get;set;}
        public List<UserAddressDto> UserAddresses { get; set; }
    }
}
