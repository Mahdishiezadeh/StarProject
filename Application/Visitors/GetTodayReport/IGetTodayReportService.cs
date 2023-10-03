using Application.Interfaces.Contexts;
using Domain.Visitors;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Visitors.GetTodayReport
{
    /// <summary>
    /// مرحله دوم یک متد اگزکیوت از دی تی او ایجاد میکنیم 
    /// این سرویس را در استارت آپ ادمین ادد میکنیم 
    /// </summary>
    public interface IGetTodayReportService
    {
        ResultTodayReportDto Execute();
    }
    /// <summary>
    /// مرحله سوم سرویس مربوطه را ایجاد از آی مونگو دی بی کانتکست
    /// و آی مونگو کالکشن برای کوئری زدن کمک میگیریم 
    /// </summary>
    public class GetTodayReportService : IGetTodayReportService
    {
        private readonly IMongoDbContext<Visitor> _mongoDbContext;
        private readonly IMongoCollection<Visitor> visitorMongoCollection;
        public GetTodayReportService(IMongoDbContext<Visitor> mongoDbContext)
        {
            _mongoDbContext = mongoDbContext;
            visitorMongoCollection= _mongoDbContext.GetCollection();
        }
        /// <summary>
        /// گزارش مبتنی بر تاریخ است 
        /// </summary>
        /// <returns></returns>
        public ResultTodayReportDto Execute()
        {
            DateTime start = DateTime.Now.Date;
            DateTime end = DateTime.Now.AddDays(1);
            ///کالکشن را به کوئری و سپس با کمک ور 
            ///شرط زمان گذاشته و با لانگ کانت شمارش میکنیم
            var TodayPageViewCount = visitorMongoCollection.AsQueryable()
                .Where(p => p.Time >= start && p.Time < end).LongCount();
            ///تعداد ویزیتور
            var TodayVisitorCount = visitorMongoCollection.AsQueryable()
                .Where(p => p.Time >= start && p.Time < end).GroupBy(p => p.VisitorId)
                .LongCount();
            ///تعداد کل بازدیدها
            var AllPageViewCount = visitorMongoCollection.AsQueryable().LongCount();
            var AllVisitorCount = visitorMongoCollection.AsQueryable()
                .GroupBy(p => p.VisitorId).LongCount();

            VisitCountDto visitPerHour = GTetVisitPerHour(start, end);

            VisitCountDto visitPerDay = GetVisitPerDay();

            var visitors = visitorMongoCollection.AsQueryable()
               .OrderByDescending(p => p.Time)
               .Take(10)
               .Select(p => new VisitorsDto
               {
                   Id = p.Id,
                   Browser = p.Browser.Family,
                   CurrentLink = p.CurrentLink,
                   Ip = p.Ip,
                   OperationSystem = p.OperationSystem.Family,
                   IsSpider = p.Device.IsSpider,
                   ReferrerLink = p.ReferrerLink,
                   Time = p.Time,
                   VisitorId = p.VisitorId
               }).ToList();


            return new ResultTodayReportDto
            {
                GeneralStats = new GeneralStatsDto
                {
                    TotalVisitors = AllVisitorCount,
                    TotalPageViews = AllPageViewCount,
                    PageViewsPerVisit = GetAvg(AllPageViewCount, AllVisitorCount),
                    VisitPerDay = visitPerDay,
                },
                Today = new TodayDto
                {
                    PageViews = TodayPageViewCount,
                    Visitors = TodayVisitorCount,
                    ViewsPerVisitor = GetAvg(TodayPageViewCount, TodayVisitorCount),
                    VisitPerhour = visitPerHour,
                },

                visitors = visitors,
            };
        }

        private VisitCountDto GTetVisitPerHour(DateTime start, DateTime end)
        {
            ///کل بازدید های پیج به دست آمد و مرتب سازی بر اساس زمان قرار گرفت
            var TodayPageViewList = visitorMongoCollection.AsQueryable().Where(
              p => p.Time >= start && p.Time < end)
                .Select(p => new { p.Time }).ToList();

            ///با کمک حلقه بازدید پیج را به ازاء هر ساعت جدا میکنیم
            ///و برای نمایش نموداری از دی تی او مربوطه کمک میگیریم

            VisitCountDto visitPerHour = new VisitCountDto()
            {
                ///چون بیست و چهار ساعت داریم 
                Display = new string[24],
                Value = new int[24],
            };
            ///چون بیست و چهار ساعته بررسی میشود 
            for (int i = 0; i <= 23; i++)
            {
                visitPerHour.Display[i] = $"h-{i}";
                visitPerHour.Value[i] = TodayPageViewList.Where(p => p.Time.Hour == i).Count();
            }

            return visitPerHour;
        }

        private VisitCountDto GetVisitPerDay()
        {
            ///سی روز گذشته از منفی سی استفاده میکنیم
            DateTime MonthStart = DateTime.Now.Date.AddDays(-30);
            DateTime MonthEnds = DateTime.Now.Date.AddDays(1);


            var Month_PageViewList = visitorMongoCollection.AsQueryable()
                .Where(p => p.Time >= MonthStart && p.Time < MonthEnds)
                ///سلکت چون کل دیتا را نمیخواهیم و الویت تایم است
                .Select(p => new { p.Time })
                .ToList();

            VisitCountDto visitPerDay = new VisitCountDto()
            { 
              Display = new string[31], 
              Value = new int[31] 
            };

            for (int i = 0; i <= 30; i++)
            {
                ///ضرب در منهای میکنیم تا روزهای قبل را به دست آورد
                var currentday = DateTime.Now.AddDays(i * (-1));


                visitPerDay.Display[i] = i.ToString();
                                                                                 ///تاریخ روز جاری
                visitPerDay.Value[i] = Month_PageViewList.Where(p => p.Time.Date == currentday.Date).Count();
            }

            return visitPerDay;
        }

        /// <summary>
        /// ///اگر در تاریخی روز ویزیتور نداشته باشیم تقسیم بر صفر شده
        ///و خروجی این متغییر را دچار مشکل میکند
        /// GetAvg این کد برای رفع این مشکل نوشته شده است
        /// رفع ارور ViewsPerVisitor
        /// <param name="VisitPage"></param>
        /// <param name="Visitor"></param>
        /// <returns></returns>
        private float GetAvg(long VisitPage, long Visitor)
        {
            if (Visitor == 0)
            {
                return 0;
            }
            else
            {
                return VisitPage / Visitor;
            }
        }
    }
    /// <summary>
    /// ابتدا تمام دی تی او های مربوطه را ادد میکنیم 
    /// </summary>
    public class ResultTodayReportDto
    {
        public GeneralStatsDto GeneralStats { get; set; }
        public TodayDto Today { get; set; }
        public List<VisitorsDto> visitors { get; set; }
    }
    public class GeneralStatsDto
    {
        public long TotalPageViews { get; set; }
        public long TotalVisitors { get; set; }
        public float PageViewsPerVisit { get; set; }
       
        public VisitCountDto VisitPerDay { get; set; }
    }

    public class TodayDto
    {
        public long PageViews { get; set; }
        public long Visitors { get; set; }
        public float ViewsPerVisitor { get; set; }
        public VisitCountDto VisitPerhour { get; set; }
    }
    /// <summary>
    /// برای نمودار به این دی تی او نیاز است
    /// </summary>
    public class VisitCountDto
    {
        public string[] Display { get; set; }
        public int[] Value { get; set; }

    }

    public class VisitorsDto
    {
        public string Id { get; set; }
        public string Ip { get; set; }
        public string CurrentLink { get; set; }
        public string ReferrerLink { get; set; }
        public string Browser { get; set; }
        public string OperationSystem { get; set; }
        public bool IsSpider { get; set; }
        public DateTime Time { get; set; }
        public string VisitorId { get; set; }

    }
}
