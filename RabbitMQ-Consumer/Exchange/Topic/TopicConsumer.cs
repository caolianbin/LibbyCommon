using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ_Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ_Consumer.Exchange.Topic
{
    public class TopicConsumer
    {
        /// <summary>
        /// 模糊匹配模式
        /// </summary>
        public static void ConsumerMessage()
        {
            var connection = RabbitMQHelper.GetConnection();
            {
                var channel = connection.CreateModel();
                {
                    //声明exchange
                    channel.ExchangeDeclare(exchange: "topic_exchange", type: "topic");

                    //创建队列
                    string queueName = "topic_queue1";
                    channel.QueueDeclare(queueName, false, false, false, null);
                    // user.data.*  可以消费所有数据 但是这个好像有Bug 
                    channel.QueueBind(queue: queueName, exchange: "topic_exchange", routingKey: "user.data.*");
                    
                    //声明consumer
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, e) =>
                    {
                        var body = e.Body;
                        var msg = Encoding.UTF8.GetString(body.ToArray());
                        var routingKey = e.RoutingKey;
                        Console.WriteLine($"消息：{msg}  key=》{routingKey}");
                         
                    };
                    // 消息签收模式
                    // 手动签收 保证正确消费，不会丢消息（基于客户端而已）
                    // 自动签收 容易丢消息
                    //签收：意味着消息从队列中删除
                    channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
                    Console.WriteLine(" exit ");

                }
            }
        }
    }
}
