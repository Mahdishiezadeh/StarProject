using Application.Interfaces.Contexts;
using Domain.Order;
using Domain.Payments;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Payments
{
    public interface IPaymentService
    {
        PaymentOfOrderDto PayForOrder(int OrderId);
        PaymentDto GetPayment(Guid Id);
        bool VerifyPayment(Guid Id, string Authority, long RefId);
    }

    public class PaymentService : IPaymentService
    {
        ///در این سرویس نیاز است یوزر آیدنتی تی را هم فراخوانی کنیم
        ///چون به اطلاعات یوزر نیاز داریم
        private readonly IDataBaseContext context;
        private readonly IIdentityDataBaseContext identityContext;

        public PaymentService(IDataBaseContext context,IIdentityDataBaseContext identityContext)
        {
            this.context = context;
            this.identityContext = identityContext;
        }


        public PaymentDto GetPayment(Guid Id)
        {
            ///نیاز به مبلغ اردر داریم
            var payment = context.Payments
                .Include(p => p.Order)
                .ThenInclude(p => p.OrderItems)
                ///برای دیسکانت یکبار دیگر اردر و
                ///زیر دسته آن اپلاید دیسکانت را فراخوانی میکنیم
                .Include(p => p.Order)
                 .ThenInclude(p => p.AppliedDiscount)
                .SingleOrDefault(p => p.Id == Id);

            ///اطلاعات یوزر را نیاز داریم
            var user = identityContext.Users.SingleOrDefault(p => p.Id == payment.Order.UserId);


            ///درون توضیحات لیست محصولات در سبد و شماره سفارش را مینویسیم
            string description = $"پرداخت سفارش شماره {payment.OrderId} " + Environment.NewLine;
            ///محصولات را به توضیحات با کمک فوریچ اضافه میکنیم
            description += "محصولات" + Environment.NewLine;
            foreach (var item in payment.Order.OrderItems.Select(p => p.ProductName))
            {
                description += $" -{item}";
            }

            PaymentDto paymentDto = new PaymentDto
            {
                Amount = payment.Order.TotalPrice(),
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Userid = user.Id,
                Id = payment.Id,
                ///توضیحات شامل بخشی است که با نگاه به آن میتوان اطلاعات دقیقی از پرداخت را به دست آورد
                Description = description,
            };
            return paymentDto;
        }

        ///اول یافتن اردر
        ///دوم آیا برای این اردر پیمنتی ثبت شده است 
        ///اگر پیمنت نال بود آن را ایجاد میکنیم
        ///درون سرویس اردر باید یک متد ایجاد کنیم تا مبلغ را برگشت دهد 
        public PaymentOfOrderDto PayForOrder(int OrderId)
        {
            var order = context.Orders
                .Include(p => p.OrderItems)
                .Include(p => p.AppliedDiscount)
                .SingleOrDefault(p => p.Id == OrderId);

            if (order == null)
                throw new Exception("");

            var payment = context.Payments.SingleOrDefault(p => p.OrderId == order.Id);

            if(payment==null)
            {
                ///پیمنت رو از اردر میگیره و ایجاد میکنه
                payment = new Payment(order.TotalPrice(), order.Id);
                context.Payments.Add(payment);
                context.SaveChanges();
            }

            return new PaymentOfOrderDto()
            {
                Amount = payment.Amount,
                PaymentId = payment.Id,
                PaymentMethod = order.PaymentMethod,
            };

        }

        public bool VerifyPayment(Guid Id, string Authority, long RefId)
        {
            var payment = context.Payments
       .Include(p => p.Order)
       .SingleOrDefault(p => p.Id == Id);

            if (payment == null)
                throw new Exception("payment not found");
            ///در اردر انجام پیمنت را تایید میکنیم
            payment.Order.PaymentDone();
            payment.PaymentIsDone(Authority, RefId);

            context.SaveChanges();
            return true;
        }
    }
    public class PaymentOfOrderDto 
    {
        public Guid PaymentId { get; set; }
        public int Amount { get; set; }
        ///اینهریت شده از اردر 
        public PaymentMethod PaymentMethod { get; set; }

    }

    public class PaymentDto
    {
        public Guid Id { get; set; }
        public string PhoneNumber { get; set; }
        ///این فیلد در موجودیت وجود ندارد و در همین سرویس ایجاد و مقداردهی میشود
        public string Description { get; set; }
        public string Email { get; set; }
        public int Amount { get; set; }
        ///برای اینکه پرداخت توسط کدام یوزر انجام شده
        public string Userid { get; set; }
    }

}
