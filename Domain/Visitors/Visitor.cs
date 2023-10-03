using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Visitors
{
    /// <summary>
    /// اطلاعات مهم شامل آیپی ،
    /// لینک که الان کاربر در آن قرارداد : کارنت لینک
    ///رفرش لینک : کاربر از چه سایتی به لینک سایت ما آمده
    ///متدی که کاربر از آن لینک بازدید کرده گت یا پست یا ... بوده
    /// متد : از چه پروتکلی استفاده کرده http or https
    /// PhysicalPath برای مشخص کردن کدام کنترلر یا کدام اکشن متد است
    /// </summary>
    public class Visitor
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Ip { get; set; }
        public string CurrentLink { get; set; }
        public string ReferrerLink { get; set; }
        public string Method { get; set; }
        public string Protocol { get; set; }
        public string PhysicalPath { get; set; }
        public VisitorVersion Browser { get; set; }
        public VisitorVersion OperationSystem { get; set; }
        public Device Device { get; set; }
        /// <summary>
        /// این اتربیوت برای تطبیق زمان سیستم با زمان ذخیره شده در دیتابیس است 
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Time { get; set; }
        public string VisitorId { get; set; }
    }
}
