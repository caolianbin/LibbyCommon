using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Common
{
    public class CheckMethod
    {
        /// <summary>
        /// 验证是否为邮箱
        /// </summary>
        /// <param name="mail"></param>
        /// <returns></returns>
        public static bool CheckMail(string mail) 
        {
            string str = @"^[a-zA-Z0-9_.-]+@[a-zA-Z0-9]+(\.[a-zA-Z0-9]+)*\.(com|cn|net)$";
            Regex mReg = new Regex(str);
            if (mReg.IsMatch(mail))
            {
                return true;
            }
            return false;
        }
    }
}
