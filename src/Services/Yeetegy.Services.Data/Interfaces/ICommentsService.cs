using System.Threading.Tasks;

using Yeetegy.Web.ViewModels;

namespace Yeetegy.Services.Data.Interfaces
{
    public interface ICommentsService
    {
        Task CreateCommentAsync(AddCommentsModel input, string userId);
    }
}