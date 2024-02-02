namespace Bloggie.Web.Models.ViewModals
{
    public class AddLikeRequest
    {
        public Guid BlogPostId { get; set; }

        public Guid UserId { get; set; }
    }
}
