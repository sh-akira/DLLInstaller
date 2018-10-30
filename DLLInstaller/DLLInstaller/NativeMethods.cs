using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DLLInstaller
{
    public static class NativeMethods
    {
        /// <summary>
        /// メッセージボックスのオプション
        /// </summary>
        [Flags]
        public enum MessageBoxOptions : uint
        {
            OkOnly = 0x000000,
            OkCancel = 0x000001,
            AbortRetryIgnore = 0x000002,
            YesNoCancel = 0x000003,
            YesNo = 0x000004,
            RetryCancel = 0x000005,
            CancelTryContinue = 0x000006,
            IconHand = 0x000010,
            IconQuestion = 0x000020,
            IconExclamation = 0x000030,
            IconAsterisk = 0x000040,
            UserIcon = 0x000080,
            IconWarning = IconExclamation,
            IconError = IconHand,
            IconInformation = IconAsterisk,
            IconStop = IconHand,
            DefButton1 = 0x000000,
            DefButton2 = 0x000100,
            DefButton3 = 0x000200,
            DefButton4 = 0x000300,
            ApplicationModal = 0x000000,
            SystemModal = 0x001000,
            TaskModal = 0x002000,
            Help = 0x004000,
            NoFocus = 0x008000,
            SetForeground = 0x010000,
            DefaultDesktopOnly = 0x020000,
            Topmost = 0x040000,
            Right = 0x080000,
            RTLReading = 0x100000
        }

        /// <summary>
        /// メッセージボックスの表示
        /// </summary>
        /// <param name="hWnd">ハンドル or IntPtr.Zero</param>
        /// <param name="text">内容</param>
        /// <param name="caption">タイトル</param>
        /// <param name="options">オプション</param>
        /// <returns></returns>
        [DllImport("User32.dll", CharSet = CharSet.Unicode)]
        public static extern int MessageBox(IntPtr hWnd, string text, string caption, MessageBoxOptions options);
        
        /// <summary>
        /// COM DLLが持つ DllRegisterServer または DllUnregisterServer 関数を
        /// 受けるためのデリゲート
        /// </summary>
        /// <returns>0:成功</returns>
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int DllRegisterDelegate();
        
        /// <summary>
        /// DLLの読込
        /// </summary>
        /// <param name="lpFileName">ファイルパス</param>
        /// <returns>モジュールハンドル</returns>
        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)]string lpFileName);

        /// <summary>
        /// 読み込んだDLLの開放
        /// </summary>
        /// <param name="hModule">モジュールハンドル</param>
        /// <returns>成否</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FreeLibrary(IntPtr hModule);

        /// <summary>
        /// 関数アドレスの取得
        /// </summary>
        /// <param name="hModule">モジュールハンドル</param>
        /// <param name="procName">関数名</param>
        /// <returns>関数へのポインタ 失敗時IntPtr.Zero</returns>
        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        /// <summary>
        /// 起動されたコンソールを取得
        /// </summary>
        /// <param name="dwProcessId"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        public static extern bool AttachConsole(uint dwProcessId);

        /// <summary>
        /// アタッチしたコンソールを開放
        /// </summary>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        public static extern bool FreeConsole();
    }
}
