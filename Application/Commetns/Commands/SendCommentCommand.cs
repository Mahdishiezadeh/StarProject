
using Application.Interfaces.Contexts;
using Domain.Catalogs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Commetns.Commands
{

    /// درون این ریکوئست یک نمونه از کامنت دی تی او ایجاد میکنیم 3
    /// و خود این کلاس در مرحله بعدی 4 حکم تی ریکوئست برای هندلر را دارد
    public class SendCommentCommand:IRequest<SendCommentResponseDto>
    {
        public CommentDto Comment { get; set; }
        public SendCommentCommand(CommentDto commentDto)
        {
            this.Comment = commentDto;
        }
    }

    ///حال یک هندلر ایجاد میکنیم 4
    public class SendCommentHandler : IRequestHandler<SendCommentCommand, SendCommentResponseDto>
    {
        ///دیتابیس کانتکست را اینجکت میکنیم 5
        private readonly IDataBaseContext context;
        public SendCommentHandler(IDataBaseContext context)
        {
            this.context = context;
        }


        ///ایمپلیمنت شده 4 میباشد
        public Task<SendCommentResponseDto> Handle(SendCommentCommand request, CancellationToken cancellationToken)
        {
            ///کاتالوگ آیتم را فایند میکنیم 6
            var catalogItem = context.CatalogItems.Find(request.Comment.CatalogItemId);

            ///یک نمونه از موجودیت کاتالوگ آیتم کامنت ایجاد میکنیم 7
            CatalogItemComment comment = new CatalogItemComment
            {
                ///فیلدهای دی تی او با کمک ریکوئست و خود کاتالوگ آیتم
                ///را نیز که فایند کردیم 6 مپ میکنیم به موجودیت
                Comment = request.Comment.Comment,
                Email = request.Comment.Email,
                Title = request.Comment.Title,
                CatalogItem = catalogItem,
            };

            ///ادد کردن در دیتابیس 8
            var entity = context.CatalogItemComments.Add(comment);
            context.SaveChanges();

            ///بازگشت دیتای متد 9
            return Task.FromResult(new SendCommentResponseDto
            {
                ///آیدی همان کامنتی که ثبت شد بازگشت داده شد
                Id = entity.Entity.Id
            });

        }

    }


    ///دی تی او اصلی و ورودی 1
    public class CommentDto
    {
        public string Title { get; set; }
        public string Comment { get; set; }
        public string Email { get; set; }
        public int CatalogItemId { get; set; }
    }

    ///دی تی او ریسپانس 2
    public class SendCommentResponseDto
    {
        public int Id { get; set; }
    }
}
