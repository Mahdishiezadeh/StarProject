using Application.Dtos;
using Application.Interfaces.Contexts;
using AutoMapper;
using Domain.Catalogs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Catalogs.CatalogItems.AddNewCatalogItem
{
    public interface IAddNewCatalogItemService
    {

        /// از جنس بیس دی تی او و آیدی کاتالوگ جدید را بازگشت میدیم
        BaseDto<int> Execute(AddNewCatalogItemDto request);
    }
    public class AddNewCatalogItemService : IAddNewCatalogItemService
    {
        private readonly IDataBaseContext context;
        /// <summary>
        /// هر جا از مپر کمک گرفتیم بایست قبل از آن به لایه
        /// ایتفراستراکتر رفته و پروفایل مپ را کامل کنیم 
        /// به ازائ هر دی تی او یک مپینگ 
        /// </summary>
        private readonly IMapper mapper;

        public AddNewCatalogItemService(IDataBaseContext context,IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }
        public BaseDto<int> Execute(AddNewCatalogItemDto request)
        {
            var catalogItem = mapper.Map<CatalogItem>(request);
            context.CatalogItems.Add(catalogItem);
            context.SaveChanges();
            return new BaseDto<int>(true, new List<string> { "با موفقیت ثبت شد" }, catalogItem.Id);
        }
    }

    public class AddNewCatalogItemDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public int CatalogTypeId { get; set; }
        public int CatalogBrandId { get; set; }
        public int AvailableStock { get; set; }
        public int RestockThreshold { get; set; }
        public int MaxStockThreshold { get; set; }
        public List<AddNewCatalogItemFeature_dto> Features { get; set; }
        public List<AddNewCatalogItemImage_Dto> Images { get; set; }

    }

    ///ولیدیتور برای ادد کردن کاتالوگ (محصول) در پنل ادمین
    public class AddNewCatalogItemDtoValidator : AbstractValidator<AddNewCatalogItemDto>
    {
        public AddNewCatalogItemDtoValidator()
        {
            RuleFor(p => p.Name).NotNull().WithMessage("نام کاتالوگ اجباری است");
            RuleFor(x => x.Name).Length(2, 100);
            RuleFor(x => x.Description).NotNull().WithMessage("توضیحات نمی تواند خالی باشد");
            RuleFor(x => x.AvailableStock).InclusiveBetween(0, int.MaxValue);
            RuleFor(x => x.Price).InclusiveBetween(0, int.MaxValue);
            RuleFor(x => x.Price).NotNull();
        }
    }

    /// برای آپشن های اضافه یک کاتالوگ
    public class AddNewCatalogItemFeature_dto
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string Group { get; set; }
    }

    /// عکس های کاتالوگ
    public class AddNewCatalogItemImage_Dto
    {
        public string Src { get; set; }
    }
}
