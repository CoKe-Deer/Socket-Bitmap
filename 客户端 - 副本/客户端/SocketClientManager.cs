using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;


namespace ClientBySocket
{
    public class SocketClientManager
    {
        public Socket _socket = null;
        public EndPoint endPoint = null;
        public SocketInfo socketInfo = null;
        public bool _isConnected = false;

        public delegate void OnConnectedHandler();
        public event OnConnectedHandler OnConnected;
        public event OnConnectedHandler OnFaildConnect;
        public delegate void OnReceiveMsgHandler();
        public event OnReceiveMsgHandler OnReceiveMsg;

        public SocketClientManager(string ip, int port)
        {
            IPAddress _ip = IPAddress.Parse(ip);
            endPoint = new IPEndPoint(_ip, port);
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        private readonly ManualResetEvent TimeoutObject = new ManualResetEvent(false);
        public int Start()
        {
            TimeoutObject.Reset();
            _socket.BeginConnect(endPoint, ConnectedCallback, _socket);
            int a;
            if (TimeoutObject.WaitOne(20, false))
            {
                _isConnected = true;
                Thread socketClient = new Thread(SocketClientReceive);
                socketClient.IsBackground = true;
                socketClient.Start();
                a= 1;
            }
            else
            {
                a= 0;
            }
            return a;
        }
        private void CallBackMethod(IAsyncResult asyncresult)
        {
            //使阻塞的线程继续          
            TimeoutObject.Set();
        }

        public void SocketClientReceive()
        {
            while (_isConnected)
            {
                SocketInfo info = new SocketInfo();
                try {
                    _socket.BeginReceive(info.buffer, 0, info.buffer.Length, SocketFlags.None, ReceiveCallback, info);
                }
                catch (SocketException ex)
                {
                    _isConnected = false;
                }
                
                Thread.Sleep(100);
            }
        }
        
        public void ReceiveCallback(IAsyncResult ar)
        {
            socketInfo = ar.AsyncState as SocketInfo;
            if (this.OnReceiveMsg != null) OnReceiveMsg();
        }

        public void ConnectedCallback(IAsyncResult ar)
        {
            Socket socket = ar.AsyncState as Socket;
            if (socket.Connected)
            {
                if (this.OnConnected != null) OnConnected();
            }
            else
            {
                if (this.OnFaildConnect != null) OnFaildConnect();
            }
        }

        public void SendMsg(string msg)
        {
            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes(msg);
                _socket.Send(buffer);
            }
            catch { }
        }

        public class SocketInfo
        {
            public Socket socket = null;
            public byte[] buffer = null;

            public SocketInfo()
            {
                buffer = new byte[1024 * 4];
            }
        }
    }
}
