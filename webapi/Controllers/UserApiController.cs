﻿using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.IO;
using System.Net;
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
        /// 用户、管理员登录 API
        /// </summary>
        /// <param name="userData">包含用户登录信息的 JSON 数据</param>
        /// 用户、管理员登录共用同一接口，根据去剪短传入isadmin判断分支。前端接收数据为reader_name实际上可以是读者也可以是管理员
        [HttpPost("login")]
        public IActionResult UserLogin([FromBody] JObject userData)
        {
            if (db == null)
            {
                InitDB();
            }
            try
            {
                // 解析从前端接收的登录数据
                string reader_name = userData["reader_name"].ToString();
                string password = userData["password"].ToString(); // 应该在前端加密密码
                string isadmin = userData["isAdmin"].ToString();
                if (isadmin == "False")
                {
                    // 构建 SQL 查询或调用服务层进行用户登录验证
                    string sql = $"SELECT reader_id FROM Reader WHERE reader_name = '{reader_name}'";
                    DataSet result = db.OracleQuery(sql);

                    if (!(result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0))
                    {
                        // 登录失败
                        return Ok(new
                        {
                            msg = "用户名不存在"
                        });
                    }


                    XDocument doc = XDocument.Load(PublicData.programPath + "\\UserApiSQL.xml");

                    string tempSql = doc.Root.Element("UserLogin").Value;

                    sql = tempSql.Replace("{reader_name}", userData["reader_name"].ToString())
                                 .Replace("{password}", userData["password"].ToString());

                    //sql = $"SELECT * FROM Reader WHERE reader_name = '{reader_name}' AND password = '{password}'";

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
                else
                {
                    string sql = $"SELECT admin_id FROM Administrator WHERE admin_name = '{reader_name}'";
                    
                    DataSet result = db.OracleQuery(sql);

                    if (!(result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0))
                    {
                        // 登录失败
                        return Ok(new
                        {
                            msg = "用户名不存在"
                        });
                    }

                    sql = $"SELECT * FROM Administrator WHERE admin_name = '{reader_name}' AND password = '{password}'";

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
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    msg = "管理员登录失败：" + ex.Message
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
                string reader_id = userData["reader_id"].ToString();
                // 构建 SQL 查询或调用服务层获取用户资料
                string sql = $"SELECT * FROM Reader WHERE reader_id = '{reader_id}'";

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
                        overdue_times= data["overdue_times"].ToString()
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
                // 解析从前端接收的用户数据
                string readerId = userData["reader_id"].ToString();
                string oldPassword = userData["oldPassword"].ToString();
                string newPassword = userData["newPassword"].ToString();
                string confirmNewPassword = userData["confirmNewPassword"].ToString();

                string sql = $"SELECT password FROM Reader WHERE reader_id = '{readerId}'";
                DataSet result = db.OracleQuery(sql);

                if (result.Tables[0].Rows[0]["password"].ToString() != oldPassword)
                {
                    return Ok(new
                    {
                        msg = "旧密码错误"
                    });
                }

                // 构建 SQL 查询或调用服务层更新用户密码
                sql = $"UPDATE Reader SET " +
      $"password = '{newPassword}' " +
      $"WHERE reader_id = '{readerId}'";

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
                string readerId = userData["reader_id"].ToString();
                // 构建 SQL 查询或调用服务层注销用户
                string sql = $"DELETE FROM Reader WHERE reader_id = '{readerId}'";

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
                // 构建 SQL 查询或调用服务层查询书名
                string searchStr = bookData["searchStr"].ToString();
                string sql = $"SELECT * FROM Book " +
                             $"WHERE book_name LIKE '%{searchStr}%' OR " +
                             $"author LIKE '%{searchStr}%' OR " +
                             $"ISBN LIKE '%{searchStr}%'";

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
                        status=200,
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
                // 解析从前端接收的预约数据
                string readerId = reserveData["reader_id"].ToString();
                string bookId = reserveData["book_id"].ToString();
                string reserve_date = reserveData["reserve_date"].ToString();
                string state = reserveData["message"].ToString();
                string bookName = reserveData["book_name"].ToString();
                string author = reserveData["author"].ToString();
                string isbn = reserveData["isbn"].ToString();


                // 检查是否禁止借书
                string checkOvertimeSql = $"SELECT overdue_times FROM Reader WHERE reader_id = '{readerId}'";
                DataSet OvertimeResult = db.OracleQuery(checkOvertimeSql);
                int overdue_times = Convert.ToInt32(OvertimeResult.Tables[0].Rows[0]["overdue_times"]);
                if (overdue_times>=5)
                {
                    return Ok(new
                    {
                        status = 0,
                        msg = "该用户禁止继续预约"
                    });
                }

                // 检查是否正在预约
                string checkReserveSql = $"SELECT COUNT(*) FROM Reserve WHERE book_id = '{bookId}'";
                DataSet ReserveResult = db.OracleQuery(checkReserveSql);
                int count = Convert.ToInt32(ReserveResult.Tables[0].Rows[0][0]);

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
                string checkNumSql = $"SELECT num FROM Book WHERE book_id = '{bookId}'";
                DataSet numResult = db.OracleQuery(checkNumSql);
                int num = Convert.ToInt32(numResult.Tables[0].Rows[0]["num"]);

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
                string updateNumSql = $"UPDATE Book SET num = {num} WHERE book_id = '{bookId}'";
                db.OracleUpdate(updateNumSql);



                // 插入预约记录
                string reserveRecordSql = $"INSERT INTO Reserve (book_id, reader_id, reserve_date, message,book_name,author,isbn) " +
                                         $"VALUES ('{bookId}', '{readerId}', '{reserve_date}', '{state}', '{bookName}', '{author}', '{isbn}')";

                db.OracleUpdate(reserveRecordSql);

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
                // 解析从前端接收的用户id
                string readerId = reserveData["reader_id"].ToString();

                string reserveRecordSql = $"SELECT * FROM Reserve WHERE reader_id = '{readerId}'";
                DataSet result = db.OracleQuery(reserveRecordSql);

                if (result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
                {
                    // 返回查询结果中的所有信息
                    DataTable dataTable = result.Tables[0];
                    List<Dictionary<string, string>> reserves = new List<Dictionary<string, string>>();

                    foreach (DataRow row in dataTable.Rows)
                    {
                        Dictionary<string, string> reserve = new Dictionary<string, string>();

                        foreach (DataColumn column in dataTable.Columns)
                        {
                            reserve[column.ColumnName] = row[column].ToString();
                        }

                        reserves.Add(reserve);
                    }
                    return Ok(new
                    {
                        status=200,
                        reserves
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
                // 解析从前端接收的用户id
                string bookId = reserveData["book_id"].ToString();
                string readerId = reserveData["reader_id"].ToString();
                string message = "已预约";
                string DeleteRecordSql = $"DELETE FROM Reserve WHERE reader_id = '{readerId}' AND book_id = '{bookId}' AND message = '{message}'";
                db.OracleQuery(DeleteRecordSql);

                // 获取书籍数量
                string checkNumSql = $"SELECT num FROM Book WHERE book_id = '{bookId}'";
                DataSet numResult = db.OracleQuery(checkNumSql);
                int num = Convert.ToInt32(numResult.Tables[0].Rows[0]["num"]);

                // 更新书籍数量
                num++;
                string updateNumSql = $"UPDATE Book SET num = {num} WHERE book_id = '{bookId}'";
                db.OracleUpdate(updateNumSql);

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

        /*
        /// <summary>
        /// 用户处理超时预约记录 API
        /// </summary>
        /// <param name="reserveData">包含预约信息</param>
        [HttpPost("reserveovertime")]
        public IActionResult ReserveOvertime([FromBody] JObject reserveData)
        {
            if (db == null)
            {
                InitDB();
            }

            try
            {
                // 解析从前端接收的用户id
                string readerId = reserveData["reader_id"].ToString();
                string now_time = reserveData["now_time"].ToString();
                 
                 
                 
                 查找所有Reserve中的reserve_date
                 若时间超过7天（不考虑时分秒）
                 则将

                 // 增加违约记录
                string checkOverdueSql = $"UPDATE Reader SET overdue_times = overdue_times + 1 WHERE reader_id = '{readerId}'";
                db.OracleQuery(checkOverdueSql);

                // 更新书籍数量
                string updateNumSql = $"UPDATE Book SET num = num + 1 WHERE book_id = '{bookId}'";
                db.OracleUpdate(updateNumSql);

                // 更新message状态
                string newmessage = "逾期未取";
                string updateMessageSql = $"UPDATE Reserve SET message = newmessage WHERE book_id = '{bookId}'";
                db.OracleUpdate(updateMessageSql);
                 


                return Ok();
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    msg = "删除失败：" + ex.Message
                });
            }
        }
        */


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
                // 解析从前端接收的借书数据
                string readerId = borrowData["reader_id"].ToString();
                string bookId = borrowData["book_id"].ToString();
                string reserveDate = borrowData["reserve_date"].ToString();
                string borrowDate = borrowData["borrow_date"].ToString();
                string bookName = borrowData["book_name"].ToString();
                string author = borrowData["author"].ToString();
                string isbn = borrowData["isbn"].ToString();
                string message = borrowData["message"].ToString();
                string returnDate = "NULL";

                // 插入借书记录
                string borrowRecordSql = $"INSERT INTO BorrowRecord (book_id, reader_id, borrow_date, return_date, book_name, author, isbn, message) " +
                                         $"VALUES ('{bookId}', '{readerId}', '{borrowDate}', '{returnDate}', '{bookName}', '{author}', '{isbn}', '{message}')";

                db.OracleUpdate(borrowRecordSql);

                // 插入续借次数
                string borrowRenewSql = $"INSERT INTO Rule (book_id, reader_id, time_limit, renew_time) " +
                                         $"VALUES ('{bookId}', '{readerId}', 0, 0)";

                db.OracleUpdate(borrowRenewSql);

                string updateMessageSql = $"UPDATE Reserve SET message = '借阅中' WHERE book_id = '{bookId}' AND reader_id = '{readerId}' AND reserve_date = '{reserveDate}'";
                db.OracleUpdate(updateMessageSql);

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
                // 解析从前端接收的用户id
                string readerId = borrowData["reader_id"].ToString();

                string borrowRecordSql = $"SELECT * FROM BorrowRecord WHERE reader_id = '{readerId}'";
                DataSet result = db.OracleQuery(borrowRecordSql);

                if (result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
                {
                    // 返回查询结果中的所有信息
                    DataTable dataTable = result.Tables[0];
                    List<Dictionary<string, string>> borrows = new List<Dictionary<string, string>>();

                    foreach (DataRow row in dataTable.Rows)
                    {
                        Dictionary<string, string> borrow = new Dictionary<string, string>();

                        foreach (DataColumn column in dataTable.Columns)
                        {
                            borrow[column.ColumnName] = row[column].ToString();
                        }

                        borrows.Add(borrow);
                    }
                    return Ok(new
                    {
                        status = 200,
                        borrows
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
                    status=0,
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
                // 解析从前端接收的还书数据
                string readerId = returnData["reader_id"].ToString();
                string bookId = returnData["book_id"].ToString();
                string borrowDate = returnData["borrow_date"].ToString();
                string returnDate = returnData["return_date"].ToString();
                string message = returnData["message"].ToString();
                string reservemessage = "借阅中";
                // 更新借书记录
                string updateReturnDateSql = $"UPDATE BorrowRecord SET return_date = '{returnDate}' , message = '{message}' " +
                                             $"WHERE book_id = '{bookId}' AND reader_id = '{readerId}' AND borrow_date = '{borrowDate}'  ";
                db.OracleUpdate(updateReturnDateSql);

                // 删除续借记录
                string updateRenewSql = $"DELETE FROM Rule WHERE reader_id = '{readerId}' AND book_id = '{bookId}'";

                db.OracleUpdate(updateRenewSql);

                // 删除预约记录
                string reserveRecordSql = $"DELETE FROM Reserve WHERE reader_id = '{readerId}' AND book_id = '{bookId}' AND message = '{reservemessage}'";

                db.OracleUpdate(reserveRecordSql);

                // 更新书籍数量
                string updateNumSql = $"UPDATE Book SET num = num + 1 WHERE book_id = '{bookId}'";
                db.OracleUpdate(updateNumSql);

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
                }) ;
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
                // 解析从前端接收的还书数据
                string readerId = returnData["reader_id"].ToString();
                string bookId = returnData["book_id"].ToString();
                string borrowDate = returnData["borrow_date"].ToString();
                string message = returnData["message"].ToString();
                string newmessage = "续借中";
                string checkRenewSql = $"SELECT renew_time FROM Rule WHERE book_id = '{bookId}' AND reader_id = '{readerId}'";
                DataSet numResult = db.OracleQuery(checkRenewSql);
                int renew_time = Convert.ToInt32(numResult.Tables[0].Rows[0]["renew_time"]);

                if(renew_time>=5)
                {
                    return Ok(new
                    {
                        status = 0,
                        msg = "最多续借五次!"
                    });
                }


                // 更新借书记录
                if(message == "已借阅")
                {
                    string updateReturnDateSql = $"UPDATE BorrowRecord SET borrow_date = '{borrowDate}' , message = '{newmessage}' " +
                                             $"WHERE book_id = '{bookId}' AND reader_id = '{readerId}' AND message = '{message}'  ";
                    db.OracleUpdate(updateReturnDateSql);
                }
                else
                {
                    string updateReturnDateSql = $"UPDATE BorrowRecord SET borrow_date = '{borrowDate}' " +
                                             $"WHERE book_id = '{bookId}' AND reader_id = '{readerId}' AND message = '{message}'  ";
                    db.OracleUpdate(updateReturnDateSql);
                }

                // 更新续借次数
                string updateRenewSql = $"UPDATE Rule SET renew_time = renew_time + 1" +
                                             $"WHERE book_id = '{bookId}' AND reader_id = '{readerId}' ";
                db.OracleUpdate(updateRenewSql);

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



        /*
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
                // 解析从前端接收的用户id
                string readerId = borrowData["reader_id"].ToString();
                string now_time = borrowData["now_time"].ToString();

             
                 
                 
                 查找所有BorrowRecord中的borrow_date
                 若时间超过7天（不考虑时分秒）
                 则将
                 
                //删除相应预约记录
                string DeleteRecordSql = $"DELETE FROM Reserve WHERE reader_id = '{readerId}' AND book_id = '{bookId}'";
                db.OracleQuery(DeleteRecordSql);

                 // 增加违约记录
                string checkOverdueSql = $"UPDATE Reader SET overdue_times = overdue_times + 1 WHERE reader_id = '{readerId}'";
                db.OracleQuery(checkOverdueSql);

                // 更新书籍数量
                string updateNumSql = $"UPDATE Book SET num = num + 1 WHERE book_id = '{bookId}'";
                db.OracleUpdate(updateNumSql);

                // 更新message状态
                string newmessage = "逾期未还";
                string updateMessageSql = $"UPDATE BorrowRecord SET message = newmessage WHERE book_id = '{bookId}'";
                db.OracleUpdate(updateMessageSql);
                 



                return Ok();
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    msg = "删除失败：" + ex.Message
                });
            }
        }
        */


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
                // 解析从前端接收的反馈
                string readerId = reportData["reader_id"].ToString();
                string feedback = reportData["content"].ToString();

                string sql = $"INSERT INTO Report (reader_id, feedback, report_date)" +
                                      $"VALUES ('{readerId}', '{feedback}', SYSDATE)";
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


    }
}