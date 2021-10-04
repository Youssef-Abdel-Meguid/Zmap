using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Zmap.Models;
using Zmap.Dtos;

namespace Zmap.Controllers
{
    public class BlogsController : Controller
    {
        private ZmapEntities db = new ZmapEntities();

        private int? userType = 0;
        private int? userId = 0;

        private void SetIdenitiy()
        {
            if (Session["UserType"] != null)
            {
                string type = Session["UserType"] == null ? null : Session["UserType"].ToString();
                userType = int.Parse(type);

                string id = Session["UserId"] == null ? null : Session["UserId"].ToString();
                userId = int.Parse(id);
            }
        }

        public async Task<ActionResult> Index()
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in blog", Error = "not authorized", UserId = userId });
            }

            var blogs = new List<Post>();

            try
            {
                blogs = await db.Posts.Where(p => p.Active == true).ToListAsync();
            }
            catch (Exception e)
            {
                throw;
            }
            return View(blogs);
        }

        public async Task<ActionResult> Details(int? id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in blog", Error = "not authorized", UserId = userId });
            }

            if (id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in blog", Error = "id is null", UserId = userId });
            }

            var post = new Post();

            try
            {
                post = await db.Posts.FindAsync(id);
                if (post == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in blog", Error = "post is null", UserId = userId });
                }
            }
            catch (Exception e)
            {
                throw;
            }

            return View(post);
        }

        public async Task<ActionResult> Create()
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in blog", Error = "not authorized", UserId = userId });
            }

            var blogCreateDto = new BlogCreateDto();

            try
            {
                var cats = await db.PostCategories.Where(p => p.Active == true).ToListAsync();

                var allCats = new List<CategoryListDto>();

                foreach (var item in cats)
                {
                    allCats.Add(new CategoryListDto()
                    {
                        Id = item.Id,
                        Name = item.CategoryName,
                        IsChecked = false
                    });
                }

                Categories categories = new Categories()
                {
                    CategorList = allCats
                };

                blogCreateDto = new BlogCreateDto()
                {
                    Categories = categories
                };
            }
            catch (Exception e)
            {
                throw;
            }

            return View(blogCreateDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(BlogCreateDto blogCreateDto, HttpPostedFileBase uploadFile)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in blog", Error = "not authorized", UserId = userId });
            }

            try
            {
                if (ModelState.IsValid)
                {
                    if(uploadFile != null)
                        uploadFile.SaveAs(HttpContext.Server.MapPath("~/images/Blogs/" + uploadFile.FileName));

                    var post = new Post()
                    {
                        Details = blogCreateDto.Details,
                        Title = blogCreateDto.Title,
                        Active = true,
                        CreatedByUserId = userId,
                        CreatedDate = DateTime.Now,
                        Views = 0,
                        PostPhotoUrl = uploadFile != null ? uploadFile.FileName : null
                    };
                    db.Posts.Add(post);
                    await db.SaveChangesAsync();

                    foreach (var item in blogCreateDto.Categories.CategorList)
                    {
                        if(item.IsChecked)
                        {
                            db.PostsCategories.Add(new PostsCategory()
                            {
                                Active = true,
                                CreatedByUserId = userId,
                                CreatedDate = DateTime.Now,
                                PostId = post.Id,
                                CategoryId = item.Id,
                            });

                            await db.SaveChangesAsync();
                        }
                    }

                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                throw;
            }

            return View(blogCreateDto);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in blog", Error = "not authorized", UserId = userId });
            }

            if (id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in blog", Error = "id is null", UserId = userId });
            }

            var post = new Post();

            try
            {
                post = await db.Posts.FindAsync(id);
                if (post == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in blog", Error = "post is null", UserId = userId });
                }
            }
            catch (Exception e)
            {
                throw;
            }

            return View(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Post post)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in blog", Error = "not authorized", UserId = userId });
            }

            try
            {
                if (ModelState.IsValid)
                {
                    post.ModifiedDate = DateTime.Now;
                    post.ModifiedByUserId = userId;
                    db.Entry(post).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                throw;
            }

            return View(post);
        }

        public async Task<ActionResult> Delete(int? id)
        {
            SetIdenitiy();
            if (userId == 0 || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != 1)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in blog", Error = "not authorized", UserId = userId });
            }

            if (id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in blog", Error = "id is null", UserId = userId });
            }

            var post = new Post();

            try
            {
                post = await db.Posts.FindAsync(id);
                if (post == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in blog", Error = "post is null", UserId = userId });
                }

                post.Active = false;
                post.ModifiedDate = DateTime.Now;
                post.ModifiedByUserId = userId;
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw;
            }

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> ReadMore(int? id)
        {
            if(id == null)
            {
                return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in blog", Error = "id is null", UserId = userId });
            }

            var blogDetails = new BlogDetailsDto();

            try
            {
                var blog = await db.Posts.Where(p => p.Active == true && p.Id == id).FirstOrDefaultAsync();

                if(blog == null)
                {
                    return RedirectToAction("TechnicalSupport", "Home", new ErrorLogger() { ActionName = "Error in blog", Error = "blog is null", UserId = userId });
                }

                blog.Views = blog.Views + 1;
                await db.SaveChangesAsync();

                var user = await db.Users.FindAsync(blog.CreatedByUserId);
                var postCats = await db.PostsCategories.Where(p => p.Active == true && p.PostId == blog.Id).ToListAsync();
                var cats = new List<PostCategory>();

                foreach (var post in postCats)
                {
                    var singleCat = await db.PostCategories.FindAsync(post.CategoryId);
                    cats.Add(new PostCategory()
                    {
                        CategoryName = singleCat.CategoryName,
                        Id = singleCat.Id,
                        Active = singleCat.Active
                    });
                }

                blogDetails = new BlogDetailsDto()
                {
                    Categories = cats,
                    CreatedDate = blog.CreatedDate,
                    Details = blog.Details,
                    Id = blog.Id,
                    PhotoUrl = blog.PostPhotoUrl,
                    Title = blog.Title,
                    Username = user.UserName
                };

            }
            catch (Exception e)
            {
                throw;
            }

            return View(blogDetails);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}