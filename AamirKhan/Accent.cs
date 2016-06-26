using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BLL;
using Mode;

namespace AamirKhan
{
    public partial class Accent : Form
    {
        public Accent()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            ApiConfig config = new ApiConfig
            {
                ApiSupplier = "Hellotrue",
                ApiUserName = txtalzName.Text.Trim(),
                ApiUserPwd = txtalzPwd.Text.Trim(),
                RequestUrl = "http://api.hellotrue.com/api/do.php",
                UpdateTime = DateTime.Now,
                TokenValidity = DateTime.Now,
                CreateDate = DateTime.Now
            };
            new ApiConfigBll().SaveApiConfig(config);
        }

        private void btnzlSave_Click(object sender, EventArgs e)
        {
            ApiConfig config = new ApiConfig
            {
                ApiSupplier = "Hellotrue",
                ApiUserName = txtalzName.Text.Trim(),
                ApiUserPwd = txtalzPwd.Text.Trim(),
                RequestUrl = "http://api.hellotrue.com/api/do.php",
                UpdateTime = DateTime.Now,
                TokenValidity = DateTime.Now,
                CreateDate = DateTime.Now
            };
            new ApiConfigBll().SaveApiConfig(config);
        }
    }
}
