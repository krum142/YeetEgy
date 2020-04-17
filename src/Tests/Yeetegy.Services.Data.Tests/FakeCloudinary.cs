using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Yeetegy.Services.Data.Interfaces;

namespace Yeetegy.Services.Data.Tests
{
    public class FakeCloudinary : ICloudinaryService
    {
        public async Task<string> SaveCloudinaryAsync(IFormFile image)
        {
            return "FakeCloudinaryUrl";
        }

        public void DeleteCloudinary(string id)
        {
        }
    }
}
