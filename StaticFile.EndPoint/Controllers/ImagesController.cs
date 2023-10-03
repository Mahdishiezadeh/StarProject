using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace StaticFile.EndPoint.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IHostingEnvironment _environment;

        public ImagesController(IHostingEnvironment hostingEnvironment)
        {
            _environment = hostingEnvironment;
        }
        /// <summary>
        /// این متد برای آپلود است
        /// apiKey 
        /// برای این است که در دسترس همه قرار نداشته باشد
        /// </summary>
        /// <returns></returns>
        public IActionResult Post(string apiKey)
        {
            if (apiKey != "mysecretkey")
            {
                return BadRequest();
            }
            try
            {
                ///فایل ها را با کمک ریکوئست به دست می آورم
                var files = Request.Form.Files;

                ///یک مسیر ایجاد میکنیم برای فولدری که این دیتا بر روی آن ذخیره شود
                var folderName = Path.Combine("Resources", "Images");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                if (files!=null)
                {
                    //upload
                    ///فایل ها رو به متد پاس میدیم تا آپلود انجام بشه
                    return Ok(UploadFile(files));
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error");
                throw new Exception("upload image error", ex);
            }
        }
        private UploadDto UploadFile(IFormFileCollection files)
        {
            ///برای تغییر نام ، فایل عکس که تکراری نباشد
            string newName = Guid.NewGuid().ToString();
            var date = DateTime.Now;

            ///از دیت برای نام گذاری فولدر محل ذخیره سازی عکس ها کمک میگیریم
            ///فولدرهایی بر اساس تاریخ روز ایجاد شوند
            string folder = $@"Resources\images\{date.Year}\{date.Year}-{date.Month}\";
            var uploadsRootFolder = Path.Combine(_environment.WebRootPath, folder);

            ///چک میکنیم اگر مسیر وجود نداشت
            if (!Directory.Exists(uploadsRootFolder))
            {
                ///ایجاد مسیر
                Directory.CreateDirectory(uploadsRootFolder);
            }
            ///یک لیست استرینگ از آدرس فایل هایی که ذخیره و ایجاد شده اند 
            List<string> address = new List<string>();
            foreach (var file in files)
            {
                ///اگر عکس مقدار داشت
                if (file != null && file.Length > 0)
                {
                    ///به اول نام عکس جی یو آیدی اضافه شود
                    string fileName = newName + file.FileName;
                    ///مسیر آپلود را مشخص میکنیم
                    var filePath = Path.Combine(uploadsRootFolder, fileName);
                    ///با کمک فایل استریم آپلود میکنیم
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    ///درنهایت مسیر رو در قالب لیست استرینگ ذخیره میکنیم
                    address.Add(folder + fileName);
                }
            }
            ///آدرس ها رو بازگشت میدیم تا در سایر سرویس ها استفاده شود
            return new UploadDto()
            {
                FileNameAddress = address,
                Status = true,
            };
        }



    }
    public class UploadDto
    {
        public bool Status { get; set; }
        public List<string> FileNameAddress { get; set; }
    }
}
