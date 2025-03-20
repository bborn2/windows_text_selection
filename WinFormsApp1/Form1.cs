using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        // 定义鼠标钩子句柄
        private static IntPtr _hookID = IntPtr.Zero;

        private static Form1 _instance; // 存储窗体实例
         

        // 鼠标钩子委托
        private static LowLevelMouseProc _proc = HookCallback;


        public Form1()
        {
            _instance = this;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.TopMost = true;
            // 安装全局鼠标钩子
            _hookID = SetHook(_proc);
        }

        private static IntPtr SetHook(LowLevelMouseProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_MOUSE_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        // 鼠标事件回调函数
        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                MouseMessages msg = (MouseMessages)wParam;
                switch (msg)
                {
                    case MouseMessages.WM_LBUTTONDOWN:
                        Console.WriteLine("鼠标左键按下");

                        break;
                    case MouseMessages.WM_RBUTTONDOWN:
                        Console.WriteLine("鼠标右键按下");
                        break;
                    case MouseMessages.WM_MOUSEMOVE:
                        Console.WriteLine("鼠标移动");


                        MSLLHOOKSTRUCT hookStruct = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);
                        int mouseX = hookStruct.pt.x;
                        int mouseY = hookStruct.pt.y;

                        // 调用 UI 线程更新 Label
                        if (_instance != null)
                        {
                            _instance.Invoke((MethodInvoker)(() =>
                            {
                                _instance.label1.Text = $"X: {mouseX}, Y: {mouseY}";
                            }));
                        }


                        break;
                }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        private const int WH_MOUSE_LL = 14;

        private enum MouseMessages
        {
            WM_LBUTTONDOWN = 0x0201,
            WM_RBUTTONDOWN = 0x0204,
            WM_MOUSEMOVE = 0x0200
        }


        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MSLLHOOKSTRUCT
        {
            public POINT pt;
        }

        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private void button2_Click(object sender, EventArgs e)
        {
            this.TopMost = false;

            UnhookWindowsHookEx(_hookID);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}

