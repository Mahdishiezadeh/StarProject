using Application.Interfaces.Contexts;
using Domain.Visitors;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Visitors.SaveVisitorInfo
{
    public interface ISaveVisitorInfoService
    {
        void Execute(RequestSaveVisitorInfoDto request);
    }
    public class SaveVisitorInfoService : ISaveVisitorInfoService
    {
        private readonly IMongoDbContext<Visitor> _mongoDbContext;
        /// <summary>
        /// این پراپرتی inject نیاز ندارد
        /// </summary>
        private readonly IMongoCollection<Visitor> _visitorMongoCollection;
        public SaveVisitorInfoService(IMongoDbContext<Visitor> mongoDbContext)
        {
            _mongoDbContext = mongoDbContext;
            _visitorMongoCollection = _mongoDbContext.GetCollection();
        }
        public void Execute(RequestSaveVisitorInfoDto request)
        {
            _visitorMongoCollection.InsertOne(new Visitor
            {
                Device=new Device
                {
                    Brand=request.Device.Brand,
                    Family=request.Device.Family,
                    Model=request.Device.Model,
                    IsSpider=request.Device.IsSpider,
                },
                Browser = new VisitorVersion
                {
                    Family=request.Browser.Family,
                    Version=request.Browser.Version,
                },
                CurrentLink = request.CurrentLink,
                Ip =request.Ip,
                Method=request.Method,
                ReferrerLink=request.ReferrerLink,
                PhysicalPath=request.PhysicalPath,
                Protocol=request.Protocol,
                VisitorId=request.VisitorId,
                Time=DateTime.Now,
                OperationSystem =new VisitorVersion
                {
                    Family=request.OperationSystem.Family,
                    Version=request.OperationSystem.Version,
                },
            });
        }
    }

    public class RequestSaveVisitorInfoDto
    {
        public string Ip { get; set; }
        public string CurrentLink { get; set; }
        public string ReferrerLink { get; set; }
        public string Method { get; set; }
        public string Protocol { get; set; }
        public string PhysicalPath { get; set; }
        public VisitorVersionDto Browser { get; set; }
        public VisitorVersionDto OperationSystem { get; set; }
        public DeviceDto Device { get; set; }
        public string VisitorId { get; set; }
        public DateTime Time { get; set; }
    }
    public class DeviceDto
    {
        public string Brand { get; set; }
        public string Family { get; set; }
        public string Model { get; set; }
        public bool IsSpider { get; set; }
    }
    public class VisitorVersionDto
    {
        public string Family { get; set; }
        public string Version { get; set; }
    }
}
