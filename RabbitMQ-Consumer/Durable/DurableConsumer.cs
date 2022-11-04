using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ_Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ_Consumer.Durable
{
    public class DurableConsumer
    {
        public static void ReceiveMessage() 
        {
            //消息者消费队列中的消息
         
            var connection = RabbitMQHelper.GetConnection("ip", 000);
            {
                var channel = connection.CreateModel();
                {
                    //1、创建持久化队列
                    channel.QueueDeclare("durable_queue", true, false, false, null);
                    //2、创建持久化的交换机
                    channel.ExchangeDeclare("durable_exchange", "fanovt", true, false, null);
                    channel.QueueBind("durable_queue", "durable_exchange", "", null);
 
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, e) =>{
                        var msg = Encoding.UTF8.GetString(e.Body.ToArray());
                    };
                    channel.BasicConsume("durable_queue", true, consumer);
                }
            }
        }
    }
}
