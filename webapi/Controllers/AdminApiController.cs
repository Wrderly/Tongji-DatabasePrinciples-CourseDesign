using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
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
            XDocument doc = XDocument.Load(PublicData.programPath + "\\SQLXML.xml");

            string tempSql = doc.Descendants("Query").Where(x => (string)x.Attribute("name") == "InsertIntoBook").FirstOrDefault()?.Value;

            string sql = tempSql.Replace("{book_id}", data["book_id"].ToString())
                                .Replace("{num}", data["num"].ToString())
                                .Replace("{ISBN}", data["ISBN"].ToString())
                                .Replace("{book_name}", data["book_name"].ToString())
                                .Replace("{author}", data["author"].ToString())
                                .Replace("{publisher}", data["publisher"].ToString())
                                .Replace("{publication_date}", data["publication_date"].ToString())
                                .Replace("{introduction}", data["introduction"].ToString())
                                .Replace("{cover_picture}", data["cover_picture"].ToString())
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
    }
}
