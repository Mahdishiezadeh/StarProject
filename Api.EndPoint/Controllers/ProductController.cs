using Application.Catalogs.CatalogItems.GetCatalogIItemPLP;
using Application.Catalogs.CatalogItems.GetCatalogItemPDP;
using Application.Commetns.Commands;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.EndPoint.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IGetCatalogIItemPLPService getCatalogIItemPLPService;
        private readonly IGetCatalogItemPDPService getCatalogItemPDPService;
        private readonly IMediator mediatr;

        public ProductController(IGetCatalogIItemPLPService
            getCatalogIItemPLPService
            , IGetCatalogItemPDPService getCatalogItemPDPService
            , IMediator mediatr)
        {
            this.getCatalogIItemPLPService = getCatalogIItemPLPService;
            this.getCatalogItemPDPService = getCatalogItemPDPService;
            this.mediatr = mediatr;
        }

        [HttpGet]
        public IActionResult Get([FromQuery] CatlogPLPRequestDto requestDto)
        {
            return Ok(getCatalogIItemPLPService.Execute(requestDto));
        }

        [HttpGet]
        [Route("PDP")]
        public IActionResult Get([FromQuery] string Slug)
        {
            return Ok(getCatalogItemPDPService.Execute(Slug));
        }

        [HttpPost]
        public IActionResult Post([FromBody] CommentDto commentDto)
        {
            SendCommentCommand sendComment = new SendCommentCommand(commentDto);
            var result = mediatr.Send(sendComment).Result;
            return Ok(result);
        }


    }
}
