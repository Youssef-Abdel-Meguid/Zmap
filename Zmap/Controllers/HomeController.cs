using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Zmap.Dtos;
using Zmap.Models;

namespace Zmap.Controllers
{
    public class HomeController : Controller
    {

        private readonly ZmapEntities db = new ZmapEntities();


        public async Task<ActionResult> Services()
        {
            var services = new ServicesDto();

            try
            {
                var ourService = await db.OurServices.FirstOrDefaultAsync();
                services = new ServicesDto()
                {
                    id = ourService.Id,
                    Title = ourService.Title,
                    Details = ourService.Details,
                    Services = await db.Services.Where(s => s.Active == true).ToListAsync()
                };
            }
            catch (Exception e)
            {
                throw;
            }

            return View(services);
        }

        private async Task<List<BlogDetailsDto>> GetBlogDetailsAsync()
        {
            List<BlogDetailsDto> blogs = new List<BlogDetailsDto>();
            var posts = await db.Posts.Where(p => p.Active == true).ToListAsync();

            foreach (var post in posts)
            {
                var cats = new List<PostCategory>();


                var user = await db.Users.FindAsync(post.CreatedByUserId);
                var postsCats = await db.PostsCategories.Where(p => p.Active == true && p.PostId == post.Id).ToListAsync();

                foreach (var cat in postsCats)
                    cats.Add(await db.PostCategories.FindAsync(cat.CategoryId));


                blogs.Add(new BlogDetailsDto()
                {
                    Id = post.Id,
                    CreatedDate = post.CreatedDate,
                    Title = post.Title,
                    Details = post.Details,
                    Username = user.UserName,
                    Categories = cats,
                    PhotoUrl = post.PostPhotoUrl,
                });
            }

            return blogs;
        }

        private async Task<Categories> GetCategoriesAsync()
        {
            var catList = new List<CategoryListDto>();
            var allCats = await db.PostCategories.Where(p => p.Active == true).ToListAsync();
            foreach (var cat in allCats)
            {
                catList.Add(new CategoryListDto()
                {
                    Id = cat.Id,
                    Name = cat.CategoryName,
                    IsChecked = false
                });
            }

            return new Categories() { CategorList = catList };
        }

        public async Task<ActionResult> Blogs()
        {
            BlogsDto blogsDto = new BlogsDto();
            try
            {
                blogsDto = new BlogsDto()
                {
                    BlogDetails = await GetBlogDetailsAsync(),
                    Categories = await GetCategoriesAsync()
                };

            }
            catch (Exception e)
            {
                throw;
            }

            return View(blogsDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Blogs(Categories categories)
        {
            BlogsDto blogsDto = new BlogsDto();
            try
            {
                bool isIn = false;
                var allPosts = await db.Posts.Where(p => p.Active == true).ToListAsync();
                int maxIndex = 0;

                if(allPosts.Count > 0)
                    maxIndex = allPosts[allPosts.Count - 1].Id;

                bool[] visited = new bool[maxIndex + 1];

                for (int i = 0; i < visited.Length; i++)
                    visited[i] = false;

                List<BlogDetailsDto> blogDetails = new List<BlogDetailsDto>();
                foreach (var item in categories.CategorList)
                {
                    if(item.IsChecked)
                    {
                        isIn = true;
                        var catPosts = await db.PostsCategories.Where(p => p.Active == true && p.CategoryId == item.Id).ToListAsync();

                        foreach (var catPost in catPosts)
                        {
                            if(visited[(int)catPost.PostId] == false)
                            {
                                visited[(int)catPost.PostId] = true;

                                var post = await db.Posts.FindAsync(catPost.PostId);
                                var user = await db.Users.FindAsync(post.CreatedByUserId);
                                var allPostCats = await db.PostsCategories.Where(p => p.Active == true && p.PostId == post.Id).ToListAsync();
                                var postCategories = new List<PostCategory>();

                                foreach (var cat in allPostCats)
                                    postCategories.Add(await db.PostCategories.FindAsync(cat.CategoryId));

                                blogDetails.Add(new BlogDetailsDto() 
                                {
                                    Id = post.Id,
                                    Details = post.Details,
                                    PhotoUrl = post.PostPhotoUrl,
                                    Title = post.Title,
                                    Username = user.UserName,
                                    CreatedDate = post.CreatedDate,
                                    Categories = postCategories
                                });
                            }
                        }
                    }
                }

                if(isIn)
                {
                    blogsDto = new BlogsDto()
                    {
                        BlogDetails = blogDetails,
                        Categories = categories
                    };
                }
                else
                {
                    blogsDto = new BlogsDto()
                    {
                        BlogDetails = await GetBlogDetailsAsync(),
                        Categories = categories
                    };
                }
            }
            catch (Exception e)
            {
                throw;
            }

            return View(blogsDto);
        }

        public ActionResult Trips()
        {
            return View();
        }

        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> About()
        {
            var about = new AboutU();
            try
            {
                about = await db.AboutUs.FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                throw;
            }
            return View(about);
        }

        public async Task<ActionResult> Contact()
        {

            var contact = new ContactU();
            try
            {
                contact = await db.ContactUs.FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                throw;
            }
            return View(contact);
        }

        public async Task<ActionResult> TechnicalSupport(ErrorLogger errorLogger)
        {

            if (MvcApplication.isIn)
            {
                MvcApplication.isIn = false;
                return View();
            }

            if (string.IsNullOrEmpty(errorLogger.ActionName))
                return RedirectToAction("Index", "Home");

            try
            {
                errorLogger.CreatedDate = DateTime.Now;
                db.ErrorLoggers.Add(errorLogger);
                await db.SaveChangesAsync();
            }
            catch (Exception)
            {
                return View();
            }

            return View();
        }
    }
}