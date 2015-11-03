using ITalkTradition.Code;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace 客户端
{
    /// <summary>
    /// socket客户端 
    /// </summary>
    public class SocketClient
    {
        private readonly ManualResetEvent TimeoutObject = new ManualResetEvent(false);
        #region Field

        /// <summary>
        /// 定义一个当前客户端的socket套链接
        /// </summary>
        private Socket client;

        /// <summary>
        /// 定义监听端口
        /// </summary>
        private int port;

        /// <summary>
        /// 发出去的数据
        /// </summary>
        private StringBuilder sendData = new StringBuilder();

        /// <summary>
        /// 发送信息字符串尺寸 默认1024字节
        /// </summary>
        private int dataSize = 1024;

        /// <summary>
        /// 错误消息
        /// </summary>
        private string errorMsg = "";


        /// <summary>
        /// 完成一个接受的委托
        /// </summary>
        /// <param name="talk"></param>
        /// <param name="e"></param>
        public delegate void CompleteHandler(SocketClient talk, EventArgs e);

        /// <summary>
        /// 完成发送事件
        /// </summary>
        public event CompleteHandler SendComplete;

        #endregion

        #region Constructor

        /// <summary>
        /// 构造函数  主要用户初始化本机的socket
        /// <param name="ip">ip地址</param>
        /// </summary>
        public SocketClient(string ip)
        {
            ip1 = ip;
            this.Initializion(IPAddress.Parse(ip));
        }
        string ip1;
        #endregion

        #region Destructor

        /// <summary>
        /// 析构函数 关闭建立的socket
        /// </summary>
        ~SocketClient()
        {
            this.client.Close();
        }

        #endregion

        #region Property


        /// <summary>
        /// 错误消息 为空时表示没有错误
        /// </summary>
        public string ErrorMsg
        {
            get
            {
                return this.errorMsg;
            }
        }

        /// <summary>
        /// 发送出去的数据
        /// </summary>
        public string SendData
        {
            get
            {
                return this.sendData.ToString();
            }
        }

        #endregion

        #region Method

        #region 初始化本机的Socket
        /// <summary>
        /// 初始化本机的Socket
        /// <param name="ipAddress">服务器的IP地址</param>
        /// </summary>
        public int Initializion(IPAddress ipAddress)
        {
            TimeoutObject.Reset();
            this.port = 12346;
            this.client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //新建远程的端点
            IPEndPoint remotePoint = new IPEndPoint(ipAddress, this.port);

            object i = this.client;

            this.client.BeginConnect(remotePoint, CallBackMethod, this.client);

            if (TimeoutObject.WaitOne(20, false))
            {
                return 1;
            }
            else
            {
                return 0;
            }

        }
        private void CallBackMethod(IAsyncResult asyncresult)
        {
            //使阻塞的线程继续          
            TimeoutObject.Set();
        }
        #endregion

        #region 发送信息
        /// <summary>
        /// 发送信息
        /// </summary>
        /// <param name="message">信息的内容</param>
        public void Send(string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            this.client.Send(data);
            return;
            int length = message.Length;
            int i = 0;


            try
            {
                while (true)
                {
                    //如果发送字节长度大于  规定字节长度 则分批发送
                    if (length > this.dataSize)
                    {
                        data = Encoding.ASCII.GetBytes(message.Substring(i, this.dataSize));
                        this.client.Send(data, this.dataSize, SocketFlags.None);
                        this.sendData.Append(Encoding.ASCII.GetString(data, 0, this.dataSize));
                        length -= this.dataSize;
                        i += length;
                    }
                    else
                    {
                        //把最后剩下的字节长度发送出去
                        data = Encoding.ASCII.GetBytes(message.Substring(i, length));
                        this.client.Send(data, length, SocketFlags.None);
                        this.sendData.Append(Encoding.ASCII.GetString(data, 0, length));
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("发送出错：" + ex.Message);
            }
           

        }


        #endregion

        #endregion
    }


    class ScOket
    {
       
    }
}
