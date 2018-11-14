using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sirius2Ch.Data;
using Sirius2Ch.Models;

namespace Sirius2Ch.Controllers
{
    public class TopicsController : Controller
    {
        private const long MaxImageSize = 4 * 1024 * 1024;
        
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager; 
        private readonly SignInManager<IdentityUser> _signInManager; 

        public TopicsController(ApplicationDbContext context, UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        private double TopicSortKey(Topic topic)
        {
            // TODO: make this better
            return -(DateTime.Now - topic.UpdateTime).TotalMinutes / 10.0 + Math.Abs(topic.Rating) * topic.Rating;
        }
        
        public IActionResult Index()
        {
            var topics = _context.Topics.OrderByDescending(TopicSortKey);
            return View(topics);
        }

        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        private IEnumerable<Post> GetTopicPosts(Topic topic) => 
            _context.Posts.Where(post => post.Topic == topic).Include(post => post.Image).Include(post => post.Author);

        private async Task<IdentityUser> GetUserAsync() => await _userManager.FindByNameAsync(User.Identity.Name);
        
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(CreateTopicView topic)
        {
            if (ModelState.IsValid)
            {
                Image image = null;
                if (topic.Image != null)
                {
                    // TODO: migrate to dedicated image store service (like @alice2k did)
                    if (topic.Image.Length <= MaxImageSize)
                        using (var str = topic.Image.OpenReadStream())
                            image = _context.UploadImage(str, topic.Image.FileName ?? "image");

                    if (image == null)
                    {
                        ModelState.AddModelError("Image", "Invalid image");
                        return View(topic);
                    }
                }

                var now = DateTime.Now;
                var user = await GetUserAsync();
                
                var newTopic = new Topic
                {
                    Name = topic.Name,
                    CreatedTime = now,
                    UpdateTime = now,
                    Creator = user,
                };
                var initialPost = new Post
                {
                    Content = topic.Content,
                    Image = image,
                    Topic = newTopic,
                    Author = user,
                    Time = now,
                };

                _context.Topics.Add(newTopic);
                _context.Posts.Add(initialPost);
                
                _context.SaveChanges();
                
                return RedirectToAction("View", new { id = newTopic.Id });
            }
            return View(topic);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddPost(AddPostView post)
        {
            var topic = _context.Topics.Find(post.TopicId);

            if (topic == null)
                return RedirectToAction("Index");

            var vote = GetUserTopicVote(topic);

            IActionResult ReturnForm()
            {
                ViewBag.ScrollTo = "reply-form";
                return View("View", new TopicView
                {
                    Topic = topic,
                    Posts = GetTopicPosts(topic),
                    AddPostView = post,
                    CurrentVote = vote?.Value ?? 0,
                });
            }

            if (ModelState.IsValid)
            {
                Image image = null;
                if (post.Image != null)
                {
                    // TODO: migrate to dedicated image store service (like @alice2k did)
                    if (post.Image.Length <= MaxImageSize)
                        using (var str = post.Image.OpenReadStream())
                            image = _context.UploadImage(str, post.Image.FileName ?? "image");

                    if (image == null)
                    {
                        ModelState.AddModelError("Image", "Invalid image");
                        return ReturnForm();
                    }
                }

                var now = DateTime.Now;
                var user = await GetUserAsync();

                var newPost = new Post
                {
                    Topic = topic,
                    Author = user,
                    Image = image,
                    Content = post.Content,
                    Time = now,
                };
                _context.Posts.Add(newPost);
                topic.UpdateTime = now;
                _context.SaveChanges();
                return Redirect(Url.Action("View", new { id = post.TopicId }) + "#post-" + newPost.Id);
            }

            return ReturnForm();
        }

        private Vote GetUserTopicVote(Topic topic) =>
            _context.Votes.FirstOrDefault(v => v.Topic == topic && v.User == User.Identity.Name);

        [Authorize]
        public IActionResult Vote(int id, bool vote)
        {
            using (var tran = _context.Database.BeginTransaction(IsolationLevel.Serializable))
            {
                var topic = _context.Topics.Find(id);
                if (topic == null)
                    return NotFound();
                
                var currentVote = GetUserTopicVote(topic);
                if (currentVote == null)
                {
                    currentVote = new Vote
                    {
                        Topic = topic,
                        User = User.Identity.Name,
                        Value = 0
                    };
                    _context.Votes.Add(currentVote);
                }
    
                var oldVal = currentVote.Value;
                var wantDiff = vote ? 1 : -1;
                var newVal = Math.Max(Math.Min(oldVal + wantDiff, 1), -1);
                var realDiff = newVal - oldVal;
    
                currentVote.Value += realDiff;
                topic.Rating += realDiff;
                
                _context.SaveChanges();
                
                tran.Commit();
            }
            
            return RedirectToAction("View", new { id });
        } 

        public IActionResult View(int id)
        {
            var topic = _context.Topics.Find(id);
            if (topic == null)
                return NotFound();

            var posts = GetTopicPosts(topic);

            var vote = GetUserTopicVote(topic);
            
            return View(new TopicView
            {
                Topic = topic,
                Posts = posts,
                AddPostView = new AddPostView { TopicId = topic.Id },
                CurrentVote = vote?.Value ?? 0,
            });
        }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}