using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Data.OracleClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;

namespace Database_CourseDesign
{
    internal class ManageSystemInit
    {
        private string? connectionStr;
        private string? tableSpacePath;
        private string? managerID;
        private string? managerPwd;
        private string? adminID;
        private string? adminPwd;
        private string? superAdminID;
        private string? superAdminPwd;
        private string? readerID;
        private string? readerPwd;
        private string? tableSpaceTemp;
        private string? tableSpaceData;

        private string? path;
        public void Init(string programPath)
        {
            path = programPath;
            //根据项目路径设置配置文件路径
            string filePath = Path.Combine(path, "config.json");
            string jsonFromFile = File.ReadAllText(filePath);
            Dictionary<string, string>? configFromFile = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonFromFile);
            //读取配置文件
            string dataSourse = configFromFile["dataSource"];
            tableSpacePath = configFromFile["tableSpacePath"];
            tableSpaceTemp = configFromFile["tableSpaceTemp"];
            tableSpaceData = configFromFile["tableSpaceData"];
            //使用SYSTEM登录
            string userID = configFromFile["systemID"];
            string userPwd = configFromFile["systemPwd"];
            connectionStr = "DATA SOURCE=" + dataSourse + ";" +
                            "USER ID='\"" + userID + "\"';" +
                            "PASSWORD='" + userPwd + "'";

            Console.WriteLine("开始数据库初始化");
            Console.WriteLine(userID + " 开始获取数据库连接");
            //根据连接配置创建数据库连接
            OracleConnection systemConnection = new OracleConnection(connectionStr);
            Console.WriteLine(userID + " 获取数据库连接成功");

            try
            {
                Console.WriteLine(userID + " 开始连接数据库");
                //启动数据库连接
                systemConnection.Open();
                Console.WriteLine(userID + " 连接数据库成功");
            }
            catch (OracleException ex)
            {
                Console.WriteLine(userID + " 连接数据库失败: " + ex.Message);
                return;
            }

            InitTableSpace(systemConnection, userID);
            InitUser(systemConnection, configFromFile);
            Console.WriteLine(userID + " 关闭数据库连接");
            //关闭数据库连接
            systemConnection.Close();

            //使用MANAGER登录
            userID = configFromFile["managerID"];
            userPwd = configFromFile["managerPwd"];
            connectionStr = "DATA SOURCE=" + dataSourse + ";" +
                            "USER ID='\"" + userID + "\"';" +
                            "PASSWORD='" + userPwd + "'";

            Console.WriteLine(userID + " 开始获取数据库连接");
            //根据连接配置创建数据库连接
            OracleConnection managerConnection = new OracleConnection(connectionStr);
            Console.WriteLine(userID + " 获取数据库连接成功");
            try
            {
                Console.WriteLine(userID + " 开始连接数据库");
                //启动数据库连接
                managerConnection.Open();
                Console.WriteLine(userID + " 连接数据库成功");
            }
            catch (OracleException ex)
            {
                Console.WriteLine(userID + " 连接数据库失败: " + ex.Message);
                return;
            }

            InitTable(managerConnection, userID);
            //InitRight(managerConnection, userID);

            Console.WriteLine(userID + " 关闭数据库连接");
            //关闭数据库连接
            systemConnection.Close();
        }

        private void InitTableSpace(OracleConnection connection, string userID)
        {
            try
            {
                Console.WriteLine(userID + " 开始创建数据库表空间 temp");
                string createTableSpaceStr_1 =
                    "CREATE TEMPORARY TABLESPACE " + tableSpaceTemp + " " +
                    "TEMPFILE '" + tableSpacePath + tableSpaceTemp + ".dbf' " +
                    "SIZE 50m " +
                    "AUTOEXTEND ON " +
                    "NEXT 50m " +
                    "MAXSIZE 20480m";
                OracleCommand command_1 = new OracleCommand(createTableSpaceStr_1, connection);
                command_1.ExecuteNonQuery();
            }
            catch (OracleException ex)
            {
                if (ex.Code == 1543)
                {
                    Console.WriteLine("已有同名表空间");
                }
                else
                {
                    Console.WriteLine("创建数据库表空间失败: " + ex.Message);
                }
            }
            try
            {
                Console.WriteLine(userID + " 开始创建数据库表空间 data");
                string createTableSpaceStr_2 =
                    "CREATE TABLESPACE " + tableSpaceData + " " +
                    "DATAFILE '" + tableSpacePath + tableSpaceData + ".dbf' " +
                    "SIZE 50m " +
                    "AUTOEXTEND ON " +
                    "NEXT 50m " +
                    "MAXSIZE 20480m";
                OracleCommand command_2 = new OracleCommand(createTableSpaceStr_2, connection);
                command_2.ExecuteNonQuery();
            }
            catch (OracleException ex)
            {
                if (ex.Code == 1543)
                {
                    Console.WriteLine("已有同名表空间");
                }
                else
                {
                    Console.WriteLine("创建数据库表空间失败: " + ex.Message);
                }
            }
        }

