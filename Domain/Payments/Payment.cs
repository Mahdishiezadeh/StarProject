using Domain.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Payments
{
    [Auditable]
    public class Payment
    {
        /// آیدی منحصر به فرد پرداخت
        public Guid Id { get; set; }
        ///مبلغ پرداختی
        public int Amount { get; private set; }
        ///این فیلد برای آن است اگر پرداخت کننده در درگاه پرداخت را کنسل کرد
        public bool IsPay { get; private set; } = false;
        ///تاریخ پرداخت را بایست ذخیره کنیم 
        public DateTime? DatePay { get; private set; } = null;
        ///دو مقدار از طرف بانک ارسال میشود و بایست دریافت کنیم
        public string Authority { get; private set; }
        public long RefId { get; private set; } = 0;
        ///اگر در سیستم خود پرداخت از کیف پول داشته باشیم میتوان اردر را نال ابیل قرار دهیم
        ///چون پوشه اردر دقیقا هم نام موجودیت اردر داره ما بایست اینطور از اردر نمونه ایجاد کنیم
        public Order.Order Order { get; private set; }
        /// ایجاد ریلیشن با اردر
        public int OrderId { get; private set; }
        public Payment(int amount,int orderId)
        {
            Amount = amount;
            OrderId = orderId;
        }
        ///اگر موارد دریافتی از بانک مورد تایید بود پرداخت را ترو و زمان حال را ثبت میکنیم
        public void PaymentIsDone(string authority, long refId)
        {
            IsPay = true;
            DatePay = DateTime.Now;
            Authority = authority;
            RefId = refId;
        }

    }
}
