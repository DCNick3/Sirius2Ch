using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using Microsoft.EntityFrameworkCore;
using Sirius2Ch.Models;
using Encoder = System.Text.Encoder;
using Image = Sirius2Ch.Models.Image;

namespace Sirius2Ch.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        
        
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Vote> Votes { get; set; }
    }
}