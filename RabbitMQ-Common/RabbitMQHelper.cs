using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ_Common
{
    public class RabbitMQHelper
    {
        /// <summary>
        /// 获取单个RabbitMQ连接
        /// </summary> 
        public static IConnection GetConnection() 
        {
            var factroy = new ConnectionFactory
            {
                HostName = "",  //IP
                Port = 555, //端口
                UserName = "",  //用户名
                Password = "",  //密码
                VirtualHost = "/"   //虚拟主机
            };
            return factroy.CreateConnection(); 
        }
        /// <summary>
        /// 根据HostName获取单个RabbitMQ连接
        /// </summary> 

        public static IConnection GetConnection(string hostName, int port) 
        {
            var factroy = new ConnectionFactory 
            {
                HostName = hostName,  //IP
                Port = port, //端口
                UserName = "",  //用户名
                Password = "",  //密码
                VirtualHost = "/"   //虚拟主机
            };
            return factroy.CreateConnection();
        }

        /// <summary>
        /// 获取集群链接对象
        /// </summary>
        /// <returns></returns>
        public static IConnection GetClusterConnection() 
        {
            var factory = new ConnectionFactory 
            {
                UserName = "",  //用户名
                Password = "",  //密码
                VirtualHost = "/"   //虚拟主机
            };
            List<AmqpTcpEndpoint> list = new List<AmqpTcpEndpoint> 
            {
                new AmqpTcpEndpoint(){ HostName="",Port=001 },
                new AmqpTcpEndpoint(){ HostName="",Port=001 },
                new AmqpTcpEndpoint(){ HostName="",Port=001 }
            };
            return factory.CreateConnection(list);
        }



    }
}
