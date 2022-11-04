using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ_Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ_Consumer.Worker
{
    public class WorkerReceive
    {
        public static void ReceiveMessage()
        {
            //消息者消费队列中消息
            string queueName = "Worker_Queue";
            var connection = RabbitMQHelper.GetConnection("", 2222);
            {
                var channel = connection.CreateModel();
                {
                    //如果你先启动是消费端就会异常
                    channel.QueueDeclare(queueName, false, false, false, null);
                    var consumer = new EventingBasicConsumer(channel);

                    //设置prefetchCount:1 来告知RabbitMQ，在未收到消息端的消息确认时，不删除数据
                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

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
