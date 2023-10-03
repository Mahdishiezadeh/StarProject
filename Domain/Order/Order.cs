using Domain.Attributes;
using Domain.Catalogs;
using Domain.Discounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Order
{
    [Auditable]
    public class Order
    {
        /// در ریچ انتیتی تنها پراپرتی که میتواند از بیرون ست شود آیدی است
        public int Id { get; set; }

        public string UserId { get; private set; }
        public DateTime OrderDate { get; private set; } = DateTime.Now;
        public Address Address { get; private set; }
        public PaymentMethod PaymentMethod { get; private set; }
        public PaymentStatus PaymentStatus { get; private set; }
        public OrderStatus OrderStatus { get; private set; }
        /// در همین بخش بایست نیو شود چون در لایه بالاتر امکان ایجاد لیست نیست
        private readonly List<OrderItem> _orderItems = new List<OrderItem>();
        /// به طور ReadOnly
        /// تعریف میکنیم تا برنامه نویسان لایه بالاتر به آن دسترسی تغییر نداشته باشند
        public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

        public decimal DiscountAmount { get; private set; }
        public Discount AppliedDiscount { get; private set; }
        public int? AppliedDiscountId { get; private set; }


        public Order(string userId, Address address,
            List<OrderItem> orderItems, PaymentMethod paymentMethod
            ,Discount discount)
        {
            UserId = userId;
            Address = address;
            ///داخل اردر آیتم پرایویت باید ست شود
            _orderItems = orderItems;
            PaymentMethod = paymentMethod;

            if (discount!=null)
            {
                ApplyDiscountCode(discount);
            }
        }
        /// 4 EFCore
        public Order()
        {

        }

        /// زمانی که پرداخت با موفقیت انجام شد استاتوس تغییر کنه
        public void PaymentDone()
        {
            PaymentStatus = PaymentStatus.Paid;
        }

        /// کالا تحویل داده شد
        public void OrderDelivered()
        {
            OrderStatus = OrderStatus.Delivered;
        }

        /// ثبت مرجوعی کالا
        public void OrderReturned()
        {
            OrderStatus = OrderStatus.Returned;
        }

        /// لغو سفارش
        public void OrderCancelled()
        {
            OrderStatus = OrderStatus.Cancelled;
        }

        public int TotalPrice()
        {
            ///قیمت بدون تخفیف
            int totalPrice = _orderItems.Sum(p => p.UnitPrice * p.Units);

            ///اگر تخفیف داشت 
            if (AppliedDiscount!=null)
            {
                totalPrice -= AppliedDiscount.GetDiscountAmount(totalPrice);
            }
            return totalPrice;

        }

        public int TotalPriceWithOutDiescount()
        {
            int totalPrice = _orderItems.Sum(p => p.UnitPrice * p.Units);
            return totalPrice;
        }

        public void ApplyDiscountCode(Discount discount)
        {
            this.AppliedDiscount = discount;
            this.AppliedDiscountId = discount.Id;
            this.DiscountAmount = discount.GetDiscountAmount(TotalPrice());
        }

    }

    /// آیتم های داخل یک اردر 
    [Auditable]
    public class OrderItem
    {
        public int Id { get; set; }

        ///برای ادیت سرویس زیر یک ریلیشن با CatalogItem میزنیم
        ///IGetCatalogItemPLPService.cs
        public CatalogItem CatalogItem { get; set; }

        public int CatalogItemId { get; private set; }
        public string ProductName { get; private set; }
        public string PictureUri { get; private set; }

        /// قیمت هر واحد از کالا
        public int UnitPrice { get; private set; }

        /// چه تعداد کاربر سفارش داده
        public int Units { get; private set; }
        public OrderItem(int catalogItemId, string productName,
            string pictureUri, int unitPrice, int units)
        {
            CatalogItemId = catalogItemId;
            ProductName = productName;
            PictureUri = pictureUri;
            UnitPrice = unitPrice;
            Units = units;
        }
        //ef core
        public OrderItem()
        {

        }
    }
    ///یک موجودیت برای نگهداری اطلاعات و نه ایجاد تیبل در دیتابیس ایجاد میکنیم
    public class Address
    {
        public string State { get; private set; }
        public string City { get; private set; }
        public string ZipCode { get; private set; }
        public string PostalAddress { get; private set; }
        public string ReciverName { get; private set; }
        public Address(string city, string state, string zipCode, string postalAddress)
        {
            this.City = city;
            State = state;
            ZipCode = zipCode;
            PostalAddress = postalAddress;
        }
    }

    ///نحوه پرداخت چطور انجام میشود و بایست یک 
    ///Enum 4 Payment Method
    ///ایجاد شود 
    public enum PaymentMethod
    {
        /// پرداخت آنلاین
        OnlinePaymnt = 0,
        /// پرداخت در محل
        PaymentOnTheSpot = 1,
    }
    public enum PaymentStatus
    {
        /// منتظر پرداخت
        WaitingForPayment = 0,
        /// پرداخت انجام شد
        Paid = 1,
    }
    public enum OrderStatus
    {
        /// در حال پردازش
        Processing = 0,
        /// تحویل داده شد
        Delivered = 1,
        /// مرجوعی
        Returned = 2,
        /// لغو شد
        Cancelled = 3,
    }
   
}
