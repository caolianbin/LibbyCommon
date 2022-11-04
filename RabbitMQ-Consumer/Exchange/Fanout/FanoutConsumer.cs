using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ_Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ_Consumer.Exchange.Fanout
{
    public class FanoutConsumer
    {
        //扇形队列读取消息
        public static void ConsumerMessage() 
        {
            var connection = RabbitMQHelper.GetConnection();
            {
                var channel = connection.CreateModel();
                {
                    //声明exchange
                    channel.ExchangeDeclare(exchange:"fanout_exchange",type:"fanout");

                    //创建队列
                    string queueName1 = "fanout_queue1";
                    channel.QueueDeclare(queueName1, false, false, false, null);
                    string queueName2 = "fanout_queue2";
                    channel.QueueDeclare(queueName2, false, false, false, null);
                    string queueName3 = "fanout_queue3";
                    channel.QueueDeclare(queueName3, false, false, false, null);

                    //绑定交换机，将三个队列绑定到交换机
                    channel.QueueBind(queue: queueName1, exchange: "fanout_exchange", routingKey: "");
                    channel.QueueBind(queue: queueName2, exchange: "fanout_exchange", routingKey: "");
                    channel.QueueBind(queue: queueName3, exchange: "fanout_exchange", routingKey: "");

                    //声明consumer
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, e) =>
                    {
                        var body = e.Body;
                        var msg = Encoding.UTF8.GetString(body.ToArray());
                        Console.WriteLine($"消息：{msg}");
                    };
                    channel.BasicConsume(queue: queueName2,autoAck:true,consumer:consumer);
                    Console.WriteLine(" exit ");
                    
                }
            }
        }
    }
}
