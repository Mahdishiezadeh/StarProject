using Application.Catalogs.CatalogItems.UriComposer;
using Application.Discounts;
using Application.Exceptions;
using Application.Interfaces.Contexts;
using AutoMapper;
using Domain.Order;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Orders
{
    public interface IOrderService
    {
        int CreateOrder(int BasketId, int UserAddressId, PaymentMethod paymentMethod);
    }
    public class OrderService : IOrderService
    {
        private readonly IDataBaseContext context;
        private readonly IMapper mapper;
        private readonly IUriComposerService uriComposerService;
        private readonly IDiscountHistoryService discountHistoryService;
        public OrderService(IDataBaseContext context,IMapper mapper,
            IUriComposerService uriComposerService,
            IDiscountHistoryService discountHistoryService)
        {
            this.context = context;
            this.mapper = mapper;
            this.uriComposerService = uriComposerService;
            this.discountHistoryService = discountHistoryService;
        }
        public int CreateOrder(int BasketId, int UserAddressId, PaymentMethod paymentMethod)
        {
            ///قبل از هر کاری بایست سبد خرید را با آیتم هایش به دست بیاوریم
            var basket = context.Baskets
                .Include(p => p.Items)
                ///دیسکانت بسکت رو اخذ میکنیم
                .Include(p=>p.AppliedDiscount)
                .SingleOrDefault(p => p.Id == BasketId);

            if (basket==null)
            {
                throw new NotFoundException(nameof(basket),BasketId);
            }


            ///اکنون آیتم های سبد خرید را بایست پیدا کنیم و دیتای آن را درون سفارش ثبت کنیم 
            ///در بالا آیتم ها را اینکلود کردیم پس با کمک دستور سلکت آنها را به آرایه عددی تبدیل میکنیم
            ///و از آرایه عددی برای دریافت کاتالوگ آیتم ها کمک بگیریم
            int[] Ids = basket.Items.Select(p => p.CatalogItemId).ToArray();

            ///اکنون کاتالوگ (پروداکت ) های این آرایه (سبد خرید) را فایند میکنیم
            var catalogItems = context.CatalogItems
                .Include(p=>p.CatalogItemImages)
                .Where(p => Ids.Contains(p.Id));

            ///حال باید یک لیست از اردر آیتم ها را بر اساس سبد خرید ایجاد کنیم و آیتم ها را برگشت دهد
            var orderItems = basket.Items.Select(basketItem =>
            {
                ///کاتالوگ آیتمی را پیدا میکنیم که با بسکت آیتم برابر است
                var catalogItem = catalogItems.First(p => p.Id == basketItem.CatalogItemId);
                
                ///اردر آیتم ایجاد و برگشت میدهیم 
                var orderItem = new OrderItem(catalogItem.Id, catalogItem.Name,
                    uriComposerService.ComposeImageUri(catalogItem?.CatalogItemImages?.FirstOrDefault()?.Src??""),
                    catalogItem.Price, basketItem.Quantity);
                return orderItem;
            }).ToList();

            ///حال بایست آدرس کاربر را نیز پیدا کنیم 
            var userAddress = context.UserAddresses.SingleOrDefault(p => p.Id == UserAddressId);

            ///یوزر آدرس را به آدرس مپ کنیم و مپر آن را در یوزر مپیگ پروفایل وارد میکینیم
            var address = mapper.Map<Address>(userAddress);

            ///اکنون بایست اردر را ایجاد کنیم
            ///در اینجا یوزر آیدی با بایر آیدی برابر است
            ///پیمنت متد را از کنترلر اخذ میکنیم 
            var order = new Order(basket.BuyerId, address, orderItems, paymentMethod, basket.AppliedDiscount);
            context.Orders.Add(order);
            ///بسکت را حذف میکنیم تا کاربر دیگر سبد خریدی نداشته باشد
            context.Baskets.Remove(basket);
            context.SaveChanges();
            if (basket.AppliedDiscount != null)
            {
                discountHistoryService.InsertDiscountUsageHistory(basket.Id, order.Id);
            }
            return order.Id;
        }
    }
}
