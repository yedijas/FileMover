using Microsoft.Win32.SafeHandles;
using System;
using System.Configuration;
using System.IO;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Principal;

namespace FileMoverLib
{
    public class FileMover
    {
        static SafeTokenHandle safeTokenHandle;

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool LogonUser(String lpszUsername, String lpszDomain, String lpszPassword,
        int dwLogonType, int dwLogonProvider, out SafeTokenHandle phToken);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public extern static bool CloseHandle(IntPtr handle);

        public static bool Impersonate(string domainName, string userName, string password)
        {
            bool returnValue = false;
            try
            {
                const int LOGON32_PROVIDER_DEFAULT = 0;
                const int LOGON32_LOGON_INTERACTIVE = 2;

                returnValue = LogonUser(userName, domainName, password,
                    LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT,
                    out safeTokenHandle);

                if (false == returnValue)
                {
                    int ret = Marshal.GetLastWin32Error();
                    throw new System.ComponentModel.Win32Exception(ret);
                }
                using (safeTokenHandle)
                {
                    using (WindowsIdentity newId = new WindowsIdentity(safeTokenHandle.DangerousGetHandle()))
                    {
                        using (WindowsImpersonationContext impersonatedUser = newId.Impersonate())
                        {
                            // part where user impersonated
                            Console.WriteLine("After impersonation: "
                                + WindowsIdentity.GetCurrent().Name);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return returnValue;
            }

            return returnValue;
        }

        public static bool PerformCopy(string input, string output)
        {
            bool returnValue = false;
            try
            {
                if (false == File.Exists(input))
                {
                    throw new FileNotFoundException();
                }
                File.Copy(input, output, true);
                if (File.Exists(output))
                {
                    returnValue = true;
                }
            }
            catch (Exception ex)
            {
                returnValue = false;
            }
            return returnValue;
        }

        public static bool PerformMove(string input, string output)
        {
            bool returnValue = false;
            try
            {
                if (false == File.Exists(input))
                {
                    throw new FileNotFoundException();
                }
                File.Copy(input, output, true); // using combination of copy and delete because move cannot replace the file.
                File.Delete(input);
                if (File.Exists(output))
                {
                    returnValue = true;
                }
            }
            catch (Exception ex)
            {
                returnValue = false;
            }
            return returnValue;
        }

        public static bool ImpersonateAndCopy(string domainName, string userName, string password, string input, string output)
        {
            bool returnValue = false;
            try
            {
                const int LOGON32_PROVIDER_DEFAULT = 0;
                const int LOGON32_LOGON_INTERACTIVE = 2;

                returnValue = LogonUser(userName, domainName, password,
                    LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT,
                    out safeTokenHandle);

                if (false == returnValue)
                {
                    int ret = Marshal.GetLastWin32Error();
                    throw new System.ComponentModel.Win32Exception(ret);
                }
                using (safeTokenHandle)
                {
                    using (WindowsIdentity newId = new WindowsIdentity(safeTokenHandle.DangerousGetHandle()))
                    {
                        using (WindowsImpersonationContext impersonatedUser = newId.Impersonate())
                        {
                            // loop all files in the source folder
                            foreach(string singleFile in Directory.GetFiles(input))
                            {
                                string singleFileName = Path.GetFileName(singleFile);
                                PerformCopy(singleFile, output + @"\" + singleFileName);
                            }
                            returnValue = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occurred. " + ex.Message);
                return returnValue;
            }

            return returnValue;
        }

        public static bool ImpersonateAndMove(string domainName, string userName, string password, string input, string output)
        {
            bool returnValue = false;
            try
            {
                const int LOGON32_PROVIDER_DEFAULT = 0;
                const int LOGON32_LOGON_INTERACTIVE = 2;

                returnValue = LogonUser(userName, domainName, password,
                    LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT,
                    out safeTokenHandle);

                if (false == returnValue)
                {
                    int ret = Marshal.GetLastWin32Error();
                    throw new System.ComponentModel.Win32Exception(ret);
                }
                using (safeTokenHandle)
                {
                    using (WindowsIdentity newId = new WindowsIdentity(safeTokenHandle.DangerousGetHandle()))
                    {
                        using (WindowsImpersonationContext impersonatedUser = newId.Impersonate())
                        {
                            // loop all files in the source folder
                            foreach (string singleFile in Directory.GetFiles(input))
                            {
                                string singleFileName = Path.GetFileName(singleFile);
                                PerformMove(singleFile, output + @"\" + singleFileName);
                            }
                            returnValue = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occurred. " + ex.Message);
                return returnValue;
            }

            return returnValue;
        }
    }
    public sealed class SafeTokenHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private SafeTokenHandle()
            : base(true)
        {
        }

        [DllImport("kernel32.dll")]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr handle);

        protected override bool ReleaseHandle()
        {
            return CloseHandle(handle);
        }
    }
}
