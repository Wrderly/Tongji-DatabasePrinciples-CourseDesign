// See https://aka.ms/new-console-template for more information
using Database_CourseDesign;
using System.IO;

//以下是程序入口
Console.WriteLine("Hello, World!");

//获取项目脚本所在路径
var directory = System.AppContext.BaseDirectory.Split(Path.DirectorySeparatorChar);
var slice = new ArraySegment<string>(directory, 0, directory.Length - 4);
var programPath = Path.Combine(slice.ToArray());
Console.WriteLine($"Get Program Path: {programPath}");

//数据库连接测试
Test test = new Test(programPath);
test.ConnectTest();
