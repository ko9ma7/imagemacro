using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace 이미지매크로
{
    public partial class 녹화폼 : Form
    {
        public List<string> 녹화리스트 = new List<string>();

        public 녹화폼()
        {
            InitializeComponent();
        }

        private void button시작_Click(object sender, EventArgs e)
        {
            label상태.Text = "녹화중...";
            listBox액션.Items.Clear();
            녹화리스트.Clear();
        }

        private void button중지_Click(object sender, EventArgs e)
        {
            label상태.Text = "중지됨";
        }

        private void button추가_Click(object sender, EventArgs e)
        {
            var s = Microsoft.VisualBasic.Interaction.InputBox("녹화 액션", "녹화추가", "");
            if (!string.IsNullOrEmpty(s))
            {
                listBox액션.Items.Add(s);
                녹화리스트.Add(s);
            }
        }

        private void button확인_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button취소_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
