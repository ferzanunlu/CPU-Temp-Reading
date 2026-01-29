using System.Collections.Generic;

namespace CpuTempMonitor.Sensors
{
    public interface ICpuTempProvider : IDisposable
    {
        bool TryInitialize(out string diagnostic);
        bool TryReadTempC(out double tempC, out string diagnostic);
        IEnumerable<string> DumpSensors();
    }
}
