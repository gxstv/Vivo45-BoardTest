using Breas.Device.Communication;
using Breas.Device.Monitoring.Measurements;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Breas.Device.Vivo.Messenger
{
    public enum VivoCommand : byte
    {
        Error = 0,
        NACK = 0x7F,
        NACK_CS = 0x7E,
        ACK = 0x7A,
        PROG_OK = 0x7B,
        ACK0 = 0x70,
        ACK1 = 0x71,
        ACK2 = 0x72,
        ACK3 = 0x73,
        ACK4 = 0x74,
        ACK5 = 0x75,
        ACK6 = 0x76,
        ACK7 = 0x77,
        GetStatus = 0x15,
        GetSetting = 0x96,
        SetSetting = 0x97,
        LOCK = 0x18,
        UNLOCK = 0x19,
        GetHistory = 0x9A,
        EraseHistory = 0x9B,
        GetModel = 0x1C,
        GetMeasure = 0xA0,
        GetMeasureInfo = 0xA6,
        SetMeasure = 0xA1,
        GetMeasuresQ = 0x22,
        GetMeasures = 0xA2,
        GetMeasDef = 0xA3,
        SetMeasDef = 0xA4,
        ClearMeasDef = 0x25,
        DumpDisplay = 0x26,
        SetKeybord = 0xA7,
        GetDebugInfo = 0x28,
        SendDebugInfo = 0xA8,
        ReadMemory = 0xA9,
        WriteMemory = 0xAA,
        GetCurrentMode = 0x2B,
        GetErrorCount = 0x2C,
        ClearErrorCount = 0xC0,
        GetCSerrorinfo = 0x2D,
        CommTest = 0xAE,
        Jump = 0xAF,
        EnableProg = 0xB0,
        GetFlashType = 0x31,
        BurnBoot = 0xB2,
        BurnFlash = 0xB3,
        Reset = 0x34,
        SetFlashType = 0xB5,
        GetBufSize = 0x36,
        GetCalibrationStatus = 0x37,
        WriteCalibration = 0xB7,
        GetString = 0xB8,
        GetStringInfo = 0xB9,
        GetStringCount = 0x39,
        SetString = 0xBA,
        Remote = 0xBB,
        GetPressAndFlow = 0xBC,
        GetLogInfo = 0x3D,
        GetLogData = 0xBE,
        SpecialFlow = 0x3F,
        ChangeBaude = 0xC1,
        BurnSPI = 0xC2,
        CheckSPI = 0x43,
        GetMultipleMeasure = 0xC5,
        GetGenericLogInfo = 0xC6,
        GetGenericLogData = 0xC7,
        GetFastGenericLogInfo = 0xC8,
        GetFastGenericLogData = 0xC9,
        GetBLogInfo = 0x50,
        GetBLogData = 0xD1,
        EraseBHistory = 0xD2,
        GetFastLogData = 0xD3,
        BootWait = 0x60,
        BootExit = 0x61,
        EnableBridgeToUI = 0x62,
        DisableBridgeToUI = 0x63
    };

    public enum VivoLogSession : byte
    {
        BreathLogStart = 1,
        DetailLogStart = 2,
        UsageLogStart = 3,
        LogNext = 4,
        LogReady = 5,
        LogError = 6
    };

    public class VivoMessenger
    {

        private const int MaxPacketDataSize = 30;
        private const int MaxSoftwareVersionLength = MaxPacketDataSize;
        private const int MaxMultipacketDataLength = 30;

        public Dictionary<object, MeasurePointDefinition> MeasurePointDefinitionMap { get; private set; }

        private IStreamCommunication communication;
        private object cmdLock = new object();

        public VivoMessenger(ICommunication communication)
        {
            this.communication = communication as IStreamCommunication;
        }

        public bool StartFlowLog()
        {
            return true;
        }

        public bool StopFlowLog()
        {
            return true;
        }

        

        public int getshift()
        {
            String serial = GetStringValue(0);
            int shift = 0;
            foreach (char c in serial.ToCharArray())
            {
                shift += c;
            }
            shift = 8 - shift % 7 - 1;
            return shift;
        }
        public int GetMeasurePointValue(ushort index)
        {
            byte[] data = BitConverter.GetBytes(index + 1); // point 0 is hidden from user
            //Decode


            //int shift = getshift();
            VivoPacket cmdPacket = new VivoPacket(VivoCommand.GetMeasure, data);
            VivoPacket retPacket = SendCommand(cmdPacket);
            //retPacket.decode(shift);
            
            //for (int i = 0; i < retPacket.Data.Length; i++)
            //{
            //    retPacket.Data[i] = Rotate(retPacket.Data[i], shift);
            //}


            //Decode End
            if (retPacket.Data == null)
            {
                return -1;
            }

            if (retPacket.Data.Length == 2)
            {
                return BitConverter.ToInt16(retPacket.Data, 0);
            }
            else if (retPacket.Data.Length == 4)
            {
                return BitConverter.ToInt32(retPacket.Data, 0);
            }
            else
            {
                return -1;
            }
        }

        public int GetMeasurePointValue(string key)
        {
            if (!MeasurePointDefinitionMap.ContainsKey(key))
                return -1;

            return GetMeasurePointValue(MeasurePointDefinitionMap[key].NativeId);
        }

        public string[] GetProfileNames()
        {
            string[] names = new string[3];
            int currentProfile = GetMeasurePointValue(VivoDeviceKeys.SelectedProfile);
            for (int i = 0; i < names.Length; i++)
            {
                SetMeasurePointValue(VivoDeviceKeys.WorkingProfile, i);
                names[i] = GetStringValue(VivoDeviceKeys.ProfileName);
            }
            SetMeasurePointValue(VivoDeviceKeys.WorkingProfile, currentProfile);
            return names;
        }

        public void SetProfileNames(string[] names)
        {
            int currentProfile = GetMeasurePointValue(VivoDeviceKeys.SelectedProfile);
            for (int i = 0; i < names.Length; i++)
            {
                SetMeasurePointValue(VivoDeviceKeys.WorkingProfile, i);
                SetStringValue(VivoDeviceKeys.ProfileName, names[i]);
            }
            SetMeasurePointValue(VivoDeviceKeys.WorkingProfile, currentProfile);
        }

        public bool SetMeasurePointValue(ushort index, int value)
        {
            byte[] data = new byte[6];

            BitConverter.GetBytes(index + 1).CopyTo(data, 0); // point 0 is hidden from user
            BitConverter.GetBytes(value).CopyTo(data, 2);

            VivoPacket cmdPacket = new VivoPacket(VivoCommand.SetMeasure, data);
            VivoPacket retPacket = SendCommand(cmdPacket);

            return retPacket.Command != VivoCommand.Error && retPacket.Command != VivoCommand.NACK;
        }

        public bool SetMeasurePointValue(string key, int value)
        {
            if (!MeasurePointDefinitionMap.ContainsKey(key))
                return false;

            return SetMeasurePointValue(MeasurePointDefinitionMap[key].NativeId, value);
        }

        public string GetStringValue(ushort index)
        {
            byte[] data = BitConverter.GetBytes(index);

            VivoPacket cmdPacket = new VivoPacket(VivoCommand.GetString, data);
            VivoPacket retPacket = SendCommand(cmdPacket);

            if (retPacket.Data == null)
            {
                return string.Empty;
            }

            return ConvertByteArrayToString(retPacket.Data);
        }

        public string GetStringValue(string key)
        {
            if (!MeasurePointDefinitionMap.ContainsKey(key))
                return string.Empty;

            return GetStringValue(MeasurePointDefinitionMap[key].NativeId);
        }

        public bool SetStringValue(ushort index, string value)
        {
            byte[] stringBytes = new byte[30];
            int i = 0;
            for(; i < value.Length; i++)
            {
                int v = value[i];
                stringBytes[i] = (byte)(v & 0x7F);
            }
            stringBytes[i] = 0;
            byte[] data = new byte[Math.Min(stringBytes.Length, MaxPacketDataSize)];

            data[0] = (byte)index;
            Array.Copy(stringBytes, 0, data, 1, data.Length - 1);

            VivoPacket cmdPacket = new VivoPacket(VivoCommand.SetString, data);
            VivoPacket retPacket = SendCommand(cmdPacket);

            return retPacket.Command != VivoCommand.Error && retPacket.Command != VivoCommand.NACK;
        }

        public bool SetStringValue(string key, string value)
        {
            if (!MeasurePointDefinitionMap.ContainsKey(key))
                return false;

            return SetStringValue(MeasurePointDefinitionMap[key].NativeId, value);
        }

        public byte[] GetCaptureData()
        {
            return communication.GetMessageStreamComm();
        }

        public void GetFastGenericLogInfo(byte logId, out int version, out int size)
        {
            version = 0;
            size = 0;

            byte[] data = new byte[] { logId };

            VivoPacket cmdPacket = new VivoPacket(VivoCommand.GetFastGenericLogInfo, data);
            VivoPacket retPacket = SendCommand(cmdPacket);

            if (retPacket.Data == null || retPacket.Data.Length < 8)
            {
                return;
            }

            version = BitConverter.ToInt32(retPacket.Data, 0);
            size = BitConverter.ToInt32(retPacket.Data, 4);
        }

        public byte[] GetFastGenericLogData(byte logId, int logVersion, int startOffset, int logLength)
        {
            byte[] data = new byte[13];

            data[0] = logId;
            BitConverter.GetBytes(logVersion).CopyTo(data, 1);
            BitConverter.GetBytes(startOffset).CopyTo(data, 5);
            BitConverter.GetBytes(logLength).CopyTo(data, 9);

            VivoPacket cmdPacket = new VivoPacket(VivoCommand.GetFastGenericLogData, data);
            VivoPacket retPacket = SendCommand(cmdPacket);

            return retPacket.Data;
        }

        public bool GetFastLogData(ref byte logSession, out uint logSeqNo, out byte[] log)
        {
            logSeqNo = 0;
            log = null;

            byte[] data = new byte[] { logSession };

            VivoPacket cmdPacket = new VivoPacket(VivoCommand.GetFastLogData, data);
            VivoPacket retPacket = SendCommand(cmdPacket);

            if (retPacket.Data == null || retPacket.Data.Length < 5)
            {
                return false;
            }

            int logLength;

            logSession = retPacket.Data[0];
            logSeqNo = BitConverter.ToUInt16(retPacket.Data, 1);
            logLength = BitConverter.ToUInt16(retPacket.Data, 3);

            if (logLength == retPacket.Data.Length - 5)
            {
                log = new byte[logLength];

                Array.Copy(retPacket.Data, 5, log, 0, logLength);
            }

            return true;
        }

        public int GetNativeId(string key)
        {
            if (MeasurePointDefinitionMap.ContainsKey(key))
                return MeasurePointDefinitionMap[key].NativeId;

            return -1;
        }

        DeviceMeasurement[] keys;
        int[] ids;
        int[] sizes;

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
            if (keys == null || keys.Length != values.Count)
            {
                keys = new DeviceMeasurement[values.Count];
                ids = new int[values.Count];
                sizes = new int[values.Count];
            }
            int idx = 0;
            int totalSize = 0;
            foreach (var keyValPair in values)
            {
                //Id's here need to be + 1
                ids[idx] = MeasurePointDefinitionMap[keyValPair.Key.Key].NativeId + 1;
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
                //fill the sub ids and sizes
                for (int i = 0; i < subLength; i++)
                {
                    subIds[i] = ids[lastBlockIndex + i];
                    subSizes[i] = sizes[lastBlockIndex + i];
                }
                //get the measurement values
                GetMultipleMeasurePointValues(subIds, subSizes, out subResults);

                //disconnection
                if (subResults == null)
                {
                    return false;
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

        private bool GetMultipleMeasurePointValues(int[] ids, int[] sizes, out int[] results)
        {
            results = null;

            if (ids.Length != sizes.Length)
            {
                return false;
            }

            byte[] data = new byte[ids.Length * 2];
            int totalSize = 0;

            // pack the points and ids into 16-bit points, with 14 bits for the id and 2 bits for the size
            for (int i = 0; i < ids.Length; i++)
            {
                // 16382 because id zero is hidden
                if (ids[i] < 0 || ids[i] >= 16382)
                {
                    return false;
                }

                if (sizes[i] < 1 || sizes[i] > 4)
                {
                    return false;
                }

                totalSize += sizes[i];
                if (totalSize > MaxMultipacketDataLength)
                {
                    return false;
                }

                // size 4 is encoded as zero
                ushort packedPoint = (ushort)((ushort)ids[i] | ((sizes[i] == 4 ? 0 : (ushort)sizes[i]) << 14));

                BitConverter.GetBytes(packedPoint).CopyTo(data, i * 2);
            }

            VivoPacket cmdPacket = new VivoPacket(VivoCommand.GetMultipleMeasure, data);
            VivoPacket retPacket = SendCommand(cmdPacket);

            if (retPacket.Data == null)
            {
                return false;
            }

            results = new int[sizes.Length];
            int bp = 0;

            for (int i = 0; i < sizes.Length && bp <= retPacket.Data.Length; i++)
            {
                switch (sizes[i])
                {
                    case 1:
                        results[i] = retPacket.Data[bp];
                        bp += 1;
                        break;

                    case 2:
                        results[i] = BitConverter.ToUInt16(retPacket.Data, bp);
                        bp += 2;
                        break;

                    case 3:
                        results[i] = (int)retPacket.Data[bp] << 16;
                        bp += 1;
                        results[i] += BitConverter.ToUInt16(retPacket.Data, bp);
                        bp += 2;
                        break;

                    case 4:
                        results[i] = BitConverter.ToInt32(retPacket.Data, bp);
                        bp += 4;
                        break;
                }
            }

            return true;
        }

        #region Helpers

        public ushort GetNumMeasurePoints()
        {
            ushort pointCount = 0;
            //int shift = getshift();
            VivoPacket cmdPacket = new VivoPacket(VivoCommand.GetMeasureInfo, new byte[2]);
            VivoPacket retPacket = SendCommand(cmdPacket);
            //retPacket.decode(shift);
            if (retPacket.Data != null && retPacket.Data.Length == MaxPacketDataSize)
            {
                pointCount = BitConverter.ToUInt16(retPacket.Data, 28);
                pointCount--; // Point 0 is hidden from user
            }

            return pointCount;
        }

        public MeasurePointDefinition GetMeasurePointInformation(ushort nativeId)
        {
            string name = string.Empty;
            float correctionFactor = 0;
            int type = 0;

            byte[] data = BitConverter.GetBytes(nativeId + 1); // point 0 is hidden from user

            VivoPacket cmdPacket = new VivoPacket(VivoCommand.GetMeasureInfo, data);
            VivoPacket retPacket = SendCommand(cmdPacket);

            if (retPacket.Data != null && retPacket.Data.Length == MaxPacketDataSize)
            {
                name = ConvertByteArrayToString(retPacket.Data, 0, 24);
                correctionFactor = BitConverter.ToSingle(retPacket.Data, 24);
                type = retPacket.Data[28];
            }

            return new MeasurePointDefinitionStringKey(name, correctionFactor, (ushort)type, nativeId);
        }

        public ushort GetNumStringPoints()
        {
            ushort count = 0;
            //int shift = getshift();
            VivoPacket cmdPacket = new VivoPacket(VivoCommand.GetStringCount, null);
            VivoPacket retPacket = SendCommand(cmdPacket);
            //retPacket.decode(shift);
            if (retPacket.Data != null && retPacket.Data.Length == 1)
            {
                count = retPacket.Data[0];
            }

            return count;
        }

        public string GetStringPointInfo(ushort index)
        {
            string name = string.Empty;

            byte[] data = BitConverter.GetBytes(index);

            VivoPacket cmdPacket = new VivoPacket(VivoCommand.GetStringInfo, data);
            VivoPacket retPacket = SendCommand(cmdPacket);

            if (retPacket.Data != null)
            {
                name = ConvertByteArrayToString(retPacket.Data);
            }

            return name;
        }

        public Dictionary<object, MeasurePointDefinition> MapMeasurePointDefinitions(Dictionary<object, MeasurePointDefinition> definitions = null)
        {
            if (definitions == null)
            {
                this.MeasurePointDefinitionMap = new Dictionary<object, MeasurePointDefinition>();

                for (ushort nativeId = 0, n = GetNumMeasurePoints(); nativeId < n; nativeId++)
                {
                    var def = GetMeasurePointInformation(nativeId);
                    MeasurePointDefinitionMap[def.Name] = def;
                    MeasurePointDefinitionMap[nativeId] = new MeasurePointDefinitionShortKey(def.Name, 0, (ushort)0, (ushort)nativeId);
                }

                for (ushort nativeId = 0, n = GetNumStringPoints(); nativeId < n; nativeId++)
                {
                    string nativeName = GetStringPointInfo(nativeId);
                    MeasurePointDefinitionMap[nativeName] = new MeasurePointDefinitionStringKey(nativeName, 0, (ushort)0, (ushort)nativeId);
                }
            }
            else
            {
                this.MeasurePointDefinitionMap = definitions;
            }

            return this.MeasurePointDefinitionMap;
        }

        #endregion Helpers

        public string GetVersion()
        {
            VivoPacket cmdPacket = new VivoPacket(VivoCommand.GetModel, null);
            VivoPacket retPacket = SendCommand(cmdPacket);

            if (retPacket.Data == null)
            {
                return string.Empty;
            }

            return ConvertByteArrayToString(retPacket.Data);
        }

        private VivoPacket SendCommand(VivoPacket packet)
        {
            lock(cmdLock)
            {
                int shift;
                byte[] receivedData = communication.SendMessage(packet.ToArray());
                if (packet.Command == VivoCommand.GetString)
                    shift = 0;
                else
                    shift = getshift();
                VivoPacket result = new VivoPacket(receivedData);
                //result.decode(shift);
                return result;
            }
        }

        private string ConvertByteArrayToString(byte[] data)
        {
            return ConvertByteArrayToString(data, 0, data.Length);
        }

        private string ConvertByteArrayToString(byte[] data, int index, int count)
        {
            if (data == null)
            {
                return null;
            }

            // Find string termination byte '\0'
            int stringLength = count - index;
            for (int i = index; i < count; i++)
            {
                if (data[i] == 0)
                {
                    stringLength = i - index;

                    break;
                }
            }

            return System.Text.Encoding.UTF8.GetString(data, index, stringLength);
        }
    }
}