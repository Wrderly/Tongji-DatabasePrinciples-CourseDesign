using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.IO;
using System.Text.Json;
using System.Xml.Linq;


namespace webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserApiController : ControllerBase
    {
        private static OracleHelper db;
        private string? userID;
        private string? userPwd;
        private void InitDB()
        {
            string jsonFromFile = System.IO.File.ReadAllText(Path.Combine(PublicData.programPath, "config.json"));
            Dictionary<string, string>? configFromFile = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonFromFile);
            string dataSourse = configFromFile["dataSource"];

            userID = configFromFile["managerID"];
            userPwd = configFromFile["managerPwd"];
            db = new OracleHelper(
                "DATA SOURCE=" + dataSourse + ";" +
                "USER ID='\"" + userID + "\"';" +
                "PASSWORD='" + userPwd + "'");
        }

        /// <summary>
        /// 用户注册 API
        /// </summary>
        /// <param name="userData">包含用户注册信息的 JSON 数据</param>
        [HttpPost("register")]
        public IActionResult RegisterUser([FromBody] JObject userData)
        {
            if (db == null)
            {
                InitDB();
            }
            if (db == null)
                Console.WriteLine("11111");
            else
                Console.WriteLine("22222");
            try
            {
                // 解析从前端接收的用户数据
                string readerName = userData["reader_name"].ToString();
                string phoneNumber = userData["phone_number"].ToString();
                string email = userData["email"].ToString();
                string address = "default";
                string readerType = "default";
                string account = "default";
                string password = userData["password"].ToString(); // 应该在前端加密密码

                string checkUsernameSql = $"SELECT COUNT(*) FROM Reader WHERE reader_name = '{readerName}'";

                // 执行查询
                DataSet result = db.OracleQuery(checkUsernameSql);

                // 检查查询结果中的记录数
                int count = Convert.ToInt32(result.Tables[0].Rows[0][0]);

                // 如果 count 大于 0，表示用户名已存在，返回相应的错误消息
                if (count > 0)
                {
                    return Ok(new
                    {
                        //status = 200,//
                        result = false,
                        msg = "用户名已存在"
                    });
                }

                // 构建SQL查询或调用服务层注册用户
                string sql = $"INSERT INTO Reader (reader_id, reader_name, phone_number, email, address, reader_type, account, password) " +
                             $"VALUES (reader_id_seq.nextval, '{readerName}', '{phoneNumber}', '{email}', '{address}', '{readerType}', '{account}', '{password}')";

                Console.WriteLine(sql);

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
                    msg = "用户注册失败：" + ex.Message
                });
            }
        }

        /// <summary>
        /// 用户登录 API
        /// </summary>
        /// <param name="loginData">包含用户登录信息的 JSON 数据</param>
        [HttpPost("login")]
        public IActionResult UserLogin([FromBody] JObject loginData)
        {
            try
            {
                // 解析从前端接收的登录数据
                string account = loginData["account"].ToString();
                string password = loginData["password"].ToString(); // 应该在前端加密密码

                // 构建 SQL 查询或调用服务层进行用户登录验证
                string sql = $"SELECT reader_id FROM Reader WHERE account = '{account}' AND password = '{password}'";

                DataSet result = db.OracleQuery(sql);

                if (result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
                {
                    // 登录成功
                    string readerId = result.Tables[0].Rows[0]["reader_id"].ToString();
                    return Ok(new
                    {
                        result = true,
                        msg = "用户登录成功",
                        reader_id = readerId
                    });
                }
                else
                {
                    // 登录失败
                    return Ok(new
                    {
                        result = false,
                        msg = "用户名或密码错误"
                    });
                }
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    result = false,
                    msg = "用户登录失败：" + ex.Message
                });
            }
        }

        // 添加其他用户相关的 API 方法，例如获取用户资料、更新用户资料等

        /// <summary>
        /// 获取用户资料 API
        /// </summary>
        /// <param name="reader_id">用户ID</param>
        [HttpGet("initreader/{reader_id}")]
        public IActionResult GetUserProfile(string reader_id)
        {
            try
            {
                // 构建 SQL 查询或调用服务层获取用户资料
                string sql = $"SELECT * FROM Reader WHERE reader_id = '{reader_id}'";

                DataSet result = db.OracleQuery(sql);

                if (result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
                {
                    // 提取用户信息
                    DataRow userData = result.Tables[0].Rows[0];

                    // 构建用户数据对象
                    var user = new
                    {
                        reader_id = userData["reader_id"].ToString(),
                        reader_name = userData["reader_name"].ToString(),
                        phone_number = userData["phone_number"].ToString(),
                        email = userData["email"].ToString(),
                        address = userData["address"].ToString(),
                        reader_type = userData["reader_type"].ToString(),
                        account = userData["account"].ToString()
                    };

                    return Ok(new
                    {
                        status = 200,
                        user
                    });
                }
                else
                {
                    // 用户不存在
                    return Ok(new
                    {
                        message = "用户不存在"
                    });
                }
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    message = "获取用户资料失败：" + ex.Message
                });
            }
        }

        /// <summary>
        /// 更新用户资料 API
        /// </summary>
        /// <param name="reader_id">用户ID</param>
        [HttpPost("updateprofile/{reader_id}")]
        public IActionResult UpdateUserProfile(string reader_id, [FromBody] JObject userData)
        {
            try
            {
                // 解析从前端接收的用户数据
                string readerName = userData["reader_name"].ToString();
                string phoneNumber = userData["phone_number"].ToString();
                string email = userData["email"].ToString();
                string address = userData["address"].ToString();
                string readerType = userData["reader_type"].ToString();

                // 构建 SQL 查询或调用服务层更新用户资料
                string sql = $"UPDATE Reader SET " +
                             $"reader_name = '{readerName}', " +
                             $"phone_number = '{phoneNumber}', " +
                             $"email = '{email}', " +
                             $"address = '{address}', " +
                             $"reader_type = '{readerType}' " +
                             $"WHERE reader_id = '{reader_id}'";

                db.OracleUpdate(sql);

                return Ok(new
                {
                    status = 200,
                    msg = "用户资料更新成功"
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    msg = "用户资料更新失败：" + ex.Message
                });
            }
        }

        /// <summary>
        /// 注销用户 API
        /// </summary>
        /// <param name="reader_id">用户ID</param>
        [HttpGet("logout/{reader_id}")]
        public IActionResult LogoutUser(string reader_id)
        {
            try
            {
                // 构建 SQL 查询或调用服务层注销用户
                string sql = $"DELETE FROM Reader WHERE reader_id = '{reader_id}'";

                db.OracleUpdate(sql);

                return Ok(new
                {
                    status = 200,
                    msg = "用户注销成功"
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    result = false,
                    msg = "用户注销失败：" + ex.Message
                });
            }
        }

        /// <summary>
        /// 用户查询书名 API
        /// </summary>
        /// <param name="bookName">书名</param>
        [HttpGet("searchbook/{bookName}")]
        public IActionResult SearchBook(string bookName)
        {
            try
            {
                // 构建 SQL 查询或调用服务层查询书名
                string sql = $"SELECT * FROM Book WHERE book_name LIKE '%{bookName}%'";

                DataSet result = db.OracleQuery(sql);

                if (result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
                {
                    // 返回查询结果中的所有信息
                    DataTable dataTable = result.Tables[0];
                    List<Dictionary<string, string>> books = new List<Dictionary<string, string>>();

                    foreach (DataRow row in dataTable.Rows)
                    {
                        Dictionary<string, string> book = new Dictionary<string, string>();

                        foreach (DataColumn column in dataTable.Columns)
                        {
                            book[column.ColumnName] = row[column].ToString();
                        }

                        books.Add(book);
                    }

                    return Ok(new
                    {
                        result = true,
                        books
                    });
                }
                else
                {
                    // 没有匹配的书籍
                    return Ok(new
                    {
                        status = 200,
                        message = "没有匹配的书籍"
                    }) ;
                }
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    message = "查询书名失败：" + ex.Message
                });
            }
        }

    }
}