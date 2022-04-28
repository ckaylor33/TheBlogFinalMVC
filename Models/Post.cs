﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TheBlogFinalMVC.Enums;

namespace TheBlogFinalMVC.Models
{
    public class Post
    {
        public int Id { get; set; } //primary key

        public int BlogId { get; set; } //foreign key

        public string AuthorId { get; set; } //whoever wrote the post - string by default under IdentityUser

        [Required]
        [StringLength(75, ErrorMessage = "The {0} must be at least {2} and no more than {1} characters long.", MinimumLength = 2)]
        public string Title { get; set; } //post title

        [Required]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and no more than {1} characters long.", MinimumLength = 2)]
        public string Abstract { get; set; }

        [Required]
        public string Content { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Created Date")]
        public DateTime Created { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Updated Date")]
        public DateTime? Updated { get; set; }

        public ReadyStatus ReadyStatus { get; set; } //is ready to be viewed by public or not? bool not robust enough so created enum

        public string Slug { get; set; } //derived from the title the user enters

        public byte[] ImageData { get; set; }
        public string ContentType { get; set; } 

        [NotMapped]
        public IFormFile Image { get; set; } //physical file used to fill in ImageData & ContentType properties
    
        //Navigation Property
        public virtual Blog Blog { get; set; }
        public virtual BlogUser Author { get; set; }

        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
        //use HashSet because we're working with DB data - tags held in DB table

        public virtual ICollection<Tag> Tags { get; set; } = new HashSet<Tag>();
    }
}
