using Application.Interfaces;
using Application.Interfaces.Contexts;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Contexts.MongoContext
{
    /// <summary>
    /// این دیتابیس بایست بتواند کالکشن را به لایه اپلیکیشن تحویل دهد
    /// و میدانیم لایه اپ نمیتواند به لایه پرسیتنس وابسته باشد
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MongoDbContext<T> : IMongoDbContext<T>
    {
        private readonly IMongoDatabase db;
        private readonly IMongoCollection<T> mongoCollection;
        public MongoDbContext()
        {
            var client = new MongoClient();
            db = client.GetDatabase("VisitorDb");
            ///در اینجا به جای تی ممکن است ویزیتور یا سایر
            ///موجودیت ها پاس داده شود پس از تایپ آو استفاده میکنیم 
            mongoCollection = db.GetCollection<T>(typeof(T).Name);
        }
        public IMongoCollection<T> GetCollection()
        {
            return mongoCollection;
        }
    }
}
