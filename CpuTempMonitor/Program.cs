using System;
using CpuTempMonitor.Cli;
using CpuTempMonitor.Monitoring;
using CpuTempMonitor.Output;
using CpuTempMonitor.Sensors;

namespace CpuTempMonitor
{
    class Program
    {
        static int Main(string[] args)
        {
            DiagnosticProgram.RunDump();
            return 0;
        }
    }
}
