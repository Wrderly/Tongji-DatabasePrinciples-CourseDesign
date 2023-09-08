using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.IO;
using System.Text.Json;
using System.Xml.Linq;
using static System.Reflection.Metadata.BlobBuilder;


namespace webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminApiController : ControllerBase
    {
        //public static string adminConnectionStr;
        private static OracleHelper db;
        private string? adminID;
        private string? adminPwd;
        XDocument doc = XDocument.Load(PublicData.programPath + "\\AdminApiSQL.xml");
        private void InitDB()
        {
            string jsonFromFile = System.IO.File.ReadAllText(Path.Combine(PublicData.programPath, "config.json"));
            Dictionary<string, string>? configFromFile = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonFromFile);
            string dataSourse = configFromFile["dataSource"];

            adminID = configFromFile["managerID"];
            adminPwd = configFromFile["managerPwd"];
            db = new OracleHelper(
                "DATA SOURCE=" + dataSourse + ";" +
                "USER ID='\"" + adminID + "\"';" +
                "PASSWORD='" + adminPwd + "'");
        }


        /// <summary>
        /// 管理员登录 API
        /// </summary>
        /// <param name="userData">包含管理员登录信息的 JSON 数据</param>
        [HttpPost("login")]
        public IActionResult AdminLogin([FromBody] JObject userData)
        {
            if (db == null)
            {
                InitDB();
            }
            try
            {
                // 解析从前端接收的登录数据
                string admin_name = userData["admin_name"].ToString();
                string password = userData["password"].ToString(); // 应该在前端加密密码
                
                string sql = $"SELECT admin_id FROM Administrator WHERE admin_name = '{admin_name}'";

                DataSet result = db.OracleQuery(sql);

                if (!(result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0))
                {
                    // 登录失败
                    return Ok(new
                    {
                        msg = "用户名不存在"
                    });
                }

                XDocument doc = XDocument.Load(PublicData.programPath + "\\AdminApiSQL.xml");
                string tempSql = doc.Root.Element("AdminLogin").Value;
                sql = tempSql.Replace("{admin_name}", userData["admin_name"].ToString())
                             .Replace("{password}", userData["password"].ToString());
                result = db.OracleQuery(sql);

                if (result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
                {
                    // 登录成功
                    DataRow data = result.Tables[0].Rows[0];
                    return Ok(new
                    {
                        status = 200,
                        admin_id = data["admin_id"].ToString(),
                        admin_name = data["admin_name"].ToString(),
                        phone_number = data["phone_number"].ToString(),
                        email = data["email"].ToString(),
                        isAdmin = true
                    });
                }
                else
                {
                    // 登录失败
                    return Ok(new
                    {
                        msg = "管理员名称或密码错误"
                    });
                }

            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    msg = "管理员登录失败：" + ex.Message
                });
            }
        }


        /// <summary>
        /// 获取管理员资料 API
        /// </summary>
        /// <param name="userData">包含管理员ID</param>
        [HttpPost("initadmin")]
        public IActionResult InitAdmin([FromBody] JObject userData)
        {
            if (db == null)
            {
                InitDB();
            }
            try
            {
                string admin_id = userData["admin_id"].ToString();
                // 构建 SQL 查询或调用服务层获取用户资料
                string sql = $"SELECT * FROM Administrator WHERE admin_id = '{admin_id}'";

                DataSet result = db.OracleQuery(sql);

                if (result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
                {
                    // 提取用户信息
                    DataRow data = result.Tables[0].Rows[0];

                    // 构建用户数据对象

                    return Ok(new
                    {
                        status = 200,
                        admin_id = data["admin_id"].ToString(),
                        admin_name = data["admin_name"].ToString(),
                        phone_number = data["phone_number"].ToString(),
                        email = data["email"].ToString(),
                        isAdmin = true
                    });
                }
                else
                {
                    // 用户不存在
                    return Ok(new
                    {
                        msg = "用户不存在"
                    });
                }
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    msg = "获取用户资料失败：" + ex.Message
                });
            }
        }

        /// <summary>
        /// 更新管理员密码
        /// </summary>
        /// <param name="userData">包含用户ID信息</param>
        [HttpPost("updatepassword")]
        public IActionResult UpdatePassword([FromBody] JObject userData)
        {
            if (db == null)
            {
                InitDB();
            }
            try
            {
                // 解析从前端接收的用户数据
                string adminId = userData["admin_id"].ToString();
                string oldPassword = userData["oldPassword"].ToString();
                string newPassword = userData["newPassword"].ToString();
                string confirmNewPassword = userData["confirmNewPassword"].ToString();

                string sql = $"SELECT password FROM Administrator WHERE admin_id = '{adminId}'";
                DataSet result = db.OracleQuery(sql);

                if (result.Tables[0].Rows[0]["password"].ToString() != oldPassword)
                {
                    return Ok(new
                    {
                        msg = "旧密码错误"
                    });
                }

                // 构建 SQL 查询或调用服务层更新用户密码
                sql = $"UPDATE Administrator SET " +
      $"password = '{newPassword}' " +
      $"WHERE admin_id = '{adminId}'";

                db.OracleUpdate(sql);

                return Ok(new
                {
                    status = 200,
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    msg = "密码修改失败：" + ex.Message
                });
            }
        }


        /// <summary>
        /// 获取用户列表 API
        /// </summary>
        /// <param name="userData">包含管理员ID</param>
        [HttpPost("initreaderlist")]
        public IActionResult InitreaderList()
        {
            if (db == null)
            {
                InitDB();
            }
            try
            {
                // 构建 SQL 查询或调用服务层获取用户资料
                string sql = $"SELECT * FROM Reader";

                DataSet result = db.OracleQuery(sql);

                if (result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
                {
                    // 提取用户信息
                    DataTable dataTable = result.Tables[0];
                    List<Dictionary<string, string>> readers = new List<Dictionary<string, string>>();

                    foreach (DataRow row in dataTable.Rows)
                    {
                        Dictionary<string, string> reader = new Dictionary<string, string>();

                        foreach (DataColumn column in dataTable.Columns)
                        {
                            reader[column.ColumnName] = row[column].ToString();
                        }

                        readers.Add(reader);
                    }

                    return Ok(new
                    {
                        status = 200,
                        readers
                    });
                }
                else
                {
                    // 用户不存在
                    return Ok(new
                    {
                        status = 0,
                    });
                }
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    msg = "获取用户资料失败：" + ex.Message
                });
            }
        }

        /// <summary>
        /// 用户违规次数 API
        /// </summary>
        /// <param name="userData">包含用户id</param>
        [HttpPost("updatecount")]
        public IActionResult UpdateCount([FromBody] JObject userData)
        {
            if (db == null)
            {
                InitDB();
            }

            try
            {
                // 解析从前端接收的用户id
                string readerId = userData["reader_id"].ToString();
                string Overdue_times = userData["overdue_times"].ToString();
                int overdue_times = Convert.ToInt32(Overdue_times);
                string UpdateCountSql = $"Update Reader SET overdue_times = {overdue_times} WHERE reader_id = '{readerId}'";
                db.OracleUpdate(UpdateCountSql);

                return Ok(new
                {
                    status = 200,
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    msg = "修改失败：" + ex.Message
                });
            }
        }



        /// <summary>
        /// 获取反馈列表 API
        /// </summary>
        /// <param name="userData">包含管理员ID</param>
        [HttpPost("initreportlist")]
        public IActionResult InitReportList()
        {
            if (db == null)
            {
                InitDB();
            }
            try
            {
                // 构建 SQL 查询或调用服务层获取用户资料
                string sql = $"SELECT * FROM Report";

                DataSet result = db.OracleQuery(sql);

                if (result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
                {
                    // 提取用户信息
                    DataTable dataTable = result.Tables[0];
                    List<Dictionary<string, string>> reports = new List<Dictionary<string, string>>();

                    foreach (DataRow row in dataTable.Rows)
                    {
                        Dictionary<string, string> report = new Dictionary<string, string>();

                        foreach (DataColumn column in dataTable.Columns)
                        {
                            report[column.ColumnName] = row[column].ToString();
                        }

                        reports.Add(report);
                    }

                    return Ok(new
                    {
                        status = 200,
                        reports
                    });
                }
                else
                {
                    // 用户不存在
                    return Ok(new
                    {
                        status = 0,
                    });
                }
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    msg = "获取反馈列表失败：" + ex.Message
                });
            }
        }


        /// <summary>
        /// 获取反馈列表 API
        /// </summary>
        /// <param name="userData">包含管理员ID</param>
        [HttpPost("initreservelist")]
        public IActionResult InitReservelist()
        {
            if (db == null)
            {
                InitDB();
            }
            try
            {
                // 构建 SQL 查询或调用服务层获取用户资料
                string sql = $"SELECT R.*, B.book_name, B.author FROM Reserve R JOIN Book B ON R.book_id = B.book_id WHERE R.message = '逾期未取'";

                DataSet result = db.OracleQuery(sql);

                if (result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
                {
                    // 提取用户信息
                    DataTable dataTable = result.Tables[0];
                    List<Dictionary<string, string>> reservelists = new List<Dictionary<string, string>>();

                    foreach (DataRow row in dataTable.Rows)
                    {
                        Dictionary<string, string> reservelist = new Dictionary<string, string>();

                        foreach (DataColumn column in dataTable.Columns)
                        {
                            reservelist[column.ColumnName] = row[column].ToString();
                        }

                        reservelists.Add(reservelist);
                    }

                    return Ok(new
                    {
                        status = 200,
                        reservelists
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = 0,
                    });
                }
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    msg = "获取预约列表失败：" + ex.Message
                });
            }
        }


        /// <summary>
        /// 获取反馈列表 API
        /// </summary>
        /// <param name="userData">包含管理员ID</param>
        [HttpPost("initborrowslist")]
        public IActionResult InitBorrowslist()
        {
            if (db == null)
            {
                InitDB();
            }
            try
            {
                // 构建 SQL 查询或调用服务层获取用户资料
                string sql = $"SELECT R.*, B.book_name, B.author FROM BorrowRecord R JOIN Book B ON R.book_id = B.book_id WHERE R.message = '逾期未还'";

                DataSet result = db.OracleQuery(sql);

                if (result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
                {
                    // 提取用户信息
                    DataTable dataTable = result.Tables[0];
                    List<Dictionary<string, string>> borrowslists = new List<Dictionary<string, string>>();

                    foreach (DataRow row in dataTable.Rows)
                    {
                        Dictionary<string, string> borrowslist = new Dictionary<string, string>();

                        foreach (DataColumn column in dataTable.Columns)
                        {
                            borrowslist[column.ColumnName] = row[column].ToString();
                        }

                        borrowslists.Add(borrowslist);
                    }

                    return Ok(new
                    {
                        status = 200,
                        borrowslists
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = 0,
                    });
                }
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    msg = "获取借阅列表失败：" + ex.Message
                });
            }
        }













        [HttpGet("insertbook")]
        public IActionResult InsertBook([FromBody] JObject data)
        {
            if (db == null)
            {
                InitDB();
            }
            XDocument doc = XDocument.Load(PublicData.programPath + "\\AdminApiSQL.xml");

            string tempSql = doc.Root.Element("InsertIntoBook").Value;

            string sql = tempSql.Replace("{book_id}", data["book_id"].ToString())
                                .Replace("{num}", data["num"].ToString())
                                .Replace("{ISBN}", data["ISBN"].ToString())
                                .Replace("{book_name}", data["book_name"].ToString())
                                .Replace("{author}", data["author"].ToString())
                                .Replace("{publisher}", data["publisher"].ToString())
                                .Replace("{publication_date}", data["publication_date"].ToString())
                                .Replace("{introduction}", data["introduction"].ToString())
                                .Replace("{cover_picture}", data["cover_picture"].ToString())
                                .Replace("{price}", data["price"].ToString())
                                .Replace("{subject_type}", data["subject_type"].ToString())
                                .Replace("{collection_type}", data["collection_type"].ToString());

            try
            {
                db.OracleUpdate(sql);
                return Ok(new
                {
                    result = 1
                });
            }
            catch
            {
                return Ok(new
                {
                    result = 0
                });
            }
        }

        [HttpGet("selectbookbyname")]
        public IActionResult SelectBookByName([FromBody] JObject data)
        {
            if (db == null)
            {
                InitDB();
            }
            /**
            string book_name = data["book_name"].ToString();
            string subject_type = data["subject_type"].ToString();
            string collection_type = data["collection_type"].ToString();
            string ISBN = data["ISBN"].ToString();


            string sql = "SELECT *" +
                         "FROM BOOK" +
                         $"WHERE BOOK.book_name = {book_name}" +
                         (subject_type == "default" ? "" : $"AND BOOK.subject_type = {subject_type}") +
                         (collection_type == "default" ? "" : $"AND BOOK.collection_type = {collection_type}") +
                         (ISBN == "default" ? "" : $"AND BOOK.ISBN = {ISBN}");
            /**/

            string book_name = data["book_name"].ToString();
            string subject_type = data["subject_type"].ToString();
            string collection_type = data["collection_type"].ToString();
            string ISBN = data["ISBN"].ToString();

            List<string> whereClauses = new List<string>();
            whereClauses.Add($"BOOK.book_name = '{book_name}'");
            if (subject_type != "default") whereClauses.Add($"BOOK.subject_type = '{subject_type}'");
            if (collection_type != "default") whereClauses.Add($"BOOK.collection_type = '{collection_type}'");
            if (ISBN != "default") whereClauses.Add($"BOOK.ISBN = '{ISBN}'");

            string sql =
                "SELECT * FROM BOOK WHERE " +
                string.Join(" AND ", whereClauses);



            try
            {
                DataSet result = db.OracleQuery(sql);
                return Ok(new
                {
                    result = true,
                    dataset = result
                });
            }
            catch
            {
                return Ok(new
                {
                    result = false
                });
            }
        }

        [HttpGet("updatebook")]
        public IActionResult UpdateBook([FromBody] JObject data)
        {
            if (db == null)
            {
                InitDB();
            }

            /**
            string book_id = data["book_id"].ToString();
            string num = data["num"].ToString();
            string ISBN = data["ISBN"].ToString();
            string book_name = data["book_name"].ToString();
            string author = data["author"].ToString();
            string publisher = data["publisher"].ToString();
            string publication_date = data["publication_date"].ToString();
            string introduction = data["introduction"].ToString();
            string cover_picture = data["cover_picture"].ToString();
            string price = data["price"].ToString();
            string subject_type = data["subject_type"].ToString();
            string collection_type = data["collection_type"].ToString();

            string sql =
                "UPDATE BOOK " +
                "SET " +
                (num == "default" ? "" : $"num = {num},") +
                (ISBN == "default" ? "" : $"ISBN = {ISBN},") +
                (book_name == "default" ? "" : $"book_name = {book_name},") +
                (author == "default" ? "" : $"author = {author},") +
                (publisher == "default" ? "" : $"publisher = {publisher},") +
                (publication_date == "default" ? "" : $"publication_date = {publication_date},") +
                (introduction == "default" ? "" : $"introduction = {introduction},") +
                (cover_picture == "default" ? "" : $"cover_picture = {cover_picture},") +
                (price == "default" ? "" : $"price = {price},") +
                (subject_type == "default" ? "" : $"subject_type = {subject_type},") +
                (collection_type == "default" ? "" : $"collection_type = {collection_type}") +
                $"WHERE book_id = {book_id}";
            /**/

            string book_id = data["book_id"].ToString();
            string num = data["num"].ToString();
            string ISBN = data["ISBN"].ToString();
            string book_name = data["book_name"].ToString();
            string author = data["author"].ToString();
            string publisher = data["publisher"].ToString();
            string publication_date = data["publication_date"].ToString();
            string introduction = data["introduction"].ToString();
            string cover_picture = data["cover_picture"].ToString();
            string price = data["price"].ToString();
            string subject_type = data["subject_type"].ToString();
            string collection_type = data["collection_type"].ToString();

            List<string> setClauses = new List<string>();
            if (num != "default") setClauses.Add($"num = '{num}'");
            if (ISBN != "default") setClauses.Add($"ISBN = '{ISBN}'");
            if (book_name != "default") setClauses.Add($"book_name = '{book_name}'");
            if (author != "default") setClauses.Add($"author = '{author}'");
            if (publisher != "default") setClauses.Add($"publisher = '{publisher}'");
            if (publication_date != "default") setClauses.Add($"publication_date = '{publication_date}'");
            if (introduction != "default") setClauses.Add($"introduction = '{introduction}'");
            if (cover_picture != "default") setClauses.Add($"cover_picture = '{cover_picture}'");
            if (price != "default") setClauses.Add($"price = '{price}'");
            if (subject_type != "default") setClauses.Add($"subject_type = '{subject_type}'");
            if (collection_type != "default") setClauses.Add($"collection_type = '{collection_type}'");

            string sql =
                "UPDATE BOOK SET " +
                string.Join(", ", setClauses) +
                $" WHERE book_id = '{book_id}'";


            try
            {
                db.OracleUpdate(sql);
                return Ok(new
                {
                    result = 1
                });
            }
            catch
            {
                return Ok(new
                {
                    result = 0
                });
            }
        }

        [HttpGet("getborrowrecord")]
        public IActionResult getBorrowRecord([FromBody] JObject data)
        {
            if (db == null)
            {
                InitDB();
            }
            string reader_id = data["reader_id"].ToString();
            string sql =
                "SELECT reader_id,book_id,book_name,borrow_date,return_date" +
                "FROM BorrowRecord as BR,Book as B,ReturnRecord as RR" +
                "WHERE BR.book_id = B.book_id AND BR.book_id = RR.book_id" + reader_id == "default" ? "" : (" AND BR.raeder_id = " + reader_id);

            try
            {
                DataSet result = db.OracleQuery(sql);
                return Ok(new
                {
                    result = true,
                    dataset = result
                });
            }
            catch
            {
                return Ok(new
                {
                    result = false
                });
            }
        }
    }
}
