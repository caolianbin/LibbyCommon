using RabbitMQ_Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ_Provider.Worker
{
    public class Send
    {
        /// <summary>
        /// 工作队列模式
        /// </summary>
        public static void SendMessage()
        {
          
            string queueName = "Worker_Queue";
            using (var connetion = RabbitMQHelper.GetConnection())
            {
                //创建信道
                using (var channel = connetion.CreateModel())
                {
                    //创建队列
                    channel.QueueDeclare(queueName, false, false, false, null);

                    for (int i = 0; i < 30; i++)
                    {
                        string msg = "你好 RabbitMQ 消息";
                        var body = Encoding.UTF8.GetBytes(msg);
                        channel.BasicPublish(exchange: "", routingKey: queueName, false, null, body);
                        Console.WriteLine("发送Normal消息");
                    }

                   
                }
            }
        }
    }
}
