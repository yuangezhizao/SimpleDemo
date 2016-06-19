namespace SpiderFormsApp
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btnStart = new System.Windows.Forms.Button();
            this.menu = new System.Windows.Forms.MenuStrip();
            this.配置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.抓取方案配置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.评论抓取ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rtbError = new System.Windows.Forms.RichTextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rtbMsg = new System.Windows.Forms.RichTextBox();
            this.btnStop = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cbbCaseInfo = new System.Windows.Forms.ComboBox();
            this.btnHostProxy = new System.Windows.Forms.Button();
            this.HostTimer = new System.Windows.Forms.Timer(this.components);
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnApiSpider = new System.Windows.Forms.Button();
            this.browserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menu.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(527, 292);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 1;
            this.btnStart.Text = "开始";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // menu
            // 
            this.menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.配置ToolStripMenuItem,
            this.评论抓取ToolStripMenuItem});
            this.menu.Location = new System.Drawing.Point(0, 0);
            this.menu.Name = "menu";
            this.menu.Size = new System.Drawing.Size(1016, 25);
            this.menu.TabIndex = 2;
            this.menu.Text = "menuStrip1";
            // 
            // 配置ToolStripMenuItem
            // 
            this.配置ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.抓取方案配置ToolStripMenuItem,
            this.browserToolStripMenuItem});
            this.配置ToolStripMenuItem.Name = "配置ToolStripMenuItem";
            this.配置ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.配置ToolStripMenuItem.Text = "配置";
            // 
            // 抓取方案配置ToolStripMenuItem
            // 
            this.抓取方案配置ToolStripMenuItem.Name = "抓取方案配置ToolStripMenuItem";
            this.抓取方案配置ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.抓取方案配置ToolStripMenuItem.Text = "抓取方案配置";
            this.抓取方案配置ToolStripMenuItem.Click += new System.EventHandler(this.抓取方案配置ToolStripMenuItem_Click);
            // 
            // 评论抓取ToolStripMenuItem
            // 
            this.评论抓取ToolStripMenuItem.Name = "评论抓取ToolStripMenuItem";
            this.评论抓取ToolStripMenuItem.Size = new System.Drawing.Size(68, 21);
            this.评论抓取ToolStripMenuItem.Text = "评论抓取";
            this.评论抓取ToolStripMenuItem.Click += new System.EventHandler(this.评论抓取ToolStripMenuItem_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rtbError);
            this.groupBox1.Location = new System.Drawing.Point(12, 42);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(479, 231);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "错误日志";
            // 
            // rtbError
            // 
            this.rtbError.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbError.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rtbError.ForeColor = System.Drawing.Color.Red;
            this.rtbError.Location = new System.Drawing.Point(3, 17);
            this.rtbError.Name = "rtbError";
            this.rtbError.Size = new System.Drawing.Size(473, 211);
            this.rtbError.TabIndex = 1;
            this.rtbError.Text = "";
            this.rtbError.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.rtbError_LinkClicked);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rtbMsg);
            this.groupBox2.Location = new System.Drawing.Point(524, 43);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(480, 230);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "更新日志";
            // 
            // rtbMsg
            // 
            this.rtbMsg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbMsg.Location = new System.Drawing.Point(3, 17);
            this.rtbMsg.Name = "rtbMsg";
            this.rtbMsg.Size = new System.Drawing.Size(474, 210);
            this.rtbMsg.TabIndex = 1;
            this.rtbMsg.Text = "";
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(620, 292);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 10;
            this.btnStop.Text = "停止";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 301);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 12;
            this.label1.Text = "方案名称：";
            // 
            // cbbCaseInfo
            // 
            this.cbbCaseInfo.FormattingEnabled = true;
            this.cbbCaseInfo.Location = new System.Drawing.Point(87, 298);
            this.cbbCaseInfo.Name = "cbbCaseInfo";
            this.cbbCaseInfo.Size = new System.Drawing.Size(121, 20);
            this.cbbCaseInfo.TabIndex = 11;
            // 
            // btnHostProxy
            // 
            this.btnHostProxy.Location = new System.Drawing.Point(701, 292);
            this.btnHostProxy.Name = "btnHostProxy";
            this.btnHostProxy.Size = new System.Drawing.Size(75, 23);
            this.btnHostProxy.TabIndex = 13;
            this.btnHostProxy.Text = "获取代理";
            this.btnHostProxy.UseVisualStyleBackColor = true;
            this.btnHostProxy.Click += new System.EventHandler(this.btnHostProxy_Click);
            // 
            // HostTimer
            // 
            this.HostTimer.Tick += new System.EventHandler(this.HostTimer_Tick);
            // 
            // btnUpdate
            // 
            this.btnUpdate.Location = new System.Drawing.Point(782, 292);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(75, 23);
            this.btnUpdate.TabIndex = 14;
            this.btnUpdate.Text = "启动代理服务";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnApiSpider
            // 
            this.btnApiSpider.Location = new System.Drawing.Point(437, 292);
            this.btnApiSpider.Name = "btnApiSpider";
            this.btnApiSpider.Size = new System.Drawing.Size(75, 23);
            this.btnApiSpider.TabIndex = 15;
            this.btnApiSpider.TabStop = false;
            this.btnApiSpider.Text = "Api抓取";
            this.btnApiSpider.UseVisualStyleBackColor = true;
            this.btnApiSpider.Click += new System.EventHandler(this.btnApiSpider_Click);
            // 
            // browserToolStripMenuItem
            // 
            this.browserToolStripMenuItem.Name = "browserToolStripMenuItem";
            this.browserToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.browserToolStripMenuItem.Text = "浏览器";
            this.browserToolStripMenuItem.Click += new System.EventHandler(this.browserToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1016, 330);
            this.Controls.Add(this.btnApiSpider);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.btnHostProxy);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbbCaseInfo);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.menu);
            this.MainMenuStrip = this.menu;
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.menu.ResumeLayout(false);
            this.menu.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.MenuStrip menu;
        private System.Windows.Forms.ToolStripMenuItem 配置ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 抓取方案配置ToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RichTextBox rtbError;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RichTextBox rtbMsg;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbbCaseInfo;
        private System.Windows.Forms.Button btnHostProxy;
        private System.Windows.Forms.Timer HostTimer;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.ToolStripMenuItem 评论抓取ToolStripMenuItem;
        private System.Windows.Forms.Button btnApiSpider;
        private System.Windows.Forms.ToolStripMenuItem browserToolStripMenuItem;
    }
}