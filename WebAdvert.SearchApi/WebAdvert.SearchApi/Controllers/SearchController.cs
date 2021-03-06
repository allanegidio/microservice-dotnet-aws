using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebAdvert.SearchApi.Models;
using WebAdvert.SearchApi.Services.Interfaces;

namespace WebAdvert.SearchApi.Controllers
{
    [Route("search/v1")]
    [ApiController]
    [Produces("application/json")]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService _searchService;

        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }

        [HttpGet]
        [Route("{keyword}")]
        public async Task<List<AdvertType>> Get(string keyword)
        {
            return await _searchService.Search(keyword);
        }
    }
}