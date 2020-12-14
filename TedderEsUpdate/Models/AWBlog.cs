using System;

namespace TedderEsUpdate.Models
{
    public class AWBlog
    {
        public uint PostId { get; set; }
        public string Title { get; set; }
        public string PostContent { get; set; }
        public short Status { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public string Identifier { get; set; }
        public string User { get; set; }
        public string UpdateUser { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public sbyte Comments { get; set; }
        public string Tags { get; set; }
        public string ShortContent { get; set; }
        public string ShorterContent { get; set; }
        public string RelatedPosts { get; set; }
        public string Image { get; set; }
        public string Author { get; set; }
    }
}