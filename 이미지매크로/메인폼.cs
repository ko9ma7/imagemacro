using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Newtonsoft.Json;

namespace 이미지매크로
{
    public partial class 메인폼 : Form
    {
        private bool _실행중 = false;
        private Thread? _스레드;
        private CancellationTokenSource? _취소;
        private readonly object _lockObj = new object();

        private List<매크로항목> _목록 = new List<매크로항목>();
        private string _데이터파일 = Path.Combine(Application.StartupPath, "매크로데이터.json");

        // 대상창 핸들 – WindowFromPoint로 직접 가져옴
        private IntPtr _대상창 = IntPtr.Zero;
        private IntPtr _대상자식 = IntPtr.Zero;
        private 윈도우API.사각형 _창Rect;
        public static 윈도우API.사각형 고정사각형;

        // 클릭 좌표 산출용 Random
        private Random _rnd = new Random();

        public 메인폼()
        {
            InitializeComponent();

            // TreeView (MultiSelectTreeView 사용)
            트리항목.AfterSelect += 트리항목_AfterSelect;
            트리항목.AfterCheck += 트리항목_AfterCheck;
            트리항목.ItemDrag += (s, e) => DoDragDrop((TreeNode)e.Item, DragDropEffects.Move);
            트리항목.DragEnter += (s, e) => e.Effect = DragDropEffects.Move;
            트리항목.DragOver += (s, e) => e.Effect = DragDropEffects.Move;
            트리항목.DragDrop += 트리항목_DragDrop;

            var ctx = new ContextMenuStrip();
            ctx.Items.AddRange(new ToolStripItem[] {
                new ToolStripMenuItem("폴더생성", null, (o, e) => 폴더생성()),
                new ToolStripMenuItem("이미지추가(ROI)", null, (o, e) => ButtonAddImage_Click(null, null)),
                new ToolStripMenuItem("삭제", null, (o, e) => 선택삭제()),
                new ToolStripMenuItem("이름편집", null, (o, e) => 이름편집()),
                new ToolStripMenuItem("설정", null, (o, e) => 열기설정()),
                new ToolStripMenuItem("ROI수정", null, (o, e) => ROI수정())
            });
            트리항목.ContextMenuStrip = ctx;
            트리항목.NodeMouseClick += (s, e) =>
            {
                if (e.Button == MouseButtons.Right)
                    트리항목.SelectedNode = e.Node;
            };

            버튼창선택.Click += 버튼창선택_Click;
            버튼시작.Click += 버튼시작_Click;
            버튼중지.Click += 버튼중지_Click;
            버튼활성.Click += (s, e) => { 버튼활성.Checked = true; 버튼비활성.Checked = false; };
            버튼비활성.Click += (s, e) => { 버튼활성.Checked = false; 버튼비활성.Checked = true; };
            메뉴한번.Click += (s, e) => { 메뉴한번.Checked = true; 메뉴매번.Checked = false; };
            메뉴매번.Click += (s, e) => { 메뉴한번.Checked = false; 메뉴매번.Checked = true; };

            this.Load += 메인폼_Load;
            this.FormClosing += 메인폼_FormClosing;
        }

        private void 메인폼_Load(object? sender, EventArgs e)
        {
            Log("프로그램 로드");
            SettingStore.Load();
            if (SettingStore.MainFormSize.Width > 100 && SettingStore.MainFormSize.Height > 100)
            {
                this.Location = SettingStore.MainFormLocation;
                this.Size = SettingStore.MainFormSize;
            }
            텍스트딜레이.Text = SettingStore.LastDelay.ToString();
            if (SettingStore.LastActiveMode)
            {
                버튼활성.Checked = true;
                버튼비활성.Checked = false;
            }
            else
            {
                버튼활성.Checked = false;
                버튼비활성.Checked = true;
            }
            if (SettingStore.LastCaptureOnce)
            {
                메뉴한번.Checked = true; 메뉴매번.Checked = false;
            }
            else
            {
                메뉴한번.Checked = false; 메뉴매번.Checked = true;
            }
            if (!string.IsNullOrEmpty(SettingStore.LastWindowTitle))
            {
                var h = 윈도우API.FindWindow(null, SettingStore.LastWindowTitle);
                if (h != IntPtr.Zero && 윈도우API.GetWindowRect(h, out _창Rect))
                {
                    _대상창 = h;
                    _대상자식 = h;
                    고정사각형 = _창Rect;
                    Log($"[이전창복원] 0x{h:X}, title={윈도우API.GetWindowTextName(h)}");
                }
            }

            LoadData();
            PopulateTreeView();

            var timer = new System.Windows.Forms.Timer();
            timer.Interval = 1000;
            timer.Tick += (_, __) =>
            {
                if (_대상창 != IntPtr.Zero)
                    윈도우API.GetWindowRect(_대상창, out _창Rect);
            };
            timer.Start();
        }

