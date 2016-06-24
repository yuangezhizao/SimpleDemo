namespace AamirKhan
{
    partial class Accent
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
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.gpbalz = new System.Windows.Forms.GroupBox();
            this.gpbalz.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "label1";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(84, 20);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 21);
            this.textBox1.TabIndex = 1;
            // 
            // gpbalz
            // 
            this.gpbalz.Controls.Add(this.textBox1);
            this.gpbalz.Controls.Add(this.label1);
            this.gpbalz.Location = new System.Drawing.Point(2, 6);
            this.gpbalz.Name = "gpbalz";
            this.gpbalz.Size = new System.Drawing.Size(370, 113);
            this.gpbalz.TabIndex = 2;
            this.gpbalz.TabStop = false;
            this.gpbalz.Text = "爱乐赞";
            // 
            // Accent
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(375, 344);
            this.Controls.Add(this.gpbalz);
            this.Name = "Accent";
            this.Text = "Accent";
            this.gpbalz.ResumeLayout(false);
            this.gpbalz.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.GroupBox gpbalz;
    }
}