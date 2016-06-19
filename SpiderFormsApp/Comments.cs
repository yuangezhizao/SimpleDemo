using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SpiderFormsApp.code;

namespace SpiderFormsApp
{
    public partial class Comments : Form
    {
        [DllImport("kernel32.dll")]
        static extern bool AttachConsole(int dwProcessId);
        MainForm mainform;
        public Comments()
        {
            InitializeComponent();
           
        }
        public Comments(MainForm form)
        {
            InitializeComponent();

            mainform = form;
         
        }
        private void mainView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                try
                {
                    var mMbRpt = mainView.PointToClient(MousePosition);
                    ListViewItem lstrow = mainView.GetItemAt(mMbRpt.X, mMbRpt.Y);
                    Clipboard.SetDataObject(lstrow.SubItems[2].Text);
                }
                catch
                {

                }
            }
        }

        private void Comments_Load(object sender, EventArgs e)
        {
            MessageCenter.RegisterListViewMsgBoxControl(mainView);
            foreach (var item in Process.GetProcessesByName("cmd"))
            {
                if (item.MainWindowTitle.Contains("spider"))
                {
                    AttachConsole(item.Id);
                    break;
                }
            }
            Console.WriteLine(@"程序启动");
            mainView.GridLines = true;
            mainView.FullRowSelect = true;
            mainView.View = View.Details;
            mainView.Scrollable = true;
            mainView.MultiSelect = false; // 不可以多行选择 
            mainView.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            mainView.Columns.Add("序号", 30, HorizontalAlignment.Center);
            mainView.Columns.Add("商城", 60, HorizontalAlignment.Left);
            mainView.Columns.Add("地址", 300, HorizontalAlignment.Left);
            mainView.Columns.Add("消息", 120, HorizontalAlignment.Left);
            mainView.Columns.Add("日期", 60, HorizontalAlignment.Left);
            mainView.MouseDown += mainView_MouseDown;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = false;
            btnContinue.Enabled = false;
            btnStop.Enabled = true;
            int count;
            if(!int.TryParse(txtCount.Text, out count))
                count = 10;
            mainView.Items.Clear();
            for (int i = 0; i < count; i++)
            {
                ListViewItem li = new ListViewItem();
                li.SubItems[0].Text = (i + 1).ToString();
                li.SubItems.Add("");
                li.SubItems.Add("");
                li.SubItems.Add("准备更新");
                li.SubItems.Add("");
                mainView.Items.Add(li);
            }
          
            SpiderCommentsServer.ThreadCount = count;

            SpiderCommentsServer.SiteIds = "11";
            new SpiderCommentsServer().AppStart();
          
        }

        private void Comments_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            mainform.Enabled = true;
            mainform.Show();
           
        }
        private void btnContinue_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = false;
            btnContinue.Enabled = false;
            btnStop.Enabled = true;
            int count;
            if (!int.TryParse(txtCount.Text, out count))
                count = 10;
            mainView.Items.Clear();
            for (int i = 0; i < count; i++)
            {
                ListViewItem li = new ListViewItem();
                li.SubItems[0].Text = (i + 1).ToString();
                li.SubItems.Add("");
                li.SubItems.Add("");
                li.SubItems.Add("准备更新");
                li.SubItems.Add("");
                mainView.Items.Add(li);
            }
            SpiderCommentsServer.ThreadCount = count;
            SpiderCommentsServer.SiteIds = "1";
            SpiderCommentsServer.Continue = true;
            new SpiderCommentsServer().AppStart();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = true;
            btnContinue.Enabled = true;
            btnStop.Enabled = false;
            SpiderCommentsServer.AppStop();
        }
    }
}
