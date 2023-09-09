using System.Data;
using System.Data.OracleClient;

//封装sql语句为c#函数，
//调用时，需要new一个OracleHelper对象并在try-catch语句中进行调用与异常处理，使用方式见下方test类的main函数

//每次增删查改操作前后已经调用数据库的打开与关闭操作，无需在外部调用开启与关闭数据库连接
//以字符串方式传入sql语句，增删改语句支持多个语句同时输入，以分号隔开
//仅测试用，后续可能更改(比如改成静态函数等)

//OracleOpen()打开数据库
//OracleClose()关闭数据库
//OracleUpdate(string sql)返回值：增删改操作受影响的行数(参数可输入多个sql语句，使用分号进行分割
//OracleQuery(string sql)返回值：查询到的数据表
public class OracleHelper
{
    private OracleConnection connection;

    public OracleHelper(string connectionString)
    {
        connection = new OracleConnection(connectionString);
    }

    public void OracleOpen()
    {
        try
        {
            connection.Open();
        }
        catch (Exception ex)
        {
            throw new Exception("连接数据库时出错.", ex);
        }
    }

    public void OracleClose()
    {
        try
        {
            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }
        catch (Exception ex)
        {
            throw new Exception("关闭数据库时出错.", ex);
        }
        finally
        {
            connection.Dispose();
        }
    }

    //返回值：增删改操作受影响的行数
    //可输入多个sql语句，使用分号进行分割
    public int OracleUpdate(string sql)
    {
        int affectedRows = 0;
        try
        {
            string[] statements = sql.Split(';');

            OracleOpen();

            foreach (string statement in statements)
            {
                if (!string.IsNullOrWhiteSpace(statement))
                {
                    using (OracleCommand command = new OracleCommand(statement, connection))
                    {
                        affectedRows += command.ExecuteNonQuery();
                    }
                }
            }

            OracleClose();
        }
        catch (Exception ex)
        {
            OracleClose();
            throw new Exception("An error occurred while updating the database.", ex);
        }
        return affectedRows;
    }

    //返回值：查询到的数据表
    public DataSet OracleQuery(string sql)
    {
        DataSet dataSet = new DataSet();
        try
        {
            OracleOpen();

            using (OracleDataAdapter adapter = new OracleDataAdapter(sql, connection))
            {
                adapter.Fill(dataSet);
            }

            OracleClose();
        }
        catch (Exception ex)
        {
            OracleClose();
            throw new Exception("An error occurred while querying the database.", ex);
        }
        // 检查结果是否为空，如果为空，则返回 null
        return dataSet;
    }
}

// public class Test
// {
//     static void Main(string[] args)
//     {
//         string connectionString = "DATA SOURCE=localhost:1521/orcl;USER ID=BOOKSYS;PASSWORD=book";
//         OracleHelper db = new OracleHelper(connectionString);

//         // 打开数据库连接
//         try
//         {
//             db.OracleOpen();
//             try
//             {
//                 // 执行插入、更新或删除操作
//                 string insertQuery = "INSERT INTO Supplier (supplier_name, phone_number, email, address) VALUES ('Supplier 1', '1234567890', 'supplier1@example.com', 'Address 1');" +
//                 "INSERT INTO Supplier (supplier_name, phone_number, email, address) VALUES ('Supplier 2', '9876543210', 'supplier2@example.com', 'Address 2');";
//                 int affectRows = db.OracleUpdate(insertQuery);
//                 Console.WriteLine(" num of affected rows = " + affectRows);
//             }
//             catch(Exception ex)
//             {
//                 throw new Exception("更新数据库时出错." + ex);
//             }
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine(ex);
//         }

//         try
//         {
//             //执行查询操作
//             string selectQuery = "SELECT * FROM supplier ORDER BY supplier_id ASC ";
//             DataSet dataset = db.OracleQuery(selectQuery);
//             foreach (DataTable dt in dataset.Tables)
//             {
//                 Console.WriteLine("----------------------------------------------------------------------------------------------------");

//                 // 输出列头
//                 foreach (DataColumn column in dt.Columns)
//                 {
//                     Console.Write("| {0,-15}", column.ColumnName); // 调整列宽度和对齐方式
//                 }
//                 Console.WriteLine("|");
//                 Console.WriteLine("----------------------------------------------------------------------------------------------------");

//                 // 输出数据行
//                 foreach (DataRow row in dt.Rows)
//                 {
//                     foreach (var item in row.ItemArray)
//                     {
//                         Console.Write("| {0,-15}", item.ToString()); // 调整列宽度和对齐方式
//                     }
//                     Console.WriteLine("|");
//                 }

//                 Console.WriteLine("----------------------------------------------------------------------------------------------------\n");
//             }
//         }
//         catch (Exception ex)
//         {
//             throw new Exception("查询数据库时出错." + ex);
//         }

//         // 关闭数据库连接
//         try
//         {
//             db.OracleClose();
//         }
//         catch(Exception ex)
//         {
//             Console.WriteLine("退出数据库时出错." + ex);
//         }
//     }
// }