        private void 메인폼_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (_취소 != null && !_취소.IsCancellationRequested)
            {
                _취소.Cancel();
                _스레드?.Join();
            }
            if (this.WindowState == FormWindowState.Normal)
            {
                SettingStore.MainFormLocation = this.Location;
                SettingStore.MainFormSize = this.Size;
            }
            if (int.TryParse(텍스트딜레이.Text, out int d))
                SettingStore.LastDelay = d;
            else
                SettingStore.LastDelay = 1000;
            SettingStore.LastActiveMode = 버튼활성.Checked;
            SettingStore.LastCaptureOnce = 메뉴한번.Checked;
            if (_대상창 != IntPtr.Zero)
            {
                var t = 윈도우API.GetWindowTextName(_대상창);
                SettingStore.LastWindowTitle = t;
            }
            SettingStore.Save();
            SaveData();
        }

        private void Log(string msg)
        {
            if (텍스트로그.InvokeRequired)
                텍스트로그.Invoke(new Action(() => 텍스트로그.AppendText($"[{DateTime.Now:HH:mm:ss}] {msg}\r\n")));
            else
                텍스트로그.AppendText($"[{DateTime.Now:HH:mm:ss}] {msg}\r\n");
        }

        #region 데이터 처리
        private void LoadData()
        {
            if (File.Exists(_데이터파일))
            {
                try
                {
                    var txt = File.ReadAllText(_데이터파일);
                    var arr = JsonConvert.DeserializeObject<List<매크로항목>>(txt);
                    if (arr != null)
                    {
                        _목록.Clear();
                        _목록.AddRange(arr);
                        Log($"데이터 로드 완료: {_목록.Count}개");
                    }
                }
                catch (Exception ex)
                {
                    Log("LoadData 오류: " + ex.Message);
                }
            }
            else
            {
                Log("매크로데이터.json 없음");
            }
        }

        private void SaveData()
        {
            try
            {
                lock (_lockObj)
                {
                    var json = JsonConvert.SerializeObject(_목록, Formatting.Indented);
                    File.WriteAllText(_데이터파일, json);
                }
                Log("데이터 저장 완료");
            }
            catch (Exception ex)
            {
                Log("SaveData 오류: " + ex.Message);
            }
        }
        #endregion

        #region TreeView 표시 및 드래그‑드롭 (MultiSelectTreeView 사용)
        private void PopulateTreeView()
        {
            트리항목.Nodes.Clear();
            // 폴더 노드 추가
            foreach (var item in _목록)
            {
                if (item.유형 == 항목유형.폴더)
                {
                    TreeNode folderNode = new TreeNode(item.표시이름)
                    {
                        Tag = item,
                        Checked = item.체크됨
                    };
                    foreach (var child in _목록)
                    {
                        if (child.유형 == 항목유형.파일 && child.부모아이디 == item.아이디)
                        {
                            TreeNode fileNode = new TreeNode(child.표시이름)
                            {
                                Tag = child,
                                Checked = child.체크됨
                            };
                            folderNode.Nodes.Add(fileNode);
                        }
                    }
                    트리항목.Nodes.Add(folderNode);
                }
            }
            // 부모아이디가 없는 파일 노드
            foreach (var item in _목록)
            {
                if (item.유형 == 항목유형.파일 && string.IsNullOrEmpty(item.부모아이디))
                {
                    TreeNode fileNode = new TreeNode(item.표시이름)
                    {
                        Tag = item,
                        Checked = item.체크됨
                    };
                    트리항목.Nodes.Add(fileNode);
                }
            }
            트리항목.ExpandAll();
        }

        private void 트리항목_DragDrop(object? sender, DragEventArgs e)
        {
            if (e.Data?.GetDataPresent(typeof(TreeNode)) == true)
            {
                Point pt = 트리항목.PointToClient(new Point(e.X, e.Y));
                TreeNode targetNode = 트리항목.GetNodeAt(pt);
                if (targetNode != null && targetNode.Tag is 매크로항목 targetItem && targetItem.유형 == 항목유형.폴더)
                {
                    if (트리항목 is MultiSelectTreeView mTree)
                    {
                        foreach (TreeNode node in mTree.SelectedNodes)
                        {
                            if (node.Tag is 매크로항목 draggedItem)
                            {
                                draggedItem.부모아이디 = targetItem.아이디;
                            }
                        }
                    }
                    SaveData();
                    PopulateTreeView();
                }
            }
        }

