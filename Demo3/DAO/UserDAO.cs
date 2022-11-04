using Demo3.Biz;
using Demo3.DBHelper;
using Demo3.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo3.DAO
{
    public class UserDAO : IUserDAO
    {
        private readonly IDbConnection conn;

        public UserDAO(IDbConnection conn)
        {
            this.conn = conn;
        }
        
        public User? GetByUserName(string userName)
        {
            using var dt = SqlHelper.ExecuteQuery(conn,
               $"select * from T_Users where UserName={userName}");
            if (dt.Rows.Count <= 0)
            {
                return null;
            }
            DataRow row = dt.Rows[0];
            long id = (long)row["Id"];
            string uname = (string)row["UserName"];
            string password = (string)row["Password"];
            return new User(id, uname, password);
        }
    }
}
