namespace DataBase.Unique
{
    using System;
    using System.Diagnostics;
    using System.Management;
    using System.Runtime.InteropServices;
    using System.Text;

    public class Ulid
    {
        private static bool forMySql = false;
        private static ulong Hi;
        private static ulong LastLo;
        private static ulong LoInc = 1;
        private static readonly object LoLock = new object();
        private static DateTime MinDateTime = new DateTime(0x7dc, 2, 10, 0, 0, 0, DateTimeKind.Utc);
        private static long TickBase;
        private static double TickFrequency = (10000000.0 / ((double) Stopwatch.Frequency));
        private static long TimestampBase;

        static Ulid()
        {
            if (!Stopwatch.IsHighResolution)
            {
                throw new InvalidOperationException("服务器不支持 QueryPerformanceFrequency");
            }
            TickBase = DateTime.UtcNow.Ticks - MinDateTime.Ticks;
            TimestampBase = Stopwatch.GetTimestamp();
            if (TickBase < 0)
            {
                throw new InvalidOperationException("服务器系统日期应不小于 " + MinDateTime);
            }
            LastLo = GetLo();
            Hi = (ulong) (GetVolumeSerialNumber() & 0xffff);
        }

        private static ulong GetLo()
        {
            double num = Stopwatch.GetTimestamp() - TimestampBase;
            num *= TickFrequency;
            return (ulong) ((TickBase + ((long) num)) / 100);
        }

        [DllImport("kernel32.dll", SetLastError=true)]
        private static extern bool GetVolumeInformation(string drive, StringBuilder volumeName, int volumeNameBufLen, out int volSerialNumber, out int maxFileNameLen, out int fileSystemFlags, StringBuilder fileSystemName, int fileSystemNameBufLen);
        private static int GetVolumeSerialNumber()
        {
            int num;
            int num2;
            int num3;
            StringBuilder volumeName = new StringBuilder(50);
            StringBuilder fileSystemName = new StringBuilder(50);
            if (GetVolumeInformation(@"C:\", volumeName, 50, out num, out num2, out num3, fileSystemName, 50))
            {
                return num;
            }
            ManagementClass class2 = new ManagementClass("WIN32_BaseBoard");
            foreach (ManagementObject obj2 in class2.GetInstances())
            {
                return obj2["SerialNumber"].GetHashCode();
            }
            throw new InvalidOperationException("取不到 硬盘序列号");
        }

        private static DateTime LoToDateTime(ulong lo)
        {
            return MinDateTime.AddTicks((long) (lo * 100));
        }

        private static DateTime LoToLocalDateTime(ulong lo)
        {
            return LoToDateTime(lo).ToLocalTime();
        }

        public static ulong NewUlid()
        {
            ulong lo;
            ulong num2;
            lock (LoLock)
            {
                lo = GetLo();
                if ((lo == LastLo) || (lo < (LastLo + LoInc)))
                {
                    lo = LastLo + LoInc;
                    LoInc++;
                }
                else
                {
                    LastLo = lo;
                    LoInc = 1;
                }
            }
            if (forMySql)
            {
                num2 = Hi << 0x30;
                return (num2 + lo);
            }
            num2 = Hi << 0x20;
            ulong num3 = (lo & 0xffff00000000L) << 0x10;
            lo &= 0xffffffffL;
            return ((num3 + num2) + lo);
        }

        public static bool ForMySql
        {
            get
            {
                return forMySql;
            }
            set
            {
                forMySql = value;
            }
        }
    }
}

