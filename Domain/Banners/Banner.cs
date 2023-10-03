using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Banners
{
    public class Banner
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Link { get; set; }
        ///به منظور عدم نمایش بنر
        public bool IsActive { get; set; }
        ///ترتیب الویت
        public int Priority { get; set; }
        public Position Position { get; set; }

    }

    ///توضیحات و دیسپلی نیم ها برای درک در پنل ادمین است
    public enum Position
    {
       [Display(Name ="اسلایدر")]
        Slider = 0,

        [Display(Name = "سطر اول")]
        Line_1 = 1,

        [Display(Name = "سطر دوم")]
        Line_2 = 2,

        [Display(Name = " سطر سوم")]
        Line_3 = 3,

        [Display(Name = "سطر چهارم")]
        Line_4 = 4,

        [Display(Name = "سطر پنجم")]
        Line_5 = 5,

    }
}
