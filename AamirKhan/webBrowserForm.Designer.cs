﻿namespace AamirKhan
{
    partial class webBrowserForm
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
            this.txtUrl = new System.Windows.Forms.TextBox();
            this.tbnGo = new System.Windows.Forms.Button();
            this.webBrowser = new System.Windows.Forms.WebBrowser();
            this.txtUserAgent = new System.Windows.Forms.TextBox();
            this.lblAgent = new System.Windows.Forms.Label();
            this.txtCookies = new System.Windows.Forms.TextBox();
            this.lblcookie = new System.Windows.Forms.Label();
            this.btnJDLogin = new System.Windows.Forms.Button();
            this.JdLoginTimer = new System.Windows.Forms.Timer(this.components);
            this.test = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtUrl
            // 
            this.txtUrl.Location = new System.Drawing.Point(9, 12);
            this.txtUrl.Name = "txtUrl";
            this.txtUrl.Size = new System.Drawing.Size(322, 21);
            this.txtUrl.TabIndex = 7;
            // 
            // tbnGo
            // 
            this.tbnGo.Location = new System.Drawing.Point(336, 10);
            this.tbnGo.Name = "tbnGo";
            this.tbnGo.Size = new System.Drawing.Size(45, 23);
            this.tbnGo.TabIndex = 6;
            this.tbnGo.Text = "确认";
            this.tbnGo.UseVisualStyleBackColor = true;
            this.tbnGo.Click += new System.EventHandler(this.tbnGo_Click);
            // 
            // webBrowser
            // 
            this.webBrowser.Location = new System.Drawing.Point(14, 41);
            this.webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser.Name = "webBrowser";
            this.webBrowser.Size = new System.Drawing.Size(523, 338);
            this.webBrowser.TabIndex = 9;
            this.webBrowser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser_DocumentCompleted);
            this.webBrowser.NewWindow += new System.ComponentModel.CancelEventHandler(this.webBrowser_NewWindow);
            // 
            // txtUserAgent
            // 
            this.txtUserAgent.Location = new System.Drawing.Point(72, 440);
            this.txtUserAgent.Name = "txtUserAgent";
            this.txtUserAgent.Size = new System.Drawing.Size(379, 21);
            this.txtUserAgent.TabIndex = 13;
            // 
            // lblAgent
            // 
            this.lblAgent.AutoSize = true;
            this.lblAgent.Location = new System.Drawing.Point(7, 440);
            this.lblAgent.Name = "lblAgent";
            this.lblAgent.Size = new System.Drawing.Size(59, 12);
            this.lblAgent.TabIndex = 12;
            this.lblAgent.Text = "UserAgent";
            // 
            // txtCookies
            // 
            this.txtCookies.Location = new System.Drawing.Point(72, 401);
            this.txtCookies.Name = "txtCookies";
            this.txtCookies.Size = new System.Drawing.Size(379, 21);
            this.txtCookies.TabIndex = 11;
            // 
            // lblcookie
            // 
            this.lblcookie.AutoSize = true;
            this.lblcookie.Location = new System.Drawing.Point(10, 404);
            this.lblcookie.Name = "lblcookie";
            this.lblcookie.Size = new System.Drawing.Size(47, 12);
            this.lblcookie.TabIndex = 10;
            this.lblcookie.Text = "Cookies";
            // 
            // btnJDLogin
            // 
            this.btnJDLogin.Location = new System.Drawing.Point(387, 10);
            this.btnJDLogin.Name = "btnJDLogin";
            this.btnJDLogin.Size = new System.Drawing.Size(64, 23);
            this.btnJDLogin.TabIndex = 14;
            this.btnJDLogin.Text = "京东登录";
            this.btnJDLogin.UseVisualStyleBackColor = true;
            this.btnJDLogin.Click += new System.EventHandler(this.btnJDLogin_Click);
            // 
            // JdLoginTimer
            // 
            this.JdLoginTimer.Tick += new System.EventHandler(this.JdLoginTimer_Tick);
            // 
            // test
            // 
            this.test.Location = new System.Drawing.Point(462, 399);
            this.test.Name = "test";
            this.test.Size = new System.Drawing.Size(75, 23);
            this.test.TabIndex = 15;
            this.test.Text = "测试";
            this.test.UseVisualStyleBackColor = true;
            this.test.Click += new System.EventHandler(this.test_Click);
            // 
            // webBrowserForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(549, 488);
            this.Controls.Add(this.test);
            this.Controls.Add(this.btnJDLogin);
            this.Controls.Add(this.txtUserAgent);
            this.Controls.Add(this.lblAgent);
            this.Controls.Add(this.txtCookies);
            this.Controls.Add(this.lblcookie);
            this.Controls.Add(this.webBrowser);
            this.Controls.Add(this.txtUrl);
            this.Controls.Add(this.tbnGo);
            this.Name = "webBrowserForm";
            this.Text = "webBrowserForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox txtUrl;
        private System.Windows.Forms.Button tbnGo;
        private System.Windows.Forms.WebBrowser webBrowser;
        private System.Windows.Forms.TextBox txtUserAgent;
        private System.Windows.Forms.Label lblAgent;
        private System.Windows.Forms.TextBox txtCookies;
        private System.Windows.Forms.Label lblcookie;
        private System.Windows.Forms.Button btnJDLogin;
        private System.Windows.Forms.Timer JdLoginTimer;
        private System.Windows.Forms.Button test;
    }
}