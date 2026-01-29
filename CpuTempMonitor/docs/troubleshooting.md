# Troubleshooting CpuTempMonitor

## Common Issues

### "No CPU temperature sensor found" or Temperature is 0.0 C
**Cause:**
This application uses `LibreHardwareMonitorLib`, which often requires **Administrator privileges** to access hardware sensors (reading MSRs, accessing bus drivers).
If you see "Found CPU: ..." but the temperature is 0, it likely means the hardware was detected but the driver could not read the sensor values due to permissions.

**Solution:**
Run the application as Administrator.
- Right-click your terminal (PowerShell/CMD) and select "Run as administrator".
- Then run the executable again.

### x86/x64 Mismatch
If you encounter crashes or "DLL not found" errors related to `LibreHardwareMonitorLib`:
This library is strictly **x64** in recent versions.
- Ensure you have the x64 .NET runtime installed.
- Setup is configured to target `win-x64`.

### "No CPU hardware found"
**Cause:**
- Virtual Machines (VMs) often hide physical CPU details.
- Some bleeding-edge CPUs might not be supported by the current version of LibreHardwareMonitor.

### Multiple Temperatures?
- **Package**: The overall package temperature (hottest spot or average). Use this for general monitoring.
- **core #**: Individual core temperatures.
- **Tctl/Tdie** (AMD): The control temperature used for fan curves. This is usually the best single-number proxy for "CPU Temp".

## Logging
CSV logs are appended to the specified file. If the file is locked (e.g., open in Excel), logging might fail or retry (depending on implementation, currently it might crash or skip). Ensure the file is closed.
