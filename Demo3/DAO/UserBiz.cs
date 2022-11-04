using Demo3.Biz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo3.DAO
{
    public class UserBiz : IUserBiz
    {
        private readonly IUserDAO userDao;

        public UserBiz(IUserDAO userDao)
        {
            this.userDao = userDao;
        }

        public bool CheckLogin(string userName, string password)
        {
            var user = userDao.GetByUserName(userName);
            if (user == null)
            {
                return false;
            }
            else
            {
                return user.Password == password;
            }
        }
    }
}
