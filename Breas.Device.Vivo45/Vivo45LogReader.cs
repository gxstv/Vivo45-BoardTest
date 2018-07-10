using Breas.Device.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Breas.Device.Vivo45.Messenger;

namespace Breas.Device.Vivo45
{
    public class Vivo45LogReader : ILogReader
    {
        public const int Timeout = 1200 * 1000; //20 mins

        public event DataReceived DataReceived;

        public LogStatus LogStatus { get; private set; }

        private Vivo45Device device;

        public Vivo45LogReader(Vivo45Device device)
        {
            this.device = device;
        }

        public IEnumerable<byte[]> ReadLogs(DateTime startTime, LogDataLevel level)
        {
            if (LogStatus != LogStatus.Idle)
                throw new InvalidOperationException("Cancel the current read op before starting a new one");
            LogStatus = LogStatus.InProgress;

            Vivo45LogPacket.Vivo45LogLevel v45Level = GetVivoLogLevel(level);
            if (!device.Messenger.StartLogDownload(v45Level, startTime, DateTime.Now, true))
            {
                LogStatus = LogStatus.Idle;
                throw new Communication.CommunicationException("Error sending log start message");
            }

            //start reading

            var logs = new List<byte[]>();

            int expectedMessageNo = 0;
            int time = 0;
            int start = Environment.TickCount;
            LogStatus = LogStatus.InProgress;
            Vivo45LogPacket logPacket = null;
            while (time < Timeout)
            {
                if (LogStatus == LogStatus.Cancelled)
                {
                    LogStatus = LogStatus.Idle;
                    throw new LogException("Cancelled", LogStatus.Cancelled);
                }
                logPacket = device.Messenger.GetNextLogPacket(v45Level);
                if (logPacket == null || !logPacket.Ack)
                {
                    LogStatus = LogStatus.Idle;
                    throw new Communication.CommunicationException("Error reading log message");
                }
                if (!ReadMessage(ref expectedMessageNo, logs, logPacket))
                {
                    LogStatus = LogStatus.Idle;
                    throw new Communication.CommunicationException("Missed a log message");
                }
                RaiseStatus(expectedMessageNo, expectedMessageNo);
                if (logPacket.LastPacket)
                {
                    LogStatus = LogStatus.Completed;
                    RaiseStatus(expectedMessageNo, expectedMessageNo);
                    break;
                }
                time += Environment.TickCount - start;
                start = Environment.TickCount;
            }
            LogStatus = LogStatus.Idle;
            return logs;
        }

        private Vivo45LogPacket.Vivo45LogLevel GetVivoLogLevel(LogDataLevel level)
        {
           switch(level)
            {
                case LogDataLevel.One:
                    return Vivo45LogPacket.Vivo45LogLevel.Level1;
                case LogDataLevel.Two:
                    return Vivo45LogPacket.Vivo45LogLevel.Level2;
                case LogDataLevel.Three:
                    return Vivo45LogPacket.Vivo45LogLevel.Level3;
                default:
                    return Vivo45LogPacket.Vivo45LogLevel.Level1;

            }
        }

        public Task<IEnumerable<byte[]>> ReadLogsAsync(DateTime startTime, LogDataLevel level)
        {
            return Task.Factory.StartNew(() => ReadLogs(startTime, level));
        }

        public void Cancel()
        {
            LogStatus = LogStatus.Cancelled;
        }

        public bool ReadMessage(ref int expectedMessageNo, List<byte[]> logs, Vivo45LogPacket message)
        {
            logs.Add(message.data);
            expectedMessageNo++;
            return true;
        }

        public void RaiseStatus(int readMessages, int totalMessages)
        {
            if (DataReceived != null)
                DataReceived(readMessages, totalMessages, LogStatus);
        }
        public string GetLogType(LogDataLevel level)
        {
            switch(level)
            {
                case LogDataLevel.One:
                    return "VIVO45_LEVEL_1_V1";
                case LogDataLevel.Two:
                    return "VIVO45_LEVEL_2_V1";
                case LogDataLevel.Three:
                    return "VIVO45_LEVEL_3_V1";
                default:
                    return "VIVO45_LEVEL_1_V1";
            }
        }
    }
}
