namespace 이미지매크로
{
    partial class 메인폼
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.ToolStrip 툴스트립;
        private System.Windows.Forms.ToolStripButton 버튼창선택, 버튼시작, 버튼중지;
        private System.Windows.Forms.ToolStripButton 버튼활성, 버튼비활성;
        private System.Windows.Forms.ToolStripDropDownButton 드롭다운캡처;
        private System.Windows.Forms.ToolStripMenuItem 메뉴한번, 메뉴매번;
        private System.Windows.Forms.ToolStripLabel 라벨딜레이;
        private System.Windows.Forms.ToolStripTextBox 텍스트딜레이;

        // 상단 설정 패널 (Threshold, MatchColor)
        private System.Windows.Forms.Panel panelSettings;
        private System.Windows.Forms.Label labelThreshold;
        private System.Windows.Forms.NumericUpDown numericUpDownThreshold;
        private System.Windows.Forms.Label labelMatchColor;
        private System.Windows.Forms.ComboBox comboBoxMatchColor;

        private System.Windows.Forms.SplitContainer 분할주;
        // 왼쪽 패널: TreeView (MultiSelectTreeView 사용)
        private MultiSelectTreeView 트리항목;
        // 오른쪽 패널: 그림미리보기 및 로그
        private System.Windows.Forms.PictureBox 그림미리보기;
        private System.Windows.Forms.TextBox 텍스트로그;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.툴스트립 = new System.Windows.Forms.ToolStrip();
            this.버튼창선택 = new System.Windows.Forms.ToolStripButton("대상창 선택");
            this.버튼시작 = new System.Windows.Forms.ToolStripButton("시작");
            this.버튼중지 = new System.Windows.Forms.ToolStripButton("중지");
            this.버튼활성 = new System.Windows.Forms.ToolStripButton("활성") { Checked = false };
            this.버튼비활성 = new System.Windows.Forms.ToolStripButton("비활성") { Checked = true };
            this.드롭다운캡처 = new System.Windows.Forms.ToolStripDropDownButton("캡처모드");
            this.메뉴한번 = new System.Windows.Forms.ToolStripMenuItem("한번") { Checked = true };
            this.메뉴매번 = new System.Windows.Forms.ToolStripMenuItem("매번");
            this.드롭다운캡처.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { 메뉴한번, 메뉴매번 });
            this.라벨딜레이 = new System.Windows.Forms.ToolStripLabel("딜레이(ms):");
            this.텍스트딜레이 = new System.Windows.Forms.ToolStripTextBox() { Text = "1000", Width = 50 };

            this.툴스트립.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                버튼창선택, 버튼시작, 버튼중지,
                new System.Windows.Forms.ToolStripSeparator(),
                버튼활성, 버튼비활성,
                new System.Windows.Forms.ToolStripSeparator(),
                드롭다운캡처,
                new System.Windows.Forms.ToolStripSeparator(),
                라벨딜레이, 텍스트딜레이
            });
            this.툴스트립.Location = new System.Drawing.Point(0, 0);
            this.툴스트립.Name = "툴스트립";
            this.툴스트립.Size = new System.Drawing.Size(1200, 25);
            this.툴스트립.TabIndex = 0;
            this.툴스트립.Text = "툴스트립";

            // panelSettings: 상단 설정 패널 (Threshold, MatchColor)
            this.panelSettings = new System.Windows.Forms.Panel();
            this.panelSettings.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelSettings.Height = 40;
            this.panelSettings.BackColor = System.Drawing.Color.LightGray;
            this.labelThreshold = new System.Windows.Forms.Label() { Text = "Threshold:", Left = 10, Top = 10, AutoSize = true };
            this.numericUpDownThreshold = new System.Windows.Forms.NumericUpDown()
            {
                Left = 80,
                Top = 8,
                DecimalPlaces = 2,
                Minimum = 0.01M,
                Maximum = 1.0M,
                Value = 0.95M,
                Width = 60
            };
            this.labelMatchColor = new System.Windows.Forms.Label() { Text = "MatchColor:", Left = 150, Top = 10, AutoSize = true };
            this.comboBoxMatchColor = new System.Windows.Forms.ComboBox()
            {
                Left = 240,
                Top = 8,
                Width = 80,
                DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            };
            this.comboBoxMatchColor.Items.AddRange(new object[] { "Color", "Grayscale" });
            this.comboBoxMatchColor.SelectedIndex = 0;
            this.panelSettings.Controls.Add(this.labelThreshold);
            this.panelSettings.Controls.Add(this.numericUpDownThreshold);
            this.panelSettings.Controls.Add(this.labelMatchColor);
            this.panelSettings.Controls.Add(this.comboBoxMatchColor);

            // SplitContainer
            this.분할주 = new System.Windows.Forms.SplitContainer();
            this.분할주.Dock = System.Windows.Forms.DockStyle.Fill;
            this.분할주.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.분할주.SplitterDistance = 300;

            // 왼쪽 패널: MultiSelectTreeView
            this.트리항목 = new MultiSelectTreeView();
            this.트리항목.CheckBoxes = true;
            this.트리항목.Dock = System.Windows.Forms.DockStyle.Fill;
            this.분할주.Panel1.Controls.Add(this.트리항목);

            // 오른쪽 패널: 그림미리보기 및 로그
            var panelRight = new System.Windows.Forms.Panel() { Dock = System.Windows.Forms.DockStyle.Fill };
            this.그림미리보기 = new System.Windows.Forms.PictureBox()
            {
                Dock = System.Windows.Forms.DockStyle.Top,
                Height = 300,
                BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle,
                SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
            };
            this.텍스트로그 = new System.Windows.Forms.TextBox()
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                Multiline = true,
                ScrollBars = System.Windows.Forms.ScrollBars.Vertical
            };
            panelRight.Controls.Add(this.텍스트로그);
            panelRight.Controls.Add(this.그림미리보기);
            this.분할주.Panel2.Controls.Add(panelRight);

            // 최종 폼 구성
            this.Controls.Add(this.분할주);
            this.Controls.Add(this.panelSettings);
            this.Controls.Add(this.툴스트립);

            this.Text = "이미지매크로 - 최종본 (오류 해결)";
            this.ClientSize = new System.Drawing.Size(1200, 700);
        }
    }
}
