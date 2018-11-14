using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Sirius2Ch.Models
{
    public class Topic
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        [DisplayName("Created")]
        [UIHint("_PrettyDateTemplate")]
        public DateTime CreatedTime { get; set; }
        
        [DisplayName("Last updated")]
        [UIHint("_PrettyDateTemplate")]
        public DateTime UpdateTime { get; set; }
        
        public double Rating { get; set; }
        public IdentityUser Creator { get; set; }
    }
}