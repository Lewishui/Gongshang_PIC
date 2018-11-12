using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;

namespace Gongshang_PIC
{
    public partial class frmWater : Form
    {
        public Image ig;
        public string FPath;
        FontStyle Fstyle = FontStyle.Regular;
        float Fsize = 18;
        Color Fcolor = System.Drawing.Color.Yellow;
        FontFamily a = FontFamily.GenericMonospace;
        int Fwidth;
        int Fheight;
        private int box1_xlcX = 0;
        private int box1_xlcY = 0;
        public BackgroundWorker bgWorker1;

        public frmWater()
        {
            InitializeComponent();

            this.pictureBox1.MouseWheel += new MouseEventHandler(pictureBox1_MouseWheel);

        }

        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            NewMethod(e);




        }

        private void NewMethod(MouseEventArgs e)
        {
            int VX, VY;
            int x = e.Location.X;
            int y = e.Location.Y;
            int ow = pictureBox1.Width;
            int oh = pictureBox1.Height;
            VX = (int)((double)x * (ow - pictureBox1.Width) / ow);
            VY = (int)((double)y * (oh - pictureBox1.Height) / oh);
            box1_xlcX = pictureBox1.Location.X + VX;
            box1_xlcY = pictureBox1.Location.Y + VY;
        }
        public void makeWatermark(int x, int y, string txt)
        {
            System.Drawing.Image image = Image.FromFile(FPath);
            System.Drawing.Graphics e = System.Drawing.Graphics.FromImage(image);
            System.Drawing.Font f = new System.Drawing.Font(a, Fsize, Fstyle);
            System.Drawing.Brush b = new System.Drawing.SolidBrush(Fcolor);
            e.DrawString(txt, f, b, x, y);
            SizeF XMaxSize = e.MeasureString(txt, f);

            Fwidth = (int)XMaxSize.Width;
            Fheight = (int)XMaxSize.Height;

            e.Dispose();
            pictureBox1.Image = image;
        }

        private void frmWater_Load(object sender, EventArgs e)
        {
            string fullname = AppDomain.CurrentDomain.BaseDirectory + "System\\tel.jpg";

            //string fullname = @"D:\用户目录\我的图片\123.jpg";
            pictureBox1.Image = Image.FromFile(fullname);
            ig = pictureBox1.Image;
            FPath = fullname;
            //pictureBox1.Image = ig;
        }

