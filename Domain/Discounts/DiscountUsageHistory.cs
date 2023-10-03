using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Discounts
{
    public class DiscountUsageHistory
    {
        public int Id { get; set; }
        public DateTime CreateOn { get; set; }
        public virtual Discount Discount { get; set; }
        public int DiscountId { get; set; }
        ///چون نام فولدر انتیتی با نام آن یکسان است باید اردر دات اردر بنویسیم
        public virtual Order.Order Order { get; set; }
        public int OrderId { get; set; }
    }
}
