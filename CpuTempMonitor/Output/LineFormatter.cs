using System;
using System.Text.Json;
using CpuTempMonitor.Cli;

namespace CpuTempMonitor.Output
{
    public static class LineFormatter
    {
        public static string Format(Options opts, DateTimeOffset timestamp, double tempC)
        {
            if (opts.Json)
            {
                var data = new
                {
                    ts = timestamp,
                    tempC = Math.Round(tempC, 1)
                };
                return JsonSerializer.Serialize(data);
            }
            else
            {
                // 2026-01-27T21:15:02.123Z, 56.2 C
                return $"{timestamp:O}, {tempC:F1} C";
            }
        }
    }
}
