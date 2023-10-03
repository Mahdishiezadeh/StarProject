﻿using Domain.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTest.Builders;
using Xunit;

namespace UnitTest.Core.Entities.OrderTest
{
    public class OrderStatusTest
    {
        ///تست برای بررسی اردر بعد از تحویل استاتوسش به دلیور تغییر میکند یا خیر ؟ 
        ///برای ایجاد یک اردر فیک به پوشه بیلدر رفته و اردر را ایجاد میکنیم 
        [Fact]
        public void When_order_is_delivered_OrderStatus_changes_to_Delivered()
        {
            var builder = new OrderBuilder();

            var order = builder.CreateOrderWithDefaultValues();

            order.OrderDelivered();

            Assert.Equal(OrderStatus.Delivered, order.OrderStatus);
        }
    }

}
