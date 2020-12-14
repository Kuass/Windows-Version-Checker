using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;

namespace Windows_Version_Checker
{
    class Program
    {
        static void Main(string[] args)
        {
            OperatingSystem os = Environment.OSVersion;
            Version v = os.Version;

            Console.WriteLine(GetWindowsVersion());
            Console.WriteLine("");

            string releaseId = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ReleaseId", "").ToString();
            Console.WriteLine("Release ID : " + releaseId);

            Console.WriteLine("디렉토리 경로 : " + Environment.CurrentDirectory); // 현재 작업 디렉터리의 정규화된 경로를 가져오거나 설정합니다.
            Console.WriteLine("프로세스종료코드 : " + Environment.ExitCode); // 프로세스의 종료 코드를 가져오거나 설정합니다.
            Console.WriteLine("NetBIOS 이름 : " + Environment.MachineName); // 이 로컬 컴퓨터의 NetBIOS 이름을 가져옵니다.
            Console.WriteLine("OS 버전1 : " + Environment.OSVersion.ToString()); // OSVersion
            Console.WriteLine("시스템디렉토리 : " + Environment.SystemDirectory); // 시스템 디렉터리의 정규화된 경로를 가져옵니다.
            Console.WriteLine("시스템걸린시간 : " + Environment.TickCount); // 시스템 시작 이후 경과 시간(밀리초)을 가져옵니다.
            Console.WriteLine("도메인이름 : " + Environment.UserDomainName); // 현재 사용자와 관련된 네트워크 도메인 이름을 가져옵니다.
            Console.WriteLine("운영체제이름 : " + Environment.UserName); // Windows 운영 체제에 현재 로그온한 사용자의 이름을 가져옵니다.
            Console.WriteLine("CLR 버전 : " + Environment.Version.ToString()); // 공용 언어 런타임의 주 번호, 보조 번호, 빌드 번호 및 수정 번호를 설명하는 Version 개체를 가져옵니다.
            Console.WriteLine("WorkingSet : " + Environment.WorkingSet); // 현재 사용자와 관련된 네트워크 도메인 이름을 가져옵니다.

            Console.WriteLine("플랫폼 : " + os.Platform);
            Console.WriteLine("서비스팩버전 : " + os.ServicePack);
            Console.WriteLine("Build : " + v.Build);
            Console.WriteLine("Revision : " + v.Revision);
        }

        [DllImport("kernel32.dll")]
        static extern IntPtr GetCurrentProcess();

        [DllImport("kernel32.dll")]
        static extern IntPtr GetModuleHandle(string moduleName);

        [DllImport("kernel32")]
        static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll")]
        static extern bool IsWow64Process(IntPtr hProcess, out bool wow64Process);

        public static bool Is64BitOperatingSystem()
        {
            // Check if this process is natively an x64 process. If it is, it will only run on x64 environments, thus, the environment must be x64.
            if (IntPtr.Size == 8)
                return true;
            // Check if this process is an x86 process running on an x64 environment.
            IntPtr moduleHandle = GetModuleHandle("kernel32");
            if (moduleHandle != IntPtr.Zero)
            {
                IntPtr processAddress = GetProcAddress(moduleHandle, "IsWow64Process");
                if (processAddress != IntPtr.Zero)
                {
                    bool result;
                    if (IsWow64Process(GetCurrentProcess(), out result) && result)
                        return true;
                }
            }
            // The environment must be an x86 environment.
            return false;
        }

        private static string HKLM_GetString(string key, string value)
        {
            try
            {
                RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(key);
                return registryKey?.GetValue(value).ToString() ?? String.Empty;
            }
            catch
            {
                return String.Empty;
            }
        }

        public static string GetWindowsVersion()
        {
            string osArchitecture;
            try
            {
                osArchitecture = Is64BitOperatingSystem() ? "64-bit" : "32-bit";
            }
            catch (Exception)
            {
                osArchitecture = "32/64-bit (Undetermined)";
            }
            string productName = HKLM_GetString(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName");
            string csdVersion = HKLM_GetString(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CSDVersion");
            string currentBuild = HKLM_GetString(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentBuild");
            if (!string.IsNullOrEmpty(productName))
            {
                return
                    $"{productName}{(!string.IsNullOrEmpty(csdVersion) ? " " + csdVersion : String.Empty)} {osArchitecture} (OS Build {currentBuild})";
            }
            return String.Empty;
        }
    }
}
