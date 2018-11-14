using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Sirius2Ch.Validators;

namespace Sirius2Ch.Models
{
    public class AddPostView
    {
        [Required]
        [StringLength(512 * 1024)]
        public string Content { get; set; }
        
        public int TopicId { get; set; }
        
        [FormFileValidator(MaxSize = 4 * 1024 * 1024, MaxNameLength = 1024)]
        public IFormFile Image { get; set; }
    }
}