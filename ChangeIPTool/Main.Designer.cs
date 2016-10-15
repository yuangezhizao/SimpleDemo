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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
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
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(281, 266);
            this.Controls.Add(this.groupBox1);
            this.Name = "Main";
            this.Text = "IpChange";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
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
        private System.Windows.Forms.GroupBox groupBox1;
    }
}

