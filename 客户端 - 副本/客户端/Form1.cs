
using ITalkTradition.Code;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace 客户端
{


    public partial class Form1 : Form
    {
        string wb = "This is CoKe_Deer!!!";
        int po = 12346;
        public string ip;


        public Form1()
        {
            InitializeComponent();

        }
        private void GetLocalIPAddress(ref ArrayList mac, ref ArrayList ip2)
        {
            ArrayList w = new ArrayList();
            System.Net.IPAddress[] addressList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
            foreach (IPAddress a in addressList)
            {
                if (((a.ToString()).Split(".".ToCharArray())).Length != 4)
                {
                    w.Add(a);
                }
                else
                {
                    ip2.Add(a);
                }
            }
            if (mac != null)
            {
                mac = w;
            }

        }
        //写线程

        //委托定义处
        public delegate void A_文本框添加内容(string a, TextBox b);
        public A_文本框添加内容 my_TextBoxApText;
        public void TextAppText(string a, TextBox b) { b.AppendText(a + "\r\n"); }

        public delegate void A_组合框添加表项(string a, ComboBox b);
        public A_组合框添加表项 my_ComboBoxAdd;
        public void ComboBoxAddItem(string a, ComboBox b) { b.Items.Add(a); }
        string qm = "";
        private void Form1_Load(object sender, EventArgs e)
        {
            my_TextBoxApText = new A_文本框添加内容(TextAppText);
            my_ComboBoxAdd = new A_组合框添加表项(ComboBoxAddItem);
            //获取IP
            ArrayList mac = new ArrayList();
            ArrayList ip = new ArrayList();
            GetLocalIPAddress(ref mac, ref ip);

            for (int i = 0; i < ip.Count; i++)
            {
                IPAddress l = (IPAddress)ip[i];
                DialogResult TS = MessageBox.Show("是否是这个IP在上网？(" + l.ToString() + ")", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (TS == DialogResult.Yes)
                {
                    string ip1 = l.ToString();
                    this.Text = string.Format("本机IP：{0}  MAC地址：{1}", ip1, mac[0]);
                    textBox1.AppendText(this.Text + "\r\n");
                    string[] fg = ip1.Split(".".ToCharArray());

                    qm = string.Format("{0}.{1}.{2}.", fg[0], fg[1], fg[2]);
                    break;
                }

            }
            if (qm == "")
            {
                this.Close();
            }
            /*
                this.Text = string.Format("本机IP：{0}  MAC地址：{1}", ip, mac);
            textBox1.AppendText(this.Text+"\r\n");
            string[] fg = ip.Split(".".ToCharArray());
            if (fg.Length != 4) { MessageBox.Show("获取的IP是错误的！"); }
            qm = string.Format("{0}.{1}.{2}.",fg[0],fg[1],fg[2]);
           */


            Thread t = new Thread(bl);

            t.IsBackground = true;

            t.Start();




        }


        string xz;
        void bl()
        {
            //遍历和发送信息
            for (int i = 1; i < 255; i++)
            {
                xz = qm + i.ToString();

                //以下是练习代码，用来练习启动多参数的线程
                /*
               //创建一个线程池
               TcpBegin TcpBeging = new TcpBegin(xz);
               Thread t = new Thread(new ThreadStart(TcpBeging.ConnectBegin));
               t.IsBackground = true;
               t.Start();
               Thread.Sleep(255 * 200);
               */

                //以下没注释的代码就是异步运行，多亏了Tcp类可以catch
                Thread thread = new Thread(new ParameterizedThreadStart(Sleep));

                thread.Start((object)xz);

                thread.Join(20);

                //以下是同步连接的做法，虽然类不同，但是思路就是一个一个等，出来的效果就是：1-255
                /*
                 if (Tcp.Start() != 0)
                 {

                     Tcp.SendMsg("This is CoKe_Deer!!!");
                     textBox1.Invoke(my_TextBoxApText, xz + ":ok", textBox1);
                 }
                 else {
                     textBox1.Invoke(my_TextBoxApText, xz + ":no", textBox1);
                 }
                 */
            }




        }

        /// <summary>
        /// 压缩
        /// </summary>
        /// <param name="inBytes"></param>
        /// <returns></returns>
        public static byte[] Compress(byte[] inBytes)
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
            outStream.Dispose();

            return outStream.GetBuffer();
        }
        /// <summary>
        /// 解压缩
        /// </summary>
        /// <param name="inStream"></param>
        /// <returns></returns>
        public static byte[] Decompress(byte[] inStream)
        {
            byte[] result = null;
            MemoryStream compressedStream = new MemoryStream(inStream);
            using (MemoryStream outStream = new MemoryStream())
            {
                using (GZipStream Decompress = new GZipStream(compressedStream,
                        CompressionMode.Decompress))
                {
                    Decompress.CopyTo(outStream);
                    result = outStream.ToArray();
                }
            }
            compressedStream.Dispose();
            return result;
        }

        public byte[] PhotoImageInsert(System.Drawing.Image imgPhoto)
        {
            //将Image转换成流数据，并保存为byte[] 
            MemoryStream mstream = new MemoryStream();
            imgPhoto.Save(mstream, System.Drawing.Imaging.ImageFormat.Bmp);
            byte[] byData = new Byte[mstream.Length];
            mstream.Position = 0;
            mstream.Read(byData, 0, byData.Length);
            mstream.Close();
            return byData;
        }
        //保存图片
        private void SaveImage(Image image)
        {

            string fileName = @"C:\Temp\1.jpg";
            string extension = Path.GetExtension(fileName);
            image.Save(fileName, ImageFormat.Jpeg);


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

        public void SendImgFiles(string FilesPath)
        {
            TcpClient Tcp = new TcpClient(comboBox1.Text, 12347);
            byte[] data = GetPictureData(FilesPath);
            NetworkStream NetStrm;
            NetStrm = Tcp.GetStream();
            NetStrm.Write(data, 0, data.Length);
            data = Encoding.UTF8.GetBytes("结束".ToCharArray());
            NetStrm.Write(data, 0, data.Length);
            NetStrm.Close();
            Tcp.Close();

            Tcp = null;

        }

        ArrayList Cg = new ArrayList();
        private void Sleep(object o)
        {

            string TcpIP = (string)o;
            TcpClient Tcp1 = new TcpClient();
            try
            {
                Tcp1 = new TcpClient(TcpIP, po);

                textBox1.Invoke(my_TextBoxApText, TcpIP + ":12346:ok", textBox1);
                Cg.Add(TcpIP);
                comboBox1.Invoke(my_ComboBoxAdd, TcpIP, comboBox1);


                NetworkStream NetStrm;
                NetStrm = Tcp1.GetStream();


                byte[] data = Encoding.ASCII.GetBytes(wb.ToCharArray());
                NetStrm.Write(data, 0, data.Length);
                NetStrm.Close();
                Tcp1.Close();

                Tcp1 = null;

            }
            catch
            {

                textBox1.Invoke(my_TextBoxApText, TcpIP + ":12346:no", textBox1);
                Tcp1.Close();
            }
        }

        public class TcpBegin
        {
            private string ip1;
            public TcpBegin(string ip2)
            {
                ip1 = ip2;
            }
            public void ConnectBegin()
            {
                string ip = ip1;
                Thread thread = new Thread(new ParameterizedThreadStart(Sleep));

                thread.Start((object)ip);


            }

            private void Sleep(object o)
            {

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            SendImgFiles(@textBox2.Text);
        }
        /*
         * 12346是打招呼的端口
         * 12347是发文件的端口
         * 12345
         * */
        private void button2_Click(object sender, EventArgs e)
        {
            TcpClient tcp = new TcpClient(comboBox1.Text, 12345);
            NetworkStream NetStrm;
            NetStrm = tcp.GetStream();

            string nr = "远程请求";

            byte[] data = Encoding.UTF8.GetBytes(nr.ToCharArray());

            NetStrm.Write(data, 0, data.Length);

            NetStrm.Close();
            tcp.Close();
            ycip = comboBox1.Text;
            Thread t = new Thread(new ThreadStart(SenBimap));
            t.IsBackground = true;
            t.Start();

            button2.Enabled = false;

        }
        public string ycip;
        void SenBimap()
        {
            int i = 0;
            int post = 18179;
            while (true)
            {

                int screenWidth = System.Windows.Forms.SystemInformation.VirtualScreen.Width;     //屏幕宽度  
                int screenHeight = System.Windows.Forms.SystemInformation.VirtualScreen.Height;     //屏幕高度  

                //创建图象，保存将来截取的图象
                Bitmap image = new Bitmap(screenWidth, screenHeight);
                Graphics imgGraphics = Graphics.FromImage(image);
                //设置截屏区域
                imgGraphics.CopyFromScreen(0, 0, 0, 0, new Size(screenWidth, screenHeight), CopyPixelOperation.SourceCopy);
                imgGraphics.Dispose();
                //保存
                //SaveImage(image);

                TcpClient TcpYclj = new TcpClient();
                if (i == 30)
                {

                    i++;
                    post++;
                    i = 0;

                }
                TcpYclj = new TcpClient(ycip, post);
                NetworkStream NetStrm;

                byte[] data = PhotoImageInsert(image);
                data = Compress(data);

                NetStrm = TcpYclj.GetStream();

                NetStrm.Write(data, 0, data.Length);

                data = Encoding.UTF8.GetBytes("完成".ToCharArray());
                NetStrm.Write(data, 0, data.Length);

                NetStrm.Close();
                TcpYclj.Close();
                i++;



                //MessageBox.Show("发送成功！");
            }

        }
    }
}
