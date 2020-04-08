using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Yeetegy.Services.Data.Interfaces;

namespace Yeetegy.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : Controller
    {
        private readonly ISearchService searchService;

        public SearchController(ISearchService searchService)
        {
            this.searchService = searchService;
        }

        [HttpGet]
        public async Task<IActionResult> SearchResults(string searchContent) // use in memory cashe to make less calls to db
        {
            var result = await this.searchService.GetSearchResultsAsync(searchContent);

            return this.Json(result);
        }
    }
}
