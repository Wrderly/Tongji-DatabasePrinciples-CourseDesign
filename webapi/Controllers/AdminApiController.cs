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

        private void InitDB()
        {
            string jsonFromFile = System.IO.File.ReadAllText(PublicData.programPath);
            Dictionary<string, string>? configFromFile = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonFromFile);
            string dataSourse = configFromFile["dataSource"];

            adminID = configFromFile["adminID"];
            adminPwd = configFromFile["adminPwd"];
            db = new OracleHelper(
                "DATA SOURCE=" + dataSourse + ";" +
                "USER ID='\"" + adminID + "\"';" +
                "PASSWORD='" + adminPwd + "'");
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
