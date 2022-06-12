using System.Collections.Generic;
using TheBlogFinalMVC.Models;

namespace TheBlogFinalMVC.ViewModels

{
    public class PostDetailViewModel
    {
        public Post Post { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
    }
}
