using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Sirius2Ch.Models
{
    public class Post
    {
        public int Id { get; set; }
        
        [StringLength(512 * 1024)]
        public string Content { get; set; }
        
        public IdentityUser Author { get; set; }
        public DateTime Time { get; set; }
        public Image Image { get; set; }
        public Topic Topic { get; set; }
    }
}