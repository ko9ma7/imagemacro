using System;
using System.Windows.Forms;

namespace 이미지매크로
{
    public partial class 설정폼 : Form
    {
        private 매크로항목 _항목;
        private 마우스후크? _후크;

        public 설정폼(매크로항목 항목)
        {
            InitializeComponent();
            _항목 = 항목;

            numericUpDown임계값.Value = (decimal)_항목.설정.임계값;
            comboBox색상모드.SelectedIndex = (_항목.설정.색상모드 == 색상모드.컬러 ? 0 : 1);

            checkBox회전.Checked = _항목.설정.회전허용;
            checkBox리사이즈.Checked = _항목.설정.리사이즈허용;
            checkBox액션활성.Checked = _항목.설정.액션활성;

            comboBox액션유형.SelectedIndex = comboBox액션유형.Items.IndexOf(_항목.설정.동작종류);
            if (comboBox액션유형.SelectedIndex < 0)
                comboBox액션유형.SelectedIndex = 0;

            // Mouse panel
            comboBox마우스동작.SelectedIndex = comboBox마우스동작.Items.IndexOf(_항목.설정.마우스동작종류);
            if (comboBox마우스동작.SelectedIndex < 0)
                comboBox마우스동작.SelectedIndex = 0;
            radioButton상대.Checked = _항목.설정.상대좌표;
            radioButton절대.Checked = !_항목.설정.상대좌표;
            textBox절대X.Text = _항목.설정.절대X.ToString();
            textBox절대Y.Text = _항목.설정.절대Y.ToString();
            textBox상대X.Text = _항목.설정.상대X.ToString();
            textBox상대Y.Text = _항목.설정.상대Y.ToString();
            checkBox랜덤.Checked = _항목.설정.랜덤옵셋;
            numericUpDown랜덤.Value = _항목.설정.랜덤범위;
            numericUpDown휠.Value = _항목.설정.휠값;
            label휠.Visible = (comboBox마우스동작.SelectedItem.ToString() == "Wheel");
            numericUpDown휠.Visible = (comboBox마우스동작.SelectedItem.ToString() == "Wheel");

            // Keyboard panel
            listBox키목록.Items.Clear();
            foreach (var k in _항목.설정.키목록)
                listBox키목록.Items.Add(k);

            // Record panel
            textBox녹화핫키.Text = _항목.설정.녹화핫키;
            listBox녹화액션.Items.Clear();
            foreach (var r in _항목.설정.녹화액션들)
                listBox녹화액션.Items.Add(r);

            // SendMsg panel
            textBoxWinTitle.Text = _항목.설정.SendMsg_WindowTitle;
            textBoxChildClass.Text = _항목.설정.SendMsg_ChildClass;
            textBoxChildText.Text = _항목.설정.SendMsg_ChildText;

            ShowPanels();
        }

        private void ShowPanels()
        {
            string sel = (comboBox액션유형.SelectedItem ?? "Default").ToString() ?? "Default";
            panelDefault.Visible = (sel == "Default");
            panelMouse.Visible = (sel == "Mouse");
            panelKeyboard.Visible = (sel == "Keyboard");
            panelRecord.Visible = (sel == "Record");
            panelSendMsg.Visible = (sel == "SendMsg");
        }

        private void comboBox액션유형_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowPanels();
        }

        private void comboBox마우스동작_SelectedIndexChanged(object sender, EventArgs e)
        {
            string sub = (comboBox마우스동작.SelectedItem ?? "LeftClick").ToString() ?? "LeftClick";
            label휠.Visible = (sub == "Wheel");
            numericUpDown휠.Visible = (sub == "Wheel");
        }

