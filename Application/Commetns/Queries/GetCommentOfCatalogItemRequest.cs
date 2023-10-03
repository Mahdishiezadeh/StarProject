using Application.Interfaces.Contexts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Commetns.Queries
{

    ///خروجی یک لیست از کامنت دی تی او (ریسپانس) باشد 2
    public class GetCommentOfCatalogItemRequest: IRequest<List<GetCommentDto>>
    {
        ///از ورودی کاتالوگ آیتم آیدی (ریکوئست) را دریافت میکنیم
        public int CatalogItemId { get; set; }
    }

    ///هندلر ایجاد میکنیم 3
    public class GetCommentOfCatalogItemQuery : IRequestHandler<GetCommentOfCatalogItemRequest, List<GetCommentDto>>
    {
        private readonly IDataBaseContext context;

        public GetCommentOfCatalogItemQuery(IDataBaseContext context)
        {
            this.context = context;
        }
        public Task<List<GetCommentDto>> Handle(GetCommentOfCatalogItemRequest request, CancellationToken cancellationToken)
        {
            var comments = context.CatalogItemComments.Where(p => p.Id == request.CatalogItemId)
                .Select(p => new GetCommentDto
                {
                    Id=p.Id,
                    Comment = p.Comment,
                    Title = p.Title,
                }).ToList();

            return Task.FromResult(comments);
        }
    }


    ///این دی تی او دیتا را برای ریسپانس میفرستد 1
    public class GetCommentDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Comment { get; set; }
    }

}
