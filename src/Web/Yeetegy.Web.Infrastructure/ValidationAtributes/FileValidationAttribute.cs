using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

        private readonly IList<string> allowedFileExtensions = new List<string>()
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
                return ValidationResult.Success;
            }

            var fileContentType = this.allowedMimeFiles.Contains(file.ContentType);

            if (file.Length > 1000000)
            {
                return new ValidationResult("File size too large max 1MB.");
            }

            if (fileContentType && this.IsExtensionValid(file, this.allowedFileExtensions)) // this might be too much size
            {
                return ValidationResult.Success;
            }

            return new ValidationResult("Invalid File Extension.");
        }

        private bool IsExtensionValid(IFormFile file,IList<string> allowedTypes)
        {
            bool validExtension = false;

            foreach (var extension in allowedTypes)
            {
                if (file.FileName.EndsWith(extension))
                {
                    validExtension = true;
                }
            }

            return validExtension;
        }
    }
}
