using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Threading;

namespace Excmd
{
    class Program
    {
	    const int numIterations = 100;
	    private static int ctr = 0;
	    const bool captureErrors = true;
        static void Main(string[] args)
        {
	        File.WriteAllText("excmd.cmd"
		      ,$"@echo off{Environment.NewLine}"
		       + $"echo aaa{Environment.NewLine}"
		       + $"ping 192.168.0.255 -n 1 -w 5 >NUL{Environment.NewLine}"
	          + $"exit 0");
	        for (int ii = 0; ii < numIterations; ii++)
	        {
		        DoIt();
	        }
        }

	    private static void DoIt()
	    {
		    ctr++;
		    ProcessStartInfo psi = new ProcessStartInfo("cmd.exe");
		    psi.UseShellExecute = false;
		    psi.ArgumentList.Add("/K");
		    psi.ArgumentList.Add("excmd.cmd");
		    psi.RedirectStandardOutput = true;
		    if (captureErrors) psi.RedirectStandardError = true;
		    List<string> output = new List<string>();
		    List<string> errs = new List<string>();
		    using (var ps = Process.Start(psi))
		    {
			    ps.OutputDataReceived += (sender, e) => output.Add(e.Data);
			    if (captureErrors) ps.ErrorDataReceived += (sender, e) => errs.Add(e.Data);
			    ps.BeginOutputReadLine();
			    if (captureErrors) ps.BeginErrorReadLine();
			    ps.WaitForExit(5000);
		    }

/*
		    for (int jj = 0; jj < 2; jj++)
		    {
			    Console.WriteLine(output.Count);
			    Thread.Sleep(200);
		    }
*/
		    if (output.Count < 2) Console.WriteLine($"{ctr:000}: captured {output.Count} stdout lines from child process");
//		    Console.WriteLine(output[0]);
	    }
    }
}
