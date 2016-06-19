namespace SpiderFormsApp
{
    partial class webBrowserSystem
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
            this.tbnGo = new System.Windows.Forms.Button();
            this.txtUrl = new System.Windows.Forms.TextBox();
            this.wbrowser = new System.Windows.Forms.WebBrowser();
            this.lblcookie = new System.Windows.Forms.Label();
            this.txtCookies = new System.Windows.Forms.TextBox();
            this.lblurl = new System.Windows.Forms.Label();
            this.lblAgent = new System.Windows.Forms.Label();
            this.txtUserAgent = new System.Windows.Forms.TextBox();
            this.lblName = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.txtPwd = new System.Windows.Forms.TextBox();
            this.lblPwd = new System.Windows.Forms.Label();
            this.btnlogin = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tbnGo
            // 
            this.tbnGo.Location = new System.Drawing.Point(881, 38);
            this.tbnGo.Name = "tbnGo";
            this.tbnGo.Size = new System.Drawing.Size(75, 23);
            this.tbnGo.TabIndex = 0;
            this.tbnGo.Text = "确认";
            this.tbnGo.UseVisualStyleBackColor = true;
            this.tbnGo.Click += new System.EventHandler(this.tbnGo_Click);
            // 
            // txtUrl
            // 
            this.txtUrl.Location = new System.Drawing.Point(72, 40);
            this.txtUrl.Name = "txtUrl";
            this.txtUrl.Size = new System.Drawing.Size(791, 21);
            this.txtUrl.TabIndex = 1;
            // 
            // wbrowser
            // 
            this.wbrowser.Location = new System.Drawing.Point(1, 138);
            this.wbrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.wbrowser.Name = "wbrowser";
            this.wbrowser.Size = new System.Drawing.Size(994, 307);
            this.wbrowser.TabIndex = 2;
            this.wbrowser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.wbrowser_DocumentCompleted);
            // 
            // lblcookie
            // 
            this.lblcookie.AutoSize = true;
            this.lblcookie.Location = new System.Drawing.Point(19, 76);
            this.lblcookie.Name = "lblcookie";
            this.lblcookie.Size = new System.Drawing.Size(47, 12);
            this.lblcookie.TabIndex = 3;
            this.lblcookie.Text = "Cookies";
            // 
            // txtCookies
            // 
            this.txtCookies.Location = new System.Drawing.Point(72, 73);
            this.txtCookies.Name = "txtCookies";
            this.txtCookies.Size = new System.Drawing.Size(791, 21);
            this.txtCookies.TabIndex = 4;
            // 
            // lblurl
            // 
            this.lblurl.AutoSize = true;
            this.lblurl.Location = new System.Drawing.Point(43, 43);
            this.lblurl.Name = "lblurl";
            this.lblurl.Size = new System.Drawing.Size(23, 12);
            this.lblurl.TabIndex = 5;
            this.lblurl.Text = "Url";
            this.lblurl.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblAgent
            // 
            this.lblAgent.AutoSize = true;
            this.lblAgent.Location = new System.Drawing.Point(7, 111);
            this.lblAgent.Name = "lblAgent";
            this.lblAgent.Size = new System.Drawing.Size(59, 12);
            this.lblAgent.TabIndex = 6;
            this.lblAgent.Text = "UserAgent";
            // 
            // txtUserAgent
            // 
            this.txtUserAgent.Location = new System.Drawing.Point(72, 108);
            this.txtUserAgent.Name = "txtUserAgent";
            this.txtUserAgent.Size = new System.Drawing.Size(791, 21);
            this.txtUserAgent.TabIndex = 7;
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(70, 9);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(53, 12);
            this.lblName.TabIndex = 8;
            this.lblName.Text = "用户名：";
            this.lblName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(118, 6);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(78, 21);
            this.txtName.TabIndex = 9;
            this.txtName.Text = "lunce188";
            // 
            // txtPwd
            // 
            this.txtPwd.Location = new System.Drawing.Point(260, 6);
            this.txtPwd.Name = "txtPwd";
            this.txtPwd.PasswordChar = '*';
            this.txtPwd.Size = new System.Drawing.Size(78, 21);
            this.txtPwd.TabIndex = 11;
            this.txtPwd.Text = "mmb188";
            // 
            // lblPwd
            // 
            this.lblPwd.AutoSize = true;
            this.lblPwd.Location = new System.Drawing.Point(212, 9);
            this.lblPwd.Name = "lblPwd";
            this.lblPwd.Size = new System.Drawing.Size(41, 12);
            this.lblPwd.TabIndex = 10;
            this.lblPwd.Text = "密码：";
            this.lblPwd.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // btnlogin
            // 
            this.btnlogin.Location = new System.Drawing.Point(373, 4);
            this.btnlogin.Name = "btnlogin";
            this.btnlogin.Size = new System.Drawing.Size(75, 23);
            this.btnlogin.TabIndex = 12;
            this.btnlogin.Text = "登录";
            this.btnlogin.UseVisualStyleBackColor = true;
            this.btnlogin.Click += new System.EventHandler(this.btnlogin_Click);
            // 
            // webBrowserSystem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(995, 446);
            this.Controls.Add(this.btnlogin);
            this.Controls.Add(this.txtPwd);
            this.Controls.Add(this.lblPwd);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.txtUserAgent);
            this.Controls.Add(this.lblAgent);
            this.Controls.Add(this.lblurl);
            this.Controls.Add(this.txtCookies);
            this.Controls.Add(this.lblcookie);
            this.Controls.Add(this.wbrowser);
            this.Controls.Add(this.txtUrl);
            this.Controls.Add(this.tbnGo);
            this.Name = "webBrowserSystem";
            this.Text = "webBrowserSystem";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.webBrowserSystem_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button tbnGo;
        private System.Windows.Forms.TextBox txtUrl;
        private System.Windows.Forms.WebBrowser wbrowser;
        private System.Windows.Forms.Label lblcookie;
        private System.Windows.Forms.TextBox txtCookies;
        private System.Windows.Forms.Label lblurl;
        private System.Windows.Forms.Label lblAgent;
        private System.Windows.Forms.TextBox txtUserAgent;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.TextBox txtPwd;
        private System.Windows.Forms.Label lblPwd;
        private System.Windows.Forms.Button btnlogin;
    }
}