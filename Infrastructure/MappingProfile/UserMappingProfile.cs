using Application.Users;
using AutoMapper;
using Domain.Order;
using Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.MappingProfile
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {

            //CreateMap<UserAddress, EditUserAddressDto>();
            //CreateMap<AddUserAddressDto, UserAddressDto>();
            //CreateMap<UserAddress, AddUserAddressDto>();

            CreateMap<UserAddress, UserAddressDto>();
            CreateMap<AddUserAddressDto, UserAddress>();
            CreateMap<UserAddress, Address>();
          
        }
    }
}
