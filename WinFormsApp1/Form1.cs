using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;


namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        // ������깳�Ӿ��
        private static IntPtr _hookID = IntPtr.Zero;

        private static Form1 _instance; // �洢����ʵ��


        // ��깳��ί��
        private static LowLevelMouseProc _proc = HookCallback;


        public Form1()
        {
            _instance = this;
            InitializeComponent();
        }

        [DllImport("C:\\Users\\songkun2\\source\\repos\\WinFormsApp1\\x64\\Debug\\textselection.dll", CallingConvention = CallingConvention.Cdecl)]

        public static extern IntPtr allocate_buffer(int size);

        [DllImport("C:\\Users\\songkun2\\source\\repos\\WinFormsApp1\\x64\\Debug\\textselection.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void getSelectionText(IntPtr buffer);

        [DllImport("C:\\Users\\songkun2\\source\\repos\\WinFormsApp1\\x64\\Debug\\textselection.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void free_buffer(IntPtr buffer);

        // ���� Rust DLL ����
        [DllImport("C:\\Users\\songkun2\\source\\repos\\WinFormsApp1\\rust_text_selection\\target\\debug\\rust_text_selection.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr get_text();

        [DllImport("C:\\Users\\songkun2\\source\\repos\\WinFormsApp1\\WinFormsApp1\\bin\\Debug\\net8.0-windows\\ScreenTranslation.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void st_Initialize();

        [DllImport("C:\\Users\\songkun2\\source\\repos\\WinFormsApp1\\WinFormsApp1\\bin\\Debug\\net8.0-windows\\ScreenTranslation.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr st_GetSelectionString(IntPtr hwnd);

   


        IntPtr _buffer = IntPtr.Zero;

        private void button1_Click(object sender, EventArgs e)
        {
            this.TopMost = true;
            // ��װȫ����깳��
            _hookID = SetHook(_proc);

            // �����ڴ�
            _buffer = allocate_buffer(1024);


            //string result = Marshal.PtrToStringAnsi( get_text());
            //label2.Text = result;

            st_Initialize();

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

        // ����¼��ص�����
        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                MouseMessages msg = (MouseMessages)wParam;
                switch (msg)
                {
                    case MouseMessages.WM_LBUTTONDOWN:
                        Console.WriteLine("����������");

                        break;
                    case MouseMessages.WM_LBUTTONUP:

                        //string result = Marshal.PtrToStringAnsi(get_text());

                        string result = Marshal.PtrToStringUTF8(st_GetSelectionString(IntPtr.Zero));

                        // �������
                        //getSelectionText(_instance._buffer);

                        // ��ȡ�ڴ�����
                        //string result = Marshal.PtrToStringUTF8(_instance._buffer);
                        Console.WriteLine("Buffer content: " + result);

                        _instance.label2.Text = result;

                        break;
                    case MouseMessages.WM_RBUTTONDOWN:
                        Console.WriteLine("����Ҽ�����");
                        break;
                    case MouseMessages.WM_MOUSEMOVE:
                        //Console.WriteLine("����ƶ�");


                        MSLLHOOKSTRUCT hookStruct = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);
                        int mouseX = hookStruct.pt.x;
                        int mouseY = hookStruct.pt.y;

                        // ���� UI �̸߳��� Label
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
            WM_LBUTTONUP = 0x0202,
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

            if (_buffer != IntPtr.Zero)
            {

                free_buffer(_buffer);
                _buffer = IntPtr.Zero;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}

