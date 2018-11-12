using China_System.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Gongshang_PIC
{
    public partial class frmMain : Form
    {
        string inputFileName;
        // 后台执行控件
        private BackgroundWorker bgWorker;
        // 消息显示窗体
        private frmMessageShow frmMessageShow;
        // 后台操作是否正常完成
        private bool blnBackGroundWorkIsOK = false;
        //后加的后台属性显
        private bool backGroundRunResult;

        List<Read__Status> RResult;
        string savepath;
        public frmMain()
        {
            InitializeComponent();
            inputFileName = AppDomain.CurrentDomain.BaseDirectory + "System\\qiyeziliao.xlsx";
            this.pathTextBox.Text = inputFileName;
            this.textBox1.Text = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + DateTime.Now.ToString("yyyyMMddHHmmss");
            savepath = this.textBox1.Text;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //
            try
            {

                InitialBackGroundWorker();
                bgWorker.DoWork += new DoWorkEventHandler(Read_arir);

                bgWorker.RunWorkerAsync();

                // 启动消息显示画面
                frmMessageShow = new frmMessageShow(clsShowMessage.MSG_001,
                                                    clsShowMessage.MSG_007,
                                                    clsConstant.Dialog_Status_Disable);
                frmMessageShow.ShowDialog();

                // 数据读取成功后在画面显示
                if (blnBackGroundWorkIsOK)
                {
                    //this.dataGridView1.DataSource = null;
                    ////this.dataGridView1.AutoGenerateColumns = false;
                    //this.dataGridView1.DataSource = Report_Server;
                    //this.toolStripLabel1.Text = "Count : " + Report_Server.Count;

                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("错误" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return ;
                throw ex;
            }
            
        }

        private void openFileBtton_Click(object sender, EventArgs e)
        {

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Excel files (*.xls,*.xlsx)|*.xls;*.xlsx";
            ofd.FileName = "";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                inputFileName = ofd.FileName;
                this.pathTextBox.Text = inputFileName;

            }
            else
            {
                return;
            }


        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();

        }

        private void InitialBackGroundWorker()
        {
            bgWorker = new BackgroundWorker();
            bgWorker.WorkerReportsProgress = true;
            bgWorker.WorkerSupportsCancellation = true;
            bgWorker.RunWorkerCompleted +=
                new RunWorkerCompletedEventHandler(bgWorker_RunWorkerCompleted);
            bgWorker.ProgressChanged +=
                new ProgressChangedEventHandler(bgWorker_ProgressChanged);
        }

        private void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                blnBackGroundWorkIsOK = false;
            }
            else if (e.Cancelled)
            {
                blnBackGroundWorkIsOK = true;
            }
            else
            {
                blnBackGroundWorkIsOK = true;
            }
        }

        private void bgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (frmMessageShow != null && frmMessageShow.Visible == true)
            {
                //设置显示的消息
                frmMessageShow.setMessage(e.UserState.ToString());
                //设置显示的按钮文字
                if (e.ProgressPercentage == clsConstant.Thread_Progress_OK)
                {
                    frmMessageShow.setStatus(clsConstant.Dialog_Status_Enable);
                }
            }
        }

        private void Read_arir(object sender, DoWorkEventArgs e)
        {
            DateTime oldDate = DateTime.Now;

            //初始化信息
            frmWater BusinessHelp = new frmWater();

            RResult = new List<Read__Status>();

            RResult = ReadfindngFile(inputFileName);


            //  var form = new frmWater();
            BusinessHelp.bgWorker1 = bgWorker;
            BusinessHelp.markpic(RResult, savepath);
            //if (form.ShowDialog() == DialogResult.OK)
            //{
            //    // InitializeDataSource();
            //}

            //string savepath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + DateTime.Now.ToString("yyyyMMddHHmmss");


            DateTime FinishTime = DateTime.Now;
            TimeSpan s = DateTime.Now - oldDate;
            string timei = s.Minutes.ToString() + ":" + s.Seconds.ToString();
            string Showtime = clsShowMessage.MSG_029 + timei.ToString();
            bgWorker.ReportProgress(clsConstant.Thread_Progress_OK, clsShowMessage.MSG_009 + "\r\n" + Showtime);

        }

        public List<Read__Status> ReadfindngFile(string casetype)
        {

            List<Read__Status> Result = new List<Read__Status>();

            //string path = AppDomain.CurrentDomain.BaseDirectory + "Resources\\qiyeziliao.xlsx";
            System.Globalization.CultureInfo CurrentCI = System.Threading.Thread.CurrentThread.CurrentCulture;
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook analyWK = excelApp.Workbooks.Open(casetype, Type.Missing, true, Type.Missing,
                "htc", Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing);

            Microsoft.Office.Interop.Excel.Worksheet WS = (Microsoft.Office.Interop.Excel.Worksheet)analyWK.Worksheets[1];
            Microsoft.Office.Interop.Excel.Range rng;
            //rng = WS.get_Range(WS.Cells[2, 1], WS.Cells[WS.UsedRange.Rows.Count, 30]);
            rng = WS.Range[WS.Cells[2, 1], WS.Cells[WS.UsedRange.Rows.Count, 30]];
            int rowCount = WS.UsedRange.Rows.Count - 1;
            object[,] o = new object[1, 1];
            o = (object[,])rng.Value2;
            clsCommHelp.CloseExcel(excelApp, analyWK);

            for (int i = 2; i <= rowCount; i++)
            {
                bgWorker.ReportProgress(0, "正在导入   :  " + i.ToString() + "/" + rowCount.ToString());
                Read__Status temp = new Read__Status();

                #region 基础信息

                temp.gongsimingcheng = "";
                if (o[i, 1] != null)
                    temp.gongsimingcheng = o[i, 1].ToString().Trim();


                temp.zhucehao = "";
                if (o[i, 2] != null)
                    temp.zhucehao = o[i, 2].ToString().Trim();

                temp.gongsileixing = "";
                if (o[i, 3] != null)
                    temp.gongsileixing = o[i, 3].ToString().Trim();

                temp.faren = "";
                if (o[i, 4] != null)
                    temp.faren = o[i, 4].ToString().Trim();
                if (temp.faren == "" || temp.faren == null)
                    continue;

                temp.zhuceriqi = "";
                if (o[i, 5] != null)
                    temp.zhuceriqi = clsCommHelp.objToDateTime(o[i, 5]);// o[i, 5].ToString().Trim(); 
                temp.zhucezijin = "";
                if (o[i, 6] != null)
                    temp.zhucezijin = o[i, 6].ToString().Trim(); //clsCommHelp.objToDateTime(o[i, 6]);

                temp.zhucedizhi = "";
                if (o[i, 7] != null)
                    temp.zhucedizhi = o[i, 7].ToString().Trim();
                temp.jingyingfanwei = "";
                if (o[i, 8] != null)
                    temp.jingyingfanwei = o[i, 8].ToString().Trim();

                //temp.NEIRONGMIAOSHU = "";
                //if (o[i, 9] != null)
                //    temp.NEIRONGMIAOSHU = o[i, 9].ToString().Trim();

                //temp.KEHUMINGCHENG = "";
                //if (o[i, 10] != null)
                //    temp.KEHUMINGCHENG = o[i, 10].ToString().Trim();

                //temp.FUKUANJINE = "";
                //if (o[i, 11] != null)
                //    temp.FUKUANJINE = o[i, 11].ToString().Trim();
                //temp.Shared_Amount = "";
                //if (o[i, 12] != null)
                //    temp.Shared_Amount = o[i, 12].ToString().Trim();

                //temp.TIGONGZHILIAO = "";
                //if (o[i, 13] != null)
                //    temp.TIGONGZHILIAO = o[i, 13].ToString().Trim();

                //temp.FSSCSHENHEJIEGUO = "";
                //if (o[i, 14] != null)
                //    temp.FSSCSHENHEJIEGUO = o[i, 14].ToString().Trim();


                //temp.SHENHERIQI = "";
                //if (o[i, 15] != null)
                //    temp.SHENHERIQI = o[i, 15].ToString().Trim();

                //temp.FSSCCHULIYIJIAN = "";
                //if (o[i, 16] != null)
                //    temp.FSSCCHULIYIJIAN = o[i, 16].ToString().Trim();

                //temp.ISSUE = "";
                //if (o[i, 17] != null)
                //    temp.ISSUE = o[i, 17].ToString().Trim();

                //temp.STATUS = "";
                //if (o[i, 18] != null)
                //    temp.STATUS = o[i, 18].ToString().Trim();

                //temp.VRNUMBER = "";
                //if (o[i, 19] != null)
                //    temp.VRNUMBER = o[i, 19].ToString().Trim();

                //temp.SAPDOCUMENT = "";
                //if (o[i, 20] != null)
                //    temp.SAPDOCUMENT = o[i, 20].ToString().Trim();

                //temp.PAIDDATE = "";
                //if (o[i, 21] != null)
                //    temp.PAIDDATE = clsCommHelp.objToDateTime1(o[i, 21]).Replace("/", "");

                //temp.BUCHONGZHILIAO_BARCODE = "";
                //if (o[i, 22] != null)
                //    temp.BUCHONGZHILIAO_BARCODE = o[i, 22].ToString().Trim();

                //temp.MPR_FUKUANSHENQINGHAO = "";
                //if (o[i, 22] != null)
                //    temp.MPR_FUKUANSHENQINGHAO = o[i, 22].ToString().Trim();


                #endregion

                Result.Add(temp);
            }
            return Result;

        }

        private void button4_Click(object sender, EventArgs e)
        {
            var form = new frmWater();
            if (form.ShowDialog() == DialogResult.OK)
            {
                // InitializeDataSource();
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Excel files (*.xls,*.xlsx)|*.xls;*.xlsx";
            ofd.FileName = "";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                savepath = ofd.FileName;
                this.textBox1.Text = savepath;

            }
            else
            {
                return;
            }

        }

    }
}
