using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

using Yeetegy.Common;

namespace Yeetegy.Web.Infrastructure.ValidationAtributes
{
    public class TagsValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var tags = value as string;

            if (string.IsNullOrWhiteSpace(tags))
            {
                return ValidationResult.Success;
            }

            var matches = Regex.Matches(tags, GlobalConstants.TagValidationRegex).Count;

            if (matches <= 6)
            {
                return ValidationResult.Success;
            }

            return new ValidationResult("Too Many Tags");
        }
    }
}
