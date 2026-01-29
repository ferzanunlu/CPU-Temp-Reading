using System;
using System.Globalization;

namespace CpuTempMonitor.Cli
{
    public class Options
    {
        public int IntervalMs { get; set; } = 1000;
        public bool Once { get; set; }
        public string? LogPath { get; set; }
        public double? Threshold { get; set; }
        public bool ExitOnThreshold { get; set; }
        public bool Json { get; set; }

        public static Options Parse(string[] args)
        {
            var options = new Options();
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i].ToLowerInvariant())
                {
                    case "--interval-ms":
                        if (i + 1 < args.Length && int.TryParse(args[i + 1], out int interval))
                        {
                            options.IntervalMs = interval;
                            i++;
                        }
                        break;
                    case "--once":
                        options.Once = true;
                        break;
                    case "--log":
                        if (i + 1 < args.Length)
                        {
                            options.LogPath = args[i + 1];
                            i++;
                        }
                        break;
                    case "--threshold":
                        if (i + 1 < args.Length && double.TryParse(args[i + 1], NumberStyles.Any, CultureInfo.InvariantCulture, out double threshold))
                        {
                            options.Threshold = threshold;
                            i++;
                        }
                        break;
                    case "--exit-on-threshold":
                        options.ExitOnThreshold = true;
                        break;
                    case "--json":
                        options.Json = true;
                        break;
                }
            }
            return options;
        }
    }
}
