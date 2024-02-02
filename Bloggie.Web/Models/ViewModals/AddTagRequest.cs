using System.ComponentModel.DataAnnotations;

namespace Bloggie.Web.Models.ViewModals
{
    public class AddTagRequest
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string DisplayName { get; set; }
    }
}
