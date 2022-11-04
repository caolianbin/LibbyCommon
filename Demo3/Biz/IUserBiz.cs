using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo3.Biz
{
    public interface IUserBiz
    {
        public bool CheckLogin(string userName, string password);// 检查用户名、密码是否匹配
    }
}
