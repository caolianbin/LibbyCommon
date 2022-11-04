using RabbitMQ_Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ_Provider.Normal
{
    public class Send
    {
        /// <summary>
        /// 集群队列
        /// </summary>
        public static void SendMessage() 
        {
            //向集群架构发送消息
            string queueName = "normal";
            using (var connetion = RabbitMQHelper.GetClusterConnection()) 
            {
                //创建信道
                using (var channel = connetion.CreateModel()) 
                {
                    //创建队列
                    channel.QueueDeclare(queueName,false,false,false,null);
                    while (true)
                    {
                        string msg = "你好 RabbitMQ 消息";
                        var body = Encoding.UTF8.GetBytes(msg);
                        //发送消息到RabbitMQ，使用RabbitMQ中默认提供交换机路由，
                        //默认的路由key和队列名称完全一致
                        channel.BasicPublish(exchange:"",routingKey:queueName,false,null, body);
                        Thread.Sleep(1000);
                        Console.WriteLine("发送Normal消息");

                    }
                }
            }
        }
    }
}
