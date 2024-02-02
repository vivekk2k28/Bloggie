using Bloggie.Web.Data;
using Bloggie.Web.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bloggie.Web.Repositories
{
    public class BlogPostRepository : IBlogPostRespository
    {
        private readonly BloggieDbContext bloggieDbContext;

        public BlogPostRepository(BloggieDbContext bloggieDbContext)
        {
            this.bloggieDbContext = bloggieDbContext;
        }

        public async Task<BlogPost> AddAsync(BlogPost blogPost)
        {
            await bloggieDbContext.AddAsync(blogPost);
            await bloggieDbContext.SaveChangesAsync();
            return blogPost;
        }

        async Task<BlogPost?> IBlogPostRespository.DeleteAsync(Guid id)
        {
            var existingBlog = await bloggieDbContext.BlogPosts.FindAsync(id);
            if (existingBlog != null)
            {
                bloggieDbContext.BlogPosts.Remove(existingBlog);
                await bloggieDbContext.SaveChangesAsync();
                return existingBlog;
            }
            return null;
        }

        public async Task<IEnumerable<BlogPost>> GetAllAsync()
        {
            return await bloggieDbContext.BlogPosts.Include(x => x.Tags).ToListAsync();
        }

        public async Task<BlogPost?> GetAsync(Guid id)
        {
            return await bloggieDbContext.BlogPosts.Include(x => x.Tags).FirstOrDefaultAsync(x => x.Id == id);    
        }

        public async Task<BlogPost?> UpdateAsync(BlogPost blogpost)
        {
            var existingBlog = await bloggieDbContext.BlogPosts.Include(x => x.Tags).FirstOrDefaultAsync(x => x.Id == blogpost.Id);

            if(existingBlog != null)
            {
                existingBlog.Id = blogpost.Id;
                existingBlog.Heading = blogpost.Heading;
                existingBlog.PageTitle = blogpost.PageTitle;
                existingBlog.Content = blogpost.Content;
                existingBlog.ShortDesciption = blogpost.ShortDesciption;
                existingBlog.Author = blogpost.Author;
                existingBlog.FeaturedImageUrl = blogpost.FeaturedImageUrl;
                existingBlog.UrlHandle = blogpost.UrlHandle;
                existingBlog.Visible = blogpost.Visible;
                existingBlog.PublishedDate = blogpost.PublishedDate;
                existingBlog.Tags = blogpost.Tags;

                await bloggieDbContext.SaveChangesAsync();
                return existingBlog;
            }
            return null;

        }

        public async Task<BlogPost?> GetByUrlHandleAsync(string urlHandle)
        {
            return await bloggieDbContext.BlogPosts.Include(x => x.Tags).FirstOrDefaultAsync(x => x.UrlHandle == urlHandle);
            
                    

            
        }
    }
}
