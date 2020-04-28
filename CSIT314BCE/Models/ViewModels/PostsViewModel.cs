using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CSIT314BCE.Models
{
    public class AskViewModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Body { get; set; }

        [Required]
        public string OwnerId { get; set; }
    }

    public class DetailsViewModel
    {
        public Post Question { get; set; }

        public List<Post> Answer { get; set; }
    }

    public class PostAnsViewModel
    {
        [Required]
        public string Body { get; set; }

        [Required]
        public int PostId { get; set; }
    }
}