using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Net;
using System.Configuration;

namespace MapDriveAs
{
    class CredentialHelper
    {
        //https://learn.microsoft.com/en-us/windows/win32/api/wincred/nf-wincred-creduipromptforwindowscredentialsa

        [DllImport("ole32.dll")]
        public static extern void CoTaskMemFree(IntPtr ptr);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct CREDUI_INFO
        {
            public int cbSize;
            public IntPtr hwndParent;
            public string pszMessageText;
            public string pszCaptionText;
            public IntPtr hbmBanner;
        }


        [DllImport("credui.dll", CharSet = CharSet.Auto)]
        private static extern bool CredUnPackAuthenticationBuffer(int dwFlags,
                                                                   IntPtr pAuthBuffer,
                                                                   uint cbAuthBuffer,
                                                                   StringBuilder pszUserName,
                                                                   ref int pcchMaxUserName,
                                                                   StringBuilder pszDomainName,
                                                                   ref int pcchMaxDomainame,
                                                                   StringBuilder pszPassword,
                                                                   ref int pcchMaxPassword);

        [DllImport("credui.dll", CharSet = CharSet.Auto)]
        private static extern int CredUIPromptForWindowsCredentials(ref CREDUI_INFO notUsedHere,
                                                                     int authError,
                                                                     ref uint authPackage,
                                                                     IntPtr InAuthBuffer,
                                                                     uint InAuthBufferSize,
                                                                     out IntPtr refOutAuthBuffer,
                                                                     out uint refOutAuthBufferSize,
                                                                     ref bool fSave,
                                                                     int flags);



        public static NetworkCredential ShowCredentialPrompt(int errorCode = 0)
        {
            CREDUI_INFO credui = new CREDUI_INFO();
            credui.pszCaptionText = "Benutzerkonto anmelden";
            credui.pszMessageText = "Benutzerkonto zum Verbinden des Netzlaufwerkes angeben";
            credui.cbSize = Marshal.SizeOf(credui);
            uint authPackage = 0;
            IntPtr outCredBuffer = new IntPtr();
            uint outCredSize;
            bool save = false;
            int result = CredUIPromptForWindowsCredentials(ref credui,
                                                           errorCode,
                                                           ref authPackage,
                                                           IntPtr.Zero,
                                                           0,
                                                           out outCredBuffer,
                                                           out outCredSize,
                                                           ref save,
                                                           1 /* Generic */);

            var usernameBuf = new StringBuilder(100);
            var passwordBuf = new StringBuilder(100);
            var domainBuf = new StringBuilder(100);

            int maxUserName = 100;
            int maxDomain = 100;
            int maxPassword = 100;
            if (result == 0)
            {
                if (CredUnPackAuthenticationBuffer(0, outCredBuffer, outCredSize, usernameBuf, ref maxUserName,
                                                   domainBuf, ref maxDomain, passwordBuf, ref maxPassword))
                {

                    //clear the memory allocated by CredUIPromptForWindowsCredentials 
                    CoTaskMemFree(outCredBuffer);
                    NetworkCredential networkCredential = ReadConfiguredDefaults();
                    networkCredential.UserName = usernameBuf.ToString();
                    networkCredential.Password = passwordBuf.ToString();

                    //Extract domain from username
                    string[] login = usernameBuf.ToString().Split('\\');
                    if (login.Length == 2)
                    {
                        networkCredential.Domain = login[0];
                        networkCredential.UserName = login[1];
                    }

                    return networkCredential;
                }
            }

            return null;
        }


        private static NetworkCredential ReadConfiguredDefaults()
        {
            NetworkCredential cred = new NetworkCredential();

            string defDomain = ConfigurationManager.AppSettings["DefaultDomain"];

            if (defDomain != "")
            {
                cred.Domain = defDomain;
            }

            return cred;
        }
    }
}
