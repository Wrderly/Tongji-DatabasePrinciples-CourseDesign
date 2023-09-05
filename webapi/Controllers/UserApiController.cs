using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.IO;
using System.Net;
using System.Security.Principal;
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
            string jsonFromFile = System.IO.File.ReadAllText(PublicData.programPath);
            Dictionary<string, string>? configFromFile = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonFromFile);
            string dataSourse = configFromFile["dataSource"];

            userID = configFromFile["userID"];
            userPwd = configFromFile["userPwd"];
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
            return Ok();
        
            /*
            try
            {
                // 解析从前端接收的用户数据
                string readerName = userData["reader_name"].ToString();
                string phoneNumber = userData["phone_number"].ToString();
                string email = userData["email"].ToString();
                string address = "1";
                //= userData["address"].ToString();
                string readerType = "1";
                //= userData["reader_type"].ToString();
                string account = "1";
                    //= userData["account"].ToString();
                string password = userData["password"].ToString(); // 应该在前端加密密码

                // 构建SQL查询或调用服务层注册用户
                string sql = $"INSERT INTO Reader (reader_name, phone_number, email, address, reader_type, account, password) " +
                             $"VALUES ('{readerName}', '{phoneNumber}', '{email}','{address}', '{readerType}', '{account}',  '{password}')";
                //address, reader_type, account,
                //'{address}', '{readerType}', '{account}', 

                db.OracleUpdate(sql);

                return Ok(new
                {
                    result = true,
                    msg = "用户注册成功"
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    result = false,
                    msg = "用户注册失败：" + ex.Message
                });
            }
            */
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
                        message = "用户登录成功",
                        reader_id = readerId
                    });
                }
                else
                {
                    // 登录失败
                    return Ok(new
                    {
                        result = false,
                        message = "用户名或密码错误"
                    });
                }
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    result = false,
                    message = "用户登录失败：" + ex.Message
                });
            }
        }

        // 添加其他用户相关的 API 方法，例如获取用户资料、更新用户资料等

        /// <summary>
        /// 获取用户资料 API
        /// </summary>
        /// <param name="reader_id">用户ID</param>
        [HttpGet("initreader")]
        public IActionResult GetUserProfile(string reader_id)
        {
            try
            {
                // 构建 SQL 查询或调用服务层获取用户资料
                string sql = $"SELECT * FROM Reader WHERE reader_id = '{reader_id}'";

                DataSet result = db.OracleQuery(sql);

                if (result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
                {
                    // 返回用户资料
                    return Ok(new
                    {
                        result = true,
                        dataset = result
                    });
                }
                else
                {
                    // 用户不存在
                    return Ok(new
                    {
                        result = false,
                        message = "用户不存在"
                    });
                }
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    result = false,
                    message = "获取用户资料失败：" + ex.Message
                });
            }
        }

        // 还需添加其他用户相关的 API 方法，例如更新用户资料、删除用户等
    }
}