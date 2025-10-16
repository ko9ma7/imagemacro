using System;
using System.Drawing;
using System.Windows.Forms;

namespace 이미지매크로
{
    public partial class 오버레이ROI폼 : Form
    {
        public event Action<Rectangle?>? OnComplete;
        private bool _드래그 = false;
        private Point _start, _now;

        public 오버레이ROI폼(Rectangle bounding)
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            if (bounding.Width <= 0 || bounding.Height <= 0)
            {
                OnComplete?.Invoke(null);
                this.Close();
                return;
            }
            this.Location = new Point(bounding.X, bounding.Y);
            this.Size = new Size(bounding.Width, bounding.Height);
            this.BackColor = Color.Black;
            this.Opacity = 0.3;
            this.TopMost = true;
            this.DoubleBuffered = true;
            this.Cursor = Cursors.Cross;

            this.MouseDown += 오버레이ROI폼_MouseDown;
            this.MouseMove += 오버레이ROI폼_MouseMove;
            this.MouseUp += 오버레이ROI폼_MouseUp;
            this.Paint += 오버레이ROI폼_Paint;
        }

        private void 오버레이ROI폼_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _드래그 = true;
                _start = e.Location;
                _now = e.Location;
            }
            else if (e.Button == MouseButtons.Right)
            {
                OnComplete?.Invoke(null);
                this.Close();
            }
        }

        private void 오버레이ROI폼_MouseMove(object? sender, MouseEventArgs e)
        {
            if (_드래그)
            {
                _now = e.Location;
                this.Invalidate();
            }
        }

        private void 오버레이ROI폼_MouseUp(object? sender, MouseEventArgs e)
        {
            if (_드래그 && e.Button == MouseButtons.Left)
            {
                _드래그 = false;
                Rectangle sel = GetRect(_start, _now);
                if (sel.Width <= 0 || sel.Height <= 0)
                {
                    OnComplete?.Invoke(null);
                    this.Close();
                    return;
                }
                sel.Offset(this.Left, this.Top);
                OnComplete?.Invoke(sel);
                this.Close();
            }
        }

        private void 오버레이ROI폼_Paint(object? sender, PaintEventArgs e)
        {
            if (_드래그)
            {
                var r = GetRect(_start, _now);
                using (Pen pen = new Pen(Color.Red, 2))
                {
                    e.Graphics.DrawRectangle(pen, r);
                }
            }
        }

        private static Rectangle GetRect(Point p1, Point p2)
        {
            return new Rectangle(
                Math.Min(p1.X, p2.X),
                Math.Min(p1.Y, p2.Y),
                Math.Abs(p1.X - p2.X),
                Math.Abs(p1.Y - p2.Y));
        }
    }
}
