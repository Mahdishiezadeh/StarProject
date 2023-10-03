using Application.Interfaces.Contexts;
using Domain.Visitors;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Visitors.VisitorOnline
{
    public interface IIVisitorOnlineService
    {
        void ConnectUser(string ClientId);
        void DisConnectUser(string ClientId);
        int GetCount();
    }
    public class VisitorOnlineService : IIVisitorOnlineService
    {
        private readonly IMongoDbContext<OnlineVisitor> mongoDbContext;
        private readonly IMongoCollection<OnlineVisitor> mongoCollection;

        public VisitorOnlineService(IMongoDbContext<OnlineVisitor> mongoDbContext)
        {
            this.mongoDbContext = mongoDbContext;
            mongoCollection= mongoDbContext.GetCollection();
        }
        /// <summary>
        /// این متد یک دیتای جدید باید در دیتابیس ادد کند
        /// </summary>
        /// <param name="ClientId"></param>
        public void ConnectUser(string ClientId)
        {

            var exisit = mongoCollection.AsQueryable().FirstOrDefault(p => p.ClientId == ClientId);
            if (exisit==null)
            {
                mongoCollection.InsertOne(new OnlineVisitor
                {
                    ClientId = ClientId,
                    Time = DateTime.Now,
                });
            }
           
        }
        /// <summary>
        /// این متد یک کلاینت را پیدا و از دیتابیس حذف میکند
        /// </summary>
        /// <param name="ClientId"></param>
        public void DisConnectUser(string ClientId)
        {
            mongoCollection.FindOneAndDelete(p=>p.ClientId==ClientId);
        }
        /// <summary>
        /// تعداد کاربران آنلاین را بازگشت میدهد
        /// </summary>
        /// <returns></returns>
        public int GetCount()
        {
           return mongoCollection.AsQueryable().Count();
        }
    }

}
