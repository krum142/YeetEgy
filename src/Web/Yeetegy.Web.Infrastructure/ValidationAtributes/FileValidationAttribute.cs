using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Yeetegy.Web.Infrastructure.ValidationAtributes
{
    public class FileValidationAttribute : ValidationAttribute
    {
        private readonly IList<string> allowedMimeFiles = new List<string>()
        {
            "image/apng",
            "image/bmp",
            "image/gif",
            "image/jpeg",
            "image/png",
        };

        private readonly IList<string> alloweFileExtensions = new List<string>()
        {
            ".png",
            ".jpeg",
            ".jpg",
            ".gif",
        };

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var file = value as IFormFile;

            if (file == null)
            {
                return new ValidationResult("Image is Required!");
            }

            var fileContentType = this.allowedMimeFiles.Contains(file.ContentType);

            if (fileContentType && file.Length <= 8000000 && this.IsExtensionValid(file, this.alloweFileExtensions))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult("Invalid File Extension!");
        }

        private bool IsExtensionValid(IFormFile file,IList<string> AllowedTypes)
        {
            bool ValidExtension = false;

            foreach (var extension in AllowedTypes)
            {
                if (file.FileName.EndsWith(extension))
                {
                    ValidExtension = true;
                }
            }

            return ValidExtension;
        }
    }
}
