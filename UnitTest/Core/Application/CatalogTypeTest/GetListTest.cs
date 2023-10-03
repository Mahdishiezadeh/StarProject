using Application.Catalogs.CatalogTypes.CrudService;
using AutoMapper;
using Infrastructure.MappingProfile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTest.Builders;
using Xunit;

namespace UnitTest.Core.Application.CatalogTypeTest
{
    /// <summary>
    /// بعد اجرا این تست به خطا برخورد میکنیم ،بایست به دیتابیس کانتکست رفته و 
    ///  DataBaseContextSeed.CatalogSeed(builder);
    /// را یافت کرده و کامنت کنیم 
    /// تست را گرفته و از کامنت خارج کنیم
    /// </summary>

    public class GetListTest
    {
        [Fact]
        public void List_should_return_list_of_catalogtypes()
        {
            ///فراخوانی دیتابیس ایجاد شده (این مموری) از پوشه بیلدر
            //Arrange
            var dataBasebuilder = new DatabaseBuilder();
            var dbContext = dataBasebuilder.GetDbContext();


            ///
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new CatalogMappingProfile());
            });
            var mapper = mockMapper.CreateMapper();

            ///یک نمونه از متد سرویس کاتالوگ تایپی که میخواهیم تست کنیم 
            ///ایجاد میکنیم و به مپر ایجاد شده مپ میکنیم
            var service = new CatalogTypeService(dbContext, mapper);

            ///قبل از تست یکسری دیتا درون کاتلوگ لیست ادد میکنیم 
            service.Add(new CatalogTypeDto
            {
                Id = 1,
                Type = "t1"
            });
            service.Add(new CatalogTypeDto
            {
                Id = 2,
                Type = "t21"
            });

            ///یک نمونه از سرویس دریافت میکنیم
            var result = service.GetList(null, 1, 20);

            // Assert
            ///چون دیتا درون ریزالت ادد کردیم نمیتواند نال باشد
            Assert.NotNull(result);
            ///چون دوتا دیتا درون دیتابیس داریم اینجا چک میکنیم تعداد دیتای درون آن را
            Assert.Equal(2, result.Count);

        }
    }
}
