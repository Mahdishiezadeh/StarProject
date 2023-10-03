using Admin.EndPoint.ViewModels.Catalogs;
using Application.Catalogs.CatalogItems.CatalogItemServices;
using Application.Catalogs.CatalogItems.RudService;
using Application.Catalogs.CatalogTypes.CrudService;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.EndPoint.MappingProfiles
{
    public class CatalogVMMappingProfile : Profile
    {
        public CatalogVMMappingProfile()
        {
            CreateMap<CatalogTypeDto, CatalogTypeViewModel>().ReverseMap();

            CreateMap<EditCatalogItemDto, CatalogItemViewModel>().ReverseMap();
        }

    }

}