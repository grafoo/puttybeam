using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace PuTTYbeam {
    class NetworkScanner {
        public string getHostname(string subnet, int i) {
            Ping ping = new Ping();
            string result = "";
            string ipAddress = subnet + "." + i.ToString();
            if (ping.Send(ipAddress).Status == IPStatus.Success) {
                try {
                    IPHostEntry hostEntry = Dns.GetHostEntry(ipAddress);
                    result = hostEntry.HostName.Split('.')[0];
                } catch(Exception) {
                    result = ipAddress;
                }
            }
            return result;
        }
    }

    class SessionManager {
        private string puttySessionsRegPath;
        private string puttySessionDefaultRegKeyPath;

        public SessionManager() {
            puttySessionsRegPath = "Software\\SimonTatham\\PuTTY\\Sessions";
            puttySessionDefaultRegKeyPath = puttySessionsRegPath + "\\Default%20Settings";
        }

        public void storeSession(string subnet, int i, string username) {
            NetworkScanner netScanr = new NetworkScanner();
            string hostName = netScanr.getHostname(subnet, i);
            if(hostName != "") {
                Registry.CurrentUser.CreateSubKey(puttySessionsRegPath + "\\" + hostName);
                RegistryKey newSessionRegKey = Registry.CurrentUser.OpenSubKey(puttySessionsRegPath + "\\" + hostName, true);
                RegistryKey puttySessionDefaultRegKey = Registry.CurrentUser.OpenSubKey(puttySessionDefaultRegKeyPath);
                foreach(var valueName in puttySessionDefaultRegKey.GetValueNames()) {
                    newSessionRegKey.SetValue(valueName, puttySessionDefaultRegKey.GetValue(valueName));
                }
                newSessionRegKey.SetValue("HostName", hostName);
                newSessionRegKey.SetValue("UserName", username);
                newSessionRegKey.SetValue("Colour0", "131,148,150");
                newSessionRegKey.SetValue("Colour1", "147,161,161");
                newSessionRegKey.SetValue("Colour2", "0,43,54");
                newSessionRegKey.SetValue("Colour3", "7,54,66");
                newSessionRegKey.SetValue("Colour4", "0,43,54");
                newSessionRegKey.SetValue("Colour5", "238,232,213");
                newSessionRegKey.SetValue("Colour6", "7,54,66");
                newSessionRegKey.SetValue("Colour7", "0,43,56");
                newSessionRegKey.SetValue("Colour8", "220,50,47");
                newSessionRegKey.SetValue("Colour9", "203,75,22");
                newSessionRegKey.SetValue("Colour10", "133,153,0");
                newSessionRegKey.SetValue("Colour11", "88,110,117");
                newSessionRegKey.SetValue("Colour12", "181,137,0");
                newSessionRegKey.SetValue("Colour13", "101,123,131");
                newSessionRegKey.SetValue("Colour14", "38,139,210");
                newSessionRegKey.SetValue("Colour15", "131,148,150");
                newSessionRegKey.SetValue("Colour16", "211,54,130");
                newSessionRegKey.SetValue("Colour17", "108,113,196");
                newSessionRegKey.SetValue("Colour18", "42,161,152");
                newSessionRegKey.SetValue("Colour19", "147,161,161");
                newSessionRegKey.SetValue("Colour20", "238,232,213");
                newSessionRegKey.SetValue("Colour21", "253,246,227");
                newSessionRegKey.Close();
                puttySessionDefaultRegKey.Close();
                Console.WriteLine(String.Format("Added new Session for Host {0}", hostName));
            }
        }

        public void deleteSessions() {
            RegistryKey puttySessionsRegKey = Registry.CurrentUser.OpenSubKey(puttySessionsRegPath, true);
            foreach(string sessionRegKeyName in puttySessionsRegKey.GetSubKeyNames()) {
                puttySessionsRegKey.DeleteSubKey(sessionRegKeyName);
            }
            puttySessionsRegKey.Close();
        }
    }

    class Cli {
        static int Main(string[] args) {
            string target = args[0];
            SessionManager sesnMgr = new SessionManager();
            switch(target) {
                case "store":
                    string subnet = args[1];
                    int subnet_start = Convert.ToInt16(args[2]);
                    int subnet_end = Convert.ToInt16(args[3]);
                    string username = args[4];
                    Parallel.For(subnet_start, subnet_end, i => sesnMgr.storeSession(subnet, i, username));
                    break;
                case "clean":
                    sesnMgr.deleteSessions();
                    break;
                default:
                    Console.WriteLine("puttybeamcli.exe {store <subnet> <start> <end> <username> | clean}");
                    break;
            }
            return 0;
        }
    }
}
