// See https://aka.ms/new-console-template for more information
using Database_CourseDesign;
using Newtonsoft.Json.Serialization;
using System.IO;
using webapi;

//以下是程序入口
Console.WriteLine("Hello, World!");
//获取项目脚本所在路径
var directory = System.AppContext.BaseDirectory.Split(Path.DirectorySeparatorChar);
var slice = new ArraySegment<string>(directory, 0, directory.Length - 4);
PublicData.programPath = Path.Combine(slice.ToArray());
Console.WriteLine($"项目路径: {PublicData.programPath}");

ManageSystemInit manageSystemInit = new ManageSystemInit();
manageSystemInit.Init(PublicData.programPath);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
});
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





//数据库连接测试
Test test = new Test(PublicData.programPath);
//test.ConnectTest();
//test.PrintTableTest();