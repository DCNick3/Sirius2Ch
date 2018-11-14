using System.Collections.Generic;

namespace Sirius2Ch.Models
{
    public class TopicView
    {
        public Topic Topic { get; set; }
        public IEnumerable<Post> Posts { get; set; }
        public AddPostView AddPostView { get; set; }
        public int CurrentVote { get; set; }
    }
}