        private void 트리항목_AfterSelect(object? sender, TreeViewEventArgs e)
        {
            if (e.Node?.Tag is 매크로항목 data && data.유형 == 항목유형.파일)
            {
                if (File.Exists(data.파일경로))
                {
                    try
                    {
                        using var b = new Bitmap(data.파일경로);
                        그림미리보기.Image?.Dispose();
                        그림미리보기.Image = new Bitmap(b);
                        그림미리보기.Visible = true;
                    }
                    catch
                    {
                        그림미리보기.Image?.Dispose();
                        그림미리보기.Image = null;
                        그림미리보기.Visible = false;
                    }
                }
                else
                {
                    그림미리보기.Image?.Dispose();
                    그림미리보기.Image = null;
                    그림미리보기.Visible = false;
                }
            }
            else
            {
                그림미리보기.Image?.Dispose();
                그림미리보기.Image = null;
                그림미리보기.Visible = false;
            }
        }

        private void 트리항목_AfterCheck(object? sender, TreeViewEventArgs e)
        {
            if (e.Node?.Tag is 매크로항목 data)
            {
                data.체크됨 = e.Node.Checked;
                if (data.유형 == 항목유형.폴더)
                {
                    foreach (TreeNode c in e.Node.Nodes)
                    {
                        c.Checked = e.Node.Checked;
                        if (c.Tag is 매크로항목 childData)
                            childData.체크됨 = c.Checked;
                    }
                }
            }
        }
        #endregion

        #region 대상창 선택
        private void 버튼창선택_Click(object? sender, EventArgs e)
        {
            this.Hide();
            bool done = false;
            using var hook = new 마우스후크();
            hook.마우스왼클릭 += (s2, e2) =>
            {
                var hWnd = 윈도우API.WindowFromPoint(new Point(e2.X, e2.Y));
                if (hWnd != IntPtr.Zero)
                {
                    _대상창 = hWnd;
                    _대상자식 = hWnd;
                    if (윈도우API.GetWindowRect(_대상창, out _창Rect))
                    {
                        고정사각형 = _창Rect;
                        Log($"[창선택] 0x{_대상창:X}, title={윈도우API.GetWindowTextName(_대상창)}");
                    }
                    else
                    {
                        Log("GetWindowRect 실패");
                    }
                }
                else
                {
                    Log("유효한 창 아님");
                }
                done = true;
            };
            while (!done)
            {
                Application.DoEvents();
                Thread.Sleep(30);
            }
            this.Show();
        }
        #endregion

        #region 이미지 추가(ROI)
        private void ButtonAddImage_Click(object? sender, EventArgs e)
        {
            if (_대상창 == IntPtr.Zero)
            {
                Log("이미지추가 실패: 대상창 없음");
                return;
            }
            if (!윈도우API.GetWindowRect(_대상창, out _창Rect))
            {
                Log("GetWindowRect 실패 -> 이미지추가 불가");
                return;
            }
            int ww = _창Rect.오른 - _창Rect.왼;
            int hh = _창Rect.아래 - _창Rect.위;
            if (ww <= 0 || hh <= 0)
            {
                Log("대상창 크기가 유효하지 않음");
                return;
            }
            var bounding = new Rectangle(_창Rect.왼, _창Rect.위, ww, hh);
            this.Hide();
            var ov = new 오버레이ROI폼(bounding);
            ov.OnComplete += (roiSel) =>
            {
                this.Show();
                if (roiSel == null)
                {
                    Log("ROI추가 취소");
                    return;
                }
                var rr = new Rectangle(
                    roiSel.Value.X - _창Rect.왼,
                    roiSel.Value.Y - _창Rect.위,
                    roiSel.Value.Width,
                    roiSel.Value.Height);
                if (rr.Width <= 0 || rr.Height <= 0)
                {
                    Log("ROI=0");
                    return;
                }
                Bitmap? cap = (버튼비활성.Checked ? 캡처비활성(_대상창) : 캡처활성(_대상창));
                if (cap == null)
                {
                    Log("캡처 실패");
                    return;
                }
                var cRect = new Rectangle(0, 0, cap.Width, cap.Height);
                rr.Intersect(cRect);
                if (rr.Width <= 0 || rr.Height <= 0)
                {
                    Log("ROI 교집합=0");
                    cap.Dispose();
                    return;
                }
                string name = "ROI_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string folder = Path.Combine(Application.StartupPath, "captures");
                Directory.CreateDirectory(folder);
                string path = Path.Combine(folder, name + ".png");
                try
                {
                    using var crop = cap.Clone(rr, cap.PixelFormat);
                    crop.Save(path, ImageFormat.Png);
                }
                catch (Exception ex2)
                {
                    Log("이미지 저장 오류: " + ex2.Message);
                    cap.Dispose();
                    return;
                }
                cap.Dispose();

                float thr = 0.95f;
                float.TryParse(numericUpDownThreshold.Value.ToString(), out thr);
                var mc = (comboBoxMatchColor.SelectedIndex == 0 ? 색상모드.컬러 : 색상모드.그레이스케일);

                var it = new 매크로항목
                {
                    아이디 = Guid.NewGuid().ToString(),
                    유형 = 항목유형.파일,
                    표시이름 = name,
                    체크됨 = true,
                    파일경로 = path,
                    ROI = new 영역정보 { X = rr.X, Y = rr.Y, 폭 = rr.Width, 높이 = rr.Height },
                    원본너비 = ww,
                    원본높이 = hh,
                    항목임계값 = thr,
                    OriginalWindowSize = new Size(ww, hh),
                    Description = ""
                };
                it.부모아이디 = "";
                // 최초 추가 시 원본ROI 저장 (이후 클릭 기준은 이 값을 사용)
                it.원본ROI = new 영역정보 { X = rr.X, Y = rr.Y, 폭 = rr.Width, 높이 = rr.Height };

                it.설정.색상모드 = mc;
                it.설정.임계값 = thr;
                it.설정.이미지찾기활성 = true;

                var ttitle = 윈도우API.GetWindowTextName(_대상창);
                if (_대상자식 != IntPtr.Zero && _대상창 != _대상자식)
                {
                    it.설정.SendMsg_WindowTitle = ttitle;
                    it.설정.SendMsg_ChildClass = 윈도우API.GetWindowClassName(_대상자식);
                    it.설정.SendMsg_ChildText = 윈도우API.GetWindowTextName(_대상자식);
                }
                else
                {
                    it.설정.SendMsg_WindowTitle = ttitle;
                }
                lock (_lockObj) { _목록.Add(it); }
                SaveData();
                PopulateTreeView();
                Log($"이미지추가 => {name}, ROI=({rr})");
            };
            ov.ShowDialog();
        }
        #endregion

