using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Attributes
{
    /// <summary>
    /// برای اتچ کردن یکسری قابلیت نظیر زمان ایجاد
    /// یا آپدیت یا پاک شدن موجودین های پروژه است 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class AuditableAttribute :Attribute
    {

    }
}
