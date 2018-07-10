using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breas.Device.Logs
{

    public delegate void DataReceived(int readMessages, int availableMessages, LogStatus status);


    public interface ILogReader
    {

        event DataReceived DataReceived;

        LogStatus LogStatus { get; }

        IEnumerable<byte[]> ReadLogs(DateTime startTime, LogDataLevel level);

        Task<IEnumerable<byte[]>> ReadLogsAsync(DateTime startTime, LogDataLevel level);

        void Cancel();
        string GetLogType(LogDataLevel one);
    }
}
