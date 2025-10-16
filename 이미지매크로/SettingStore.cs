using System;
using System.Drawing;
using System.IO;
using Newtonsoft.Json;

namespace 이미지매크로
{
    public static class SettingStore
    {
        private static readonly string _cfgFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settingStore.json");

        public static Point MainFormLocation = new Point(100, 100);
        public static Size MainFormSize = new Size(1200, 700);
        public static bool LastActiveMode = false;
        public static bool LastCaptureOnce = true;
        public static int LastDelay = 1000;
        public static string LastWindowTitle = "";

        public static void Load()
        {
            if (File.Exists(_cfgFile))
            {
                try
                {
                    var json = File.ReadAllText(_cfgFile);
                    var obj = JsonConvert.DeserializeObject<SettingData>(json);
                    if (obj != null)
                    {
                        MainFormLocation = new Point(obj.MainX, obj.MainY);
                        MainFormSize = new Size(obj.MainW, obj.MainH);
                        LastActiveMode = obj.LastActiveMode;
                        LastCaptureOnce = obj.LastCaptureOnce;
                        LastDelay = obj.LastDelay;
                        LastWindowTitle = obj.LastWindowTitle ?? "";
                    }
                }
                catch { }
            }
        }

        public static void Save()
        {
            var obj = new SettingData()
            {
                MainX = MainFormLocation.X,
                MainY = MainFormLocation.Y,
                MainW = MainFormSize.Width,
                MainH = MainFormSize.Height,
                LastActiveMode = LastActiveMode,
                LastCaptureOnce = LastCaptureOnce,
                LastDelay = LastDelay,
                LastWindowTitle = LastWindowTitle
            };
            var json = JsonConvert.SerializeObject(obj, Formatting.Indented);
            File.WriteAllText(_cfgFile, json);
        }

        class SettingData
        {
            public int MainX;
            public int MainY;
            public int MainW;
            public int MainH;
            public bool LastActiveMode;
            public bool LastCaptureOnce;
            public int LastDelay;
            public string? LastWindowTitle;
        }
    }
}
