using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheBlogFinalMVC.Models
{
    public class Blog
    {
        public int Id { get; set; } //primary key - publicly accessible
        public string BlogUserId { get; set; } //foreign key - will be primary key for ASP.Net UserModel

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at most {1} characters.", MinimumLength = 2)]
        public string Name { get; set; } //name of the blog 

        [Required]
        [StringLength(500, ErrorMessage = "The {0} must be at least {2} and at most {1} characters.", MinimumLength = 2)]
        public string Description { get; set; } //blog description - purpose of blog

        [DataType(DataType.Date)] //Treats this as a Date on a form - won't prompt for time
        [Display(Name = "Created Date")] //Created property on a form, usually as a label
        public DateTime Created { get; set; } //when blog was created

        [DataType(DataType.Date)]
        [Display(Name = "Updated Date")]
        public DateTime? Updated { get; set; } //if blog has been edited or updated - ? indicates it can be nullable
        
        [Display(Name = "Blog Image")]
        public byte[] ImageData { get; set; } //byte stream of physical file to go into database

        [Display(Name = "Image Type")]
        public string ContentType { get; set; } //jpg, png, gif, etc

        [NotMapped]
        public IFormFile Image { get; set; } //represents physical file user selects during blog creation and selecting img - imgdata and contentType get their values from properties of this 
    
        //Navigation

        public virtual BlogUser BlogUser { get; set; }

        public virtual ICollection<Post> Posts { get; set; } = new HashSet<Post>();
        //declares publicly accessible property called Posts that is of interface type ICollection, then implemented with concrete class HashSet
        //good selection because the list of posts will come out of the DB with a unique primary key, HashSets are good with dealing with DB Data that house unique info
    
        
    }
}
