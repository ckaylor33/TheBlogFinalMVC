using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using TheBlogFinalMVC.Enums;

namespace TheBlogFinalMVC.Models
{
    public class Comment
    {
        public int Id { get; set; }

        public int PostId { get; set; } //relates comment to it's parent post

        public string AuthorId { get; set; } //relates comment to the user

        public string ModeratorId { get; set; }

        [Required]
        [StringLength(500, ErrorMessage ="The {0} must be at least {2) and no more than {1} characters long.", MinimumLength = 2)]
        [Display(Name = "Comment")]
        public string Body { get; set; }

        [DataType(DataType.Date)]
        public DateTime Created { get; set; }

        public DateTime? Updated { get; set; }

        public DateTime? Moderated { get; set; }

        public DateTime? Deleted { get; set; } //soft delete

        [StringLength(500, ErrorMessage = "The {0} must be at least {2) and no more than {1} characters long.",MinimumLength = 2)]
        [Display(Name = "Moderated Comment")]
        public string ModeratedBody { get; set; } //if comment moderated, this will display instead of original comment

        public ModerationType ModerationType { get; set; } //enum limits choices of the moderator or programmer

        //Navigation Properties
        public virtual Post Post { get; set; } //holds record represented by PostId
        
        public virtual BlogUser Author { get; set; } //holds record represented by AuthorId

        public virtual BlogUser Moderator { get; set; } //use moderatorId string to go from the comment to IdentityUser record represnted by the single string

    }
}
