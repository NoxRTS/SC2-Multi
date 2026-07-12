using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SC2_Multi;

internal static class HandleCloser
{
    private static readonly string[] TargetSuffixes =
    [
        @"\StarCraft II Game Application (Global)",
        @"\StarCraft II Game Application",
        @"\StarCraft II IPC Mem",
    ];

    public static Task<List<string>> CloseHandlesAsync() => Task.Run(CloseHandles);

    private static List<string> CloseHandles()
    {
        var log = new List<string>();

        var procs = Process.GetProcessesByName("SC2_x64");
        if (procs.Length == 0)
        {
            log.Add("No SC2_x64.exe processes found.");
            return log;
        }

        var pids = new HashSet<uint>(procs.Select(p => (uint)p.Id));
        log.Add($"Found {pids.Count} SC2_x64.exe process(es): PID {string.Join(", ", pids)}");

        nint buffer = 0;
        int size = 0x200000;

        try
        {
            uint status;
            int returnLength;
            do
            {
                buffer = Marshal.AllocHGlobal(size);
                status = NativeMethods.NtQuerySystemInformation(
                    NativeMethods.SystemExtendedHandleInformation, buffer, size, out returnLength);
                if (status == NativeMethods.STATUS_INFO_LENGTH_MISMATCH)
                {
                    Marshal.FreeHGlobal(buffer);
                    buffer = 0;
                    size = returnLength + 0x10000;
                }
            }
            while (status == NativeMethods.STATUS_INFO_LENGTH_MISMATCH);

            if (status != NativeMethods.STATUS_SUCCESS)
            {
                log.Add($"NtQuerySystemInformation failed: 0x{status:X8}");
                return log;
            }

            long handleCount = Marshal.ReadIntPtr(buffer).ToInt64();
            int headerSize = nint.Size * 2; // NumberOfHandles + Reserved
            int entrySize = Marshal.SizeOf<NativeMethods.SYSTEM_HANDLE_TABLE_ENTRY_INFO_EX>();

            var processHandles = new Dictionary<uint, nint>();
            int closed = 0;

            try
            {
                for (long i = 0; i < handleCount; i++)
                {
                    var entry = Marshal.PtrToStructure<NativeMethods.SYSTEM_HANDLE_TABLE_ENTRY_INFO_EX>(
                        buffer + headerSize + (nint)(i * entrySize));

                    uint pid = (uint)entry.UniqueProcessId;
                    if (!pids.Contains(pid))
                        continue;

                    if (!processHandles.TryGetValue(pid, out nint procHandle))
                    {
                        procHandle = NativeMethods.OpenProcess(NativeMethods.PROCESS_DUP_HANDLE, false, (int)pid);
                        processHandles[pid] = procHandle;
                        if (procHandle == 0)
                        {
                            log.Add($"Failed to open PID {pid} — run as Administrator.");
                            continue;
                        }
                    }

                    if (procHandle == 0)
                        continue;

                    nint srcHandle = (nint)(long)entry.HandleValue;

                    if (!NativeMethods.DuplicateHandle(
                            procHandle, srcHandle,
                            NativeMethods.GetCurrentProcess(), out nint dup,
                            0, false, NativeMethods.DUPLICATE_SAME_ACCESS))
                        continue;

                    try
                    {
                        // Query type first — this never deadlocks.
                        string? typeName = QueryType(dup);
                        if (typeName is not ("Event" or "Section"))
                            continue;

                        // Only query names for safe types (Event/Section).
                        string? name = QueryName(dup);
                        if (name is null)
                            continue;

                        foreach (var suffix in TargetSuffixes)
                        {
                            if (!name.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
                                continue;

                            // Close our duplicate first so it doesn't keep the kernel object alive.
                            NativeMethods.CloseHandle(dup);
                            dup = 0;

                            // Close the handle inside the SC2 process.
                            NativeMethods.DuplicateHandle(
                                procHandle, srcHandle,
                                0, out _, 0, false,
                                NativeMethods.DUPLICATE_CLOSE_SOURCE);

                            log.Add($"  Closed {typeName} in PID {pid}: {name}");
                            closed++;
                            break;
                        }
                    }
                    finally
                    {
                        if (dup != 0)
                            NativeMethods.CloseHandle(dup);
                    }
                }
            }
            finally
            {
                foreach (var h in processHandles.Values)
                {
                    if (h != 0)
                        NativeMethods.CloseHandle(h);
                }
            }

            log.Add($"Done — closed {closed} handle(s).");
        }
        finally
        {
            if (buffer != 0)
                Marshal.FreeHGlobal(buffer);
        }

        return log;
    }

    private static string? QueryType(nint handle)
    {
        int size = 0x200;
        nint buf = Marshal.AllocHGlobal(size);
        try
        {
            uint status = NativeMethods.NtQueryObject(
                handle, NativeMethods.ObjectTypeInformation, buf, size, out _);
            if (status != NativeMethods.STATUS_SUCCESS)
                return null;

            var info = Marshal.PtrToStructure<NativeMethods.OBJECT_TYPE_INFORMATION>(buf);
            if (info.TypeName.Length == 0 || info.TypeName.Buffer == 0)
                return null;

            return Marshal.PtrToStringUni(info.TypeName.Buffer, info.TypeName.Length / 2);
        }
        finally
        {
            Marshal.FreeHGlobal(buf);
        }
    }

    private static string? QueryName(nint handle)
    {
        int size = 0x1000;
        nint buf = Marshal.AllocHGlobal(size);
        try
        {
            uint status = NativeMethods.NtQueryObject(
                handle, NativeMethods.ObjectNameInformation, buf, size, out int needed);

            if (status is NativeMethods.STATUS_BUFFER_OVERFLOW
                       or NativeMethods.STATUS_BUFFER_TOO_SMALL
                       or NativeMethods.STATUS_INFO_LENGTH_MISMATCH)
            {
                Marshal.FreeHGlobal(buf);
                size = needed;
                buf = Marshal.AllocHGlobal(size);
                status = NativeMethods.NtQueryObject(
                    handle, NativeMethods.ObjectNameInformation, buf, size, out _);
            }

            if (status != NativeMethods.STATUS_SUCCESS)
                return null;

            var info = Marshal.PtrToStructure<NativeMethods.OBJECT_NAME_INFORMATION>(buf);
            if (info.Name.Length == 0 || info.Name.Buffer == 0)
                return null;

            return Marshal.PtrToStringUni(info.Name.Buffer, info.Name.Length / 2);
        }
        finally
        {
            Marshal.FreeHGlobal(buf);
        }
    }
}
