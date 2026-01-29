using System;
using System.Threading;
using System.Threading.Tasks;
using CpuTempMonitor.Cli;
using CpuTempMonitor.Output;
using CpuTempMonitor.Sensors;

namespace CpuTempMonitor.Monitoring
{
    public static class MonitorLoop
    {
        public static int Run(Options opts, ICpuTempProvider provider)
        {
            using var cts = new CancellationTokenSource();
            
            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
            };

            var logger = !string.IsNullOrEmpty(opts.LogPath) ? new CsvLogger(opts.LogPath) : null;
            
            try
            {
                while (!cts.Token.IsCancellationRequested)
                {
                    if (provider.TryReadTempC(out var temp, out var diag))
                    {
                        var now = DateTimeOffset.UtcNow;
                        var line = LineFormatter.Format(opts, now, temp);
                        Console.WriteLine(line);

                        logger?.Log(now, temp);

                        if (opts.Threshold.HasValue && temp >= opts.Threshold.Value)
                        {
                            Console.Error.WriteLine($"WARNING: Temperature {temp:F1} C exceeds threshold {opts.Threshold.Value} C");
                            if (opts.ExitOnThreshold)
                            {
                                return 3;
                            }
                        }
                    }
                    else
                    {
                        Console.Error.WriteLine($"Read Error: {diag}");
                    }

                    // Precise wait is overkill, plain delay is fine for 1s
                    // Handling cancellation during delay
                    if (cts.Token.WaitHandle.WaitOne(opts.IntervalMs))
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                 Console.Error.WriteLine($"Fatal loop error: {ex.Message}");
                 return 1;
            }
            finally
            {
                logger?.Dispose();
            }

            return 0;
        }
    }
}
