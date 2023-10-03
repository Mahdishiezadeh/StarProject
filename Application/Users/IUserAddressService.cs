using Application.Interfaces.Contexts;
using AutoMapper;
using Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users
{
    public interface IUserAddressService
    {
        List<UserAddressDto> GetAddress(string UserId);

        void AddnewAddress(AddUserAddressDto address);

        List<EditUserAddressDto> EditAddress(EditUserAddressDto addressDto);
    }
    public class UserAddressService : IUserAddressService
    {
        private readonly IDataBaseContext context;
        private readonly IMapper mapper;

        public UserAddressService(IDataBaseContext Context, IMapper mapper)
        {
            this.context = Context;
            this.mapper = mapper;
        }

        public void AddnewAddress(AddUserAddressDto address)
        {
            ///دیتا دریافتی رو مستقیم از انتیتی ، مپ میکنیم به دی تی او
            var data = mapper.Map<UserAddress>(address);
            ///چون فرایند ادد کردن است به دی بی کانتکست ادد میکنیم
            context.UserAddresses.Add(data);
            context.SaveChanges();

        }

        public List<EditUserAddressDto> EditAddress(EditUserAddressDto addressDto)
        {
            var result = context.UserAddresses.Where(p => p.UserId == addressDto.UserId).ToList();
            var data = mapper.Map<List<EditUserAddressDto>>(result);
            context.SaveChanges();
            return data;
        }

        public List<UserAddressDto> GetAddress(string UserId)
        {
            ///دیتا رو از انتیتی میگیریم چون شرط داره و فرآیند گت کردن است
            var address = context.UserAddresses.Where(p => p.UserId == UserId);
            ///مپ میکنیم به دی تی او
            var data = mapper.Map<List<UserAddressDto>>(address);
            ///بازگشت میدیم
            return data;
        }

    }
    /// <summary>
    ///این دی تی او برای بازگشت آدرس های کاربر است
    /// </summary>
    public class UserAddressDto
    {
        public int Id { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string PostalAddress { get; set; }
        public string UserId { get; set; }
        public string ReciverName { get; set; }
    }

    ///این دی تی او برای افزودن آدرس به آدرسهای کاربر است
    public class AddUserAddressDto
    {
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string PostalAddress { get; set; }
        public string ReciverName { get; private set; }
        public string UserId { get; set; }
    }
    public class EditUserAddressDto
    {
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string PostalAddress { get; set; }
        public string ReciverName { get; private set; }
        public string UserId { get; set; }
    }
}
