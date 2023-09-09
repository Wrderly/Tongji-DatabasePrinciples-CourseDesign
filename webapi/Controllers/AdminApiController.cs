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

        #region 管理账号相关操作
        // 包括登录、管理账号信息、更改密码

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
                string tempSql, sql; // 用于拼装sql字符串的临时变量

                tempSql = doc.Root.Element("AdminNameConfirm").Value;
                sql = tempSql.Replace("{admin_name}", userData["admin_name"].ToString());
                DataSet result = db.OracleQuery(sql);

                if (!(result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0))
                {
                    // 登录失败
                    return Ok(new
                    {
                        msg = "管理员名称不存在"
                    });
                }
                tempSql = doc.Root.Element("AdminLogin").Value;
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
        public IActionResult GetAdminProfile([FromBody] JObject userData)
        {
            if (db == null)
            {
                InitDB();
            }
            try
            {
                string tempSql, sql; // 用于拼装sql字符串的临时变量

                tempSql = doc.Root.Element("AdminProfile").Value;
                sql = tempSql.Replace("{admin_id}", userData["admin_id"].ToString());
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
                string tempSql, sql; // 用于拼装sql字符串的临时变量

                // 解析从前端接收的用户数据
                string oldPassword = userData["oldPassword"].ToString();

                tempSql = doc.Root.Element("AdminProfile").Value;
                sql = tempSql.Replace("{admin_id}", userData["admin_id"].ToString());
                DataSet result = db.OracleQuery(sql);

                if (result.Tables[0].Rows[0]["password"].ToString() != oldPassword)
                {
                    return Ok(new
                    {
                        msg = "旧密码错误"
                    });
                }

                // 构建 SQL 查询或调用服务层更新用户密码
                tempSql = doc.Root.Element("ChangePwdAd").Value;
                sql = tempSql.Replace("{new_password}", userData["newPassword"].ToString())
                             .Replace("{admin_id}", userData["admin_id"].ToString());
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

        #endregion

        #region 获取信息
        // 包括所有用户、所有反馈、所有违约预约、所有违约借阅、所有图书类型、所有购买记录、所有供应商
        /// <summary>
        /// 获取用户列表 API
        /// </summary>
        [HttpPost("initreaderlist")]
        public IActionResult InitReaderList()
        {
            if (db == null)
            {
                InitDB();
            }
            try
            {
                string tempSql, sql; // 用于拼装sql字符串的临时变量

                sql = doc.Root.Element("AllReader").Value;
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
        /// 获取反馈列表 API
        /// </summary>
        [HttpPost("initreportlist")]
        public IActionResult InitReportList()
        {
            if (db == null)
            {
                InitDB();
            }
            try
            {
                string tempSql, sql; // 用于拼装sql字符串的临时变量

                sql = doc.Root.Element("AllReport").Value;
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
        /// 获取已违约的预约列表 API
        /// </summary>
        [HttpPost("initreservelist")]
        public IActionResult InitReserveList()
        {
            if (db == null)
            {
                InitDB();
            }
            try
            {
                string tempSql, sql; // 用于拼装sql字符串的临时变量

                sql = doc.Root.Element("AllODReserve").Value;
                DataSet result = db.OracleQuery(sql);

                if (result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
                {
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
        /// 获取已违约的借书列表 API
        /// </summary>
        [HttpPost("initborrowslist")]
        public IActionResult InitBorrowList()
        {
            if (db == null)
            {
                InitDB();
            }
            try
            {
                string tempSql, sql; // 用于拼装sql字符串的临时变量

                sql = doc.Root.Element("AllODBorrow").Value;
                DataSet result = db.OracleQuery(sql);

                if (result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
                {
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

        /// <summary>
        /// 获取图书类型 API
        /// </summary>
        [HttpPost("initbooktypelist")]
        public IActionResult InitBkTypeList()
        {
            if (db == null)
            {
                InitDB();
            }
            try
            {
                string tempSql, sql; // 用于拼装sql字符串的临时变量

                sql = doc.Root.Element("AllBkType").Value;
                DataSet result = db.OracleQuery(sql);

                if (result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
                {
                    DataTable dataTable = result.Tables[0];
                    List<Dictionary<string, string>> typeLists = new List<Dictionary<string, string>>();

                    foreach (DataRow row in dataTable.Rows)
                    {
                        Dictionary<string, string> typeList = new Dictionary<string, string>();

                        foreach (DataColumn column in dataTable.Columns)
                        {
                            typeList[column.ColumnName] = row[column].ToString();
                        }

                        typeLists.Add(typeList);
                    }

                    return Ok(new
                    {
                        status = 200,
                        typeLists
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

        /// <summary>
        /// 查询所有购买记录 API
        /// </summary>
        [HttpPost("initpurchaserecord")]
        public IActionResult InitPurchaseRecord([FromBody] JObject requestData)
        {
            if (db == null)
            {
                InitDB();
            }

            try
            {
                string tempSql, sql; // 用于拼装sql字符串的临时变量

                // 获取购买记录的 SQL 查询，包括相关书籍信息
                tempSql = doc.Root.Element("AllPurchaseRecord").Value;
                sql = tempSql.Replace("{supplier_id}", requestData["supplier_id"].ToString());
                DataSet result = db.OracleQuery(sql);

                if (result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
                {
                    DataTable purchaseRecordDataTable = result.Tables[0];

                    return Ok(new
                    {
                        status = 200,
                        purchaseRecords = purchaseRecordDataTable
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
                    msg = "初始化购买记录失败：" + ex.Message
                });
            }
        }

        /// <summary>
        /// 查询所有供应商 API
        /// </summary>
        [HttpPost("initsupplierlist")]
        public IActionResult InitSupplierList()
        {
            if (db == null)
            {
                InitDB();
            }

            try
            {
                string tempSql, sql; // 用于拼装sql字符串的临时变量

                // 获取所有供应商的 SQL 查询
                sql = doc.Root.Element("AllSupplier").Value;
                DataSet result = db.OracleQuery(sql);

                if (result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
                {
                    DataTable supplierDataTable = result.Tables[0];

                    return Ok(new
                    {
                        status = 200,
                        suppliers = supplierDataTable
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
                    msg = "初始化供应商失败：" + ex.Message
                });
            }
        }

        #endregion

        #region 处理用户相关
        // 包括更新用户违规次数

        /// <summary>
        /// 更新用户违规次数 API
        /// </summary>
        /// <param name="userData">包含用户id</param>
        [HttpPost("updatecount")]
        public IActionResult UpdateODTimes([FromBody] JObject userData)
        {
            if (db == null)
            {
                InitDB();
            }

            try
            {
                string tempSql, sql; // 用于拼装sql字符串的临时变量

                tempSql = doc.Root.Element("UpdateODTimes").Value;
                sql = tempSql.Replace("{overdue_times}", userData["overdue_times"].ToString())
                             .Replace("{reader_id}", userData["reader_id"].ToString());
                DataSet result = db.OracleQuery(sql);
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
                    msg = "修改失败：" + ex.Message
                });
            }
        }

        #endregion

        #region 书籍处理相关
        // 包括插入新书、更新书籍信息、添加图书类别

        /// <summary>
        /// 插入新书 API
        /// </summary>
        /// <param name="data">包含图书信息</param>
        [HttpPost("insertbook")]
        public IActionResult InsertBook([FromBody] JObject data)
        {
            if (db == null)
            {
                InitDB();
            }
            try
            {
                string tempSql, sql; // 用于拼装sql字符串的临时变量

                // 检查该ISBN书籍是否已存在
                tempSql = doc.Root.Element("ISBNConfirm").Value;
                sql = tempSql.Replace("{ISBN}", data["ISBN"].ToString());
                DataSet result = db.OracleQuery(sql);

                int ISBNCount = Convert.ToInt32(result.Tables[0].Rows[0][0]);
                if (ISBNCount > 0)
                {
                    return Ok(new
                    {
                        msg = "该ISBN对应书籍已存在"
                    });
                }

                // 更新书籍信息
                tempSql = doc.Root.Element("InsertBk").Value;
                sql = tempSql.Replace("{book_name}", data["book_name"].ToString())
                             .Replace("{ISBN}", data["ISBN"].ToString())
                             .Replace("{author}", data["author"].ToString())
                             .Replace("{introduction}", data["introduction"].ToString())
                             .Replace("{collection_type}", data["collection_type"].ToString());
                            
                db.OracleUpdate(sql);

                return Ok(new
                {
                    status = 200
                });
            }
            catch(Exception ex)
            {
                return Ok(new
                {
                    msg = "插入书籍失败：" + ex.Message
                });
            }
        }

        /// <summary>
        /// 更新图书信息 API
        /// </summary>
        /// <param name="data">包含图书信息</param>
        [HttpPost("updatebook")]
        public IActionResult UpdateBook([FromBody] JObject data)
        {
            if (db == null)
            {
                InitDB();
            }
            try
            {
                string tempSql, sql; // 用于拼装sql字符串的临时变量

                tempSql = doc.Root.Element("UpdateBk").Value;
                sql = tempSql.Replace("{book_id}", data["book_id"].ToString())
                             .Replace("{book_name}", data["book_name"].ToString())
                             .Replace("{ISBN}", data["ISBN"].ToString())
                             .Replace("{author}", data["author"].ToString())
                             .Replace("{introduction}", data["introduction"].ToString())
                             .Replace("{collection_type}", data["collection_type"].ToString());
                db.OracleUpdate(sql);

                return Ok(new
                {
                    status = 200
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    msg = "更新失败：" + ex.Message
                });
            }
        }

        /// <summary>
        /// 添加图书类别 API
        /// </summary>
        /// <param name="collectionTypeData">包含图书类别信息</param>
        [HttpPost("addcollectiontype")]
        public IActionResult AddCollectionType([FromBody] JObject collectionTypeData)
        {
            if (db == null)
            {
                InitDB();
            }

            try
            {
                string tempSql, sql; // 用于拼装sql字符串的临时变量

                // 检查是否已存在相同的供应商名称
                tempSql = doc.Root.Element("CheckCollectionTypeExistence").Value;
                sql = tempSql.Replace("{collection_type}", collectionTypeData["collection_type"].ToString());
                DataSet result = db.OracleQuery(sql);

                int existingCount = Convert.ToInt32(result.Tables[0].Rows[0][0]);
                if (existingCount > 0)
                {
                    return Ok(new
                    {
                        status = 400,
                        msg = "图书类别已存在"
                    });
                }

                tempSql = doc.Root.Element("InsertCollectionType").Value;
                sql = tempSql.Replace("{collection_type}", collectionTypeData["collection_type"].ToString())
                             .Replace("{note}", collectionTypeData["note"].ToString());
                db.OracleUpdate(sql);

                return Ok(new
                {
                    status = 200,
                    msg = "图书类别添加成功"
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = 500,
                    msg = "图书类别添加失败：" + ex.Message
                });
            }
        }

        #endregion

        #region 书籍供应相关
        // 包括添加供应商、添加购买记录

        /// <summary>
        /// 添加供应商 API
        /// </summary>
        /// <param name="supplierData">包含供应商信息</param>
        [HttpPost("addsupplier")]
        public IActionResult AddSupplier([FromBody] JObject supplierData)
        {
            if (db == null)
            {
                InitDB();
            }

            try
            {
                string tempSql, sql; // 用于拼装sql字符串的临时变量

                // 检查是否已存在相同的供应商名称
                tempSql = doc.Root.Element("CheckSupplierExistence").Value;
                sql = tempSql.Replace("{supplier_name}", supplierData["supplier_name"].ToString());
                DataSet result = db.OracleQuery(sql);

                int existingCount = Convert.ToInt32(result.Tables[0].Rows[0][0]);
                if (existingCount > 0)
                {
                    return Ok(new
                    {
                        status = 400,
                        msg = "供应商名称已存在"
                    });
                }

                tempSql = doc.Root.Element("InsertSupplier").Value;
                sql = tempSql.Replace("{supplier_name}", supplierData["supplier_name"].ToString())
                             .Replace("{phone_number}", supplierData["phone_number"].ToString())
                             .Replace("{email}", supplierData["email"].ToString())
                             .Replace("{address}", supplierData["address"].ToString());
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
                    status = 500,
                    msg = "供应商添加失败：" + ex.Message
                });
            }
        }

        /// <summary>
        /// 添加购买记录 API
        /// </summary>
        /// <param name="purchaseData">包含图书类别信息</param>
        [HttpPost("addpurchaserecord")]
        public IActionResult AddPurchaseRecord([FromBody] JObject purchaseData)
        {
            if (db == null)
            {
                InitDB();
            }

            try
            {
                string tempSql, sql; // 用于拼装sql字符串的临时变量

                decimal totalPrice = Convert.ToDecimal(purchaseData["unit_price"]) * Convert.ToInt32(purchaseData["quantity"]);

                // 检查Book是否存在
                tempSql = doc.Root.Element("BookConfirm").Value;
                sql = tempSql.Replace("{book_id}", purchaseData["book_id"].ToString());
                DataSet result = db.OracleQuery(sql);

                int existingCount = Convert.ToInt32(result.Tables[0].Rows[0][0]);
                if (existingCount <= 0)
                {
                    return Ok(new
                    {
                        status = 400,
                        msg = "该书不存在"
                    });
                }

                // 构建 SQL 插入购买记录语句
                tempSql = doc.Root.Element("InsertPurchaseRecord").Value;
                sql = tempSql.Replace("{admin_id}", purchaseData["admin_id"].ToString())
                             .Replace("{supplier_id}", purchaseData["supplier_id"].ToString())
                             .Replace("{book_id}", purchaseData["book_id"].ToString())
                             .Replace("{purchase_date}", purchaseData["now_time"].ToString())
                             .Replace("{quantity}", purchaseData["quantity"].ToString())
                             .Replace("{unit_price}", purchaseData["unit_price"].ToString())
                             .Replace("{total_price}", totalPrice.ToString())
                             .Replace("{is_approved}", "1");

                // 执行插入操作
                db.OracleUpdate(sql);

                tempSql = doc.Root.Element("BkCountUpdate").Value;
                sql = tempSql.Replace("{book_id}", purchaseData["book_id"].ToString())
                             .Replace("{num}", "num + " + purchaseData["quantity"].ToString());

                // 执行更新图书数量操作
                db.OracleUpdate(sql);

                return Ok(new
                {
                    status = 200,
                    msg = "购买记录添加成功"
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = 500,
                    msg = "购买记录添加失败：" + ex.Message
                });
            }
        }

        #endregion

    }
}
