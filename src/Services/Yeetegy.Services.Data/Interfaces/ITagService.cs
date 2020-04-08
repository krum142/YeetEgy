using System.Threading.Tasks;

namespace Yeetegy.Services.Data.Interfaces
{
    public interface ITagService
    {
        Task<string> CreateAsync(string tagName);
    }
}