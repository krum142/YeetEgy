using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Yeetegy.Services.Data.Interfaces;

namespace Yeetegy.Services.Data.Tests
{
    public class FakeCloudinary : ICloudinaryService
    {
        public bool ReturnNullOnCreate { get; set; } = false;

        public async Task<string> SaveCloudinaryAsync(IFormFile image)
        {
            if (this.ReturnNullOnCreate)
            {
                return null;
            }

            return image.Name;
        }

        public void DeleteCloudinary(string id)
        {
        }
    }
}
