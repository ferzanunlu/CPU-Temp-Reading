using System;
using System.IO;

namespace CpuTempMonitor.Output
{
    public class CsvLogger : IDisposable
    {
        private readonly StreamWriter _writer;
        private readonly object _lock = new object();

        public CsvLogger(string path)
        {
            bool exists = File.Exists(path);
            _writer = new StreamWriter(path, append: true)
            {
                AutoFlush = true
            };

            if (!exists)
            {
                lock (_lock)
                {
                    _writer.WriteLine("Timestamp,TempC");
                }
            }
        }

        public void Log(DateTimeOffset timestamp, double tempC)
        {
            lock (_lock)
            {
                _writer.WriteLine($"{timestamp:O},{tempC:F1}");
            }
        }

        public void Dispose()
        {
            _writer.Dispose();
        }
    }
}
