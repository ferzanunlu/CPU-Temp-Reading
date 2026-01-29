using System;
using System.Collections.Generic;
using System.Linq;
using LibreHardwareMonitor.Hardware;

namespace CpuTempMonitor.Sensors
{
    public class SensorPicker
    {
        public ISensor? SelectBestTempSensor(IEnumerable<ISensor> sensors)
        {
            var candidates = sensors
                .Where(s => s.SensorType == SensorType.Temperature && s.Value.HasValue)
                .ToList();

            if (!candidates.Any())
                return null;

            // Heuristic 1: Prefer names containing Package, Tctl, Tdie
            // Heuristic 2: 'CPU Package' indicates high scope
            // Tie-breaker: Name (for determinism)

             var best = candidates
                .OrderByDescending(s => GetScore(s.Name))
                .ThenBy(s => s.Name, StringComparer.OrdinalIgnoreCase) // Deterministic tie-breaker
                .FirstOrDefault();

             return best;
        }

        private int GetScore(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return 0;
            
            int score = 0;
            string upper = name.ToUpperInvariant();

            if (upper.Contains("PACKAGE") || upper.Contains("TCTL") || upper.Contains("TDIE"))
                score += 100;
            
            // "CPU Package" is very specific and usually the one we want
            if (upper.Contains("CPU PACKAGE"))
                score += 50;

            // Core should be lower priority than package
            if (upper.Contains("CORE"))
                score -= 10;

            return score;
        }
    }
}
