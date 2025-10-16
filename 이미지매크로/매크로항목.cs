using System.Collections.Generic;
using System.Drawing;

namespace 이미지매크로
{
    public enum 항목유형
    {
        폴더,
        파일
    }

    public enum 색상모드
    {
        컬러,
        그레이스케일
    }

    public class 영역정보
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int 폭 { get; set; }
        public int 높이 { get; set; }
    }

    public class 액션설정
    {
        public string 동작종류 { get; set; } = "Default";
        public string 마우스동작종류 { get; set; } = "LeftClick";
        public bool 상대좌표 { get; set; } = true;
        public int 절대X { get; set; }
        public int 절대Y { get; set; }
        public int 상대X { get; set; }
        public int 상대Y { get; set; }
        public bool 랜덤옵셋 { get; set; } = true;
        public int 랜덤범위 { get; set; } = 10;
        public int 휠값 { get; set; } = 0;
        public List<string> 키목록 { get; set; } = new List<string>();
        public string 녹화핫키 { get; set; } = "";
        public List<string> 녹화액션들 { get; set; } = new List<string>();
        public float 임계값 { get; set; } = 0.80f;
        public 색상모드 색상모드 { get; set; } = 색상모드.컬러;
        public bool 회전허용 { get; set; } = true;
        public bool 리사이즈허용 { get; set; } = true;
        public bool 액션활성 { get; set; } = true;
        public string? SendMsg_WindowTitle { get; set; } = "";
        public string? SendMsg_ChildClass { get; set; } = "";
        public string? SendMsg_ChildText { get; set; } = "";
        public bool 이미지찾기활성 { get; set; } = true;
    }

    public class 매크로항목
    {
        public string 아이디 { get; set; } = "";
        public string 부모아이디 { get; set; } = "";
        public 항목유형 유형 { get; set; } = 항목유형.파일;
        public string 표시이름 { get; set; } = "";
        public bool 체크됨 { get; set; } = true;
        public string 파일경로 { get; set; } = "";
        public 영역정보 ROI { get; set; } = new 영역정보();
        public 영역정보 원본ROI { get; set; } = new 영역정보();
        public int 원본너비 { get; set; }
        public int 원본높이 { get; set; }
        public float 항목임계값 { get; set; } = 0.95f;
        public 액션설정 설정 { get; set; } = new 액션설정();
        public Size OriginalWindowSize { get; set; } = Size.Empty;
        public string? Description { get; set; } = "";
    }
}
