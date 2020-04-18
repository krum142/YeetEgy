using Yeetegy.Services.Data.Tests.TestModels;
using Yeetegy.Services.Mapping;
using Yeetegy.Web.ViewModels;

namespace Yeetegy.Services.Data.Tests
{
    public class BaseServiceTests
    {
        public BaseServiceTests()
        {
            AutoMapperConfig.RegisterMappings(typeof(CategoryViewModel).Assembly, typeof(TestPostClass).Assembly);
        }
    }
}