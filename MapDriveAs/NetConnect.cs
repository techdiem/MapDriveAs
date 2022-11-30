using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Net;
using System.Text;

namespace MapDriveAs
{
    internal class NetConnect
    {
        //https://learn.microsoft.com/en-us/windows/win32/api/winnetwk/nf-winnetwk-wnetaddconnection2a

        [StructLayout(LayoutKind.Sequential)]
        public class NETRESOURCE
        {
            public int dwScope;
            public int dwType;
            public int dwDisplayType;
            public int dwUsage;
            public string LocalName;
            public string RemoteName;
            public string Comment;
            public string Provider;
        }

        [DllImport("mpr.dll")]
        public static extern int WNetAddConnection2(NETRESOURCE netResource, string password, string username, int flags);

        [DllImport("mpr.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int WNetGetConnection([MarshalAs(UnmanagedType.LPTStr)] string localName, [MarshalAs(UnmanagedType.LPTStr)] StringBuilder remoteName, ref int length);

        [DllImport("mpr.dll")]
        static extern int WNetCancelConnection2(string lpName, int dwFlags, bool bForce);


        public static int MapNetworkDrive(string letter, string remoteName, NetworkCredential credentials)
        {
            string userName = string.IsNullOrEmpty(credentials.Domain)
                ? credentials.UserName
                : string.Format(@"{0}\{1}", credentials.Domain, credentials.UserName);

            NETRESOURCE myResource = new NETRESOURCE();
            myResource.dwScope = 0;
            myResource.dwType = 0; //RESOURCETYPE_ANY
            myResource.dwDisplayType = 0;
            myResource.LocalName = letter;
            myResource.RemoteName = @remoteName;
            myResource.dwUsage = 0;
            myResource.Comment = "";
            myResource.Provider = "";

            int returnValue = WNetAddConnection2(myResource, credentials.Password, userName, 0);
            return returnValue;
        }

        public static int UnmountNetworkDrive(string letter)
        {
            int returnValue = WNetCancelConnection2(letter, 0, false);
            return returnValue;
        }

        public static bool GetConnectionStatus([MarshalAs(UnmanagedType.LPTStr)] string letter) {
            int bufferLenght = 260;
            StringBuilder remoteName = new StringBuilder(bufferLenght);
            int dwResult;
            dwResult = WNetGetConnection(letter, remoteName, ref bufferLenght);
            //Error Code 0 = NO_ERROR -> drive is mapped
            return (dwResult == 0);
        }

        public static void ConnectAs(string letter, string sharePath)
        {
            int errorCode = -1;
            while (errorCode != 0)
            {

                NetworkCredential cred = CredentialHelper.ShowCredentialPrompt(errorCode);
                if (cred == null)
                {
                    Environment.Exit(0);
                }
                errorCode = NetConnect.MapNetworkDrive(letter, sharePath, cred);
            }

            Environment.Exit(0);
        }

    }
}
