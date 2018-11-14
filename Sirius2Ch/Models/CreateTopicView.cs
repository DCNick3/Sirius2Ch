using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Sirius2Ch.Validators;

namespace Sirius2Ch.Models
{
    public class CreateTopicView
    {
        [Required]
        [StringLength(1024)]
        [DataType(DataType.Text)]
        public string Name { get; set; }
        
        [Required]
        [StringLength(512 * 1024)]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }
        
        [DataType(DataType.Upload)]
        [FormFileValidator(MaxSize = 4 * 1024 * 1024, MaxNameLength = 1024)]
        public IFormFile Image { get; set; }
    }
}