using Application.Dtos;

using Application.Interfaces.Contexts;
using AutoMapper;
using Common;
using Domain.Catalogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Catalogs.CatalogTypes.CrudService
{
    public interface ICatalogTypeService
    {
        BaseDto<CatalogTypeDto> Add(CatalogTypeDto catalogType);
        BaseDto Remove(int Id);
        BaseDto<CatalogTypeDto> Edit(CatalogTypeDto catalogType);
        BaseDto<CatalogTypeDto> FindById(int Id);
        PaginatedItemsDto<CatalogTypeListDto> GetList(int? parentId, int page, int pageSize);
    }

    public class CatalogTypeService : ICatalogTypeService
    {
        private readonly IDataBaseContext context;
        private readonly IMapper mapper;

        public CatalogTypeService(IDataBaseContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }
        public BaseDto<CatalogTypeDto> Add(CatalogTypeDto catalogType)
        {
            ///مپ موجودیت به دی تی او
            var model = mapper.Map<CatalogType>(catalogType);
            context.CatalogTypes.Add(model);
            context.SaveChanges();
            return new BaseDto<CatalogTypeDto>(true,
                 new List<string> { $"تایپ {model.Type}  با موفقیت در سیستم ثبت شد" },
                mapper.Map<CatalogTypeDto>(model)
                );
        }

        public BaseDto<CatalogTypeDto> Edit(CatalogTypeDto catalogType)
        {
            ///Find
            var model = context.CatalogTypes.SingleOrDefault(p => p.Id == catalogType.Id);
            ///Edit **** source Enter By User
            mapper.Map(catalogType, model);
            context.SaveChanges();
            ///ارسال بیس دی تی او 
            return new BaseDto<CatalogTypeDto>(true,
               new List<string> { $"تایپ {model.Type} با موفقیت ویرایش شد" },
                 mapper.Map<CatalogTypeDto>(model)
                 );
        }

        public BaseDto<CatalogTypeDto> FindById(int Id)
        {
            var data = context.CatalogTypes.Find(Id);
            var result = mapper.Map<CatalogTypeDto>(data);
            return new BaseDto<CatalogTypeDto>(
                true,
                null,
                result
                );
        }

        public PaginatedItemsDto<CatalogTypeListDto> GetList(int? parentId, int page, int pageSize)
        {
            int totalCount = 0;
            var model = context.CatalogTypes
                .Where(p=>p.ParentCatalogTypeId==parentId)
                .AsQueryable().PagedResult(page,pageSize,out totalCount);
            /// به لایه اینفراستراکتر رفته و پوشه مپینگ پروفایل 
            /// CatalogMappingProfile
            /// کدزیر را اضافه میکنیم
            /// CreateMap<CatalogType, CatalogTypeListDto>().ForMember(dest => dest.SubTypeCount, option => option.MapFrom(src => src.SubType.Count));
            var result = mapper.ProjectTo<CatalogTypeListDto>(model).ToList();

            return new PaginatedItemsDto<CatalogTypeListDto>(page, pageSize, totalCount, result);
        }

        public BaseDto Remove(int Id)
        {
            var catalogType = context.CatalogTypes.Find(Id);
            context.CatalogTypes.Remove(catalogType);
            context.SaveChanges();
            return new BaseDto
            (
             true,
             new List<string> { $"ایتم با موفقیت حذف شد" }
             );


        }
    }

    /// <summary>
    /// ابتدا دی تی او را مشخص میکنیم
    /// سپس به موجودیت رفته و ریلیشن به خودش ایجاد مکینیم
    ///  public int? ParentCatalogTypeId { get; set; }
    ///  public CatalogType ParentCatalogType { get; set; }
    ///  public ICollection<CatalogType> SubType { get; set; }
    ///  اضافه میشود
    /// </summary>
    public class CatalogTypeDto
    {
        public int Id { get; set; }
        public string Type { get; set; }
        /// <summary>
        /// چند والد دارد
        /// </summary>
        public int? ParentCatalogTypeId { get; set; }
    }
    public class CatalogTypeListDto
    {
        public int Id { get; set; }
        public string Type { get; set; }
        /// <summary>
        /// چند فرزند دارد
        /// </summary>
        public int SubTypeCount { get; set; }
    }
}
