using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Yeetegy.Services.Data.Interfaces;
using Yeetegy.Web.ViewModels;
using Yeetegy.Web.ViewModels.PostViewModels;

namespace Yeetegy.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SearchController : Controller
    {
        private readonly ISearchService searchService;
        private readonly IPostsService postsService;

        public SearchController(ISearchService searchService, IPostsService postsService)
        {
            this.searchService = searchService;
            this.postsService = postsService;
        }

        [HttpGet]
        public async Task<IActionResult> SearchResults(string searchContent) // use in memory cashe to make less calls to db
        {
            if (!string.IsNullOrWhiteSpace(searchContent))
            {
                var result = await this.searchService.GetSearchResultsAsync<SearchResponseModel>(searchContent);

                return this.Json(result);
            }

            return this.NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> SearchPosts(int page, string searchContent)
        {
            if (!string.IsNullOrWhiteSpace(searchContent))
            {
                var searchPosts = await this.postsService.GetAllByTagAsync<PostsViewModel>(page, 5, searchContent);

                return this.PartialView("_GetPostsPartial", searchPosts);
            }

            return this.NotFound();
        }
    }
}
