using Application.Catalogs.CatalogItems.AddNewCatalogItem;
using Application.Catalogs.CatalogItems.CatalogItemServices;
using Application.Catalogs.CatalogItems.RudService;
using Application.Catalogs.CatalogTypes.CrudService;
using Application.Catalogs.GetMenuItem;
using AutoMapper;
using Domain.Catalogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.MappingProfile
{
    public class CatalogMappingProfile:Profile
    {
        public CatalogMappingProfile()
        {
            CreateMap<CatalogType, CatalogTypeDto>().ReverseMap();
            CreateMap<CatalogType, CatalogTypeListDto>().ForMember
                (dest => dest.SubTypeCount, option => option.MapFrom(src => src.SubType.Count));
            ///این مپینگ به این علت به این صورت انجام شد 
            ///چون در موجودیت و دی تی او نام پراپرتی ها یکسان نیست 
            CreateMap<CatalogType, MenuItemDto>()
                ///نام رو از اس آر سی که همان دیتا دیتابیس است بخوان
                .ForMember(dest => dest.Name, opt =>
                 opt.MapFrom(src => src.Type))

                .ForMember(dest => dest.ParentId, opt =>
                 opt.MapFrom(src => src.ParentCatalogTypeId))

                .ForMember(dest => dest.SubMenu, opt =>
                opt.MapFrom(src => src.SubType));

            CreateMap<CatalogItemImage, AddNewCatalogItemImage_Dto>().ReverseMap();
            CreateMap<CatalogItemFeature, AddNewCatalogItemFeature_dto>().ReverseMap();

            CreateMap<CatalogItem, AddNewCatalogItemDto>()
                .ForMember(dest => dest.Features, opt =>
             opt.MapFrom(src => src.CatalogItemFeatures))
                .ForMember(dest => dest.Images, opt =>
                   opt.MapFrom(src => src.CatalogItemImages)).ReverseMap();

            CreateMap<CatalogBrand, CatalogBrandDto>().ReverseMap();
            CreateMap<CatalogType, CatalogTypeDto>().ReverseMap();

            //////////////////////Edit//////////////
            CreateMap<CatalogItem, EditCatalogItemDto>()
               .ForMember(dest => dest.Features, opt =>
            opt.MapFrom(src => src.CatalogItemFeatures))
               .ForMember(dest => dest.Images, opt =>
                  opt.MapFrom(src => src.CatalogItemImages)).ReverseMap();

            CreateMap<CatalogItemImage, EditCatalogItemImage_Dto>().ReverseMap();
            CreateMap<CatalogItemFeature, EditCatalogItemFeature_dto>().ReverseMap();

          
        }
    }
}
