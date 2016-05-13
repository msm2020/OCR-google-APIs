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

namespace Google_Orc
{
    public partial class Form1 : Form
    {
        public string ImagePath { get; set; }
        public Form1()
        {
            InitializeComponent();

            cmbLanguage.DataSource = GoogleLanguage.GetAll;
            cmbLanguage.ValueMember = "Code";
            cmbLanguage.DisplayMember = "Name";
            cmbLanguage.SelectedValue = "en";

        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            btnProcess.Enabled = false;
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "jpeg files|*.jpg;*.JPG";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    FileInfo fileInfo = new FileInfo(dlg.FileName);
                    if (fileInfo.Length > 1024 * 1024)
                    {
                        MessageBox.Show("jpeg file's size can not be larger than 1Mb");
                        return;
                    }
                    pxImage.Image = Image.FromFile(dlg.FileName);
                    ImagePath = dlg.FileName;
                    btnProcess.Enabled = true;
                }
            }
        }


        private string ResultJsonString = "";
        private string ResultTextString = "";

        private void rbJson_CheckedChanged(object sender, EventArgs e)
        {
            txtResult.Text = ResultJsonString.Replace("\\r", "").Replace("\\n", "");
        }

        private void rbText_CheckedChanged(object sender, EventArgs e)
        {
            txtResult.Text = ResultTextString;
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ImagePath))
                return;
            
                        
            txtResult.Text = "";
            GoogleAnnotate annotate = new GoogleAnnotate();
            annotate.GetText(ImagePath, cmbLanguage.SelectedValue+"");
            if (string.IsNullOrEmpty(annotate.Error) == false)
                MessageBox.Show("ERROR: " + annotate.Error);
            else
            {
                ResultJsonString = annotate.JsonResult;                 
                ResultTextString = txtResult.Text = annotate.TextResult;
                if (rbJson.Checked)
                    txtResult.Text = ResultJsonString.Replace("\\r", "").Replace("\\n", "");
            }
        }
    }
}
