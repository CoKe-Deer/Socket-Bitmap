using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
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
    public partial class YcLj : Form
    {
        public YcLj()
        {
            InitializeComponent();
        }
        public delegate void A_SetPictureBoxImg(Bitmap a, PictureBox b);
        public A_SetPictureBoxImg my_PictureBoxImg;
        public void SetPictureBoxImgm(Bitmap a, PictureBox b) { b.Image = a; }

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
            try
            {
                result = null;
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
            }
            catch { }
            return result;
        }

        public string xz = Form1.xz;
        private void YcLj_Load(object sender, EventArgs e)
        {
            this.Text = "正在与：" + xz + "通信！";

            my_PictureBoxImg = new A_SetPictureBoxImg(SetPictureBoxImgm);


            /*
            int screenWidth = System.Windows.Forms.SystemInformation.VirtualScreen.Width;     //屏幕宽度  
            int screenHeight = System.Windows.Forms.SystemInformation.VirtualScreen.Height;     //屏幕高度  

            //创建图象，保存将来截取的图象
            Bitmap image = new Bitmap(screenWidth, screenHeight);
            Graphics imgGraphics = Graphics.FromImage(image);
            //设置截屏区域
            imgGraphics.CopyFromScreen(0, 0, 0, 0, new Size(screenWidth, screenHeight), CopyPixelOperation.SourceCopy);
            imgGraphics.Dispose();

            //Byte[] bbb = PhotoImageInsert(image);

            //Image y = BytesToImage(bbb);

            //this.pictureBox1.Image = y;
            

            this.pictureBox1.Image = image;
            */
            IPEndPoint hostEntry = new IPEndPoint(IPAddress.Any, 18179);
            YcljPort.Bind(hostEntry);
            //开始监听
            YcljPort.Listen(10);

            Thread tt = new Thread(new ThreadStart(Cgzs));
            tt.IsBackground = true;
            tt.Start();

        }
        Socket YcljPort = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
        //成功在这
        void Cgzs()
        {
            int i = 0;
            int post = 18179;
            while (true)
            {
                if (i == 30) {
                    YcljPort = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                    i = 0;
                    post++;
                    IPEndPoint hostEntry = new IPEndPoint(IPAddress.Any, post);
                    YcljPort.Bind(hostEntry);
                    //开始监听
                    YcljPort.Listen(10);
                }
                //存储数据的字节
                byte[] data;
                //收到信息的长度
                int length = 0;
                //接收到远程的连接
                Socket remoteClient = YcljPort.Accept();
                //重新初始化下接收数据的变量 防止连续接收时数据叠加
                Bitmap image;
                byte[] imgdata = new byte[0];

                StringBuilder receiveData = new StringBuilder();
                while (true)
                {


                    data = new byte[1048576];
                    length = remoteClient.Receive(data);

                    byte[] newA = data.Skip(0).Take(length).ToArray();
                    string wi = Encoding.UTF8.GetString(newA);
                    if (wi.IndexOf("完成") != -1)
                    {
                        wi = wi.Replace("完成", "");
                        //将文件尾写入
                        newA = Encoding.UTF8.GetBytes(wi);

                        imgdata = imgdata.Concat(newA).ToArray();

                        imgdata = Decompress(imgdata);

                        image = (Bitmap)BytesToImage(imgdata);

                        this.pictureBox1.Invoke(my_PictureBoxImg, image, this.pictureBox1);

                        i++;
                        break;
                    }
                    else
                    {
                        imgdata = imgdata.Concat(newA).ToArray();

                    }
                }

            }

        }
        /// <summary>
        /// 去掉byte[] 中特定的byte  
        /// </summary>
        /// <param name="b"> 需要处理的byte[]</param>
        /// <param name="cut">byte[] 中需要除去的特定 byte (此处: byte cut = 0x00 ;) </param>
        /// <returns> 返回处理完毕的byte[] </returns>
        public static byte[] byteCut(byte[] b, byte cut)
        {
            List<byte> list = new List<byte>();
            list.AddRange(b);
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i] == cut)
                    list.RemoveAt(i);
            }
            byte[] lastbyte = new byte[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                lastbyte[i] = list[i];
            }
            return lastbyte;
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
        public static Image BytesToImage(byte[] buffer)
        {

            Image image = null;
            try
            {
                MemoryStream ms = new MemoryStream(buffer);
                image = null;
                image = System.Drawing.Bitmap.FromStream(ms);
            }
            catch { }
            return image;
        }


        private void YcLj_SizeChanged(object sender, EventArgs e)
        {
            this.pictureBox1.Height = this.Height;
            this.pictureBox1.Width = this.Width;

        }
    }
}
