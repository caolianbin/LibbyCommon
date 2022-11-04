using RabbitMQ.Client;
using RabbitMQ_Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ_Provider.Exchange.Fanout
{
    public class FanoutSend
    {
        /// <summary>
        /// 扇形队列模式
        /// </summary>
        public static void SendMessage()
        {
             
            using (var connetion = RabbitMQHelper.GetConnection())
            {
                //创建信道
                using (var channel = connetion.CreateModel())
                {
                    //声明交换机
                    channel.ExchangeDeclare("fanout_exchange","fanout");

                    //创建队列
                    string queueName1 = "fanout_queue1"; 
                    channel.QueueDeclare(queueName1, false, false, false, null);
                    string queueName2 = "fanout_queue2";
                    channel.QueueDeclare(queueName2, false, false, false, null);
                    string queueName3 = "fanout_queue3";
                    channel.QueueDeclare(queueName3, false, false, false, null);

                    //绑定交换机，将三个队列绑定到交换机
                    channel.QueueBind(queue: queueName1,exchange: "fanout_exchange",routingKey:"");
                    channel.QueueBind(queue: queueName2, exchange: "fanout_exchange", routingKey: "");
                    channel.QueueBind(queue: queueName3, exchange: "fanout_exchange", routingKey: "");


                    for (int i = 0; i < 10; i++)
                    {
                        string msg = "你好 RabbitMQ 消息";
                        var body = Encoding.UTF8.GetBytes(msg);
                        channel.BasicPublish("fanout_exchange", "", null, body); //发送到交换机
                        Console.WriteLine("发送Fanout消息");
                    }


                }
            }
        }
    }
}
