using prjSocialMediaPlatform.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Web.Helpers;
using System.Web.Mvc;

namespace prjGroupBuying.Controllers
{
    public class UserController : Controller
    {
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Register user)
        {
            if (ModelState.IsValid)
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

                string sqlStatement = @"select count(userid) from Users where userid = @UserId";

                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        using (SqlCommand command = new SqlCommand(sqlStatement, connection))
                        {
                            command.Parameters.AddWithValue("@UserId", user.UserID);

                            int count = Convert.ToInt32(command.ExecuteScalar());

                            if (count == 0)
                            {
                                string hashPassword = HashPassword(user.PasswordHash);

                                sqlStatement = @"SET IDENTITY_INSERT Users ON
                                                 insert into Users (UserID, UserName, Email, PasswordHash, CoverImage, Biography) 
                                                 values (@UserID, @UserName, @Email, @PasswordHash, @CoverImage, @Biography)
                                                 SET IDENTITY_INSERT Users OFF";

                                using (SqlCommand commandInsert = new SqlCommand(sqlStatement, connection))
                                {
                                    commandInsert.Parameters.Clear();
                                    commandInsert.Parameters.AddWithValue("@UserID", user.UserID);
                                    commandInsert.Parameters.AddWithValue("@UserName", user.UserName);
                                    commandInsert.Parameters.AddWithValue("@Email", user.Email);
                                    commandInsert.Parameters.AddWithValue("@PasswordHash", hashPassword);
                                    commandInsert.Parameters.AddWithValue("@CoverImage", user.CoverImage == null ? DBNull.Value.ToString() : user.CoverImage);
                                    commandInsert.Parameters.AddWithValue("@Biography", user.Biography == null ? DBNull.Value.ToString() : user.Biography);
                                    commandInsert.ExecuteNonQuery();
                                }

                                connection.Close();

                                return RedirectToAction("Login");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                }
            }

            ViewBag.Message = "請重新確認欄位內容!";
            return View(user);
        }

        public static string HashPassword(string password)
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] salt = new byte[16];
                rng.GetBytes(salt);

                using (var rfc2898 = new Rfc2898DeriveBytes(password, salt, 10000))
                {
                    byte[] key = rfc2898.GetBytes(32);

                    return Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(key);
                }
            }
        }

        public static bool VerifyPassword(string enteredPassword, string storedHash)
        {
            var parts = storedHash.Split(':');
            var salt = Convert.FromBase64String(parts[0]);
            var key = Convert.FromBase64String(parts[1]);

            using (var rfc2898 = new Rfc2898DeriveBytes(enteredPassword, salt, 10000))
            {
                byte[] enteredKey = rfc2898.GetBytes(32);

                return IsCryptographicEqual(key, enteredKey);
            }
        }

        private static bool IsCryptographicEqual(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
            {
                return false;
            }

            int result = 0;
            for (int i = 0; i < a.Length; i++)
            {
                result |= a[i] ^ b[i];
            }

            return result == 0;
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LogIn user)
        {
            if (ModelState.IsValid)
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

                string sqlStatement = @"SELECT UserID, UserName, PasswordHash FROM Users WHERE UserID = @UserId";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(sqlStatement, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", "0" + user.UserID);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string userName = reader.GetString(reader.GetOrdinal("UserName"));
                                string storedHash = reader.GetString(reader.GetOrdinal("PasswordHash"));

                                bool isPass = VerifyPassword(user.PasswordHash, storedHash);

                                if (!isPass)
                                {
                                    ViewBag.Message = "請再次確認用戶密碼!";
                                    return View(user);
                                }

                                Session["UserName"] = userName;
                                Session["UserID"] = user.UserID;

                                return RedirectToAction("Index", "Home");
                            }
                            else
                            {
                                ViewBag.Message = "使用者不存在或密碼錯誤!";
                                return View(user);
                            }
                        }
                    }
                }
            }

            ViewBag.Message = "請重新確認填寫內容!";
            return View(user);
        }


        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "User");
        }
    }
}