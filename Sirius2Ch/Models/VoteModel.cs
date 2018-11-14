using System.ComponentModel.DataAnnotations;

namespace Sirius2Ch.Models
{
    public class Vote
    {
        public int Id { get; set; }
        public string User { get; set; }
        public Topic Topic { get; set; }
        
        [Range(-1, 1)]
        public int Value { get; set; }
    }
}