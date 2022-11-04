using RabbitMQ.Client;
using RabbitMQ_Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ_Provider.Exchange.Direct
{
    public class DirectSend
    {
        /// <summary>
        ///  直接队列模式
        /// </summary>
        public static void SendMessage()
        {

            using (var connetion = RabbitMQHelper.GetConnection())
            {
                //创建信道
                using (var channel = connetion.CreateModel())
                {
                    //声明direct交换机
                    channel.ExchangeDeclare("direct_exchange", "direct");

                    //创建队列
                    string queueName1 = "direct_queue1";
                    channel.QueueDeclare(queueName1, false, false, false, null);
                    string queueName2 = "direct_queue2";
                    channel.QueueDeclare(queueName2, false, false, false, null);
                    string queueName3 = "direct_queue3";
                    channel.QueueDeclare(queueName3, false, false, false, null);

                    //绑定交换机,指定routingKey
                    channel.QueueBind(queue: queueName1, exchange: "direct_exchange", routingKey: "red");
                    channel.QueueBind(queue: queueName2, exchange: "direct_exchange", routingKey: "blue");
                    channel.QueueBind(queue: queueName3, exchange: "direct_exchange", routingKey: "green");


                    for (int i = 0; i < 10; i++)
                    {
                        string msg = "你好 RabbitMQ 消息";
                        var body = Encoding.UTF8.GetBytes(msg);
                        // 发送消息的时候需要指定routingKey发送
                        channel.BasicPublish("direct_exchange", "blue", null, body); //发送到交换机
                        Console.WriteLine("发送Fanout消息");
                    }


                }
            }
        }
    }
}
