using System.Collections.Generic;
using TheBlogFinalMVC.Models;
using X.PagedList;
using X.PagedList.Web.Common;
using X.PagedList.Mvc.Core;

namespace TheBlogFinalMVC.ViewModels
{
    public class HomepageViewModel
    {
        public List<Blog> Blogs { get; set; }

        public List<Post> Posts { get; set; }

    }
}
