using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Catalogs.CatalogItems.UriComposer
{

    /// لینک دادن به عکس ها (آدرس استرینگ ذخیره شده عکس روی استاتیک اندپوینت )در
    /// وبسایت اندپوینت اگر عکسی را بخواهیم نمایش دهیم این سرویس کاربرد دارد 
    public interface IUriComposerService
    {
        string ComposeImageUri(string src);
    }
    public class UriComposerService : IUriComposerService
    {
        public string ComposeImageUri(string src)
        {
            ///آدرس سایت به اضافه اس آر سی
            ///آدرس سایت را روی اندپوینت استاتیک فایل راست کلیک پراپرتیس و از بخش دیباگ پایین روی کپی کلیک میکنیم
            return "https://localhost:44367/" + src.Replace("\\", "//");
        }
    }
}
