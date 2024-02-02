using Bloggie.Web.Models.Domain;
using Bloggie.Web.Models.ViewModals;
using Bloggie.Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bloggie.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminBlogPostsController : Controller
    {
        private readonly ITagRepository tagRepository;
        private readonly IBlogPostRespository blogPostRespository;

        public AdminBlogPostsController(ITagRepository tagRepository, IBlogPostRespository blogPostRespository)
        {
            this.tagRepository = tagRepository;
            this.blogPostRespository = blogPostRespository;
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            // get tags from respository
            var tags = await tagRepository.GetAllAsync();

            var model = new AddBlogPostRequest
            {
                Tags = tags.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddBlogPostRequest addBlogPostRequest)
        {
            // Map view model to domain model
            var blogPost = new BlogPost
            {
                Heading = addBlogPostRequest.Heading,
                PageTitle = addBlogPostRequest.PageTitle,
                Content = addBlogPostRequest.Content,
                ShortDesciption = addBlogPostRequest.ShortDesciption,
                FeaturedImageUrl = addBlogPostRequest.FeaturedImageUrl,
                UrlHandle = addBlogPostRequest.UrlHandle,
                PublishedDate = addBlogPostRequest.PublishedDate,
                Author = addBlogPostRequest.Author,
                Visible = addBlogPostRequest.Visible,
            };

            // Map Tags from selected tags
            var selectedTags = new List<Tag>();
            foreach (var selectedTagID in addBlogPostRequest.SelectedTag)
            {
                var selectedTagIdAsGuid = Guid.Parse(selectedTagID);
                var existingTag = await tagRepository.GetAsync(selectedTagIdAsGuid);    
                if (existingTag != null) 
                {
                    selectedTags.Add(existingTag);
                }
            }
            // Maooing tags back to domain model
            blogPost.Tags = selectedTags;

            await blogPostRespository.AddAsync(blogPost);

            return RedirectToAction("Add");
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            //Call theb repository
            var blogPost = await blogPostRespository.GetAllAsync();

            return View(blogPost);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            // Retrieve the result from the repository
            var blogPost = await blogPostRespository.GetAsync(id);
            var tagsDomainModel = await tagRepository.GetAllAsync();

            if(blogPost != null)
            {
                // map the domain model into the view model
                var model = new EditBlogPostRequest
                {
                    Id = blogPost.Id,
                    Heading = blogPost.Heading,
                    PageTitle = blogPost.PageTitle,
                    Content = blogPost.Content,
                    Author = blogPost.Author,
                    FeaturedImageUrl = blogPost.FeaturedImageUrl,
                    UrlHandle = blogPost.UrlHandle,
                    ShortDesciption = blogPost.ShortDesciption,
                    PublishedDate = blogPost.PublishedDate,
                    Visible = blogPost.Visible,
                    Tags = tagsDomainModel.Select(x => new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.Id.ToString()
                    }),
                    SelectedTags = blogPost.Tags.Select(x => x.Id.ToString()).ToArray()
                };
                return View(model);
            }
            
            // pass data to view
            return View(null);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditBlogPostRequest editBlogPostRequest)
        {
            // map view model back to domain model
            var blogPostDomainModel = new BlogPost
            {
                Id = editBlogPostRequest.Id,
                Heading = editBlogPostRequest.Heading,
                PageTitle = editBlogPostRequest.PageTitle,
                Content = editBlogPostRequest.Content,
                Author = editBlogPostRequest.Author,
                ShortDesciption = editBlogPostRequest.ShortDesciption,
                FeaturedImageUrl = editBlogPostRequest.FeaturedImageUrl,
                PublishedDate = editBlogPostRequest.PublishedDate,
                UrlHandle = editBlogPostRequest.UrlHandle,
                Visible = editBlogPostRequest.Visible
            };

            // Map tags into domain model
            var selectedTags = new List<Tag>();
            foreach (var selectedTag in editBlogPostRequest.SelectedTags)
            {
                if (Guid.TryParse(selectedTag, out var tag))
                {
                    var foundTag = await tagRepository.GetAsync(tag);

                    if(foundTag != null)
                    {
                        selectedTags.Add(foundTag);
                    }
                }
            }

            blogPostDomainModel.Tags = selectedTags;

            // Submit information to repository to update
            var updatedBlog = await blogPostRespository.UpdateAsync(blogPostDomainModel);
            if(updatedBlog != null)
            {
                //Show success notification
                return RedirectToAction("Edit");
            }

            //Show error notification
            return RedirectToAction("Edit");
            // redirect to GET 
        }

        [HttpPost]
        public async Task<IActionResult> Delete(EditBlogPostRequest editBlogPostRequest)
        {
            // Talk to repository to delete this blog and tags
            var deletedBlogPost = await blogPostRespository.DeleteAsync(editBlogPostRequest.Id);
            if(deletedBlogPost != null)
            {
                //Show success notification
                return RedirectToAction("List");
            }
            //Show error notification
            return RedirectToAction("Edit", new { id = editBlogPostRequest.Id });

        }
    }
}
