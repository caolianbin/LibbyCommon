using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Partials
{

    /// <summary>
    /// Users局部类
    /// </summary>
    public partial class Users
    {
        public int Id { get; set; }
        public string UserName { get; set; }

        public Users()
        {

        }
    }

    /// <summary>
    /// Users局部类
    /// </summary>
    public partial class Users
    {
        
        public int GetCount()
        {
            return 1;
        }
    }
}
