// See https://aka.ms/new-console-template for more information
using Database_CourseDesign;
using System.IO;

//以下是程序入口
Console.WriteLine("Hello, World!");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

//获取项目脚本所在路径
var directory = System.AppContext.BaseDirectory.Split(Path.DirectorySeparatorChar);
var slice = new ArraySegment<string>(directory, 0, directory.Length - 4);
var programPath = Path.Combine(slice.ToArray());
Console.WriteLine($"项目路径: {programPath}");

ManageSystemInit manageSystemInit = new ManageSystemInit();
manageSystemInit.Init(programPath);

/**/
//数据库连接测试
Test test = new Test(programPath);
//test.ConnectTest();
//test.PrintTableTest();
/**/