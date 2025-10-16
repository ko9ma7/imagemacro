using System.Drawing;

namespace 이미지매크로
{
    public static class ResourceHelper
    {
        private static Bitmap? _defaultThumb;
        public static Bitmap DefaultThumb
        {
            get
            {
                if (_defaultThumb == null)
                {
                    var bmp = new Bitmap(48, 48);
                    using (var g = Graphics.FromImage(bmp))
                    {
                        g.Clear(Color.Red);
                    }
                    _defaultThumb = bmp;
                }
                return _defaultThumb;
            }
        }
    }
}