        private void button확인_Click(object sender, EventArgs e)
        {
            _항목.설정.임계값 = (float)numericUpDown임계값.Value;
            _항목.설정.색상모드 = (comboBox색상모드.SelectedIndex == 0 ? 색상모드.컬러 : 색상모드.그레이스케일);
            _항목.설정.회전허용 = checkBox회전.Checked;
            _항목.설정.리사이즈허용 = checkBox리사이즈.Checked;
            _항목.설정.액션활성 = checkBox액션활성.Checked;
            _항목.설정.동작종류 = (comboBox액션유형.SelectedItem ?? "Default").ToString() ?? "Default";

            if (panelMouse.Visible)
            {
                _항목.설정.마우스동작종류 = (comboBox마우스동작.SelectedItem ?? "LeftClick").ToString() ?? "LeftClick";
                _항목.설정.상대좌표 = radioButton상대.Checked;
                int.TryParse(textBox절대X.Text, out int ax);
                int.TryParse(textBox절대Y.Text, out int ay);
                _항목.설정.절대X = ax;
                _항목.설정.절대Y = ay;
                int.TryParse(textBox상대X.Text, out int rx);
                int.TryParse(textBox상대Y.Text, out int ry);
                _항목.설정.상대X = rx;
                _항목.설정.상대Y = ry;
                _항목.설정.랜덤옵셋 = checkBox랜덤.Checked;
                _항목.설정.랜덤범위 = (int)numericUpDown랜덤.Value;
                _항목.설정.휠값 = (int)numericUpDown휠.Value;
            }
            if (panelKeyboard.Visible)
            {
                _항목.설정.키목록.Clear();
                foreach (var i in listBox키목록.Items)
                    _항목.설정.키목록.Add(i.ToString() ?? "");
            }
            if (panelRecord.Visible)
            {
                _항목.설정.녹화핫키 = textBox녹화핫키.Text;
                _항목.설정.녹화액션들.Clear();
                foreach (var i in listBox녹화액션.Items)
                    _항목.설정.녹화액션들.Add(i.ToString() ?? "");
            }
            if (panelSendMsg.Visible)
            {
                _항목.설정.SendMsg_WindowTitle = textBoxWinTitle.Text;
                _항목.설정.SendMsg_ChildClass = textBoxChildClass.Text;
                _항목.설정.SendMsg_ChildText = textBoxChildText.Text;
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button취소_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void button절대캡쳐_Click(object sender, EventArgs e)
        {
            this.Hide();
            _후크?.Dispose();
            _후크 = new 마우스후크();
            _후크.마우스왼클릭 += (s2, e2) =>
            {
                this.Invoke(new Action(() =>
                {
                    textBox절대X.Text = e2.X.ToString();
                    textBox절대Y.Text = e2.Y.ToString();
                    this.Show();
                }));
                _후크.Dispose();
                _후크 = null;
            };
        }

        private void button상대캡쳐_Click(object sender, EventArgs e)
        {
            this.Hide();
            _후크?.Dispose();
            _후크 = new 마우스후크();
            _후크.마우스왼클릭 += (s2, e2) =>
            {
                var r = 메인폼.고정사각형;
                int rx = e2.X - r.왼;
                int ry = e2.Y - r.위;
                this.Invoke(new Action(() =>
                {
                    textBox상대X.Text = rx.ToString();
                    textBox상대Y.Text = ry.ToString();
                    this.Show();
                }));
                _후크.Dispose();
                _후크 = null;
            };
        }

        private void button키추가_Click(object sender, EventArgs e)
        {
            var s = Microsoft.VisualBasic.Interaction.InputBox("키입력", "키추가", "");
            if (!string.IsNullOrEmpty(s))
                listBox키목록.Items.Add(s);
        }

        private void button키삭제_Click(object sender, EventArgs e)
        {
            if (listBox키목록.SelectedIndex >= 0)
                listBox키목록.Items.RemoveAt(listBox키목록.SelectedIndex);
        }

        private void button녹화추가_Click(object sender, EventArgs e)
        {
            var s = Microsoft.VisualBasic.Interaction.InputBox("녹화 액션", "녹화추가", "");
            if (!string.IsNullOrEmpty(s))
                listBox녹화액션.Items.Add(s);
        }

        private void button녹화삭제_Click(object sender, EventArgs e)
        {
            if (listBox녹화액션.SelectedIndex >= 0)
                listBox녹화액션.Items.RemoveAt(listBox녹화액션.SelectedIndex);
        }
    }
}
