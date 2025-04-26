using prjSocialMediaPlatform.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace prjSocialMediaPlatform.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            List<Post> posts = new List<Post>();

            string sqlStatement = @"select content, Image, CreatedAt, UserId FROM Posts order by CreatedAt desc";

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
                                PostID = int.Parse(userId.ToString().Substring(6) + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString().Substring(8)),
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
                string sqlStatement = @"SET IDENTITY_INSERT Users ON
                                        insert into Users (postid, userid, content, image, createdat) 
                                        values (@postid, @userid, @content, @image, @createdat)
                                        SET IDENTITY_INSERT Users OFF";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(sqlStatement, connection))
                    {
                        using (SqlCommand commandInsert = new SqlCommand(sqlStatement, connection))
                        {
                            commandInsert.Parameters.Clear();
                            commandInsert.Parameters.AddWithValue("@postid", post.PostID);
                            commandInsert.Parameters.AddWithValue("@userid", Session["UserID"].ToString());
                            commandInsert.Parameters.AddWithValue("@content", post.Content);
                            commandInsert.Parameters.AddWithValue("@image", post.Image);
                            commandInsert.Parameters.AddWithValue("@createdat", post.CreatedAt);
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

        public ActionResult Comment()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Comment(Comment comment)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            try
            {
                string sqlStatement = @"SET IDENTITY_INSERT Users ON
                                        insert into Users (commentid, userid, postid, content, createdat) 
                                        values (@commentid, @userid, @postid, @content, @createdat)
                                        SET IDENTITY_INSERT Users OFF";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(sqlStatement, connection))
                    {
                        using (SqlCommand commandInsert = new SqlCommand(sqlStatement, connection))
                        {
                            commandInsert.Parameters.Clear();
                            commandInsert.Parameters.AddWithValue("@commentid", comment.CommentID);
                            commandInsert.Parameters.AddWithValue("@userid", Session["UserId"].ToString());
                            commandInsert.Parameters.AddWithValue("@postid", comment.PostID);
                            commandInsert.Parameters.AddWithValue("@content", comment.Content);
                            commandInsert.Parameters.AddWithValue("@createdat", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
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
    }
}