using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Sirius2Ch.Validators
{
    public class FormFileValidatorAttribute : ValidationAttribute
    {
        public long MaxSize { get; set; }
        public int MaxNameLength { get; set; }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;
            if (!(value is IFormFile))
                return new ValidationResult("Invalid type");
            var file = (IFormFile) value;
            if (file.Length > MaxSize)
                return new ValidationResult("Maximum file length exceeded");
            if (file.FileName.Length > MaxNameLength)
                return new ValidationResult("Maximum file name length exceeded");
            
            return ValidationResult.Success;
        }
    }
}