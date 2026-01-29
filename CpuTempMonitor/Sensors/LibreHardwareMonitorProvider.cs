using System;
using System.Collections.Generic;
using System.Linq;
using LibreHardwareMonitor.Hardware;

namespace CpuTempMonitor.Sensors
{
    public class LibreHardwareMonitorProvider : ICpuTempProvider
    {
        private Computer? _computer;
        private IHardware? _cpu;
        private ISensor? _selectedSensor;
        private readonly SensorPicker _picker = new SensorPicker();

        public bool TryInitialize(out string diagnostic)
        {
            try
            {
                _computer = new Computer
                {
                    IsCpuEnabled = true,
                    IsGpuEnabled = false,
                    IsMemoryEnabled = false,
                    IsMotherboardEnabled = false,
                    IsControllerEnabled = false,
                    IsNetworkEnabled = false,
                    IsStorageEnabled = false
                };

                _computer.Open();

                _cpu = _computer.Hardware.FirstOrDefault(h => h.HardwareType == HardwareType.Cpu);

                if (_cpu == null)
                {
                    diagnostic = "No CPU hardware found. Are you running as Administrator? (required for hardware acccess)";
                    return false;
                }

                diagnostic = $"Found CPU: {_cpu.Name}";
                return true;
            }
            catch (Exception ex)
            {
                diagnostic = $"Initialization failed: {ex.Message}";
                return false;
            }
        }

        public bool TryReadTempC(out double tempC, out string diagnostic)
        {
            if (_cpu == null)
            {
                diagnostic = "Provider not initialized.";
                tempC = 0;
                return false;
            }

            try 
            {
                _cpu.Update();

                // If we haven't selected a sensor yet, or if we want to be dynamic
                if (_selectedSensor == null)
                {
                    _selectedSensor = _picker.SelectBestTempSensor(_cpu.Sensors);
                    
                    if (_selectedSensor == null)
                    {
                        var sensorNames = string.Join(", ", _cpu.Sensors.Select(s => $"{s.Name} ({s.SensorType})"));
                        diagnostic = $"No temperature sensors found on CPU. Available: {sensorNames}";
                        tempC = 0;
                        return false;
                    }
                }

                if (_selectedSensor.Value.HasValue)
                {
                    tempC = _selectedSensor.Value.Value;
                    diagnostic = $"Read from {_selectedSensor.Name}";
                    return true;
                }
                else
                {
                    diagnostic = $"Sensor {_selectedSensor.Name} returned null value";
                    tempC = 0;
                    return false;
                }
            }
            catch (Exception ex)
            {
                diagnostic = $"Read error: {ex.Message}";
                tempC = 0;
                return false;
            }
        }

        public IEnumerable<string> DumpSensors()
        {
            if (_cpu == null) return Enumerable.Empty<string>();
            _cpu.Update();
            return _cpu.Sensors.Select(s => $"{s.Name} [{s.SensorType}]: {s.Value}");
        }

        public void Dispose()
        {
            _computer?.Close();
            _computer = null;
        }
    }
}