        private void InitUser(OracleConnection connection, Dictionary<string, string>? configFromFile)
        {
            managerID = configFromFile["managerID"];
            managerPwd = configFromFile["managerPwd"];
            string createManagerStr =
                "CREATE USER " + managerID + " IDENTIFIED BY " + managerPwd + " DEFAULT TABLESPACE " + tableSpaceData + " TEMPORARY TABLESPACE " + tableSpaceTemp;
            string grantManagerStr =
                "GRANT connect,resource,alter TO " + managerID;

            adminID = configFromFile["adminID"];
            adminPwd = configFromFile["adminPwd"];
            string createAdminStr =
                "CREATE USER " + adminID + " IDENTIFIED BY " + adminPwd + " DEFAULT TABLESPACE " + tableSpaceData + " TEMPORARY TABLESPACE " + tableSpaceTemp;
            string grantAdminStr = 
                "GRANT connect TO "+adminID;
            /*
            superAdminID = configFromFile["superAdminID"];
            superAdminPwd = configFromFile["superAdminPwd"];
            string createSuperAdminStr =
                "CREATE USER " + superAdminID + " IDENTIFIED BY " + superAdminPwd + " DEFAULT TABLESPACE " + tableSpaceData + " TEMPORARY TABLESPACE " + tableSpaceTemp;
            string grantSuperAdminStr =
                "GRANT connect TO " + superAdminID;*/

            readerID = configFromFile["readerID"];
            readerPwd = configFromFile["readerPwd"];
            string createReaderStr =
                "CREATE USER " + readerID + " IDENTIFIED BY " + readerPwd + " DEFAULT TABLESPACE " + tableSpaceData + " TEMPORARY TABLESPACE " + tableSpaceTemp;
            string grantReaderStr = 
                "GRANT connect TO "+readerID;

            CreateUser(connection, createManagerStr, grantManagerStr, managerID);
            CreateUser(connection, createAdminStr, grantAdminStr, adminID);
            //CreateUser(connection, createSuperAdminStr, grantSuperAdminStr, superAdminID);
            CreateUser(connection, createReaderStr, grantReaderStr, readerID);
        }

        private void CreateUser(OracleConnection connection, string createUserStr, string grantUserStr, string userID)
        {
            try
            {
                Console.WriteLine("开始创建用户 " + userID);
                OracleCommand createCommand = new OracleCommand(createUserStr, connection);
                createCommand.ExecuteNonQuery();
                Console.WriteLine("创建用户成功");

                Console.WriteLine("开始授权用户 " + userID);
                Console.WriteLine(grantUserStr);
                OracleCommand grantCommand = new OracleCommand(grantUserStr, connection);
                grantCommand.ExecuteNonQuery();
                Console.WriteLine("授权用户成功");
            }
            catch (OracleException ex)
            {
                if (ex.Code == 1920)
                {
                    Console.WriteLine("已有同名用户");
                }
                else
                {
                    Console.WriteLine("exception: " + ex.Message);
                }
            }
        }

        private void InitTable(OracleConnection connection, string userID)
        {
            Console.WriteLine(userID + " 开始创建数据库表");
            XDocument doc = XDocument.Load(path + "\\SystemInitSQL.xml");
            for (int i = 0; i < 14; i++)
            {
                try
                {
                    string sql = doc.Descendants("Query").Where(x => (string)x.Attribute("name") == ("CreateTable" + i.ToString())).FirstOrDefault()?.Value;
                    OracleCommand command = new OracleCommand(sql, connection);
                    command.ExecuteNonQuery();
                }
                catch (OracleException ex)
                {
                    if (ex.Code == 955)
                    {
                        Console.WriteLine(i.ToString() + " 已存在同名表");
                    }
                    else
                    {
                        Console.WriteLine(i.ToString() + " exception: " + ex.Message);
                    }
                }
            }
        }

        private void InitRight(OracleConnection connection, string userID)
        {
            Console.WriteLine(userID + " 开始进行授权");
            //OracleCommand command = new OracleCommand("", connection);
            //command.ExecuteNonQuery();
        }
    }
}
