using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DLLInstaller
{
    class Program
    {
        /// <summary>
        /// 使用方法出力
        /// </summary>
        private static void DisplayUsage()
        {
            Console.WriteLine("Usage : DLLInstaller [/i or /u] [/s] [DLL file path].");
            Console.WriteLine("    /i Install");
            Console.WriteLine("    /u Uninstall");
            Console.WriteLine("    /s Silent (Disable result message box)");
            Console.WriteLine("Example :");
            Console.WriteLine("DLLInstaller /i C:\\comapi.dll");
        }

        /// <summary>
        /// 実行時引数チェック
        /// </summary>
        /// <param name="args">Main args</param>
        /// <returns>パース結果</returns>
        private static (bool success, bool isInstall, bool isSilent, string dllPath) CheckArguments(string[] args)
        {
            bool success = true;
            bool isInstall = true;
            bool isSilent = false;
            string dllPath = null;
            if (args.Length < 2)
            {
                success = false;
            }
            else
            {
                foreach (var arg in args)
                {
                    switch (arg)
                    {
                        case "/i":  // インストール
                            isInstall = true;
                            break;
                        case "/u":  // アンインストール
                            isInstall = false;
                            break;
                        case "/s":  // サイレント
                            isSilent = true;
                            break;
                        default:    // ファイルパス
                            if (dllPath == null)
                                dllPath = arg;
                            else    // 引数が多すぎる                            
                                success = false;
                            break;
                    }
                }
                if (args[0] == "/i")
                {
                    isInstall = true;
                }
                else if (args[0] == "/u")
                {
                    isInstall = false;
                }
                else
                {
                    Console.WriteLine($"Unknown flag:{args[0]}");
                    success = false;
                }
            }
            return (success, isInstall, isSilent, dllPath);
        }

        /// <summary>
        /// メッセージ表示
        /// </summary>
        /// <param name="text"></param>
        private static void MessageBox(string text)
        {
            NativeMethods.MessageBox(IntPtr.Zero, text, "インストーラ", NativeMethods.MessageBoxOptions.IconInformation);
        }

        /// <summary>
        /// DLL登録処理
        /// </summary>
        /// <param name="args">実行時引数</param>
        /// <returns>実行結果</returns>
        private static int Execute(string[] args)
        {
            // 引数チェック
            var (success, isInstall, isSilent, dllPath) = CheckArguments(args);

            // 引数とdllファイル存在確認
            if (success == false)
            {
                DisplayUsage();
                return -1;
            }

            //ファイル存在確認
            if (File.Exists(dllPath) == false)
            {
                Console.WriteLine("Dll file not found");
                return -1;
            }

            // 管理者権限か確認
            if (AdminExecute.IsAdmin == false)
            {
                var (successExecute, exitCode) = AdminExecute.RestartAsAdmin(args);
                if (successExecute == false)
                {
                    Console.WriteLine("Administrator is required for installation");
                    return -1;
                }
                //管理者権限でなかった場合、管理者権限で実行したプロセスの結果を返して終わる
                return exitCode;
            }

            // DLL読み込み
            var moduleHandle = NativeMethods.LoadLibrary(dllPath); ;

            if (moduleHandle == IntPtr.Zero)
            {
                Console.WriteLine($"Failed to load DLL : {dllPath}");
                DisplayUsage();
                return -1;
            }

            // DLLから登録/解除の関数ポインタ取得
            var functionPointer = NativeMethods.GetProcAddress(moduleHandle, isInstall ? "DllRegisterServer" : "DllUnregisterServer");

            if (functionPointer == IntPtr.Zero)
            {
                Console.WriteLine("Failed to get reg/unreg function from DLL");
                return -1;
            }

            // 関数ポインタをデリゲートに変換して実行できるようにする
            var dllRegisterDelegate = Marshal.GetDelegateForFunctionPointer(functionPointer, typeof(NativeMethods.DllRegisterDelegate)) as NativeMethods.DllRegisterDelegate;

            // デリゲートを実行する
            var hResult = dllRegisterDelegate();

            //メッセージ抑制モードでなければ
            if (isSilent == false)
            {
                if (hResult == 0)
                {
                    MessageBox(isInstall ? "インストール成功" : "アンインストール成功");
                }
                else
                {
                    MessageBox(isInstall ? "インストール失敗" : "アンインストール失敗");
                }
            }

            // 読み込んだDLLを開放
            NativeMethods.FreeLibrary(moduleHandle);

            return hResult;
        }

        /// <summary>
        /// エントリポイント
        /// </summary>
        /// <param name="args">実行時引数</param>
        [STAThread]
        private static int Main(string[] args)
        {
            // コンソールから起動した場合にログ出力
            StreamWriter standard = null;
            var isOnConsole = NativeMethods.AttachConsole(uint.MaxValue);
            if (isOnConsole)
            {
                // 標準出力ストリームを作成
                standard = new StreamWriter(Console.OpenStandardOutput())
                {
                    AutoFlush = true
                };
                Console.SetOut(standard);
                Console.WriteLine("");
            }
            try
            {
                return Execute(args);
            }
            finally
            {
                // コンソール出力していた場合は開放する
                if (isOnConsole)
                {
                    standard.Dispose();
                    NativeMethods.FreeConsole();
                }
            }
        }
    }
}