using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Visitors.GetTodayReport;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Admin.EndPoint.Pages.Visitor
{
    public class IndexModel : PageModel
    {
        /// <summary>
        /// ابتدا سرویس مربوطه را اینجکت نموده و سپس یک نمونه پابلیک از 
        /// Dto مربوطه ایجاد میکنیم تا خروجی را براساس آن به سرویس پاس دهیم.
        /// </summary>
        private readonly IGetTodayReportService _getTodayReportService;
        public ResultTodayReportDto ResultTodayReport;
        public IndexModel(IGetTodayReportService getTodayReportService)
        {
            _getTodayReportService = getTodayReportService;
        }
        public void OnGet()
        {
            ResultTodayReport= _getTodayReportService.Execute();
        }
    }
}
