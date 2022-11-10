using System.Diagnostics;
using System.Threading;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace 多线程01
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        /// 1、单线程卡界面，多线程不卡界面
        /// 单线程卡界面表示线程在忙碌 
        private void button1_Click(object sender, EventArgs e)
        {
            Debug.Write("");
            Debug.Write("----------  同步方法---------- -----  ");
            Debug.Write(Thread.CurrentThread.ManagedThreadId);
          
			for(int i=0;i<5;i++)
			{
                string name = $"===={i}=====";
                this.DoSomethingLong(name);
                //委托
                Action action = () => this.DoSomethingLong(name);//lamba表达式
                action.Invoke(); //同步
            }
        }
        /// <summary>
        /// 多线程简单API
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            Debug.Write("----------  多线程简单API---------- -----  ");
            Debug.Write(Thread.CurrentThread.ManagedThreadId);

            for (int i = 0; i < 10; i++)
            {
                string name = $"===={i}=====";
                this.DoSomethingLong(name);
              
                Action action = () => this.DoSomethingLong(name);//lamba表达式
                Task.Run(action);
            }
        }
        private void DoSomethingLong(string name) 
        {
            Debug.WriteLine($" {name} ===》 时间：{DateTime.Now.ToString("HHmmss:fff")}");
            long r = 0;
            for (int i = 0; i < 1_000_000_000; i++)
            {
                r += i;
            }
            Debug.WriteLine($" {name} ===》 时间：{DateTime.Now.ToString("HHmmss:fff")}");

        }

        private void button7_Click(object sender, EventArgs e)
        {
            Debug.WriteLine($"  ===》 时间：{DateTime.Now.ToString("HHmmss:fff")}");
            //单线程有前一步才有后一步，不能并发。
            //开发中，并不是好耗时的任务就一定能多线程
            //什么样的业务才能多线程？ 任务能拆分，能并行，才能多线程

            Debug.WriteLine($" 业务沟通{DateTime.Now.ToString("HHmmss:fff")}");
            Debug.WriteLine($" 确认需求  ");
            Debug.WriteLine($" 签合同  ");
            Debug.WriteLine($" 数据库设计  ");
            Debug.WriteLine($" 程序编码 ");
            Debug.WriteLine($" 测试上线  ");

            //多线程
            List<Task> taskList = new List<Task>();
            taskList.Add(Task.Run(()=>this.Coding("11111","aaaa")));
            taskList.Add(Task.Run(() => this.Coding("22222", "bbbb")));
            taskList.Add(Task.Run(() => this.Coding("33333", "cccc")));
            taskList.Add(Task.Run(() => this.Coding("4444", "dddd")));
            taskList.Add(Task.Run(() => this.Coding("5555", "eeee")));
            taskList.Add(Task.Run(() => this.Coding("66666", "ffff")));

            //不要阻塞主线程，但是又能再全部/任意任务完成后，执行一个动作，例如：写日志


            //------------------------------ 回调 -----------------------------------------------

            taskList[0].ContinueWith(t => Debug.WriteLine($" this is single callback {Thread.CurrentThread.ManagedThreadId}")); //单个Task回调
                                                                                                                                //任意一个完成的回调
            taskList.Add(Task.Factory.ContinueWhenAny(taskList.ToArray(), t => {
                Debug.WriteLine($"{Thread.CurrentThread.ManagedThreadId} 第一个完成 领取红包");
            }));

            //全部任务完成的回调
            taskList.Add(Task.Factory.ContinueWhenAll(taskList.ToArray(), tlist => {
                Debug.WriteLine("项目全部开发完成");
            }));



            //后续业务
            //--------------------------------- 等待--------------------------------------------
            //Task.Run(() => {
            Task.WaitAny(taskList.ToArray());//阻塞到任意一个Task完成, --- UI界面不卡
            Debug.WriteLine($"恭喜：{Thread.CurrentThread.ManagedThreadId} 完成工作！  ");

            Task.WaitAll(taskList.ToArray()); // 阻塞所有Task完成。主线程在等待。UI界面会卡
            Debug.WriteLine($"  项目验收，付款。  ");
            //}); //包一层Task.Run() UI界面就不会卡，但是不建议包一层，线程嵌套多层会很难搞。




        }
        private void Coding(string name,string project)
        {
            Debug.WriteLine($" {name}====={project}===》 时间：{DateTime.Now.ToString("HHmmss:fff")}");
            long r = 0;
            for (int i = 0; i < 1000000000; i++)
            {
                r += i;
            }
            Debug.WriteLine($" {name}====={project}===》 时间：{DateTime.Now.ToString("HHmmss:fff")}");

        }


        #region 线程任务分配
        // 线程计划任务分配01
        private void button3_Click(object sender, EventArgs e)
        {
            //任务数
            int[][] typeArray = new int[100][];
            for (int i = 0; i < 100; i++)
            {
                int[] innerArray = new int[i];
                for (int j = 0; j < i; j++)
                {
                    innerArray[j] = j;
                }
                typeArray[i] = innerArray;
            }

            List<Task> taskList = new List<Task>();
            int threadNum = 27;  //线程数

            #region 线程任务计划
            List<List<int[]>> dispacherTypeList = new List<List<int[]>>();
            for (int i = 0; i < threadNum; i++)
            { 
                //分配27个线程
                dispacherTypeList.Add(new List<int[]>() { });
            }
            // 第一个线程：0 5 10
            // 第二个线程：1 6 11
            // 第三个线程：2 7 12 
            // 第四个线程：3 8 13
            // 第五个线程：4 9 14
            // .............

            for (int i = 0; i < 100; i++)
            {
                var s = i % threadNum; //取余 获得0-27 
                dispacherTypeList[s].Add(typeArray[i]); 
            }
            #endregion

            //循环
            for (int i = 0; i < threadNum; i++)
            {
                List<int[]> currentTypeArray = dispacherTypeList[i];
                taskList.Add(Task.Run(() =>
                {
                    foreach (var array in currentTypeArray)
                    {
                        Debug.WriteLine($"CurrentThread={Thread.CurrentThread.ManagedThreadId},执行{string.Join(",", array)}");
                        foreach (var num in array)
                        {
                            Thread.Sleep(num);
                        }
                    }
                    Debug.WriteLine($"线程ID={Thread.CurrentThread.ManagedThreadId},任务完成");
                }));
            }

            Task.WaitAll(taskList.ToArray());
        }
        // 线程计划任务分配02
        private void button4_Click(object sender, EventArgs e)
        {
            //任务数
            int[][] typeArray = new int[100][];
            for (int i = 0; i < 100; i++)
            {
                int[] innerArray = new int[i];
                for (int j = 0; j < i; j++)
                {
                    innerArray[j] = j;
                }
                typeArray[i] = innerArray;
            }

            List<Task> taskList = new List<Task>();
            int threadNum = 27;  //线程数



            #region 线程任务均分计划
            List<List<int[]>> dispacherTypeList = new List<List<int[]>>();
            for (int i = 0; i < threadNum; i++)
            {
                //分配27个线程
                dispacherTypeList.Add(new List<int[]>() { });
            }

            // 第一个线程：0-3
            // 第二个线程：4-7
            // 第三个线程：8-11
            // ......
            int numberPerThread = 100 / threadNum + 1;
            int index = -1;
            for (int i = 0; i < 100; i++)
            {
                if (i % numberPerThread == 0)
                {
                    index++;
                }
                dispacherTypeList[index].Add(typeArray[i]);
            }
            #endregion

            //循环
            for (int i = 0; i < threadNum; i++)
            {
                List<int[]> currentTypeArray = dispacherTypeList[i];
                taskList.Add(Task.Run(() =>
                {
                    foreach (var array in currentTypeArray)
                    {
                        Debug.WriteLine($"CurrentThread={Thread.CurrentThread.ManagedThreadId},执行{string.Join(",", array)}");
                        foreach (var num in array)
                        {
                            Thread.Sleep(num);
                        }
                    }
                    Debug.WriteLine($"线程ID={Thread.CurrentThread.ManagedThreadId},任务完成");
                }));
            }

            Task.WaitAll(taskList.ToArray());
        }
        //线程计划任务分配03
        private void button5_Click(object sender, EventArgs e)
        { 
            //任务数
            int[][] typeArray = new int[100][];
            for (int i = 0; i < 100; i++)
            {
                int[] innerArray = new int[i];
                for (int j = 0; j < i; j++)
                {
                    innerArray[j] = j;
                }
                typeArray[i] = innerArray;
            }

            List<Task> taskList = new List<Task>();
            int threadNum = 27;  //线程数
            // 遍历100个任务。将任务线程加入集合中。如果集合中的线程数量等于27，那么就使用Tak.WaitAny（等待某一个完成）然后重新获取在线的线程任务。进入下一个任务。
            
            for (int i = 0; i < typeArray.Length; i++)
            {
                var currentTypeArray = typeArray[i];
                // 加入集合
                taskList.Add(
                    Task.Run(() => {
                        Debug.WriteLine($"线程ID={Thread.CurrentThread.ManagedThreadId}，执行{string.Join(",", currentTypeArray)}");
                        foreach (var num in currentTypeArray)
                        {
                            Thread.Sleep(num);
                        }
                        Debug.WriteLine($"线程ID={Thread.CurrentThread.ManagedThreadId},任务完成");
                    })
                );
                //集合数量等于27，那么就等待其中某一个线程完成。继续下个任务。
                if (taskList.Count() == threadNum)
                {
                    Task.WaitAny(taskList.ToArray());// 等待某一个完成后执行下一个任务
                    taskList = taskList.Where(t => t.Status == TaskStatus.Running).ToList();// 获取当前还在执行的线程任务
                }
            }

            Task.WaitAll(taskList.ToArray());
        }
        //线程计划任务分配04
        private void button6_Click(object sender, EventArgs e)
        {
            //任务数
            int[][] typeArray = new int[100][];
            for (int i = 0; i < 100; i++)
            {
                int[] innerArray = new int[i];
                for (int j = 0; j < i; j++)
                {
                    innerArray[j] = j;
                }
                typeArray[i] = innerArray;
            }
          
            List<Task> taskList = new List<Task>();
            int threadNum = 27;  //线程数

            ParallelOptions parallelOptions = new ParallelOptions()
            {
                MaxDegreeOfParallelism = threadNum
            };
            // Parallel 就是把所有任务（typeArray）用这么多线程（parallelOptions）进行合理分配
            // typeArray 任务
            // parallelOptions 线程数
            // currentTypeArray 执行
            Parallel.ForEach(typeArray, parallelOptions, currentTypeArray =>
            {
                Debug.WriteLine($"线程ID={Thread.CurrentThread.ManagedThreadId}，执行{string.Join(",", currentTypeArray)}");
                foreach (var num in currentTypeArray)
                {
                    Thread.Sleep(num);
                }
                Debug.WriteLine($"线程ID={Thread.CurrentThread.ManagedThreadId},任务完成");
            });

            Task.WaitAll(taskList.ToArray());
        }
        #endregion

    }
}