        #region 시작/중지
        private void 버튼시작_Click(object? sender, EventArgs e)
        {
            if (_스레드 != null && _스레드.IsAlive)
            {
                Log("이미 실행중");
                return;
            }
            if (_대상창 == IntPtr.Zero)
            {
                Log("대상창 없음");
                return;
            }
            lock (_lockObj)
            {
                if (_목록.Count == 0)
                {
                    Log("목록이 비어있음");
                    return;
                }
            }
            _취소 = new CancellationTokenSource();
            _실행중 = true;
            _스레드 = new Thread(() => 메인루프(_취소.Token));
            _스레드.IsBackground = true;
            _스레드.Start();
            Log("스레드 시작");
        }

        private void 버튼중지_Click(object? sender, EventArgs e)
        {
            if (!_실행중 || _취소 == null)
            {
                Log("실행중 아님");
                return;
            }
            _취소.Cancel();
            Log("중지 요청");
        }
        #endregion

        #region 메인루프 및 템플릿매칭 (폴더/파일 구분)
        private void 메인루프(CancellationToken token)
        {
            Log("메인루프 시작");
            try
            {
                int delay = 1000;
                int.TryParse(텍스트딜레이.Text, out delay);
                bool once = 메뉴한번.Checked;
                Bitmap? onceCap = null;

                while (!token.IsCancellationRequested)
                {
                    if (!윈도우API.GetWindowRect(_대상창, out _창Rect))
                    {
                        Log("대상창 rect 실패");
                        Thread.Sleep(delay);
                        continue;
                    }
                    List<매크로항목> copy;
                    lock (_lockObj)
                    {
                        copy = new List<매크로항목>(_목록);
                    }
                    if (once && onceCap == null)
                    {
                        onceCap = (버튼비활성.Checked ? 캡처비활성(_대상창) : 캡처활성(_대상창));
                        if (onceCap == null)
                        {
                            Log("캡처 실패(once)");
                            Thread.Sleep(delay);
                            continue;
                        }
                        Log("[캡처모드=한번]");
                    }
                    // 최상위 항목 (부모아이디가 없는 항목)만 순회
                    foreach (var it in copy)
                    {
                        if (token.IsCancellationRequested) break;
                        if (!it.체크됨)
                            continue;
                        if (!string.IsNullOrEmpty(it.부모아이디))
                            continue; // 하위 항목은 스킵

                        if (!it.설정.이미지찾기활성)
                            continue;

                        if (it.유형 == 항목유형.파일)
                        {
                            Bitmap? cap = once ? onceCap : (버튼비활성.Checked ? 캡처비활성(_대상창) : 캡처활성(_대상창));
                            if (cap == null)
                            {
                                Log("캡처 실패(each)");
                                Thread.Sleep(delay);
                                continue;
                            }
                            bool ok = DoTemplateMatch(cap, it);
                            if (ok)
                            {
                                Log($"매칭성공 => {it.표시이름}");
                                DoAction_Click(it, cap.Width, cap.Height);
                            }
                            else
                            {
                                Log($"매칭실패 => {it.표시이름}");
                            }
                            if (!once) cap.Dispose();
                        }
                        else if (it.유형 == 항목유형.폴더)
                        {
                            // 폴더: 자식 중 대표(첫 번째 체크된 항목)를 대상으로 처리
                            var rep = copy.Find(x => x.부모아이디 == it.아이디 && x.체크됨 && x.설정.이미지찾기활성);
                            if (rep == null)
                            {
                                Log($"폴더 {it.표시이름} - 자식 없음");
                            }
                            else
                            {
                                Bitmap? cap = once ? onceCap : (버튼비활성.Checked ? 캡처비활성(_대상창) : 캡처활성(_대상창));
                                if (cap == null)
                                {
                                    Log("캡처 실패(folder)");
                                    Thread.Sleep(delay);
                                    continue;
                                }
                                bool ok = DoTemplateMatch(cap, rep);
                                if (ok)
                                {
                                    Log($"폴더 {it.표시이름} 매칭성공 => {rep.표시이름}");
                                    DoAction_Click(rep, cap.Width, cap.Height);
                                }
                                else
                                {
                                    Log($"폴더 {it.표시이름} 매칭실패 => {rep.표시이름}");
                                }
                                if (!once) cap.Dispose();
                            }
                        }
                        Thread.Sleep(delay);
                        if (token.IsCancellationRequested) break;
                    }
                    Thread.Sleep(delay);
                }
                onceCap?.Dispose();
            }
            catch (Exception ex)
            {
                Log("메인루프오류: " + ex.Message);
            }
            finally
            {
                _실행중 = false;
                Log("메인루프 종료");
            }
        }
        // 템플릿 매칭 개선 – 회전 및 리사이즈 옵션 적용
        private bool DoTemplateMatch(Bitmap scr, 매크로항목 it)
        {
            if (string.IsNullOrEmpty(it.파일경로) || !File.Exists(it.파일경로))
                return false;
            var adj = AdjustROI(it, scr.Width, scr.Height);
            if (adj.Width <= 0 || adj.Height <= 0)
                return false;
            using var roi = scr.Clone(adj, scr.PixelFormat);
            using var origTemplate = new Bitmap(it.파일경로);

            float threshold = (it.설정.임계값 > 0 && it.설정.임계값 <= 1.0f) ? it.설정.임계값 : it.항목임계값;

            List<float> angleCandidates = new List<float>() { 0 };
            if (it.설정.회전허용)
            {
                angleCandidates = new List<float>() { -30, -20, -10, 0, 10, 20, 30 };
            }
            List<float> scaleCandidates = new List<float>() { 1.0f };
            if (it.설정.리사이즈허용)
            {
                scaleCandidates = new List<float>() { 0.9f, 1.0f, 1.1f };
            }

            bool found = false;
            foreach (var scale in scaleCandidates)
            {
                foreach (var angle in angleCandidates)
                {
                    using var template = TransformTemplate(origTemplate, scale, angle);
                    if (template.Width > roi.Width || template.Height > roi.Height)
                        continue;
                    using var matScr = BitmapToMat(roi);
                    using var matRef = BitmapToMat(template);
                    if (matScr == null || matRef == null)
                        continue;
                    using var result = new Emgu.CV.Mat();
                    CvInvoke.MatchTemplate(matScr, matRef, result, TemplateMatchingType.CcoeffNormed);
                    double minVal = 0, maxVal = 0;
                    Point minLoc = Point.Empty, maxLoc = Point.Empty;
                    CvInvoke.MinMaxLoc(result, ref minVal, ref maxVal, ref minLoc, ref maxLoc);
                    if (maxVal >= threshold)
                    {
                        found = true;
                        break;
                    }
                }
                if (found) break;
            }
            return found;
        }

