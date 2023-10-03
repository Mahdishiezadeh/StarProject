using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos
{
    public class BaseDto<T>
    {
        /// <summary>
        /// زمانی که دیتا داریم
        /// </summary>
        /// <param name="IsSuccess"></param>
        /// <param name="Message"></param>
        /// <param name="Data"></param>
        public BaseDto(bool IsSuccess, List<string> Message, T Data)
        {
            this.IsSuccess = IsSuccess;
            this.Message = Message;
            this.Data = Data;
        }

        public T Data { get; private set; }
        public List<string> Message { get; private set; }
        public bool IsSuccess { get; private set; }

    }

    /// <summary>
    /// زمانی که دارای دیتا نیست
    /// </summary>
    public class BaseDto
    {
        public BaseDto(bool IsSuccess, List<string> Message)
        {
            this.IsSuccess = IsSuccess;
            this.Message = Message;
        }
        public List<string> Message { get; private set; }
        public bool IsSuccess { get; private set; }
    }
}
