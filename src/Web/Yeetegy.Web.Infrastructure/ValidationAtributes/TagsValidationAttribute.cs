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

            var matches = Regex.Matches(tags, GlobalConstants.TagValidationRegex);

            for (int i = 0; i < matches.Count; i++)
            {
                if (matches[i].Value.Length > 15)
                {
                    return new ValidationResult("Tag too long");
                }
            }

            if (matches.Count > 6)
            {
                return new ValidationResult("Too Many Tags");
            }

            return ValidationResult.Success;
        }
    }
}