        // AdjustROI: 스크린 캡처 크기에 맞춰 ROI 스케일 조정
        private Rectangle AdjustROI(매크로항목 it, int w, int h)
        {
            if (it.OriginalWindowSize.Width <= 0 || it.OriginalWindowSize.Height <= 0)
                return new Rectangle(0, 0, w, h);
            float sx = (float)w / it.OriginalWindowSize.Width;
            float sy = (float)h / it.OriginalWindowSize.Height;
            int rx = (int)(it.ROI.X * sx);
            int ry = (int)(it.ROI.Y * sy);
            int rw = (int)(it.ROI.폭 * sx);
            int rh = (int)(it.ROI.높이 * sy);
            var r = new Rectangle(rx, ry, rw, rh);
            r.Intersect(new Rectangle(0, 0, w, h));
            return r;
        }

        // BitmapToMat: Emgu CV Mat 변환
        private Emgu.CV.Mat? BitmapToMat(Bitmap bmp)
        {
            if (bmp == null)
                return null;
            var pf = bmp.PixelFormat;
            if (pf != PixelFormat.Format24bppRgb && pf != PixelFormat.Format32bppArgb)
                return null;
            int w = bmp.Width, h = bmp.Height;
            int ch = Image.GetPixelFormatSize(pf) / 8;
            var mat = new Emgu.CV.Mat(h, w, DepthType.Cv8U, ch);
            var bd = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, pf);
            try
            {
                int bytes = Math.Abs(bd.Stride) * h;
                byte[] arr = new byte[bytes];
                Marshal.Copy(bd.Scan0, arr, 0, bytes);
                if (ch == 3)
                {
                    for (int i = 0; i < bytes; i += 3)
                    {
                        byte t = arr[i];
                        arr[i] = arr[i + 2];
                        arr[i + 2] = t;
                    }
                }
                else if (ch == 4)
                {
                    for (int i = 0; i < bytes; i += 4)
                    {
                        byte t = arr[i];
                        arr[i] = arr[i + 2];
                        arr[i + 2] = t;
                    }
                }
                Marshal.Copy(arr, 0, mat.DataPointer, bytes);
            }
            finally
            {
                bmp.UnlockBits(bd);
            }
            return mat;
        }

        // TransformTemplate: 원본 템플릿을 scale 및 angle 만큼 변환 (회전 및 리사이즈)
        private Bitmap TransformTemplate(Bitmap orig, float scale, float angle)
        {
            int newWidth = (int)(orig.Width * scale);
            int newHeight = (int)(orig.Height * scale);
            Bitmap scaled = new Bitmap(newWidth, newHeight);
            using (Graphics g = Graphics.FromImage(scaled))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(orig, new Rectangle(0, 0, newWidth, newHeight));
            }
            double rad = angle * Math.PI / 180.0;
            double cos = Math.Abs(Math.Cos(rad));
            double sin = Math.Abs(Math.Sin(rad));
            int rotatedWidth = (int)(newWidth * cos + newHeight * sin);
            int rotatedHeight = (int)(newWidth * sin + newHeight * cos);
            Bitmap rotated = new Bitmap(rotatedWidth, rotatedHeight);
            rotated.SetResolution(scaled.HorizontalResolution, scaled.VerticalResolution);
            using (Graphics g = Graphics.FromImage(rotated))
            {
                g.Clear(Color.Transparent);
                g.TranslateTransform(rotatedWidth / 2f, rotatedHeight / 2f);
                g.RotateTransform(angle);
                g.TranslateTransform(-newWidth / 2f, -newHeight / 2f);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(scaled, new Point(0, 0));
            }
            scaled.Dispose();
            return rotated;
        }

        #region 액션 실행
        private void DoAction_Click(매크로항목 it, int scrW, int scrH)
        {
            if (!it.설정.액션활성)
            {
                Log($"[{it.표시이름}] 액션비활성");
                return;
            }
            switch (it.설정.동작종류)
            {
                case "Default":
                    {
                        Point clickPt = GetClickPoint(it, scrW, scrH);
                        if (버튼비활성.Checked)
                            InactiveClick(_대상창, clickPt);
                        else
                            ActiveClick(_대상창, clickPt);
                        break;
                    }
                case "Mouse":
                    {
                        Point clickPt = it.설정.상대좌표 ? new Point(it.설정.상대X, it.설정.상대Y)
                                                          : new Point(it.설정.절대X, it.설정.절대Y);
                        if (it.설정.랜덤옵셋)
                        {
                            int range = it.설정.랜덤범위;
                            clickPt.X += _rnd.Next(-range, range + 1);
                            clickPt.Y += _rnd.Next(-range, range + 1);
                        }
                        if (버튼비활성.Checked)
                            InactiveClick(_대상창, clickPt);
                        else
                            ActiveClick(_대상창, clickPt);
                        break;
                    }
                case "Keyboard":
                    {
                        if (버튼활성.Checked)
                        {
                            윈도우API.SetForegroundWindow(_대상창);
                            Thread.Sleep(100);
                        }
                        foreach (var key in it.설정.키목록)
                        {
                            SendKeys.SendWait(key);
                            Thread.Sleep(50);
                        }
                        break;
                    }
                case "Record":
                    {
                        if (버튼활성.Checked)
                        {
                            윈도우API.SetForegroundWindow(_대상창);
                            Thread.Sleep(100);
                        }
                        foreach (var act in it.설정.녹화액션들)
                        {
                            SendKeys.SendWait(act);
                            Thread.Sleep(50);
                        }
                        break;
                    }
                case "SendMsg":
                    {
                        IntPtr target = 윈도우API.FindWindow(null, it.설정.SendMsg_WindowTitle);
                        if (target == IntPtr.Zero)
                        {
                            Log("SendMsg: 대상창을 찾을 수 없음");
                            return;
                        }
                        IntPtr child = target;
                        if (!string.IsNullOrEmpty(it.설정.SendMsg_ChildClass))
                        {
                            child = 윈도우API.FindWindowEx(target, IntPtr.Zero, it.설정.SendMsg_ChildClass, it.설정.SendMsg_ChildText);
                            if (child == IntPtr.Zero)
                            {
                                Log("SendMsg: 자식창을 찾을 수 없음");
                            }
                        }
                        const int BM_CLICK = 0x00F5;
                        윈도우API.PostMessage(child, BM_CLICK, IntPtr.Zero, IntPtr.Zero);
                        Log("SendMsg: 메시지 전송 완료");
                        break;
                    }
                default:
                    {
                        Log("알 수 없는 동작종류: " + it.설정.동작종류);
                        break;
                    }
            }
        }

        // GetClickPoint: 원본ROI(최초 저장된 값)와 원본창 크기를 기준으로 현재 창 크기에 맞게 클릭 좌표 계산
        private Point GetClickPoint(매크로항목 it, int scrW, int scrH)
        {
            float centerX = it.원본ROI.X + it.원본ROI.폭 / 2f;
            float centerY = it.원본ROI.Y + it.원본ROI.높이 / 2f;
            float ratioX = centerX / it.OriginalWindowSize.Width;
            float ratioY = centerY / it.OriginalWindowSize.Height;
            int baseX = (int)(ratioX * scrW);
            int baseY = (int)(ratioY * scrH);
            if (it.설정.랜덤옵셋)
            {
                int range = it.설정.랜덤범위;
                baseX += _rnd.Next(-range, range + 1);
                baseY += _rnd.Next(-range, range + 1);
            }
            return new Point(baseX, baseY);
        }
        #endregion

        #region 캡처 (활성/비활성)
        private Bitmap? 캡처활성(IntPtr h)
        {
            if (!윈도우API.GetWindowRect(h, out _창Rect)) return null;
            int w = _창Rect.오른 - _창Rect.왼;
            int hh = _창Rect.아래 - _창Rect.위;
            if (w <= 0 || hh <= 0) return null;
            var bmp = new Bitmap(w, hh, PixelFormat.Format32bppArgb);
            using var g = Graphics.FromImage(bmp);
            g.CopyFromScreen(_창Rect.왼, _창Rect.위, 0, 0, new Size(w, hh), CopyPixelOperation.SourceCopy);
            return bmp;
        }

        private Bitmap? 캡처비활성(IntPtr h)
        {
            if (!윈도우API.GetWindowRect(h, out _창Rect)) return null;
            int w = _창Rect.오른 - _창Rect.왼;
            int hh = _창Rect.아래 - _창Rect.위;
            if (w <= 0 || hh <= 0) return null;
            var bmp = new Bitmap(w, hh, PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(bmp))
            {
                IntPtr hdc = g.GetHdc();
                bool ok = 윈도우API.PrintWindow(h, hdc, (uint)윈도우API.전체렌더링);
                g.ReleaseHdc(hdc);
                if (!ok)
                {
                    IntPtr windowDC = 윈도우API.GetWindowDC(h);
                    if (windowDC != IntPtr.Zero)
                    {
                        using (Graphics gdc = Graphics.FromHdc(windowDC))
                        {
                            gdc.CopyFromScreen(_창Rect.왼, _창Rect.위, 0, 0, new Size(w, hh), CopyPixelOperation.SourceCopy);
                        }
                        윈도우API.ReleaseDC(h, windowDC);
                    }
                }
            }
            return bmp;
        }
        #endregion

        #region 클릭 (Inactive/Active) – Point 매개변수 사용
        [DllImport("user32.dll")]
        private static extern void mouse_event(uint flags, uint dx, uint dy, uint cButtons, UIntPtr extra);
        private const uint LEFTDOWN = 0x02;
        private const uint LEFTUP = 0x04;

        private void InactiveClick(IntPtr hWnd, Point pt)
        {
            try
            {
                int param = 윈도우API.MAKELPARAM(pt.X, pt.Y);
                윈도우API.PostMessage(hWnd, 윈도우API.WM_MOUSEMOVE, IntPtr.Zero, (IntPtr)param);
                Thread.Sleep(30);
                윈도우API.PostMessage(hWnd, 윈도우API.WM_LBUTTONDOWN, new IntPtr(1), (IntPtr)param);
                Thread.Sleep(50);
                윈도우API.PostMessage(hWnd, 윈도우API.WM_LBUTTONUP, IntPtr.Zero, (IntPtr)param);
            }
            catch (Exception ex)
            {
                Log("InactiveClick 오류: " + ex.Message);
            }
        }

        private void ActiveClick(IntPtr hWnd, Point pt)
        {
            try
            {
                if (!윈도우API.GetWindowRect(hWnd, out var rc)) return;
                int scrX = rc.왼 + pt.X;
                int scrY = rc.위 + pt.Y;
                Log($"ActiveClick => scr=({scrX},{scrY})");
                Cursor.Position = new Point(scrX, scrY);
                Thread.Sleep(30);
                mouse_event(LEFTDOWN, 0, 0, 0, UIntPtr.Zero);
                Thread.Sleep(80);
                mouse_event(LEFTUP, 0, 0, 0, UIntPtr.Zero);
            }
            catch (Exception ex)
            {
                Log("ActiveClick 오류: " + ex.Message);
            }
        }
        #endregion

        #region 컨텍스트 메뉴 메서드 (다중 선택 지원)
        private void 폴더생성()
        {
            var s = Microsoft.VisualBasic.Interaction.InputBox("폴더명(논리그룹)", "폴더생성", "");
            if (string.IsNullOrWhiteSpace(s))
                return;
            var fold = new 매크로항목
            {
                아이디 = Guid.NewGuid().ToString(),
                유형 = 항목유형.폴더,
                표시이름 = s,
                체크됨 = true
            };
            lock (_lockObj)
            {
                _목록.Add(fold);
            }
            SaveData();
            PopulateTreeView();
            Log("폴더생성 => " + s);
        }

        private void 선택삭제()
        {
            if (트리항목 is MultiSelectTreeView mTree)
            {
                foreach (var node in mTree.SelectedNodes)
                {
                    if (node.Tag is 매크로항목 data)
                    {
                        재귀삭제(data.아이디);
                    }
                }
                SaveData();
                PopulateTreeView();
                Log("선택삭제 완료");
            }
            else if (트리항목.SelectedNode != null)
            {
                if (트리항목.SelectedNode.Tag is 매크로항목 data)
                {
                    재귀삭제(data.아이디);
                    SaveData();
                    PopulateTreeView();
                    Log("삭제 => " + data.표시이름);
                }
            }
        }

        private void 재귀삭제(string id)
        {
            var t = _목록.Find(x => x.아이디 == id);
            if (t != null)
                _목록.Remove(t);
            var kids = _목록.FindAll(x => x.부모아이디 == id);
            foreach (var c in kids)
                재귀삭제(c.아이디);
        }

        private void 이름편집()
        {
            if (트리항목.SelectedNode == null)
                return;
            if (트리항목.SelectedNode.Tag is 매크로항목 mm)
            {
                var nn = Microsoft.VisualBasic.Interaction.InputBox("새이름", "이름편집", mm.표시이름);
                if (!string.IsNullOrEmpty(nn))
                {
                    mm.표시이름 = nn;
                    SaveData();
                    PopulateTreeView();
                    Log("이름변경 => " + nn);
                }
            }
        }

        private void 열기설정()
        {
            if (트리항목.SelectedNode == null)
                return;
            if (트리항목.SelectedNode.Tag is 매크로항목 data)
            {
                using var sf = new 설정폼(data);
                if (sf.ShowDialog() == DialogResult.OK)
                {
                    SaveData();
                    PopulateTreeView();
                    Log("설정변경 => " + data.표시이름);
                }
            }
        }

        private void ROI수정()
        {
            if (트리항목.SelectedNode == null)
                return;
            if (트리항목.SelectedNode.Tag is 매크로항목 mm && mm.유형 == 항목유형.파일)
            {
                if (_대상창 == IntPtr.Zero)
                {
                    Log("대상창 없음 => ROI수정 불가");
                    return;
                }
                if (!윈도우API.GetWindowRect(_대상창, out _창Rect))
                {
                    Log("GetWindowRect 실패 => ROI수정 불가");
                    return;
                }
                int ww = _창Rect.오른 - _창Rect.왼;
                int hh = _창Rect.아래 - _창Rect.위;
                if (ww <= 0 || hh <= 0)
                {
                    Log("대상창 크기 0 => ROI수정 불가");
                    return;
                }
                var bounding = new Rectangle(_창Rect.왼, _창Rect.위, ww, hh);
                this.Hide();
                var ov = new 오버레이ROI폼(bounding);
                ov.OnComplete += (roiSel) =>
                {
                    this.Show();
                    if (roiSel == null)
                    {
                        Log("ROI수정 취소");
                        return;
                    }
                    var rr = new Rectangle(
                        roiSel.Value.X - _창Rect.왼,
                        roiSel.Value.Y - _창Rect.위,
                        roiSel.Value.Width,
                        roiSel.Value.Height);
                    if (rr.Width <= 0 || rr.Height <= 0)
                    {
                        Log("ROI=0");
                        return;
                    }
                    // ROI 수정 시 수정된 ROI만 업데이트 (원본ROI는 그대로 유지)
                    mm.ROI.X = rr.X;
                    mm.ROI.Y = rr.Y;
                    mm.ROI.폭 = rr.Width;
                    mm.ROI.높이 = rr.Height;
                    mm.원본너비 = ww;
                    mm.원본높이 = hh;
                    mm.OriginalWindowSize = new Size(ww, hh);

                    SaveData();
                    PopulateTreeView();
                    Log($"ROI수정 => {mm.표시이름}, ROI=({rr})");
                };
                ov.ShowDialog();
            }
        }
    }
    #endregion
    #endregion
}
