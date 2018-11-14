using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Sirius2Ch.Data;

namespace Sirius2Ch.Controllers
{
    public class ImagesController : Controller
    {
        private ApplicationDbContext _context;
        
        public ImagesController(ApplicationDbContext dbContext)
        {
            _context = dbContext;
        }
        
        public IActionResult Fullsize(int id)
        {
            var img = _context.Images.Find(id);
            if (img == null)
                return NotFound();

            return File(img.MaxRes, img.ContentType);
        }
        
        public IActionResult Preview256(int id)
        {
            var img = _context.Images.Find(id);
            if (img == null)
                return NotFound();
            
            return File(img.Preview256, img.ContentType);
        }
    }
}