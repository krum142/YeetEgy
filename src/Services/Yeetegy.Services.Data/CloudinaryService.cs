using System;
using System.Threading.Tasks;

using CloudinaryDotNet;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Yeetegy.Services.Data.CommonServices;
using Yeetegy.Services.Data.Interfaces;

namespace Yeetegy.Services.Data
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly IConfiguration configuration;

        public CloudinaryService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<string> SaveCloudinaryAsync(IFormFile image)
        {
            var settings = this.configuration["CloudSettings"].Split("$");
            var cloudName = settings[0];
            var apiKey = settings[1];
            var apiSec = settings[2];
            var fileName = Guid.NewGuid().ToString();

            var account = new Account(cloudName, apiKey, apiSec);
            var cloud = new Cloudinary(account);

            return await ApplicationCloudinary.UploadImage(cloud, image, fileName);
        }

        public void DeleteCloudinaryAsync(string id)
        {
            var settings = this.configuration["CloudSettings"].Split("$");
            var cloudName = settings[0];
            var apiKey = settings[1];
            var apiSec = settings[2];
            var account = new Account(cloudName, apiKey, apiSec);
            var cloud = new Cloudinary(account);

            ApplicationCloudinary.DeleteImage(cloud, id);
        }
    }
}
