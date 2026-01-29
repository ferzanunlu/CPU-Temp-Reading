using System;
using System.Linq;
using LibreHardwareMonitor.Hardware;

namespace CpuTempMonitor
{
    class DiagnosticProgram
    {
        // Standalone diagnostic entry point (will replace Main temporarily or be run separately)
        public static void RunDump()
        {
            Console.WriteLine("=== DIAGNOSTIC HARDWARE DUMP ===");
            
            var computer = new Computer
            {
                IsCpuEnabled = true,
                IsGpuEnabled = true,
                IsMemoryEnabled = true,
                IsMotherboardEnabled = true,
                IsControllerEnabled = true,
                IsNetworkEnabled = true,
                IsStorageEnabled = true
            };

            try
            {
                computer.Open();
                computer.Accept(new UpdateVisitor());

                foreach (var hardware in computer.Hardware)
                {
                    Console.WriteLine($"Hardware: {hardware.Name} [{hardware.HardwareType}]");
                    
                    // Sub-hardware (e.g. SuperIO)
                    foreach (var subHardware in hardware.SubHardware)
                    {
                        Console.WriteLine($"  SubHardware: {subHardware.Name} [{subHardware.HardwareType}]");
                        subHardware.Update();
                        foreach (var sensor in subHardware.Sensors)
                        {
                            if (sensor.SensorType == SensorType.Temperature)
                            {
                                Console.WriteLine($"    SENSOR (TEMP): {sensor.Name} = {sensor.Value} C");
                            }
                            else
                            {
                                // Uncomment to see all sensors
                                // Console.WriteLine($"    Sensor: {sensor.Name} [{sensor.SensorType}] = {sensor.Value}");
                            }
                        }
                    }

                    hardware.Update();
                    foreach (var sensor in hardware.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Temperature)
                        {
                            Console.WriteLine($"  SENSOR (TEMP): {sensor.Name} = {sensor.Value} C");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CRITICAL ERROR: {ex}");
            }
            finally
            {
                computer.Close();
            }
        }
    }

    public class UpdateVisitor : IVisitor
    {
        public void VisitComputer(IComputer computer)
        {
            computer.Traverse(this);
        }

        public void VisitHardware(IHardware hardware)
        {
            hardware.Update();
            foreach (var subHardware in hardware.SubHardware) subHardware.Accept(this);
        }

        public void VisitSensor(ISensor sensor) { }
        public void VisitParameter(IParameter parameter) { }
    }
}
