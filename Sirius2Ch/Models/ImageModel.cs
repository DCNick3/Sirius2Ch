using System.Net.Mime;

namespace Sirius2Ch.Models
{
    public class Image
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ContentType { get; set; }
        public byte[] Preview256 { get; set; }
        public byte[] MaxRes { get; set; }
    }
}