        private void button3_Click(object sender, EventArgs e)
        {

            fontDialog1.ShowColor = true;
            fontDialog1.ShowHelp = false;
            fontDialog1.ShowApply = false;
            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                Fstyle = fontDialog1.Font.Style;
                Fcolor = fontDialog1.Color;
                Fsize = fontDialog1.Font.Size;

                a = fontDialog1.Font.FontFamily;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            {
                pictureBox1.Image = ig;
                if (txtChar.Text.Trim() != "")
                {
                    if (radioButton1.Checked)
                    {
                        int x = 10, y = 10;
                        makeWatermark(x, y, txtChar.Text.Trim());
                    }
                    if (radioButton2.Checked)
                    {
                        int x1 = 10, y1 = ig.Height - Fheight;
                        makeWatermark(x1, y1, txtChar.Text.Trim());
                    }
                    if (radioButton3.Checked)
                    {
                        int x2 = (int)(ig.Width - Fwidth) / 2;
                        int y2 = (int)(ig.Height - Fheight) / 2;
                        makeWatermark(x2, y2, txtChar.Text.Trim());
                    }
                    if (radioButton4.Checked)
                    {
                        int x3 = ig.Width - Fwidth;
                        int y3 = 10;
                        makeWatermark(x3, y3, txtChar.Text.Trim());
                    }
                    if (radioButton5.Checked)
                    {
                        int x4 = ig.Width - Fwidth;
                        int y4 = ig.Height - Fheight;
                        makeWatermark(x4, y4, txtChar.Text.Trim());
                    }
                }
            }

        }
        private static void CreateFolder(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
        public bool markpic(List<Read__Status> Result, string savepath)
        {
            string fullname = AppDomain.CurrentDomain.BaseDirectory + "System\\tel.jpg";

            int oi = 0;
            if (savepath != null && savepath != "")
                CreateFolder(savepath);
            else
            {

                //  return false;

            }
            foreach (Read__Status item in Result)
            {
                pictureBox1.Image = Image.FromFile(fullname);
                ig = pictureBox1.Image;
                FPath = fullname;


                bgWorker1.ReportProgress(0, item.gongsimingcheng + "  \r\n\r\n"+oi.ToString() + "/" + Result.Count.ToString());
                string savefile = savepath + "\\" + item.gongsimingcheng + "-" + item.zhucehao + ".jpg";
                bool isave = false;

                float fontSize = 11.0f;
                //名称
                float rectX = 280;
                float rectY = 373;


                addtxtpic(item.gongsimingcheng, savefile, isave, fontSize, rectX, rectY);
                //类型
                rectX = 280;
                rectY = 419;
                addtxtpic(item.gongsileixing, savefile, isave, fontSize, rectX, rectY);

                //经营场所
                rectX = 280;
                rectY = 459;
                addtxtpic(item.zhucedizhi, savefile, isave, fontSize, rectX, rectY);

                //经营者
                rectX = 280;
                rectY = 500;
                addtxtpic(item.faren, savefile, isave, fontSize, rectX, rectY);

                //注册资金
                rectX = 280;
                rectY = 540;
                addtxtpic(item.zhucezijin, savefile, isave, fontSize, rectX, rectY);


                //注册日期
                rectX = 280;
                rectY = 582;
                string ad = Convert.ToDateTime(item.zhuceriqi).ToString("yyyy年MM月dd日");
                // 
                addtxtpic(ad, savefile, isave, fontSize, rectX, rectY);

                //营业期限
                rectX = 280;
                rectY = 622;
                addtxtpic("长期", savefile, isave, fontSize, rectX, rectY);

                //经营范围

                rectX = 280;
                rectY = 670;
                List<string> jing = new List<string>();
                int il = item.jingyingfanwei.Length;
                char[] ch = item.jingyingfanwei.ToCharArray();
                int ongo = 0;
                string tx = "";
                for (int i = 0; i < ch.Length; i++)
                {

                    if (ongo < 40)
                    {
                        tx += ch[i].ToString();
                        ongo++;
                    }
                    else
                    {
                        jing.Add(tx);
                        tx = ch[i].ToString();
                        ongo = 0;
                    }


                }
                jing.Add(tx);
                tx = "";

                for (int i = 0; i < jing.Count; i++)
                {
                    tx += jing[i] + "\r\n";
                }
                fontSize = 7.0f;
                addtxtpic(tx, savefile, isave, fontSize, rectX, rectY);


                fontSize = 16.0f;
                //登记机关 日期
            
                rectX = 450;
                rectY = 940;
                addtxtpic(ad, savefile, isave, fontSize, rectX, rectY);

                //注册号                
                rectX = 475;
                rectY = 325;
                isave = true;
                fontSize = 12.0f;
                oi++;
                addtxtpic(item.zhucehao, savefile, isave, fontSize, rectX, rectY);
                
            }
            return true;




        }

        private void addtxtpic(string item, string savefile, bool isave, float fontSize, float rectX, float rectY)
        {
            AddTextToImg(item, rectX, rectY, fontSize, isave, savefile);
        }
        private void button2_Click(object sender, EventArgs e)
        {

            #region MyRegion
            ////名称
            //float rectX = 280;
            //float rectY = 305;

            ////类型
            //rectX = 280;
            //rectY = 353;
            ////经营场所
            //rectX = 280;
            //rectY = 403;
            ////经营者
            //rectX = 280;
            //rectY = 450;
            ////组成形式
            //rectX = 280;
            //rectY = 500;
            ////注册日期
            //rectX = 280;
            //rectY = 550;
            ////经营范围

            //rectX = 280;
            //rectY = 600;

            ////登记机关 日期

            //rectX = 450;
            //rectY = 795;

            ////注册号
            //rectX = 460;
            //rectY = 245;


            #endregion
            //名称
            float rectX = 280;
            float rectY = 375;

            //类型
            rectX = 280;
            rectY = 417;
            //经营场所
            rectX = 280;
            rectY = 457;
            //经营者
            rectX = 280;
            rectY = 497;
            //注册资金
            rectX = 280;
            rectY = 537;
            //注册日期
            rectX = 280;
            rectY = 577;
            //营业期限

            rectX = 280;
            rectY = 617;
            //经营范围

            rectX = 280;
            rectY = 660;
            //登记机关 日期

            rectX = 450;
            rectY = 930;

            //注册号
            rectX = 475;
            rectY = 320;

            float fontSize = 18.0f;


            AddTextToImg("nihao ", rectX, rectY, fontSize, false, "");
            //makeWatermark(box1_xlcX, box1_xlcY, "123456789");

            return;

            saveFileDialog1.Filter = "BMP|*.bmp|JPEG|*.jpeg|GIF|*.gif|PNG|*.png";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string picPath = saveFileDialog1.FileName;
                string picType = picPath.Substring(picPath.LastIndexOf(".") + 1, (picPath.Length - picPath.LastIndexOf(".") - 1));
                switch (picType)
                {
                    case "bmp":
                        Bitmap bt = new Bitmap(pictureBox1.Image);
                        Bitmap mybmp = new Bitmap(bt, ig.Width, ig.Height);
                        mybmp.Save(picPath, ImageFormat.Bmp); break;
                    case "jpeg":
                        Bitmap bt1 = new Bitmap(pictureBox1.Image);
                        Bitmap mybmp1 = new Bitmap(bt1, ig.Width, ig.Height);
                        mybmp1.Save(picPath, ImageFormat.Jpeg); break;
                    case "gif":
                        Bitmap bt2 = new Bitmap(pictureBox1.Image);
                        Bitmap mybmp2 = new Bitmap(bt2, ig.Width, ig.Height);
                        mybmp2.Save(picPath, ImageFormat.Gif); break;
                    case "png":
                        Bitmap bt3 = new Bitmap(pictureBox1.Image);
                        Bitmap mybmp3 = new Bitmap(bt3, ig.Width, ig.Height);
                        mybmp3.Save(picPath, ImageFormat.Png); break;
                }
            }
        }


