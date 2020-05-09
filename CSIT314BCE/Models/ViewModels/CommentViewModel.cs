using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CSIT314BCE.Models
{
    public class PostCommentViewModel
    {
        [Required]
        public string Text { get; set; }

        [Required]
        public int PostId { get; set; }

        public string OwnerId { get; set; }
    }
}