using Application.Catalogs.CatalogItems.GetCatalogIItemPLP;
using Application.Catalogs.CatalogItems.GetCatalogItemPDP;
using Application.Commetns.Commands;
using Application.Commetns.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace WebSite.EndPoint.Controllers
{
    public class ProductController : Controller
    {
        private readonly IGetCatalogIItemPLPService getCatalogIItemPLPService;
        private readonly IGetCatalogItemPDPService getCatalogItemPDPService;
        private readonly IMediator mediatr;

        public ProductController(IGetCatalogIItemPLPService
            getCatalogIItemPLPService
            , IGetCatalogItemPDPService getCatalogItemPDPService
            ,IMediator mediatr)
        {
            this.getCatalogIItemPLPService = getCatalogIItemPLPService;
            this.getCatalogItemPDPService = getCatalogItemPDPService;
            this.mediatr = mediatr;
        }
        public IActionResult Index(CatlogPLPRequestDto catlogPLPRequestDto)
        {
            var data = getCatalogIItemPLPService.Execute(catlogPLPRequestDto);
            return View(data);
        }

        public IActionResult Details(string Slug)
        {
            var data = getCatalogItemPDPService.Execute(Slug);
            ///یک نمونه از دی تی او مربوطه ایجاد و با کاتالوگ فایند شده آیدی را به دی تی او میدهیم
            GetCommentOfCatalogItemRequest itemDto = new GetCommentOfCatalogItemRequest
            {
                CatalogItemId = data.Id,
            };

            ///سپس با متد سند دی تی او را پاس میدهیم
            var result = mediatr.Send(itemDto).Result;

            ///در اینجا میتوان علاوه بر دیتا کامنت ها را نیز به ویو پاس داد 
            return View(data);
        }


        public IActionResult SendComment(CommentDto commentDto, string Slug)
        {
            ///یک نمونه از سرویس سند کامنت ایجاد میکنیم 
            SendCommentCommand sendComment = new SendCommentCommand(commentDto);

            ///مدیاتور فقط یک متد سند دارد 
            var result = mediatr.Send(sendComment).Result;

            ///درنهایت اسلاگ را برابر اسلاگ قرار میدهیم تا کامنت را با آن ثبت کند 
            return RedirectToAction(nameof(Details), new { Slug = Slug });
        }

    }
}
