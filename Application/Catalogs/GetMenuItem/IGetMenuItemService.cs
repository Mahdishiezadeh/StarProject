using Application.Interfaces.Contexts;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Catalogs.GetMenuItem
{
    /// <summary>
    /// بر اساس دی تی او ها یک خروجی مشخص میشود
    /// </summary>
    public interface IGetMenuItemService
    {
        List<MenuItemDto> Execute();
    }

    public class GetMenuItemService : IGetMenuItemService
    {
        private readonly IDataBaseContext context;
        private readonly IMapper mapper;

        public GetMenuItemService(IDataBaseContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }
        public List<MenuItemDto> Execute()
        {
            var catalogType = context.CatalogTypes.Include(p => p.ParentCatalogType).ToList();
            var data = mapper.Map<List<MenuItemDto>>(catalogType);
            return data;
        }
    }

    /// <summary>
    /// در گام اول دی تی او مشخص میشود
    /// </summary>
    public class MenuItemDto
    {
        public int Id { get; set; }
        /// <summary>
        /// برای لینک دادن از نام استفاده میکنیم
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// چون به صورت چایلد نمایش میدهیم 
        /// </summary>
        public int? ParentId { get; set; }
        /// <summary>
        /// یک مورد از همین کلاس که فرزندان را درون آن قرار دهیم 
        /// </summary>
        public List<MenuItemDto> SubMenu { get; set; }
    }
}
