//用户api控制器，调用OracleHelper中函数向前端返回数据和状态码。
//包括部分用户和管理员共用的功能，例如登录、查询书籍等

using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
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
        XDocument doc = XDocument.Load(PublicData.programPath + "\\UserApiSQL.xml");
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
            try
            {
                string tempSql, sql; // 用于拼装sql字符串的临时变量

                tempSql = doc.Root.Element("UserNameConfirm").Value;
                sql = tempSql.Replace("{reader_name}", userData["reader_name"].ToString());
                DataSet result = db.OracleQuery(sql);

                // 如果查询结果中有记录，表示用户名已存在
                if (result.Tables[0].Rows.Count > 0)
                {
                    return Ok(new
                    {
                        msg = "用户名已存在"
                    });
                }

                // 构建SQL查询或调用服务层注册用户
                tempSql = doc.Root.Element("InsertUser").Value;
                sql = tempSql.Replace("{reader_name}", userData["reader_name"].ToString())
                             .Replace("{phone_number}", userData["phone_number"].ToString())
                             .Replace("{email}", userData["email"].ToString())
                             .Replace("{address}", "NULL")
                             .Replace("{reader_type}", "default")
                             .Replace("{account}", "NULL")
                             .Replace("{password}", userData["password"].ToString())
                             .Replace("{overdue_times}", "0");

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
        /// <param name="userData">包含用户登录信息的 JSON 数据</param>
        [HttpPost("login")]
        public IActionResult UserLogin([FromBody] JObject userData)
        {
            if (db == null)
            {
                InitDB();
            }
            try
            {
                string tempSql, sql; // 用于拼装sql字符串的临时变量
                // 构建 SQL 查询或调用服务层进行用户登录验证
                tempSql = doc.Root.Element("UserNameConfirm").Value;
                sql = tempSql.Replace("{reader_name}", userData["reader_name"].ToString());
                DataSet result = db.OracleQuery(sql);

                if (!(result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0))
                {
                    // 登录失败
                    return Ok(new
                    {
                        msg = "用户名不存在"
                    });
                }

                tempSql = doc.Root.Element("UserLogin").Value;
                sql = tempSql.Replace("{reader_name}", userData["reader_name"].ToString())
                             .Replace("{password}", userData["password"].ToString());
                result = db.OracleQuery(sql);

                if (result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
                {
                    // 登录成功
                    DataRow data = result.Tables[0].Rows[0];
                    return Ok(new
                    {
                        status = 200,
                        reader_id = data["reader_id"].ToString(),
                        reader_name = data["reader_name"].ToString(),
                        phone_number = data["phone_number"].ToString(),
                        email = data["email"].ToString(),
                        overdue_times = data["overdue_times"].ToString(),
                        isAdmin = false
                    });
                }
                else
                {
                    // 登录失败
                    return Ok(new
                    {
                        msg = "用户名或密码错误"
                    });
                }
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    msg = "用户登录失败：" + ex.Message
                });
            }
        }

        // 添加其他用户相关的 API 方法，例如获取用户资料、更新用户资料等

        /// <summary>
        /// 获取用户资料 API
        /// </summary>
        /// <param name="userData">包含用户ID</param>
        [HttpPost("initreader")]
        public IActionResult GetUserProfile([FromBody] JObject userData)
        {
            if (db == null)
            {
                InitDB();
            }
            try
            {
                string tempSql, sql; // 用于拼装sql字符串的临时变量

                string reader_id = userData["reader_id"].ToString();
                // 构建 SQL 查询或调用服务层获取用户资料
                tempSql = doc.Root.Element("UserProfile").Value;
                sql = tempSql.Replace("{reader_id}", userData["reader_id"].ToString());
                DataSet result = db.OracleQuery(sql);

                if (result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
                {
                    // 提取用户信息
                    DataRow data = result.Tables[0].Rows[0];

                    // 构建用户数据对象

                    return Ok(new
                    {
                        status = 200,
                        reader_id = data["reader_id"].ToString(),
                        reader_name = data["reader_name"].ToString(),
                        phone_number = data["phone_number"].ToString(),
                        email = data["email"].ToString(),
                        address = data["address"].ToString(),
                        reader_type = data["reader_type"].ToString(),
                        account = data["account"].ToString(),
                        overdue_times = data["overdue_times"].ToString(),
                        isAdmin = false
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
        /// 更新用户密码
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
                string tempSql, sql; // 用于拼装sql字符串的临时变量

                // 解析从前端接收的用户数据
                string oldPassword = userData["oldPassword"].ToString();

                tempSql = doc.Root.Element("UserProfile").Value;
                sql = tempSql.Replace("{reader_id}", userData["reader_id"].ToString());
                DataSet result = db.OracleQuery(sql);

                if (result.Tables[0].Rows[0]["password"].ToString() != oldPassword)
                {
                    return Ok(new
                    {
                        msg = "旧密码错误"
                    });
                }

                // 构建 SQL 查询或调用服务层更新用户密码
                tempSql = doc.Root.Element("ChangePwd").Value;
                sql = tempSql.Replace("{new_password}", userData["newPassword"].ToString())
                             .Replace("{reader_id}", userData["reader_id"].ToString());
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
        /// 注销用户 API
        /// </summary>
        /// <param name="userData">包含用户ID信息</param>
        [HttpPost("logout")]
        public IActionResult LogoutUser([FromBody] JObject userData)
        {
            if (db == null)
            {
                InitDB();
            }

            try
            {
                string tempSql, sql; // 用于拼装sql字符串的临时变量

                // 查询是否有未还的借书记录或预约记录
                tempSql = doc.Root.Element("BorrowCount").Value;
                sql = tempSql.Replace("{reader_id}", userData["reader_id"].ToString());
                DataSet result = db.OracleQuery(sql);

                int borrowCount = Convert.ToInt32(result.Tables[0].Rows[0][0]);
                if (borrowCount > 0)
                {
                    return Ok(new
                    {
                        msg = "您有未处理的借书记录，请先处理后再注销账户。"
                    });
                }
                else
                {
                    // 查询所有预约记录中message为已预约的数量
                    tempSql = doc.Root.Element("ReserveCount").Value;
                    sql = tempSql.Replace("{reader_id}", userData["reader_id"].ToString());
                    result = db.OracleQuery(sql);
                    if (result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
                    {
                        DataTable reservedBooksTable = result.Tables[0];
                        foreach (DataRow reservedBookRow in reservedBooksTable.Rows)
                        {
                                string bookId = reservedBookRow["book_id"].ToString();
                                // 更新相关图书的数量
                                tempSql = doc.Root.Element("ReserveBkNUpdate").Value;
                                sql = tempSql.Replace("{book_id}", bookId);
                                db.OracleUpdate(sql);
                        }
                    }

                    // 删除该读者的所有借阅记录
                    tempSql = doc.Root.Element("DelBorrow").Value;
                    sql = tempSql.Replace("{reader_id}", userData["reader_id"].ToString());
                    db.OracleUpdate(sql);

                    // 删除该读者的所有预约记录
                    tempSql = doc.Root.Element("DelReserve").Value;
                    sql = tempSql.Replace("{reader_id}", userData["reader_id"].ToString());
                    db.OracleUpdate(sql);

                    // 删除该读者的所有反馈记录
                    tempSql = doc.Root.Element("DelReport").Value;
                    sql = tempSql.Replace("{reader_id}", userData["reader_id"].ToString());
                    db.OracleUpdate(sql);

                    // 删除该读者的所有评论记录
                    tempSql = doc.Root.Element("DelComments").Value;
                    sql = tempSql.Replace("{reader_id}", userData["reader_id"].ToString());
                    db.OracleUpdate(sql);

                    // 注销用户
                    tempSql = doc.Root.Element("DelReader").Value;
                    sql = tempSql.Replace("{reader_id}", userData["reader_id"].ToString());
                    db.OracleUpdate(sql);

                    return Ok(new
                    {
                        status = 200, // 成功注销
                        msg = "用户注销成功"
                    });
                }
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = 500, // 内部服务器错误
                    msg = "用户注销失败：" + ex.Message
                });
            }
        }
        

        /// <summary>
        /// 用户查询指定书籍 API
        /// </summary>
        /// <param name="bookData">包含要查询的字符串</param>
        /// 允许通过书名、作者名、ISBN号进行模糊查找
        [HttpPost("searchBook")]
        public IActionResult SearchBook([FromBody] JObject bookData)
        {
            if (db == null)
            {
                InitDB();
            }
            try
            {
                string tempSql, sql; // 用于拼装sql字符串的临时变量

                // 构建 SQL 查询或调用服务层查询书名

                tempSql = doc.Root.Element("SearchBk").Value;
                sql = tempSql.Replace("{searchStr}", bookData["searchStr"].ToString());
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
                        status = 200,
                        books
                    });
                }
                else
                {
                    // 没有匹配的书籍
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
                    msg = "查询书名失败：" + ex.Message
                });
            }
        }

        /// <summary>
        /// 用户预约 API
        /// </summary>
        /// <param name="reserveData">包含预约信息</param>
        [HttpPost("reservebook")]
        public IActionResult ReserveBook([FromBody] JObject reserveData)
        {
            if (db == null)
            {
                InitDB();
            }

            try
            {
                string tempSql, sql; // 用于拼装sql字符串的临时变量

                // 检查是否禁止借书
                tempSql = doc.Root.Element("ODTimes").Value;
                sql = tempSql.Replace("{reader_id}", reserveData["reader_id"].ToString());
                DataSet result = db.OracleQuery(sql);

                int overdue_times = Convert.ToInt32(result.Tables[0].Rows[0]["overdue_times"]);
                if (overdue_times >= 5)
                {
                    return Ok(new
                    {
                        status = 0,
                        msg = "该用户禁止继续预约"
                    });
                }

                // 检查是否正在预约
                tempSql = doc.Root.Element("ReservedCount").Value;
                sql = tempSql.Replace("{reader_id}", reserveData["reader_id"].ToString())
                             .Replace("{book_id}", reserveData["book_id"].ToString());
                result = db.OracleQuery(sql);

                int count = Convert.ToInt32(result.Tables[0].Rows[0][0]);
                // 如果 count 大于 0，表示记录已存在，返回相应的错误消息
                if (count > 0)
                {
                    return Ok(new
                    {
                        status = 0,
                        msg = "近期已预约/借阅该书"
                    });
                }

                // 检查书籍数量是否足够
                tempSql = doc.Root.Element("BkCount").Value;
                sql = tempSql.Replace("{book_id}", reserveData["book_id"].ToString());
                result = db.OracleQuery(sql);

                int num = Convert.ToInt32(result.Tables[0].Rows[0]["num"]);
                if (num <= 0)
                {
                    return Ok(new
                    {
                        status = 0,
                        msg = "存量不足，当前无法预约借书"
                    });
                }

                // 更新书籍数量
                num--;
                tempSql = doc.Root.Element("BkCountUpdate").Value;
                sql = tempSql.Replace("{num}", "num - 1")
                             .Replace("{book_id}", reserveData["book_id"].ToString());
                db.OracleUpdate(sql);



                // 插入预约记录
                tempSql = doc.Root.Element("InsertReserve").Value;
                sql = tempSql.Replace("{reader_id}", reserveData["reader_id"].ToString())
                             .Replace("{book_id}", reserveData["book_id"].ToString())
                             .Replace("{reserve_date}", reserveData["reserve_date"].ToString())
                             .Replace("{message}", reserveData["message"].ToString());
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
                    msg = "预约失败：" + ex.Message
                });
            }
        }

        /// <summary>
        /// 用户预约记录 API
        /// </summary>
        /// <param name="reserveData">包含预约信息</param>
        [HttpPost("initreserve")]
        public IActionResult initReserve([FromBody] JObject reserveData)
        {
            if (db == null)
            {
                InitDB();
            }

            try
            {
                string tempSql, sql; // 用于拼装sql字符串的临时变量

                // 获取预约记录的 SQL 查询，包括相关书籍信息
                tempSql = doc.Root.Element("ReserveInfo").Value;
                sql = tempSql.Replace("{reader_id}", reserveData["reader_id"].ToString());
                DataSet result = db.OracleQuery(sql);

                if (result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
                {
                    DataTable reserveRecordDataTable = result.Tables[0];

                    return Ok(new
                    {
                        status = 200,
                        reserves = reserveRecordDataTable
                    });
                }
                else
                {
                    // 没有记录
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
                    msg = "初始化失败：" + ex.Message
                });
            }
        }

        /// <summary>
        /// 用户删除预约记录 API
        /// </summary>
        /// <param name="reserveData">包含预约信息</param>
        [HttpPost("deletereserve")]
        public IActionResult DeleteReserve([FromBody] JObject reserveData)
        {
            if (db == null)
            {
                InitDB();
            }

            try
            {
                string tempSql, sql; // 用于拼装sql字符串的临时变量

                tempSql = doc.Root.Element("DelReserve1").Value;
                sql = tempSql.Replace("{reader_id}", reserveData["reader_id"].ToString())
                             .Replace("{book_id}", reserveData["book_id"].ToString());
                db.OracleUpdate(sql);

                tempSql = doc.Root.Element("BkCount").Value;
                sql = tempSql.Replace("{book_id}", reserveData["book_id"].ToString());
                DataSet result = db.OracleQuery(sql);
                int num = Convert.ToInt32(result.Tables[0].Rows[0]["num"]);

                // 更新书籍数量
                num++;
                tempSql = doc.Root.Element("BkCountUpdate").Value;
                sql = tempSql.Replace("{num}", "num + 1")
                             .Replace("{book_id}", reserveData["book_id"].ToString());
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
                    msg = "删除失败：" + ex.Message
                });
            }
        }


        /// <summary>
        /// 处理超时预约记录 API
        /// </summary>
        /// <param name="reserveData">包含读者ID和当前时间</param>
        [HttpPost("reserveovertime")]
        public IActionResult ReserveOvertime([FromBody] JObject reserveData)
        {
            if (db == null)
            {
                InitDB();
            }

            try
            {
                string tempSql, sql; // 用于拼装sql字符串的临时变量

                // 解析从前端接收的数据
                string nowTimeString = reserveData["now_time"].ToString();
                DateTime nowTime = DateTime.ParseExact(nowTimeString, "yyyy-MM-dd HH:mm:ss", null);

                // 获取该读者的所有“已预约”的预约记录
                tempSql = doc.Root.Element("ReserveDate").Value;
                sql = tempSql.Replace("{reader_id}", reserveData["reader_id"].ToString());
                DataSet result = db.OracleQuery(sql);

                if (result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
                {
                    DataTable reservedBooksTable = result.Tables[0];

                    foreach (DataRow reservedBookRow in reservedBooksTable.Rows)
                    {
                        // 获取预约时间
                        DateTime reserveTime = DateTime.ParseExact(reservedBookRow["reserve_date"].ToString(), "yyyy-MM-dd HH:mm:ss", null);

                        // 计算时间差
                        TimeSpan timeDiff = nowTime - reserveTime;

                        // 如果时间差超过7天则视为违约
                        if (timeDiff.Seconds > 30)
                        //if (timeDiff.Days > 7)
                        {
                            // 更新消息状态
                            string bookId = reservedBookRow["book_id"].ToString();
                            tempSql = doc.Root.Element("ReserveMsgUpdate").Value;
                            sql = tempSql.Replace("{reader_id}", reserveData["reader_id"].ToString())
                                         .Replace("{book_id}", bookId);
                            db.OracleUpdate(sql);

                            // 更新书籍数量
                            tempSql = doc.Root.Element("ReserveBkNUpdate").Value;
                            sql = tempSql.Replace("{book_id}", bookId);
                            db.OracleUpdate(sql);

                            // 增加违约记录
                            tempSql = doc.Root.Element("ReserveReaderODUpdate").Value;
                            sql = tempSql.Replace("{reader_id}", reserveData["reader_id"].ToString());
                            db.OracleUpdate(sql);
                        }
                    }

                    // 返回成功消息
                    return Ok(new
                    {
                        msg = "处理成功"
                    });
                }
                else
                {
                    // 未找到预约记录
                    return Ok(new
                    {
                        msg = "未找到预约记录"
                    });
                }
            }
            catch (Exception ex)
            {
                // 返回错误消息
                return Ok(new
                {
                    msg = "处理失败：" + ex.Message
                });
            }
        }



        /// <summary>
        /// 用户借书 API
        /// </summary>
        /// <param name="borrowData">包含借书信息</param>
        [HttpPost("borrowbook")]
        public IActionResult BorrowBook([FromBody] JObject borrowData)
        {
            if (db == null)
            {
                InitDB();
            }

            try
            {
                string tempSql, sql; // 用于拼装sql字符串的临时变量

                // 插入借书记录

                tempSql = doc.Root.Element("InsertBorrow").Value;
                sql = tempSql.Replace("{reader_id}", borrowData["reader_id"].ToString())
                             .Replace("{book_id}", borrowData["book_id"].ToString())
                             .Replace("{borrow_date}", borrowData["borrow_date"].ToString())
                             .Replace("{return_date}", "NULL")
                             .Replace("{message}", borrowData["message"].ToString());
                db.OracleUpdate(sql);

                // 插入续借次数
                // 插入借阅规则记录？
                tempSql = doc.Root.Element("InsertRules").Value;
                sql = tempSql.Replace("{reader_id}", borrowData["reader_id"].ToString())
                             .Replace("{book_id}", borrowData["book_id"].ToString());
                db.OracleUpdate(sql);

                //更新借阅信息
                tempSql = doc.Root.Element("UpdateReserve").Value;
                sql = tempSql.Replace("{reader_id}", borrowData["reader_id"].ToString())
                             .Replace("{book_id}", borrowData["book_id"].ToString())
                             .Replace("{reserve_date}", borrowData["reserve_date"].ToString());
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
                    msg = "借书失败：" + ex.Message
                });
            }
        }

        /// <summary>
        /// 用户借阅记录 API
        /// </summary>
        /// <param name="borrowData">包含借阅信息</param>
        [HttpPost("initborrows")]
        public IActionResult initBorrows([FromBody] JObject borrowData)
        {
            if (db == null)
            {
                InitDB();
            }

            try
            {
                string tempSql, sql; // 用于拼装sql字符串的临时变量

                // 查询所有借书记录
                tempSql = doc.Root.Element("AllBorrowRecord").Value;
                sql = tempSql.Replace("{reader_id}", borrowData["reader_id"].ToString());
                DataSet borrowRecordResult = db.OracleQuery(sql);

                if (borrowRecordResult.Tables.Count > 0 && borrowRecordResult.Tables[0].Rows.Count > 0)
                {
                    DataTable borrowRecordDataTable = borrowRecordResult.Tables[0];
                    // 处理查询结果，添加 renew_time 列
                    foreach (DataRow row in borrowRecordDataTable.Rows)
                    {
                        if (row["renew_time"] == DBNull.Value)
                        {
                            row["renew_time"] = -1;
                        }
                    }
                    return Ok(new
                    {
                        status = 200,
                        borrows = borrowRecordDataTable
                    });
                }
                else
                {
                    // 没有记录
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
                    status = 0,
                    msg = "初始化失败：" + ex.Message
                });
            }
        }


        /// <summary>
        /// 用户还书 API
        /// </summary>
        /// <param name="returnData">包含还书信息</param>
        /// 允许通过书名、作者名、ISBN号进行模糊查找
        [HttpPost("returnbook")]
        public IActionResult ReturnBook([FromBody] JObject returnData)
        {
            if (db == null)
            {
                InitDB();
            }

            try
            {
                string tempSql, sql; // 用于拼装sql字符串的临时变量

                // 更新借书记录
                tempSql = doc.Root.Element("UpdateBorrowRecord").Value;
                sql = tempSql.Replace("{reader_id}", returnData["reader_id"].ToString())
                             .Replace("{book_id}", returnData["book_id"].ToString())
                             .Replace("{borrow_date}", returnData["borrow_date"].ToString())
                             .Replace("{return_date}", returnData["return_date"].ToString())
                             .Replace("{message}", returnData["message"].ToString());
                db.OracleUpdate(sql);

                // 删除续借记录
                tempSql = doc.Root.Element("DelRule").Value;
                sql = tempSql.Replace("{reader_id}", returnData["reader_id"].ToString())
                             .Replace("{book_id}", returnData["book_id"].ToString());
                db.OracleUpdate(sql);

                // 删除预约记录
                tempSql = doc.Root.Element("DelReserve2").Value;
                sql = tempSql.Replace("{reader_id}", returnData["reader_id"].ToString())
                             .Replace("{book_id}", returnData["book_id"].ToString());
                db.OracleUpdate(sql);

                tempSql = doc.Root.Element("BkCount").Value;
                sql = tempSql.Replace("{book_id}", returnData["book_id"].ToString());
                DataSet result = db.OracleQuery(sql);
                int num = Convert.ToInt32(result.Tables[0].Rows[0]["num"]);

                // 更新书籍数量
                num++;
                tempSql = doc.Root.Element("BkCountUpdate").Value;
                sql = tempSql.Replace("{num}", "num + 1")
                             .Replace("{book_id}", returnData["book_id"].ToString());
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
                    status = 0,
                    msg = "还书失败：" + ex.Message
                });
            }
        }

        /// <summary>
        /// 用户续借 API
        /// </summary>
        /// <param name="returnData">包含续借信息</param>
        [HttpPost("continueborrow")]
        public IActionResult continueBorrow([FromBody] JObject returnData)
        {
            if (db == null)
            {
                InitDB();
            }

            try
            {
                string tempSql, sql; // 用于拼装sql字符串的临时变量

                string message = returnData["message"].ToString();
                string newmessage = "续借中";

                tempSql = doc.Root.Element("CheckMessage").Value;
                sql = tempSql.Replace("{reader_id}", returnData["reader_id"].ToString())
                             .Replace("{book_id}", returnData["book_id"].ToString())
                             .Replace("{old_borrowDate}", returnData["old_borrow_date"].ToString());
                DataSet MessageResult = db.OracleQuery(sql);
                string checkmessage = MessageResult.Tables[0].Rows[0]["message"].ToString();

                if (checkmessage == "逾期未还")
                {
                    return Ok(new
                    {
                        status = 0,
                        msg = "已逾期，请还书!"
                    });
                }

                // 查询所有借书记录
                tempSql = doc.Root.Element("RenewTimes").Value;
                sql = tempSql.Replace("{reader_id}", returnData["reader_id"].ToString())
                             .Replace("{book_id}", returnData["book_id"].ToString());
                DataSet result = db.OracleQuery(sql);

                int renew_time = Convert.ToInt32(result.Tables[0].Rows[0]["renew_time"]);

                if (renew_time >= 5)
                {
                    return Ok(new
                    {
                        status = 0,
                        msg = "最多续借五次!"
                    });
                }

                // 更新借书记录
                if (message == "已借阅")
                {
                    tempSql = doc.Root.Element("UpdateBorrowRecordRenew1").Value;
                    sql = tempSql.Replace("{reader_id}", returnData["reader_id"].ToString())
                                 .Replace("{book_id}", returnData["book_id"].ToString())
                                 .Replace("{borrow_date}", returnData["borrow_date"].ToString())
                                 .Replace("{message}", returnData["message"].ToString())
                                 .Replace("{newmessage}", newmessage);
                    db.OracleUpdate(sql);
                }
                else
                {
                    tempSql = doc.Root.Element("UpdateBorrowRecordRenew2").Value;
                    sql = tempSql.Replace("{reader_id}", returnData["reader_id"].ToString())
                                 .Replace("{book_id}", returnData["book_id"].ToString())
                                 .Replace("{borrow_date}", returnData["borrow_date"].ToString())
                                 .Replace("{message}", returnData["message"].ToString());
                    db.OracleUpdate(sql);
                }

                // 更新续借次数
                tempSql = doc.Root.Element("UpdateRenewTimes").Value;
                sql = tempSql.Replace("{reader_id}", returnData["reader_id"].ToString())
                             .Replace("{book_id}", returnData["book_id"].ToString());
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
                    status = 0,
                    msg = "续借失败：" + ex.Message
                });
            }
        }



        /// <summary>
        /// 用户处理超时借书记录 API
        /// </summary>
        /// <param name="borrowData">包含预约信息</param>
        [HttpPost("borrowovertime")]
        public IActionResult BorrowOvertime([FromBody] JObject borrowData)
        {
            if (db == null)
            {
                InitDB();
            }

            try
            {
                // 解析从前端接收的数据
                string nowTimeString = borrowData["now_time"].ToString();
                DateTime nowTime = DateTime.ParseExact(nowTimeString, "yyyy-MM-dd HH:mm:ss", null);

                // 获取该读者的所有“已借阅”/“续借中”的借阅记录
                string tempSql = doc.Root.Element("BorrowDate").Value;
                string sql = tempSql.Replace("{reader_id}", borrowData["reader_id"].ToString());

                DataSet result = db.OracleQuery(sql);
                 
                
                 
                if (result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
                {
                    DataTable borrowBooksTable = result.Tables[0];

                    foreach (DataRow borrowBookRow in borrowBooksTable.Rows)
                    {
                        // 获取预约时间
                        DateTime borrowTime = DateTime.ParseExact(borrowBookRow["borrow_date"].ToString(), "yyyy-MM-dd HH:mm:ss", null);

                        Console.WriteLine(borrowBookRow);
                        // 计算时间差
                        TimeSpan timeDiff = nowTime - borrowTime;

                        // 如果时间差超过7天则视为违约
                        if (timeDiff.Seconds > 30)
                        //if (timeDiff.Days > 7)
                        {
                            // 更新消息状态
                            string bookId = borrowBookRow["book_id"].ToString();
                            tempSql = doc.Root.Element("BorrowMsgUpdate").Value;
                            sql = tempSql.Replace("{reader_id}", borrowData["reader_id"].ToString())
                                         .Replace("{book_id}", bookId);
                            db.OracleUpdate(sql);

                            // 增加违约记录
                            tempSql = doc.Root.Element("BorrowReaderODUpdate").Value;
                            sql = tempSql.Replace("{reader_id}", borrowData["reader_id"].ToString());
                            db.OracleUpdate(sql);
                        }
                    }

                    // 返回成功消息
                    return Ok(new
                    {
                        status=200,
                        msg = "处理成功"
                    });
                }
                else
                {
                    // 未找到预约记录
                    return Ok(new
                    {
                        msg = "未找到借阅记录"
                    });
                }
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    msg = "处理失败：" + ex.Message
                });
            }
        }


        /// <summary>
        /// 用户反馈 API
        /// </summary>
        /// <param name="reportData">包含用户ID信息以及反馈内容</param>
        /// 允许通过书名、作者名、ISBN号进行模糊查找
        [HttpPost("readerreport")]
        public IActionResult UserReport([FromBody] JObject reportData)
        {
            if (db == null)
            {
                InitDB();
            }

            try
            {
                string tempSql, sql; // 用于拼装sql字符串的临时变量

                //这里不用SYSDATE了，沿用前面的方式，前端需要扔进来一个当前时间now_time，类似于reserve里的

                tempSql = doc.Root.Element("InsertReport").Value;
                sql = tempSql.Replace("{reader_id}", reportData["reader_id"].ToString())
                             .Replace("{feedback}", reportData["content"].ToString())
                             .Replace("{report_date}", reportData["now_time"].ToString());
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
                    msg = "反馈失败：" + ex.Message
                });
            }
        }

        /// <summary>
        /// 用户评论记录 API
        /// </summary>
        /// <param name="commentsData">包含评论信息</param>
        [HttpPost("initcommentslist")]
        public IActionResult InitCommentsList([FromBody] JObject commentsData)
        {
            if (db == null)
            {
                InitDB();
            }

            try
            {
                string tempSql, sql; // 用于拼装sql字符串的临时变量

                // 获取预约记录的 SQL 查询，包括相关书籍信息
                tempSql = doc.Root.Element("CommentsInfo").Value;
                sql = tempSql.Replace("{book_id}", commentsData["book_id"].ToString());
                DataSet result = db.OracleQuery(sql);

                if (result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
                {
                    DataTable reserveRecordDataTable = result.Tables[0];

                    return Ok(new
                    {
                        status = 200,
                        comments = reserveRecordDataTable
                    });
                }
                else
                {
                    // 没有记录
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
                    status = 0,
                    msg = "初始化失败：" + ex.Message
                });
            }
        }




        /// <summary>
        /// 添加图书评论 API
        /// </summary>
        /// <param name="reviewData">包含评论信息</param>
        [HttpPost("addreview")]
        public IActionResult AddReview([FromBody] JObject reviewData)
        {
            if (db == null)
            {
                InitDB();
            }

            try
            {
                string tempSql, sql; // 用于拼装sql字符串的临时变量

                tempSql = doc.Root.Element("InsertReview").Value;
                sql = tempSql.Replace("{reader_id}", reviewData["reader_id"].ToString())
                             .Replace("{book_id}", reviewData["book_id"].ToString())
                             .Replace("{review_text}", reviewData["review_text"].ToString())
                             .Replace("{review_date}", reviewData["now_time"].ToString());
                // 执行插入操作
                db.OracleUpdate(sql);

                return Ok(new
                {
                    status = 200,
                    msg = "评论添加成功"
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    msg = "评论添加失败：" + ex.Message
                });
            }
        }

        /// <summary>
        /// 删除图书评论 API
        /// </summary>
        /// <param name="reviewData">包含评论信息</param>
        [HttpPost("deletereview")]
        public IActionResult DeleteReview([FromBody] JObject reviewData)
        {
            if (db == null)
            {
                InitDB();
            }
            try
            {
                string tempSql, sql; // 用于拼装sql字符串的临时变量

                tempSql = doc.Root.Element("DeleteReview").Value;
                sql = tempSql.Replace("{review_id}", reviewData["review_id"].ToString());
                db.OracleUpdate(sql);

                return Ok(new
                {
                    status = 200,
                    msg = "评论删除成功"
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    msg = "评论删除失败：" + ex.Message
                });
            }
        }

    }
}