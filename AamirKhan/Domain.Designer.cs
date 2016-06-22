namespace AamirKhan
{
    partial class Domain
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
            this.btnStart = new System.Windows.Forms.Button();
            this.lblTheadCount = new System.Windows.Forms.Label();
            this.txtTheadCount = new System.Windows.Forms.TextBox();
            this.txTotalTask = new System.Windows.Forms.TextBox();
            this.lblTotal = new System.Windows.Forms.Label();
            this.btnStop = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.lvTheadDetial = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(314, 27);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "运行";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // lblTheadCount
            // 
            this.lblTheadCount.AutoSize = true;
            this.lblTheadCount.Location = new System.Drawing.Point(12, 32);
            this.lblTheadCount.Name = "lblTheadCount";
            this.lblTheadCount.Size = new System.Drawing.Size(53, 12);
            this.lblTheadCount.TabIndex = 1;
            this.lblTheadCount.Text = "线程数：";
            // 
            // txtTheadCount
            // 
            this.txtTheadCount.Location = new System.Drawing.Point(67, 29);
            this.txtTheadCount.Name = "txtTheadCount";
            this.txtTheadCount.Size = new System.Drawing.Size(55, 21);
            this.txtTheadCount.TabIndex = 2;
            this.txtTheadCount.Text = "5";
            // 
            // txTotalTask
            // 
            this.txTotalTask.Location = new System.Drawing.Point(204, 29);
            this.txTotalTask.Name = "txTotalTask";
            this.txTotalTask.Size = new System.Drawing.Size(55, 21);
            this.txTotalTask.TabIndex = 4;
            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = true;
            this.lblTotal.Location = new System.Drawing.Point(133, 32);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(65, 12);
            this.lblTotal.TabIndex = 3;
            this.lblTotal.Text = "总执行数：";
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(411, 27);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 5;
            this.btnStop.Text = "停止";
            this.btnStop.UseVisualStyleBackColor = true;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(518, 24);
            this.menuStrip1.TabIndex = 6;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(0, 533);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(518, 23);
            this.progressBar.TabIndex = 7;
            // 
            // lvTheadDetial
            // 
            this.lvTheadDetial.Location = new System.Drawing.Point(0, 74);
            this.lvTheadDetial.Name = "lvTheadDetial";
            this.lvTheadDetial.Size = new System.Drawing.Size(518, 245);
            this.lvTheadDetial.TabIndex = 8;
            this.lvTheadDetial.UseCompatibleStateImageBehavior = false;
            // 
            // Domain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(518, 556);
            this.Controls.Add(this.lvTheadDetial);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.txTotalTask);
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.txtTheadCount);
            this.Controls.Add(this.lblTheadCount);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Domain";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Domain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Label lblTheadCount;
        private System.Windows.Forms.TextBox txtTheadCount;
        private System.Windows.Forms.TextBox txTotalTask;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.ListView lvTheadDetial;
    }
}

