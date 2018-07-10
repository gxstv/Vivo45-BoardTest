using Breas.Device.Communication;
using Breas.Device.Monitoring.Measurements;
using log4net;
using System;
using System.Collections.Generic;

namespace Breas.Device.Vivo
{
    public class VivoMessenger
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(VivoMessenger));

        private const int MaxPacketDataSize = 30;
        private const int MaxSoftwareVersionLength = MaxPacketDataSize;

        //the bcp dll doesnt like when we read from two threads
        private object bcpLock = new object();

        public Dictionary<object, MeasurePointDefinition> MeasurePointDefinitions { get; set; }

        private IBcpCommunication communication;

        public VivoMessenger(ICommunication communication)
        {
            this.communication = communication as IBcpCommunication;
        }

        public bool StartFlowLog()
        {
            lock (bcpLock)
            {
                try
                {
                    communication.ClearBuffer();
                    communication.StartFlowLog();
                }
                catch (Exception e)
                {
                    Log.Error(e.Message, e);
                    return false;
                }
            }
            return true;
        }

        public bool StopFlowLog()
        {
            lock (bcpLock)
            {
                try
                {
                    communication.StopFlowLog();
                }
                catch (Exception e)
                {
                    Log.Error(e.Message, e);
                    return false;
                }
            }
            return true;
        }

        public int GetMeasurePointValue(ushort index)
        {
            lock (bcpLock)
            {
                try
                {
                    return communication.GetMeasurePointValue(index);
                }
                catch (Exception e)
                {
                    Log.Error(e.Message, e);
                    return -1;
                }
            }
        }

        public int GetMeasurePointValue(string key)
        {
            lock (bcpLock)
            {
                if (!MeasurePointDefinitions.ContainsKey(key))
                    return -1;
                return GetMeasurePointValue(MeasurePointDefinitions[key].NativeId);
            }
        }

        public bool SetMeasurePointValue(ushort index, int value)
        {
            lock (bcpLock)
            {
                try
                {
                    communication.SetMeasurePointValue(index, value);
                    return true;
                }
                catch (Exception e)
                {
                    Log.Error(e.Message, e);
                    return false;
                }
            }
        }

        public bool SetMeasurePointValue(string key, int value)
        {
            lock (bcpLock)
            {
                if (!MeasurePointDefinitions.ContainsKey(key))
                    return false;
                return SetMeasurePointValue(MeasurePointDefinitions[key].NativeId, value);
            }
        }

        public string GetStringValue(ushort index)
        {
            lock (bcpLock)
            {
                try
                {
                    return communication.GetStringValue(index);
                }
                catch (Exception e)
                {
                    Log.Error(e.Message, e);
                    return string.Empty;
                }
            }
        }

        public string GetStringValue(string key)
        {
            lock (bcpLock)
            {
                if (!MeasurePointDefinitions.ContainsKey(key))
                    return "";
                return GetStringValue(MeasurePointDefinitions[key].NativeId);
            }
        }

        public bool SetStringValue(ushort index, string value)
        {
            lock (bcpLock)
            {
                try
                {
                    communication.SetStringValue(index, value);
                    return true;
                }
                catch (Exception e)
                {
                    Log.Error(e.Message, e);
                    return false;
                }
            }
        }

        public bool SetStringValue(string key, string value)
        {
            lock (bcpLock)
            {
                if (!MeasurePointDefinitions.ContainsKey(key))
                    return false;
                return SetStringValue(MeasurePointDefinitions[key].NativeId, value);
            }
        }

        public byte[] GetCaptureData()
        {
            return communication.GetCaptureData();
        }

        public int GetNativeId(string key)
        {
            if (MeasurePointDefinitions.ContainsKey(key))
                return MeasurePointDefinitions[key].NativeId;
            return -1;
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
        public bool GetMeasurementPointValues(IDictionary<DeviceMeasurement, int> values)
        {
            lock (bcpLock)
            {
                DeviceMeasurement[] keys = new DeviceMeasurement[values.Count];
                int[] ids = new int[values.Count];
                int[] sizes = new int[values.Count];
                int idx = 0;
                int totalSize = 0;
                foreach (var keyValPair in values)
                {
                    //Id's here need to be + 1
                    ids[idx] = MeasurePointDefinitions[keyValPair.Key.Key].NativeId + 1;
                    keys[idx] = keyValPair.Key;
                    sizes[idx] = keyValPair.Key.StorageType.size;
                    totalSize += sizes[idx];
                    idx++;
                }
                //an array of the end index of each block
                int[] blockIndexes = new int[(int)Math.Ceiling((double)totalSize / MaxPacketDataSize)];
                totalSize = 0;
                idx = 0;
                //set the last block index's end index to the amount of measurements since
                //the last index has to end with the amount of measurements
                blockIndexes[blockIndexes.Length - 1] = sizes.Length;
                for (int i = 0; i < sizes.Length; i++)
                {
                    //check if this blocks size is over, or is going over, the max packet size
                    if (totalSize > MaxPacketDataSize || totalSize + sizes[i] > MaxPacketDataSize)
                    {
                        //set this blocks end index
                        blockIndexes[idx++] = i;
                        totalSize = 0;
                    }
                    totalSize += sizes[i];
                }
                int[] results = new int[ids.Length];
                int lastBlockIndex = 0;
                //split the measurements into blocks 15 bytes each
                for (int block = 0; block < blockIndexes.Length; block++)
                {
                    int blockEndIndex = blockIndexes[block];
                    //determine this blocks length
                    int subLength = blockEndIndex - lastBlockIndex;
                    int[] subIds = new int[subLength];
                    int[] subSizes = new int[subLength];
                    int[] subResults = new int[subLength];
                    //not needed?
                    int resultsLen = 0;
                    //fill the sub ids and sizes
                    for (int i = 0; i < subLength; i++)
                    {
                        subIds[i] = ids[lastBlockIndex + i];
                        subSizes[i] = sizes[lastBlockIndex + i];
                    }
                    //get the measurement values
                    try
                    {
                        communication.FillMeasurementPointValues(subLength, subIds, subSizes, subResults, out resultsLen);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e.Message, e);
                    }
                    //set the results in the map
                    for (int i = 0; i < subLength; i++)
                    {
                        int index = lastBlockIndex + i;
                        values[keys[index]] = subResults[i];
                    }
                    lastBlockIndex = blockEndIndex;
                }
                return true;
            }
        }

        #region Helpers

        public Dictionary<object, MeasurePointDefinition> MapMeasurePointDefinitions(Dictionary<object, MeasurePointDefinition> definitions = null)
        {
            lock (bcpLock)
            {
                if (definitions == null)
                {
                    this.MeasurePointDefinitions = new Dictionary<object, MeasurePointDefinition>();
                    for (ushort nativeId = 0, n = (ushort)communication.GetNumMeasurePoints(); nativeId < n; nativeId++)
                    {
                        var def = communication.GetMeasurePointDefinition(nativeId);
                        MeasurePointDefinitions[def.Name] = def;
                    }
                    for (ushort nativeId = 0, n = (ushort)communication.GetNumStringPoints(); nativeId < n; nativeId++)
                    {
                        string nativeName = communication.GetStringPointInfo(nativeId);
                        MeasurePointDefinitions[nativeName] = new MeasurePointDefinitionStringKey(nativeName, 0, (ushort)0, (ushort)nativeId);
                    }
                }
                else
                {
                    this.MeasurePointDefinitions = definitions;
                }
                return this.MeasurePointDefinitions;
            }
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

        public string GetVersion()
        {
            lock (bcpLock)
            {
                try
                {
                    return communication.GetVersion();
                }
                catch (Exception e)
                {
                    Log.Error(e.Message, e);
                    return null;
                }
            }
        }
    }
}