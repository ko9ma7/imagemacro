namespace 이미지매크로
{
    partial class 녹화폼
    {
        private System.ComponentModel.IContainer components = null;
        private Button button시작, button중지, button추가, button확인, button취소;
        private ListBox listBox액션;
        private Label label상태;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.button시작 = new Button();
            this.button중지 = new Button();
            this.button추가 = new Button();
            this.button확인 = new Button();
            this.button취소 = new Button();
            this.listBox액션 = new ListBox();
            this.label상태 = new Label();

            this.SuspendLayout();

            this.button시작.Text = "녹화시작";
            this.button시작.Location = new System.Drawing.Point(10, 10);
            this.button시작.Click += button시작_Click!;

            this.button중지.Text = "녹화중지";
            this.button중지.Location = new System.Drawing.Point(100, 10);
            this.button중지.Click += button중지_Click!;

            this.button추가.Text = "액션추가";
            this.button추가.Location = new System.Drawing.Point(190, 10);
            this.button추가.Click += button추가_Click!;

            this.listBox액션.Location = new System.Drawing.Point(10, 50);
            this.listBox액션.Width = 300;
            this.listBox액션.Height = 120;

            this.label상태.Text = "대기중";
            this.label상태.Location = new System.Drawing.Point(10, 180);

            this.button확인.Text = "확인";
            this.button확인.Location = new System.Drawing.Point(200, 180);
            this.button확인.Click += button확인_Click!;

            this.button취소.Text = "취소";
            this.button취소.Location = new System.Drawing.Point(260, 180);
            this.button취소.Click += button취소_Click!;

            this.ClientSize = new System.Drawing.Size(340, 220);
            this.Controls.Add(this.button시작);
            this.Controls.Add(this.button중지);
            this.Controls.Add(this.button추가);
            this.Controls.Add(this.listBox액션);
            this.Controls.Add(this.label상태);
            this.Controls.Add(this.button확인);
            this.Controls.Add(this.button취소);

            this.Text = "녹화 폼";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
