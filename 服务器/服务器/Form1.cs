using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace 服务器
{
    public partial class Form1 : Form
    {
        //委托定义处
        public delegate void A_文本框添加内容(string a, TextBox b);
        public A_文本框添加内容 my_TextBoxApText;
        public void TextAppText(string a, TextBox b) { b.AppendText(a + "\r\n"); }

        public delegate void A_组合框添加表项(string a, ComboBox b);
        public A_组合框添加表项 my_ComboBoxAdd;
        public void ComboBoxAddItem(string a, ComboBox b) { b.Items.Add(a); }

        public Form1()
        {
            InitializeComponent();
            my_TextBoxApText = new A_文本框添加内容(TextAppText);
            my_ComboBoxAdd = new A_组合框添加表项(ComboBoxAddItem);

        }
        Socket Listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

        Socket ImgPort = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

        Socket Ckgn = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

        ArrayList Cg = new ArrayList();

        private void Form1_Load(object sender, EventArgs e)
        {
            IPEndPoint hostEntry = new IPEndPoint(IPAddress.Any, 12346);
            Listener.Bind(hostEntry);
            //开始监听
            Listener.Listen(10);

            hostEntry = new IPEndPoint(IPAddress.Any, 12347);
            ImgPort.Bind(hostEntry);
            //开始监听
            ImgPort.Listen(10);

            //功能：12345
            hostEntry = new IPEndPoint(IPAddress.Any, 12345);
            Ckgn.Bind(hostEntry);
            //开始监听
            Ckgn.Listen(10);

            Thread ti = new Thread(new ThreadStart(Holle_Gn));
            ti.IsBackground = true;
            ti.Start();

            for (int i = 1; i <= 2; i++)
            {
                Thread tt = new Thread(new ThreadStart(Holle_Coke_Deer));
                tt.IsBackground = true;
                tt.Start();
                textBox1.Invoke(my_TextBoxApText, "监听：已开启第" + i + "个线程监听12346端口的第一次握手信息！", textBox1);

                Thread t = new Thread(new ThreadStart(ImgTake));
                t.IsBackground = true;
                t.Start();
            }


        }
        /// <summary>
        /// 功能代码
        /// </summary>
        private void Holle_Gn()
        {
            //存储数据的字节
            byte[] data;
            //收到信息的长度
            int length = 0;
            while (true)
            {
                //接收到远程的连接
                Socket remoteClient = Ckgn.Accept();
                //重新初始化下接收数据的变量 防止连续接收时数据叠加
                StringBuilder receiveData = new StringBuilder();

                data = new byte[1024];
                length = remoteClient.Receive(data);

                string wi = Encoding.UTF8.GetString(data);
                if (wi.IndexOf("远程请求") != -1)
                {
                    string[] fg = remoteClient.RemoteEndPoint.ToString().Split(":".ToCharArray());

                    
                    Debug.WriteLine(fg[0]);
                    YcLj a = new YcLj();
                    a.xz = fg[0];
                    xz = fg[0];
                    a.ShowDialog();
                   
                   

                }
            }
        }

        

        /// <summary>
        /// 连接开始的信息处理
        /// </summary>
        private void Holle_Coke_Deer()
        {
            //存储数据的字节
            byte[] data;
            //收到信息的长度
            int length = 0;
            //接收到远程的连接
            Socket remoteClient = Listener.Accept();
            //重新初始化下接收数据的变量 防止连续接收时数据叠加
            StringBuilder receiveData = new StringBuilder();
            data = new byte[1024];
            length = remoteClient.Receive(data);
            string wi = Encoding.ASCII.GetString(data);
            string ip = (remoteClient.RemoteEndPoint.ToString().Split(":".ToCharArray()))[0];
            int boolean = 0;
            for (int i = 0; i < Cg.Count; i++)
            {
                if (Cg[i].ToString().IndexOf(ip) != -1)
                {
                    boolean++;
                }

            }

            if (wi.IndexOf("CoKe_Deer") != -1 && boolean != 1)
            {
                // RemoteEndPoint	

                Cg.Add(ip);
                comboBox1.Invoke(my_ComboBoxAdd, ip, comboBox1);
                textBox1.Invoke(my_TextBoxApText, "来自：" + remoteClient.RemoteEndPoint.ToString() + "的第一次握手信息！", textBox1);
                remoteClient.Close();
                remoteClient = null;



            }
        }

        /// <summary>
        /// 压缩
        /// </summary>
        /// <param name="inBytes"></param>
        /// <returns></returns>
        public static MemoryStream Compress(byte[] inBytes)
        {
            MemoryStream outStream = new MemoryStream();
            using (MemoryStream intStream = new MemoryStream(inBytes))
            {
                using (GZipStream Compress =
                    new GZipStream(outStream,
                    CompressionMode.Compress))
                {
                    intStream.CopyTo(Compress);
                }
            }
            return outStream;
        }
        /// <summary>
        /// 解压缩
        /// </summary>
        /// <param name="inStream"></param>
        /// <returns></returns>
        public static byte[] Decompress(MemoryStream inStream)
        {
            byte[] result = null;
            MemoryStream compressedStream = new MemoryStream(inStream.ToArray());
            using (MemoryStream outStream = new MemoryStream())
            {
                using (GZipStream Decompress = new GZipStream(compressedStream,
                        CompressionMode.Decompress))
                {
                    Decompress.CopyTo(outStream);
                    result = outStream.ToArray();
                }
            }
            return result;
        }

        private void ImgTake()
        {

            textBox1.Invoke(my_TextBoxApText, "监听：已开始监听是否有图片信息", textBox1);
            int i = 0;
            while (true)
            {
                //存储数据的字节
                byte[] data;
                //收到信息的长度
                int length = 0;
                //接收到远程的连接
                Socket remoteClient = ImgPort.Accept();
                //重新初始化下接收数据的变量 防止连续接收时数据叠加

                while (true)
                {
                    StringBuilder receiveData = new StringBuilder();
                    data = new byte[1024];
                    length = remoteClient.Receive(data);
                    string wi = Encoding.UTF8.GetString(data);
                    if (wi.IndexOf("结束") != -1)
                    {
                        //将文件尾写入
                        wi = wi.Replace("结束", "");
                        data = Encoding.UTF8.GetBytes(wi);
                        FileStream fStream = new FileStream(@"F:\img\" + i + ".jpg", FileMode.Append);

                        fStream.Write(data, 0, data.Length);
                        fStream.Close();
                        remoteClient.Close();

                        i++;
                        break;
                    }
                    else
                    {
                        FileStream fStream = new FileStream(@"F:\img\" + i + ".jpg", FileMode.Append);

                        fStream.Write(data, 0, length);
                        fStream.Close();
                    }
                }

            }


        }

        public byte[] GetPictureData(string imagepath)
        {
            /**/
            ////根据图片文件的路径使用文件流打开，并保存为byte[] 
            FileStream fs = new FileStream(imagepath, FileMode.Open);//可以是其他重载方法 
            byte[] byData = new byte[fs.Length];
            fs.Read(byData, 0, byData.Length);
            fs.Close();
            return byData;
        }

        public static string xz = "";

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text.Length == 0) { MessageBox.Show("请重新确认!"); return; }

            xz = comboBox1.Text;

            YcLj a = new YcLj();
            a.Show();
        }



    }
}
