using System;
using System.IO;
using System.Threading;

public class LockFile
{
    public static void Main()
    {
        var f = new FileStream("logs-20180730.txt", FileMode.Open, FileAccess.Read, FileShare.None, 4096, false);
        Thread.Sleep(60000);
    }
}