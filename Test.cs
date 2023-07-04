using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Data.OracleClient;
using System.Diagnostics;
using System.Text.Json;


namespace Database_CourseDesign
{
    internal class Test
    {
        private string connectionString;
        private string path;
        public Test(string programPath)
        { 
            path = programPath;
            //根据项目路径设置配置文件路径
            string filePath = Path.Combine(path, "config.json");
            string jsonFromFile = File.ReadAllText(filePath);
            var configFromFile = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonFromFile);
            connectionString = configFromFile["connectionString"];
        }

        public void ConnectTest()
        {
            Console.WriteLine("ConnectTest start");
            Console.WriteLine("Get Connection start");
            //根据连接配置创建数据库连接
            OracleConnection connection = new OracleConnection(connectionString);
            Console.WriteLine("Get Connection end");

            try
            {
                Console.WriteLine("Try start");
                //启动数据库连接
                connection.Open();
                Console.WriteLine("Connection successful!");
            }
            catch (OracleException ex)
            {
                Console.WriteLine("Connection failed: " + ex.Message);
            }
            finally
            {
                Console.WriteLine("Close start");
                //关闭数据库连接
                connection.Close();
            }


        }
    }
}
