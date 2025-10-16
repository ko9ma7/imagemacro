namespace 이미지매크로
{
    partial class 설정폼
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.ComboBox comboBox액션유형;
        private System.Windows.Forms.ComboBox comboBox색상모드;
        private System.Windows.Forms.NumericUpDown numericUpDown임계값;
        private System.Windows.Forms.CheckBox checkBox회전;
        private System.Windows.Forms.CheckBox checkBox리사이즈;
        private System.Windows.Forms.CheckBox checkBox액션활성;

        private System.Windows.Forms.Panel panelMouse;
        private System.Windows.Forms.ComboBox comboBox마우스동작;
        private System.Windows.Forms.RadioButton radioButton상대;
        private System.Windows.Forms.RadioButton radioButton절대;
        private System.Windows.Forms.TextBox textBox절대X;
        private System.Windows.Forms.TextBox textBox절대Y;
        private System.Windows.Forms.TextBox textBox상대X;
        private System.Windows.Forms.TextBox textBox상대Y;
        private System.Windows.Forms.CheckBox checkBox랜덤;
        private System.Windows.Forms.NumericUpDown numericUpDown랜덤;
        private System.Windows.Forms.Label label휠;
        private System.Windows.Forms.NumericUpDown numericUpDown휠;

        private System.Windows.Forms.Panel panelKeyboard;
        private System.Windows.Forms.ListBox listBox키목록;

        private System.Windows.Forms.Panel panelRecord;
        private System.Windows.Forms.TextBox textBox녹화핫키;
        private System.Windows.Forms.ListBox listBox녹화액션;

        private System.Windows.Forms.Panel panelSendMsg;
        private System.Windows.Forms.TextBox textBoxWinTitle;
        private System.Windows.Forms.TextBox textBoxChildClass;
        private System.Windows.Forms.TextBox textBoxChildText;

        private System.Windows.Forms.Panel panelDefault;

        private System.Windows.Forms.Button button확인;
        private System.Windows.Forms.Button button취소;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.Text = "설정폼";
            this.ClientSize = new System.Drawing.Size(720, 420);

            // 상단 컨트롤들
            var lblAct = new System.Windows.Forms.Label() { Text = "액션유형:", Left = 20, Top = 20, AutoSize = true };
            this.comboBox액션유형 = new System.Windows.Forms.ComboBox() { Left = 90, Top = 20, Width = 120 };
            this.comboBox액션유형.Items.AddRange(new object[] { "Default", "Mouse", "Keyboard", "Record", "SendMsg" });
            this.comboBox액션유형.SelectedIndexChanged += comboBox액션유형_SelectedIndexChanged;

            var lblThr = new System.Windows.Forms.Label() { Text = "임계값:", Left = 230, Top = 20, AutoSize = true };
            this.numericUpDown임계값 = new System.Windows.Forms.NumericUpDown()
            {
                Left = 290,
                Top = 18,
                DecimalPlaces = 2,
                Minimum = 0.01M,
                Maximum = 1.0M,
                Value = 0.80M,
                Width = 60
            };

            var lblColor = new System.Windows.Forms.Label() { Text = "색상모드:", Left = 370, Top = 20, AutoSize = true };
            this.comboBox색상모드 = new System.Windows.Forms.ComboBox() { Left = 450, Top = 18, Width = 80, DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList };
            this.comboBox색상모드.Items.AddRange(new object[] { "컬러", "그레이스케일" });
            this.comboBox색상모드.SelectedIndex = 0;

            this.checkBox회전 = new System.Windows.Forms.CheckBox() { Text = "회전", Left = 20, Top = 50, AutoSize = true };
            this.checkBox리사이즈 = new System.Windows.Forms.CheckBox() { Text = "리사이즈", Left = 90, Top = 50, AutoSize = true };
            this.checkBox액션활성 = new System.Windows.Forms.CheckBox() { Text = "액션활성", Left = 170, Top = 50, AutoSize = true };

            this.Controls.Add(lblAct);
            this.Controls.Add(this.comboBox액션유형);
            this.Controls.Add(lblThr);
            this.Controls.Add(this.numericUpDown임계값);
            this.Controls.Add(lblColor);
            this.Controls.Add(this.comboBox색상모드);
            this.Controls.Add(this.checkBox회전);
            this.Controls.Add(this.checkBox리사이즈);
            this.Controls.Add(this.checkBox액션활성);

            // Mouse 패널
            this.panelMouse = new System.Windows.Forms.Panel() { Left = 20, Top = 80, Width = 680, Height = 100, BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle };
            this.comboBox마우스동작 = new System.Windows.Forms.ComboBox() { Left = 10, Top = 10, Width = 100, DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList };
            this.comboBox마우스동작.Items.AddRange(new object[] { "LeftClick", "RightClick", "MiddleClick", "Wheel", "Drag" });
            this.comboBox마우스동작.SelectedIndexChanged += comboBox마우스동작_SelectedIndexChanged;

            this.radioButton상대 = new System.Windows.Forms.RadioButton() { Text = "상대", Left = 120, Top = 10, AutoSize = true };
            this.radioButton절대 = new System.Windows.Forms.RadioButton() { Text = "절대", Left = 180, Top = 10, AutoSize = true };

            this.textBox절대X = new System.Windows.Forms.TextBox() { Left = 10, Top = 40, Width = 40 };
            this.textBox절대Y = new System.Windows.Forms.TextBox() { Left = 60, Top = 40, Width = 40 };
            this.textBox상대X = new System.Windows.Forms.TextBox() { Left = 10, Top = 70, Width = 40 };
            this.textBox상대Y = new System.Windows.Forms.TextBox() { Left = 60, Top = 70, Width = 40 };

            this.checkBox랜덤 = new System.Windows.Forms.CheckBox() { Text = "랜덤±", Left = 120, Top = 40, AutoSize = true };
            this.numericUpDown랜덤 = new System.Windows.Forms.NumericUpDown() { Left = 190, Top = 38, Width = 50, Value = 10 };
            this.label휠 = new System.Windows.Forms.Label() { Text = "휠:", Left = 260, Top = 40, AutoSize = true, Visible = false };
            this.numericUpDown휠 = new System.Windows.Forms.NumericUpDown() { Left = 300, Top = 38, Width = 60, Minimum = -100, Maximum = 100, Visible = false };

            this.panelMouse.Controls.Add(this.comboBox마우스동작);
            this.panelMouse.Controls.Add(this.radioButton상대);
            this.panelMouse.Controls.Add(this.radioButton절대);
            this.panelMouse.Controls.Add(this.textBox절대X);
            this.panelMouse.Controls.Add(this.textBox절대Y);
            this.panelMouse.Controls.Add(this.textBox상대X);
            this.panelMouse.Controls.Add(this.textBox상대Y);
            this.panelMouse.Controls.Add(this.checkBox랜덤);
            this.panelMouse.Controls.Add(this.numericUpDown랜덤);
            this.panelMouse.Controls.Add(this.label휠);
            this.panelMouse.Controls.Add(this.numericUpDown휠);
            this.Controls.Add(this.panelMouse);

            // Keyboard 패널
            this.panelKeyboard = new System.Windows.Forms.Panel() { Left = 20, Top = 190, Width = 680, Height = 80, BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle };
            var lblKey = new System.Windows.Forms.Label() { Text = "[Keyboard] 키 목록", Left = 10, Top = 10, AutoSize = true };
            this.listBox키목록 = new System.Windows.Forms.ListBox() { Left = 10, Top = 30, Width = 200, Height = 40 };
            this.panelKeyboard.Controls.Add(lblKey);
            this.panelKeyboard.Controls.Add(this.listBox키목록);
            this.Controls.Add(this.panelKeyboard);

            // Record 패널
            this.panelRecord = new System.Windows.Forms.Panel() { Left = 20, Top = 280, Width = 680, Height = 80, BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle };
            var lblRec = new System.Windows.Forms.Label() { Text = "[Record] 핫키 및 액션", Left = 10, Top = 10, AutoSize = true };
            this.textBox녹화핫키 = new System.Windows.Forms.TextBox() { Left = 10, Top = 30, Width = 100 };
            this.listBox녹화액션 = new System.Windows.Forms.ListBox() { Left = 120, Top = 30, Width = 200, Height = 40 };
            this.panelRecord.Controls.Add(lblRec);
            this.panelRecord.Controls.Add(this.textBox녹화핫키);
            this.panelRecord.Controls.Add(this.listBox녹화액션);
            this.Controls.Add(this.panelRecord);

            // SendMsg 패널
            this.panelSendMsg = new System.Windows.Forms.Panel() { Left = 20, Top = 370, Width = 680, Height = 80, BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle };
            var lblSend = new System.Windows.Forms.Label() { Text = "[SendMsg] 창제목 및 자식 정보", Left = 10, Top = 10, AutoSize = true };
            this.textBoxWinTitle = new System.Windows.Forms.TextBox() { Left = 10, Top = 30, Width = 150 };
            this.textBoxChildClass = new System.Windows.Forms.TextBox() { Left = 170, Top = 30, Width = 150 };
            this.textBoxChildText = new System.Windows.Forms.TextBox() { Left = 330, Top = 30, Width = 150 };
            this.panelSendMsg.Controls.Add(lblSend);
            this.panelSendMsg.Controls.Add(this.textBoxWinTitle);
            this.panelSendMsg.Controls.Add(this.textBoxChildClass);
            this.panelSendMsg.Controls.Add(this.textBoxChildText);
            this.Controls.Add(this.panelSendMsg);

            // Default 패널
            this.panelDefault = new System.Windows.Forms.Panel() { Left = 20, Top = 460, Width = 680, Height = 50, BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle };
            var lblDef = new System.Windows.Forms.Label() { Text = "[Default] 템플릿 중심 근처 무작위 클릭", Left = 10, Top = 15, AutoSize = true };
            this.panelDefault.Controls.Add(lblDef);
            this.Controls.Add(this.panelDefault);

            // 확인/취소 버튼
            this.button확인 = new System.Windows.Forms.Button() { Text = "확인", Left = 540, Top = 520, Width = 60 };
            this.button취소 = new System.Windows.Forms.Button() { Text = "취소", Left = 610, Top = 520, Width = 60 };
            this.button확인.Click += button확인_Click;
            this.button취소.Click += button취소_Click;
            this.Controls.Add(this.button확인);
            this.Controls.Add(this.button취소);
        }
    }
}
