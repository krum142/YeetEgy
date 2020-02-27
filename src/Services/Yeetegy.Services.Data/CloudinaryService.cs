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

        public async Task<string> SaveCloudinary(IFormFile image)
        {
            var settings = this.configuration["CloudSettings"].Split("$");
            var cloudName = settings[0];
            var apikey = settings[1];
            var apiSec = settings[2];
            var fileName = Guid.NewGuid().ToString();

            var account = new Account(cloudName, apikey, apiSec);
            var cloud = new Cloudinary(account);

            return await ApplicationCloudinary.UploadImage(cloud, image, fileName);
        }
    }
}