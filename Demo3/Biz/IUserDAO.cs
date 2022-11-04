using Demo3.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo3.Biz
{
    public interface IUserDAO
    {
        public User? GetByUserName(string userName);//查询用户名为userName的用户信息
    }
}