        private void AddTextToImg(string text, float rectX, float rectY, float fontSize, bool islast, string path)
        {
            //判断指定图片是否存在
            if (!File.Exists(FPath))
            {
                throw new FileNotFoundException("The file don't exist!");
            }
            if (text == string.Empty)
            {
                return;
            }
            // Image image = Image.FromFile(FPath);
            Image image = ig;

            Bitmap bitmap = new Bitmap(image, image.Width, image.Height);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);
            //字体大小
            //float fontSize = 18.0f;
            //文本的长度
            float textWidth = text.Length * fontSize;
            //下面定义一个矩形区域，以后在这个矩形里画上白底黑字
            //float rectX = 120;
            //float rectY = 200;
            //float rectX = 280;
            //float rectY = 305;
            float rectWidth = text.Length * (fontSize + 40);
            float rectHeight = fontSize + 40;
            //声明矩形域
            RectangleF textArea = new RectangleF(rectX, rectY, rectWidth, rectHeight);
            //定义字体
            System.Drawing.Font font = new System.Drawing.Font("黑体", fontSize, System.Drawing.FontStyle.Regular);
            //font.Bold = true;
            //白笔刷，画文字用
            Brush whiteBrush = new SolidBrush(System.Drawing.Color.Black);
            //黑笔刷，画背景用
            //Brush blackBrush = new SolidBrush(Color.Black);   
            //g.FillRectangle(blackBrush, rectX, rectY, rectWidth, rectHeight);
            g.DrawString(text, font, whiteBrush, textArea);
            //输出方法一：将文件生成并保存到C盘
            // = @Application.streamingAssetsPath + "/test2.jpg";
            //string path = AppDomain.CurrentDomain.BaseDirectory + "tel1.jpg";
            pictureBox1.Image = bitmap;
            ig = bitmap;
            if (islast == true)
            {
                bitmap.Save(path, System.Drawing.Imaging.ImageFormat.Jpeg);
                g.Dispose();
                bitmap.Dispose();
                image.Dispose();
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {


        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            NewMethod(e);

        }

    }
}
