using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Squares
    {
        /// <summary>
        /// 取平方数
        /// </summary>
        public void SquareGet()
        {

            IEnumerable<int> s = foo();
            foreach (int i in s)
            {
                Console.WriteLine(i);
            }


            IEnumerable<int> foo()
            {
                int i = 0;
                while (i * i <= 100)
                {
                    yield return i * i;
                    i++;
                }
            }

        }
    }
}
