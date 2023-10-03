using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Visitors
{
    /// <summary>
    /// ورژن سیستم عامل و نام مرورگر
    /// </summary>
    public class VisitorVersion
    {
        public string Family { get; set; }
        public string Version { get; set; }
    }
}
