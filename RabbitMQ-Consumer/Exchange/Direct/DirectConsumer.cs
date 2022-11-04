using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ_Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ_Consumer.Exchange.Direct
{
    public class DirectConsumer
    {
        /// <summary>
        /// 直接模式
        /// </summary>
        public static void ConsumerMessage()
        {
            var connection = RabbitMQHelper.GetConnection();
            {
                var channel = connection.CreateModel();
                {
                    //声明exchange
                    channel.ExchangeDeclare(exchange: "direct_exchange", type: "direct");

                    //创建队列
                    string queueName = "direct_queue1";
                    channel.QueueDeclare(queueName, false, false, false, null);
              
                    channel.QueueBind(queue: queueName, exchange: "direct_exchange", routingKey: "red");
                    channel.QueueBind(queue: queueName, exchange: "direct_exchange", routingKey: "blue");
                    channel.QueueBind(queue: queueName, exchange: "direct_exchange", routingKey: "green");

                    //声明consumer
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, e) =>
                    {
                        var body = e.Body;
                        var msg = Encoding.UTF8.GetString(body.ToArray());
                        var routingKey = e.RoutingKey;
                        Console.WriteLine($"消息：{msg}  key=》{routingKey}");
                        
                        // 消费完成后需要手动签收消息，如果不写该代码就容易导致重复消费问题
                        channel.BasicAck(e.DeliveryTag, false); //可以降低每次签收性能损耗
                    };
                    // 消息签收模式
                    // 手动签收 保证正确消费，不会丢消息（基于客户端而已）
                    // 自动签收 容易丢消息
                    //签收：意味着消息从队列中删除
                    channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
                    Console.WriteLine(" exit ");

                }
            }
        }
    }
}
