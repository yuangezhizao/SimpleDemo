namespace ChangeIPTool
{
    partial class Main
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lbllinkName = new System.Windows.Forms.Label();
            this.txtLinkName = new System.Windows.Forms.TextBox();
            this.lblUserName = new System.Windows.Forms.Label();
            this.lblpwd = new System.Windows.Forms.Label();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.txtpwd = new System.Windows.Forms.TextBox();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.timerInfo = new System.Windows.Forms.Timer(this.components);
            this.txtTimeSpan = new System.Windows.Forms.TextBox();
            this.btmTime = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.timerIpChang = new System.Windows.Forms.Timer(this.components);
            this.btnlySubmit = new System.Windows.Forms.Button();
            this.txtlyPwd = new System.Windows.Forms.TextBox();
            this.txtlyName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtLyUrl = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtlyTimeSpan = new System.Windows.Forms.TextBox();
            this.btmlyTime = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbllinkName
            // 
            this.lbllinkName.AutoSize = true;
            this.lbllinkName.Location = new System.Drawing.Point(16, 34);
            this.lbllinkName.Name = "lbllinkName";
            this.lbllinkName.Size = new System.Drawing.Size(53, 12);
            this.lbllinkName.TabIndex = 0;
            this.lbllinkName.Text = "网络名称";
            // 
            // txtLinkName
            // 
            this.txtLinkName.Location = new System.Drawing.Point(84, 31);
            this.txtLinkName.Name = "txtLinkName";
            this.txtLinkName.Size = new System.Drawing.Size(130, 21);
            this.txtLinkName.TabIndex = 1;
            this.txtLinkName.Text = "宽带连接";
            // 
            // lblUserName
            // 
            this.lblUserName.AutoSize = true;
            this.lblUserName.Location = new System.Drawing.Point(16, 81);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(53, 12);
            this.lblUserName.TabIndex = 2;
            this.lblUserName.Text = "用户名称";
            // 
            // lblpwd
            // 
            this.lblpwd.AutoSize = true;
            this.lblpwd.Location = new System.Drawing.Point(40, 125);
            this.lblpwd.Name = "lblpwd";
            this.lblpwd.Size = new System.Drawing.Size(29, 12);
            this.lblpwd.TabIndex = 3;
            this.lblpwd.Text = "密码";
            // 
            // txtUserName
            // 
            this.txtUserName.Location = new System.Drawing.Point(84, 81);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(130, 21);
            this.txtUserName.TabIndex = 4;
            this.txtUserName.Text = "057475426335";
            // 
            // txtpwd
            // 
            this.txtpwd.Location = new System.Drawing.Point(84, 125);
            this.txtpwd.Name = "txtpwd";
            this.txtpwd.PasswordChar = '*';
            this.txtpwd.Size = new System.Drawing.Size(130, 21);
            this.txtpwd.TabIndex = 5;
            this.txtpwd.Text = "718285";
            // 
            // btnSubmit
            // 
            this.btnSubmit.Location = new System.Drawing.Point(84, 164);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(75, 23);
            this.btnSubmit.TabIndex = 6;
            this.btnSubmit.Text = "重新连接";
            this.btnSubmit.UseVisualStyleBackColor = true;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // timerInfo
            // 
            this.timerInfo.Tick += new System.EventHandler(this.timerInfo_Tick);
            // 
            // txtTimeSpan
            // 
            this.txtTimeSpan.Location = new System.Drawing.Point(82, 203);
            this.txtTimeSpan.Name = "txtTimeSpan";
            this.txtTimeSpan.Size = new System.Drawing.Size(64, 21);
            this.txtTimeSpan.TabIndex = 7;
            this.txtTimeSpan.Text = "10";
            // 
            // btmTime
            // 
            this.btmTime.Location = new System.Drawing.Point(162, 204);
            this.btmTime.Name = "btmTime";
            this.btmTime.Size = new System.Drawing.Size(75, 23);
            this.btmTime.TabIndex = 8;
            this.btmTime.Text = "定时重拨";
            this.btmTime.UseVisualStyleBackColor = true;
            this.btmTime.Click += new System.EventHandler(this.btmTime_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 209);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 9;
            this.label1.Text = "时间分钟";
            // 
            // timerIpChang
            // 
            this.timerIpChang.Tick += new System.EventHandler(this.timerIpChang_Tick);
            // 
            // btnlySubmit
            // 
            this.btnlySubmit.Location = new System.Drawing.Point(87, 168);
            this.btnlySubmit.Name = "btnlySubmit";
            this.btnlySubmit.Size = new System.Drawing.Size(75, 23);
            this.btnlySubmit.TabIndex = 16;
            this.btnlySubmit.Text = "重新连接";
            this.btnlySubmit.UseVisualStyleBackColor = true;
            this.btnlySubmit.Click += new System.EventHandler(this.btnlySubmit_Click);
            // 
            // txtlyPwd
            // 
            this.txtlyPwd.Location = new System.Drawing.Point(87, 129);
            this.txtlyPwd.Name = "txtlyPwd";
            this.txtlyPwd.PasswordChar = '*';
            this.txtlyPwd.Size = new System.Drawing.Size(130, 21);
            this.txtlyPwd.TabIndex = 15;
            this.txtlyPwd.Text = "woshihaha";
            // 
            // txtlyName
            // 
            this.txtlyName.Location = new System.Drawing.Point(87, 85);
            this.txtlyName.Name = "txtlyName";
            this.txtlyName.Size = new System.Drawing.Size(130, 21);
            this.txtlyName.TabIndex = 14;
            this.txtlyName.Text = "admin";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(43, 129);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 13;
            this.label2.Text = "密码";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 85);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 12;
            this.label3.Text = "用户名称";
            // 
            // txtLyUrl
            // 
            this.txtLyUrl.Location = new System.Drawing.Point(87, 35);
            this.txtLyUrl.Name = "txtLyUrl";
            this.txtLyUrl.Size = new System.Drawing.Size(130, 21);
            this.txtLyUrl.TabIndex = 11;
            this.txtLyUrl.Text = "192.168.6.1";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(19, 38);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 10;
            this.label4.Text = "路由器地址";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblUserName);
            this.groupBox1.Controls.Add(this.lbllinkName);
            this.groupBox1.Controls.Add(this.txtLinkName);
            this.groupBox1.Controls.Add(this.lblpwd);
            this.groupBox1.Controls.Add(this.txtUserName);
            this.groupBox1.Controls.Add(this.txtpwd);
            this.groupBox1.Controls.Add(this.btnSubmit);
            this.groupBox1.Controls.Add(this.txtTimeSpan);
            this.groupBox1.Controls.Add(this.btmTime);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(258, 247);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "宽带拨号";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtlyTimeSpan);
            this.groupBox2.Controls.Add(this.btmlyTime);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.txtLyUrl);
            this.groupBox2.Controls.Add(this.btnlySubmit);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.txtlyPwd);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.txtlyName);
            this.groupBox2.Location = new System.Drawing.Point(287, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(263, 247);
            this.groupBox2.TabIndex = 18;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "路由器拨号";
            // 
            // txtlyTimeSpan
            // 
            this.txtlyTimeSpan.Location = new System.Drawing.Point(87, 200);
            this.txtlyTimeSpan.Name = "txtlyTimeSpan";
            this.txtlyTimeSpan.Size = new System.Drawing.Size(64, 21);
            this.txtlyTimeSpan.TabIndex = 17;
            this.txtlyTimeSpan.Text = "10";
            // 
            // btmlyTime
            // 
            this.btmlyTime.Location = new System.Drawing.Point(166, 198);
            this.btmlyTime.Name = "btmlyTime";
            this.btmlyTime.Size = new System.Drawing.Size(75, 23);
            this.btmlyTime.TabIndex = 18;
            this.btmlyTime.Text = "定时重拨";
            this.btmlyTime.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(19, 206);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 19;
            this.label5.Text = "时间分钟";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(575, 275);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Main";
            this.Text = "IpChange";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lbllinkName;
        private System.Windows.Forms.TextBox txtLinkName;
        private System.Windows.Forms.Label lblUserName;
        private System.Windows.Forms.Label lblpwd;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.TextBox txtpwd;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.Timer timerInfo;
        private System.Windows.Forms.TextBox txtTimeSpan;
        private System.Windows.Forms.Button btmTime;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Timer timerIpChang;
        private System.Windows.Forms.Button btnlySubmit;
        private System.Windows.Forms.TextBox txtlyPwd;
        private System.Windows.Forms.TextBox txtlyName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtLyUrl;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtlyTimeSpan;
        private System.Windows.Forms.Button btmlyTime;
        private System.Windows.Forms.Label label5;
    }
}

