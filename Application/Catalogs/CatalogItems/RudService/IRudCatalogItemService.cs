using Application.Dtos;
using Application.Interfaces.Contexts;
using AutoMapper;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Catalogs.CatalogItems.RudService
{
    public interface IRudCatalogItemService
    {
        BaseDto<EditCatalogItemDto> Edit(EditCatalogItemDto catalogItemDto);
        BaseDto<int> Remove(int Id);

        BaseDto<EditCatalogItemDto> FindById(int Id);
    }

    public class RudCatalogItemService : IRudCatalogItemService
    {
        private readonly IDataBaseContext context;
        private readonly IMapper mapper;

        public RudCatalogItemService(IDataBaseContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public BaseDto<EditCatalogItemDto> Edit(EditCatalogItemDto catalogItemDto)
        {
            ///Find
            var model = context.CatalogItems.SingleOrDefault(p => p.Id == catalogItemDto.Id);
            ///Map Dto To Finding model
            mapper.Map(catalogItemDto, model);
            context.SaveChanges();
            return new BaseDto<EditCatalogItemDto>
          (true,
          new List<string> { $"{model.Name } با موفقیت ویرایش شد" },
          mapper.Map<EditCatalogItemDto>(model)
          );
        }

        public BaseDto<int> Remove(int Id)
        {
            var catalogItem = context.CatalogItems.Find(Id);
            context.CatalogItems.Remove(catalogItem);
            context.SaveChanges();
            return new BaseDto<int>(true, new List<string> { $"ایتم با موفقیت حذف شد" },catalogItem.Id);
        }

        public BaseDto<EditCatalogItemDto> FindById(int Id)
        {

            var data = context.CatalogItems.Find(Id);
            var result = mapper.Map<EditCatalogItemDto>(data);
            return new BaseDto<EditCatalogItemDto>
                (true, new List<string> { $"آیتم با موفقیت یافت شد" }, result);
        }
    }

    /// <summary>
    /// دی تی او ها از 
    /// IAddNewCatalogItemService
    /// عینا کپی شده است
    /// </summary>
    public class EditCatalogItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public int CatalogTypeId { get; set; }
        public int CatalogBrandId { get; set; }
        public int AvailableStock { get; set; }
        public int RestockThreshold { get; set; }
        public int MaxStockThreshold { get; set; }
        public List<EditCatalogItemFeature_dto> Features { get; set; }
        public List<EditCatalogItemImage_Dto> Images { get; set; }

    }
    //public class EditCatalogItemDtoValidator : AbstractValidator<EditCatalogItemDto>
    //{
    //    public EditCatalogItemDtoValidator()
    //    {
    //        RuleFor(p => p.Name).NotNull().WithMessage("نام کاتالوگ اجباری است");
    //        RuleFor(x => x.Name).Length(2, 100);
    //        RuleFor(x => x.Description).NotNull().WithMessage("توضیحات نمی تواند خالی باشد");
    //        RuleFor(x => x.AvailableStock).InclusiveBetween(0, int.MaxValue);
    //        RuleFor(x => x.Price).InclusiveBetween(0, int.MaxValue);
    //        RuleFor(x => x.Price).NotNull();
    //    }
    //}

    public class EditCatalogItemFeature_dto
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string Group { get; set; }
    }

    public class EditCatalogItemImage_Dto
    {
        public string Src { get; set; }
    }
}
