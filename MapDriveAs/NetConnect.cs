using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Net;

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
