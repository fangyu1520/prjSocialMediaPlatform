using prjSocialMediaPlatform.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace prjSocialMediaPlatform.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            List<Post> posts = new List<Post>();

            string sqlStatement = @"select content, Image, CreatedAt, UserId, postId FROM Posts order by CreatedAt desc";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(sqlStatement, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int userId = reader.GetInt32(reader.GetOrdinal("UserID"));
                            string userName = string.Empty;

                            using (SqlConnection connectionUserName = new SqlConnection(connectionString))
                            {
                                connectionUserName.Open();

                                sqlStatement = "select username from Users where userid = @UserId";

                                using (SqlCommand commandUserName = new SqlCommand(sqlStatement, connectionUserName))
                                {
                                    commandUserName.Parameters.Clear();
                                    commandUserName.Parameters.AddWithValue("UserId", "0" + userId.ToString());
                                    userName = commandUserName.ExecuteScalar().ToString();
                                }
                            }

                            Post post = new Post
                            {
                                UserName = userName,
                                PostID = (int)reader.GetInt64(reader.GetOrdinal("PostId")),
                                Content = reader.GetString(reader.GetOrdinal("Content")),
                                Image = reader.IsDBNull(reader.GetOrdinal("Image")) ? string.Empty : reader.GetString(reader.GetOrdinal("Image")),
                                CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                            };
                            posts.Add(post);
                        }
                    }
                }

                return View(posts);
            }
        }

        public ActionResult CreatePost()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreatePost(Post post)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            try
            {
                string sqlStatement = @"SET IDENTITY_INSERT Posts ON
                                        insert into Posts (PostId, UserId, Content, Image, CreatedAt) 
                                        values (@PostId, @UserId, @Content, @Image, @CreatedAt)
                                        SET IDENTITY_INSERT Posts OFF";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(sqlStatement, connection))
                    {
                        using (SqlCommand commandInsert = new SqlCommand(sqlStatement, connection))
                        {
                            commandInsert.Parameters.Clear();
                            commandInsert.Parameters.AddWithValue("@PostId", int.Parse(Session["UserId"].ToString().Substring(6) + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString().Substring(8)));
                            commandInsert.Parameters.AddWithValue("@UserId", Session["UserID"].ToString());
                            commandInsert.Parameters.AddWithValue("@Content", post.Content);
                            commandInsert.Parameters.AddWithValue("@Image", post.Image == null ? DBNull.Value.ToString() : post.Image);
                            commandInsert.Parameters.AddWithValue("@CreatedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                            commandInsert.ExecuteNonQuery();
                        }

                        connection.Close();

                        return RedirectToAction("index");
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
            }

            return View();
        }

        [HttpGet]
        public ActionResult Comment(int id)
        {
            var post = GetPostById(id);
            return View(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Comment(Comment comment, Post post)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            try
            {
                string sqlStatement = @"SET IDENTITY_INSERT Comments ON
                                        insert into Comments (commentid, userid, postid, content, createdat) 
                                        values (@commentid, @userid, @postid, @content, @createdat)
                                        SET IDENTITY_INSERT Comments OFF";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(sqlStatement, connection))
                    {
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@commentid", int.Parse(Session["UserId"].ToString().Substring(6) + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString().Substring(8)));
                        command.Parameters.AddWithValue("@userid", Session["UserId"].ToString());
                        command.Parameters.AddWithValue("@postid", comment.PostID);
                        command.Parameters.AddWithValue("@content", comment.Content);
                        command.Parameters.AddWithValue("@createdat", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        command.ExecuteNonQuery();
                    }

                    connection.Close();

                    return RedirectToAction("index");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
            }

            return View();
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var post = GetPostById(id);
            return View(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Post post)
        {
            if (ModelState.IsValid)
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

                try
                {
                    string sqlStatement = @"SET IDENTITY_INSERT Posts ON
                                            update Posts set Content = @Content, Image = @Image, CreatedAt = @CreatedAt where PostId = @PostId
                                            SET IDENTITY_INSERT Posts OFF";

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        using (SqlCommand command = new SqlCommand(sqlStatement, connection))
                        {
                            using (SqlCommand commandInsert = new SqlCommand(sqlStatement, connection))
                            {
                                commandInsert.Parameters.Clear();
                                commandInsert.Parameters.AddWithValue("@PostId", post.PostID);
                                commandInsert.Parameters.AddWithValue("@Content", post.Content);
                                commandInsert.Parameters.AddWithValue("@Image", post.Image == null ? DBNull.Value.ToString() : post.Image);
                                commandInsert.Parameters.AddWithValue("@CreatedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                commandInsert.ExecuteNonQuery();
                            }

                            connection.Close();

                            return RedirectToAction("index");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                }

                return RedirectToAction("Index");
            }
            return View(post);
        }

        public Post GetPostById(int id)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            string sqlStatement = @"select content, Image, CreatedAt, UserId, postId FROM Posts where postid = @PostId";

            Post post = new Post();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(sqlStatement, connection))
                {
                    command.Parameters.AddWithValue("@PostId", id);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            post = new Post
                            {
                                UserName = Session["UserName"].ToString(),
                                PostID = (int)reader.GetInt64(reader.GetOrdinal("PostId")),
                                Content = reader.GetString(reader.GetOrdinal("Content")),
                                Image = reader.IsDBNull(reader.GetOrdinal("Image")) ? string.Empty : reader.GetString(reader.GetOrdinal("Image")),
                                CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                            };
                        }
                    }
                }
            }

            return post;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            try
            {
                string sqlStatement = @"delete Posts where PostId = @PostId";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(sqlStatement, connection))
                    {
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@PostId", id);
                        command.ExecuteNonQuery();
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
            }

            return RedirectToAction("Index");
        }

    }
}