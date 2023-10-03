using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions
{
    public class NotFoundException : Exception
    {
        ///کدام موجودیت با کدام کلید یا سرچ کی به دست نیامده است
        public NotFoundException(string entityName, object key)
            ///مسیج آن هم پیام زیر باشد
             : base($"Entity  {entityName} with key {key} was not found.")
        {

        }
    }
}
