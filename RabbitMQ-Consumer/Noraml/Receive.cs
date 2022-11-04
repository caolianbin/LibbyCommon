using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ_Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ_Consumer.Noraml
{
    public class Receive
    {
        public static void ReceiveMessage() 
        {
            //消息者消费队列中消息
            string queueName = "normal";
            var connection = RabbitMQHelper.GetConnection("",2222);
            {
                var channel = connection.CreateModel();
                {
                    //如果你先启动是消费端就会异常
                    channel.QueueDeclare(queueName,false,false,false,null);
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, e) =>
                    {
                        var msg = Encoding.UTF8.GetString(e.Body.ToArray());
                        Console.WriteLine(" Normal Received => {0}", msg);
                    };
                    channel.BasicConsume(queueName, true, consumer);
                }
            }
        }
    }
}
