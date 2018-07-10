using Breas.Device.Communication;
using Breas.Device.Monitoring.Measurements;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Breas.Device.Finder.Windows
{
    public class UsbBcpCommunication : IBcpCommunication
    {
        private const int MaxPacketDataSize = 30;
        private const int MaxSoftwareVersionLength = MaxPacketDataSize;

        private string _version;
        private IntPtr handle = new IntPtr();
        private UsbBcpResolverInfo _resolverInfo;
        private bool connected;

        public string Version { get { return _version; } }

        public IResolverInfo ResolverInfo
        {
            get { return _resolverInfo; }
        }

        public bool Heartbeat
        {
            get
            {
                try
                {
                    GetVersion();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public UsbBcpCommunication(UsbBcpResolverInfo info)
        {
            this._resolverInfo = info;
        }

        public byte[] GetMessage(int timeout = 1000)
        {
            throw new NotImplementedException();
        }

        public byte[] SendMessage(byte[] message, int timeout = 1000)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<byte[]> GetMessageAsync(int timeout = 1000)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<byte[]> SendMessageAsync(byte[] message, int timeout = 1000)
        {
            throw new NotImplementedException();
        }

        #region Interface Implementation

        public void ClearBuffer()
        {
            if (BCPClearBuffer_h(handle) == 0)
            {
                //ignore. most of the time it will work without clearing the buffer.
            }
        }

        public bool Connect()
        {
            if (connected)
            {
                Disconnect();
            }
            var result = BCPInit_h(out handle, FormatPortName(_resolverInfo.Port));
            if (handle == IntPtr.Zero)
            {
                connected = false;
                return false;
            }
            _version = GetVersion();
            connected = true;
            return true;
        }

        public bool Disconnect()
        {
            connected = false;
            if (BCPClose_h(out handle) == 0)
            {
                //Log.ErrorFormat("Error closing comm: {0}", _resolverInfo.Port);
                return false;
            }
            return true;
        }

        /// <summary>
        /// BCP doesn't implement subscriptions so just ignore the points
        /// </summary>
        public bool StartFlowLog()
        {
            if (BCPStartFlowLog_h(handle) != 0)
            {
                throw new BCPException(string.Format("Unable to start flow log, port: {0}", _resolverInfo.Port));
            }
            return true;
        }

        /// <summary>
        /// BCP doesn't implement subscriptions so just ignore the points
        /// </summary>
        public bool StopFlowLog()
        {
            if (BCPStopFlowLog_h(handle) != 0)
            {
                throw new BCPException(string.Format("Unable to stop flow log, port: {0}", _resolverInfo.Port));
            }
            return true;
        }

        public int GetMeasurePointValue(ushort index)
        {
            int value = 0;
            if (BCPGetMeasurePointValue_h(handle, (short)index, ref value) == 0)
            {
                throw new BCPException(string.Format("Unable to get measure point value for index: {0}, port: {1}", index, _resolverInfo.Port));
            }
            return value;
        }

        public void SetMeasurePointValue(ushort index, int value)
        {
            if (BCPSetMeasurePointValue_h(handle, (short)index, value) == 0)
            {
                throw new BCPException(string.Format("Unable to set measure point value for index: {0}, port: {1}", index, _resolverInfo.Port));
            }
        }

        public string GetStringValue(ushort index)
        {
            StringBuilder stringBldr = new StringBuilder(30, 30);
            if (BCPGetStringValue_h(handle, (short)index, stringBldr) == 0)
            {
                throw new BCPException(string.Format("Unable to set string point info for index: {0}, port: {1}", index, _resolverInfo.Port));
            }
            return stringBldr.ToString();
        }

        public void SetStringValue(ushort index, string value)
        {
            if (BCPSetStringValue_h(handle, (short)index, value) == 0)
            {
                throw new BCPException(string.Format("Unable to set string value for index: {0}, port: {1}", index, _resolverInfo.Port));
            }
        }

        public byte[] GetCaptureData()
        {
            int size = 0;
            byte[] data = new byte[512];
            if (BCPGetCaptureData_h(handle, out size, data) == 0)
            {
                throw new BCPException(string.Format("Unable to get capture data, port: {0}", _resolverInfo.Port));
            }
            return data;
        }

        /// <summary>
        /// Gets a bulk list of measurement points from the device.
        /// This is more efficient than calling GetMeasurementValue separately.
        ///
        /// The max packet size over USB for Vivo is 30 bytes. The method will split
        /// up the given list into blocks of 30 bytes each
        /// </summary>
        /// <param name="values">The dictionary to fill with the values from the device</param>
        /// <returns>True if the read was successfull</returns>
        public void FillMeasurementPointValues(int numPoints, int[] ids, int[] sizes, int[] results, out int resultsLen)
        {
            if (BCPGetMultipleMeasurePointValues_h(handle, numPoints, ids, sizes, results, out resultsLen) == 0)
            {
                throw new BCPException(string.Format("Unable to get measurement point values, ids: {0}, port: {1}", string.Join(", ", ids), _resolverInfo.Port));
            }
        }

        #endregion Interface Implementation

        #region Helpers

        public string GetVersion()
        {
            var modelBldr = new StringBuilder(MaxSoftwareVersionLength);
            if (BCPGetModel_h(handle, modelBldr) == 0)
            {
                throw new BCPException(string.Format("Unable to get model on: {0}", _resolverInfo.Port));
            }
            return modelBldr.ToString();
        }

        public short GetNumMeasurePoints()
        {
            char[] swVersion = new char[MaxSoftwareVersionLength];
            short value = BCPGetMeasurePointsInfo_h(handle, swVersion);
            if (value == 0)
            {
                throw new BCPException(string.Format("Unable to get number of measurepoints: {0}", _resolverInfo.Port));
            }
            return value;
        }

        public short GetNumStringPoints()
        {
            short value = BCPGetStringListInfo_h(handle);
            if (value == 0)
            {
                throw new BCPException(string.Format("Unable to get number of string points: {0}", _resolverInfo.Port));
            }
            return value;
        }

        public MeasurePointDefinitionStringKey GetMeasurePointDefinition(ushort index)
        {
            var nameBldr = new StringBuilder(40);
            float correctionFactor = 0;
            short type = 0;
            if (BCPGetMeasurePointInfo_h(handle, (short)index, nameBldr, ref correctionFactor, ref type) == 0)
            {
                throw new BCPException(string.Format("Unable to get measure point info for index: {0}, port: {1}", index, _resolverInfo.Port));
            }
            return new MeasurePointDefinitionStringKey(nameBldr.ToString(), correctionFactor, (ushort)type, (ushort)index);
        }

        public string GetStringPointInfo(ushort index)
        {
            StringBuilder nameBldr = new StringBuilder(30);
            if (BCPGetStringList_h(handle, (short)index, nameBldr) == 0)
            {
                throw new BCPException(string.Format("Unable to get string point info for index: {0}, port: {1}", index, _resolverInfo.Port));
            }
            return nameBldr.ToString();
        }

        #endregion Helpers

        private static string FormatPortName(string portName)
        {
            if (!portName.StartsWith("\\"))
            {
                return "\\\\.\\" + portName;
            }
            return portName;
        }

        [DllImport("bcp.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern short BCPInit_h(out IntPtr handle, string port);

        [DllImport("bcp.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern short BCPClose_h(out IntPtr handle);

        [DllImport("bcp.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern short BCPGetModel_h(IntPtr handle, StringBuilder version); 

        [DllImport("bcp.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern short BCPClearBuffer_h(IntPtr handle);

        [DllImport("bcp.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern short BCPGetMeasurePointsInfo_h(IntPtr handle, char[] swRev);

        [DllImport("bcp.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern short BCPGetStringListInfo_h(IntPtr handle);

        [DllImport("bcp.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern short BCPGetMeasurePointInfo_h(IntPtr handle, short id, StringBuilder name, ref float correctionFactor, ref short type);

        [DllImport("bcp.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern short BCPGetStringList_h(IntPtr handle, short id, StringBuilder name);

        [DllImport("bcp.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern short BCPGetMeasurePointValue_h(IntPtr handle, short id, ref int value);

        [DllImport("bcp.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern short BCPSetMeasurePointValue_h(IntPtr handle, short id, int value);

        [DllImport("bcp.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern short BCPGetStringValue_h(IntPtr handle, short id, StringBuilder value);

        [DllImport("bcp.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern short BCPSetStringValue_h(IntPtr handle, short id, string value);

        [DllImport("bcp.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern short BCPGetCaptureData_h(IntPtr handle, out int size, byte[] data);

        [DllImport("bcp.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern short BCPGetMultipleMeasurePointValues_h(IntPtr handle, int numPoints, int[] ids, int[] sizes, int[] results, out int resultsLen);

        [DllImport("bcp.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern short BCPStartFlowLog_h(IntPtr handle);

        [DllImport("bcp.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern short BCPStopFlowLog_h(IntPtr handle);
    }
}