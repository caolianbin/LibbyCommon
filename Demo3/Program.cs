
// DI 依赖注入

using Demo3.Biz;
using Demo3.DAO;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using System.Data.SqlClient;

ServiceCollection services = new ServiceCollection();
services.AddScoped<IDbConnection>(sp => {
    string connStr = "Data Source=.;Initial Catalog=DI_DB;Integrated Security=true";
    var conn = new SqlConnection(connStr);
    conn.Open();
    return conn;
});
services.AddScoped<IUserDAO, UserDAO>();
services.AddScoped<IUserBiz, UserBiz>();
using (ServiceProvider sp = services.BuildServiceProvider())
{
    var userBiz = sp.GetRequiredService<IUserBiz>();
    bool b = userBiz.CheckLogin("yzk", "123456");
    Console.WriteLine(b);
}


