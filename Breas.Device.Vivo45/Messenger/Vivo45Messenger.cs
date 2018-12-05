using Breas.Device.Communication;
using Breas.Device.Monitoring.Measurements;
using System;
using System.Collections.Generic;
using System.Linq;
using Breas.Device.Monitoring.Alarms;
using Breas.Device.Finder ;

namespace Breas.Device.Vivo45.Messenger
{
    public class Vivo45Messenger
    {
        public delegate void NewAlarmsAvailable();

        public struct AvailableMeasurePoints
        {
            public UInt16 Count { get; set; }
            public UInt16[] MeasurePoints { get; set; }
            public bool ReadAcces { get; set; }
            public UInt16[] WriteAcces { get; set; }
        }

        public struct AvailableSettings
        {
            public UInt16 Count { get; set; }
            public UInt16[] Settings { get; set; }
        }

        private IStreamCommunication communication;

        private Dictionary<object, MeasurePointDefinition> measurepointDefinitions;

        private byte sender;
        private byte receiver;

        public Dictionary<object, MeasurePointDefinition> MeasurepointDefinitions { get { return measurepointDefinitions; } }

        public event NewAlarmsAvailable NewAlarmsAvailableNow;

        public Vivo45Messenger(ICommunication communication)
        {
            this.communication = communication as IStreamCommunication;
        }

        public bool StartCapturing(List<MeasurePointDefinition> measurepoints)
        {
            //subscribe to points
            SubscribeToMeasurepoints(measurepoints.Select(s => s.NativeId).ToList());
            return true;
        }

        public bool StopCapturing()
        {
            //unsubscribe
            return true;
        }

        private void checkSenderandReceiver(V45Packet packet)
        {
            if (this.sender == 0)
            {
                this.sender = packet.Receiver;
            }
            if (this.receiver == 0)
            {
                this.receiver = packet.Sender;
            }
        }

        public V45Packet SendMessage(V45Packet outPacket)
        {
            try
            {

                var packet = new V45Packet(communication.SendMessage(outPacket.toArray(), 5000));

                checkSenderandReceiver(packet);

                CheckStatusByte(packet);
                return packet;

                //communication.SendMessage(outPacket.toArray(), 5000);
                //checkSenderandReceiver(outPacket);

                //CheckStatusByte(outPacket);
                //return outPacket;

            }
            catch (Exception)
            {

                throw;
            }
        }

        public V45Packet GetMessage()
        {
            try
            {

                var packet = new V45Packet(communication.GetMessage(5000));
                
                checkSenderandReceiver(packet);

                CheckStatusByte(packet);
                return packet;

                //communication.SendMessage(outPacket.toArray(), 5000);
                //checkSenderandReceiver(outPacket);

                //CheckStatusByte(outPacket);
                //return outPacket;

            }
            catch (Exception)
            {

                throw;
            }
        }




        internal Dictionary<object, MeasurePointDefinition> MapMeasurepointDefinitions(Dictionary<object, MeasurePointDefinition> definitions)
        {
            //definitions = null; //TODO fix caching
            if (definitions == null)
            {
                this.measurepointDefinitions = new Dictionary<object, MeasurePointDefinition>();
                UInt16[] availablemeasurepoints = GetAvailableMeasurePoints();
                for (int i = 0; i < availablemeasurepoints.Length; i++)
                {
                    try
                    {
                        var mpd = GetMeasurePointInformation(availablemeasurepoints[i]);
                        measurepointDefinitions[mpd.Key] = mpd;
                    }
                    catch
                    {
                        //Log.ErrorFormat("{0} failed", i);
                    }
                }
                foreach (TSettingsId settingId in Enum.GetValues(typeof(TSettingsId)))
                {
                    ushort usId = (ushort)(V45Setting.MeasurementKeyOffset + ((ushort)settingId * 3));
                    for (ushort i = 0; i < 3; i++)
                    {
                        measurepointDefinitions[usId + i] = new MeasurePointDefinitionShortKey("", 1.0, 2, (ushort)(usId + i));
                    }
                }
            }
            else
            {
                this.measurepointDefinitions = definitions;
            }
            return this.measurepointDefinitions;
        }

        public UInt16[] GetAvailableMeasurePoints()
        {
            UInt16[] availablemeasurepoints;
            try
            {
                byte[] data = new byte[0];

                V45Packet cmdPack = new V45Packet(V45Packet.V45Commands.cmdGetAvailableMeasPoints, data, this.sender, this.receiver);

                V45Packet packet = SendMessage(cmdPack);
                
               UInt16 measurePointCount = BitConverter.ToUInt16(packet.Payload, 0);
               availablemeasurepoints = new UInt16[measurePointCount];
                for (int i = 0; i < measurePointCount; i++)
                {
                    availablemeasurepoints[i] = BitConverter.ToUInt16(packet.Payload, i * 2 + 2);
                }

                return availablemeasurepoints;
            }
            catch (DisconnectedException e)
            {
                if (communication.Connect())
                    return GetAvailableMeasurePoints();
                else
                    throw e;
            }
        }

        public MeasurePointDefinition GetMeasurePointInformation(ushort mpId)
        {
            int i = 0;
            V45Packet cmdPack = new V45Packet(V45Packet.V45Commands.cmdGetMeasPointInfo, BitConverter.GetBytes(mpId), this.sender, this.receiver);
            
            var packet = SendMessage(cmdPack);
            var inBytes = packet.Payload;
            ushort id = BitConverter.ToUInt16(inBytes, i);
            i += 2;
            ushort tmp16 = BitConverter.ToUInt16(inBytes, i);            
            double m_multiplier = Convert.ToDouble(tmp16) / 1000.0;
            i += 2;
            byte access = inBytes[i++];
            byte[] tmparray = inBytes.Skip(i).TakeWhile(t => t != 0).ToArray();// responsePack.Payload.Skip(4).ToArray();
            string mpName = System.Text.Encoding.UTF8.GetString(tmparray, 0, tmparray.Length);

            return new MeasurePointDefinitionShortKey(mpName, m_multiplier, tmp16, id);
        }


        public MeasurePointDefinition GetSettingInformation(ushort mpId)
        {
            int i = 0;
            V45Packet cmdPack = new V45Packet(V45Packet.V45Commands.cmdGetSettingInfo, BitConverter.GetBytes(mpId), this.sender, this.receiver);

            var packet = SendMessage(cmdPack);
            var inBytes = packet.Payload;
            ushort id = BitConverter.ToUInt16(inBytes, i);
            i += 2;
            ushort tmp16 = BitConverter.ToUInt16(inBytes, i);
            double m_multiplier = Convert.ToDouble(tmp16) / 1000.0;
            i += 2;
            byte access = inBytes[i++];
            byte[] tmparray = inBytes.Skip(i).TakeWhile(t => t != 0).ToArray();// responsePack.Payload.Skip(4).ToArray();
            string mpName = System.Text.Encoding.UTF8.GetString(tmparray, 0, tmparray.Length);

            return new MeasurePointDefinitionShortKey(mpName, m_multiplier, tmp16, id);
        }
        public int GetMeasurePointValue(ushort id)
        {
            byte[] data = new byte[2];

            int offset = 0;
            data.Add(ref offset, BitConverter.GetBytes(id));

            V45Packet cmdPack = new V45Packet(V45Packet.V45Commands.cmdGetMeasPointValue, data, this.sender, this.receiver);
            V45Packet ret = SendMessage(cmdPack);
            return BitConverter.ToInt32(ret.Payload, 2);
        }

        public int SetMeasurePointValue(ushort id, uint value)
        {
            byte[] data = new byte[6];

            int offset = 0;
            data.Add(ref offset, BitConverter.GetBytes(id));
            data.Add(ref offset, BitConverter.GetBytes(value));

            V45Packet cmdPack = new V45Packet(V45Packet.V45Commands.cmdSetMeasPointValue, data, sender, receiver);

            V45Packet ret = SendMessage(cmdPack);
            //return BitConverter.ToInt32(ret.Payload, 2);
            return 1;

            
        }

        public int SetCalibrationValue (byte type, byte cmd, short value)
        {
            byte[] data = new byte[4];

            int offset = 0;
            //data.Add(ref offset, BitConverter.GetBytes(type));
            //data.Add(ref offset, BitConverter.GetBytes(cmd));
            //data.Add(ref offset, BitConverter.GetBytes(value));
            data.Add(ref offset, type);
            data.Add(ref offset, cmd);
            data.Add(ref offset, BitConverter.GetBytes(value));
            V45Packet cmdPack = new V45Packet(V45Packet.V45Commands.cmdHandleCalibration, data, sender, receiver);
            
            V45Packet ret = SendMessage(cmdPack);
            return ret.Payload[0];
            //SendMessage(cmdPack);
            //if (ret.Payload[0]==0)
            //{
            //    return 0;
            //}
            //else
            //{
            //    return 1;
            //}
            //return BitConverter.ToInt32(ret.Payload, 2);
            //return BitConverter.ToUInt16(ret.Payload, 0);
        }

        public SubscriptionResult SubscribeToMeasurepoints(List<UInt16> measurePoints)
        {
            try
            {
                // TODO check count < 11 ?
                byte[] data = new byte[3 + (2 * measurePoints.Count)];

                data[0] = 1; // Start
                data[1] = 2; // Interval 4 ms
                data[2] = Convert.ToByte(measurePoints.Count);

                int i = 0;
                int offset = 3;
                while (i < 10 && i < measurePoints.Count)
                {
                    data.Add(ref offset, BitConverter.GetBytes(measurePoints[i]));
                    i++;
                }

                V45Packet cmdPack = new V45Packet(V45Packet.V45Commands.cmdHandleMeasurePointSub, data, this.sender, this.receiver);
                V45Packet responsePack = SendMessage(cmdPack);
                return new SubscriptionResult(responsePack.Payload);
            }
            catch (DisconnectedException e)
            {
                if (communication.Connect())
                    return SubscribeToMeasurepoints(measurePoints);
                else
                {
                    throw e;
                }
            }
        }

        public SubscriptionResult UnSubscribeToMeasurepoints(List<UInt16> measurePoints)
        {
            try
            {
                byte[] data = new byte[3 + (2 * measurePoints.Count)];

                data[0] = 3;
                data[1] = 1;
                data[2] = Convert.ToByte(measurePoints.Count);

                int i = 0;
                int offset = 3;
                while (i < 10 && i < measurePoints.Count)
                {
                    data.Add(ref offset, BitConverter.GetBytes(measurePoints[i]));
                    i++;
                }

                V45Packet cmdPack = new V45Packet(V45Packet.V45Commands.cmdHandleMeasurePointSub, data, this.sender, this.receiver);
                V45Packet responsePack = SendMessage(cmdPack);
                return new SubscriptionResult(responsePack.Payload);
            }
            catch (DisconnectedException e)
            {
                if (communication.Connect())
                    return UnSubscribeToMeasurepoints(measurePoints);
                else
                    throw e;
            }
        }

        public byte[] GetStream()
        {
            return new V45Packet(communication.GetMessageStreamComm()).Payload;
        }

        public AvailableSettings GetAvailableSettings()
        {
            AvailableSettings availablesettings = new AvailableSettings();

            V45Packet cmdPack = new V45Packet(V45Packet.V45Commands.cmdGetAvailableSettings, new byte[0], sender, receiver);
            var packet = SendMessage(cmdPack);

            availablesettings.Count = BitConverter.ToUInt16(packet.Payload, 0);
            availablesettings.Settings = new UInt16[availablesettings.Count];
            for (int i = 0; i < availablesettings.Count; i++)
            {                
                availablesettings.Settings[i] = BitConverter.ToUInt16(packet.Payload, i * 3 + 2);
                var tmp = packet.Payload[i * 3 + 3];
            }

            return availablesettings;
        }

        public ushort GetSettingValue(byte profile, byte type, ushort settingId)
        {
            byte[] data = new byte[4];

            int offset = 0;
            data.Add(ref offset, profile);
            data.Add(ref offset, BitConverter.GetBytes(settingId));
            data.Add(ref offset, type);
            V45Packet cmdPack = new V45Packet(V45Packet.V45Commands.cmdGetSetting, data, sender, receiver);
            var ret = SendMessage(cmdPack);
            return BitConverter.ToUInt16(ret.Payload, 4);
        }

        public struct ChangeSettingResult
        {
            public enum TChangeResult
            {
                Ack = 0,
                Nack = 1
            }

            public TChangeResult Result { get; set; }
            public byte Profile { get; set; }
            public UInt16 Setting { get; set; }
            public byte Type { get; set; }
            public UInt16 Value { get; set; }
            public UInt16 OldValue { get; set; }

            public ChangeSettingResult(byte[] inData)
                : this()
            {
                int i = 0;
                this.Result = (TChangeResult)inData[i++];
                this.Profile = inData[i++];
                this.Setting = BitConverter.ToUInt16(inData, i);
                i += 2;
                this.Type = inData[i++];
                this.Value = BitConverter.ToUInt16(inData, i);
                i += 2;
                this.OldValue = BitConverter.ToUInt16(inData, i);
            }
        }

        public struct ChangeRtcResult
        {
            
            public DateTime time { get; set; }
            public DateTime OldTime { get; set; }

            public ChangeRtcResult(byte[] inData)
                : this()
            {
                DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                int i = 0;
                this.time = epoch.AddSeconds(BitConverter.ToUInt32(inData, i));
                i += 4;
                epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                this.OldTime = epoch.AddSeconds(BitConverter.ToUInt32(inData, i));

            }
        }

        public struct GetRtcResult
        {

            public DateTime time { get; set; }

            public GetRtcResult(byte[] inData)
                : this()
            {
                this.time = new DateTime(2018, inData[0]+1, inData[1], inData[2]+1, inData[3], inData[4]);
            }
        }

        public ChangeSettingResult StepSetting(byte profile, byte type, ushort settingId, StepSettingDirection direction)
        {
            byte[] data = new byte[5];

            int offset = 0;
            data.Add(ref offset, profile);
            data.Add(ref offset, BitConverter.GetBytes(settingId));
            data.Add(ref offset, type);
            data.Add(ref offset, (byte)direction);
            V45Packet cmdPack = new V45Packet(V45Packet.V45Commands.cmdStepSetting, data, sender, receiver);

            var ret = SendMessage(cmdPack);
            ChangeSettingResult changesettingresult = new ChangeSettingResult(ret.Payload);

            return changesettingresult;
        }

        public ChangeSettingResult SetSetting(byte profile, byte type, ushort settingId, ushort value)
        {
            byte[] data = new byte[6];

            int offset = 0;
            data.Add(ref offset, profile);
            data.Add(ref offset, BitConverter.GetBytes(settingId));
            data.Add(ref offset, type);
            data.Add(ref offset, BitConverter.GetBytes(value));
            V45Packet cmdPack = new V45Packet(V45Packet.V45Commands.cmdSetSetting, data, sender, receiver);
            //var ret = SendMessage1(cmdPack);
            var ret = SendMessage(cmdPack);
            ChangeSettingResult changesettingresult = new ChangeSettingResult(ret.Payload);

            return changesettingresult;
        }

        public bool ChangeProfileName(byte ProfileId, string profileName)
        {
            byte[] data = new byte[13];
            for(int i=0;i<data.Length;i++)
            {
                data[i] = 0;
            }
            int offset = 0;
            data.Add(ref offset, ProfileId);
            data.Add(ref offset, System.Text.Encoding.UTF8.GetBytes(profileName));
            V45Packet cmdPack = new V45Packet(V45Packet.V45Commands.cmdSetProfileName, data, sender, receiver);
            var ret = SendMessage(cmdPack);

            return Convert.ToBoolean(ret.Payload[0]);
        }

        public ChangeRtcResult setRTC(byte year, byte month, byte day, byte hour, byte minute, byte sec)
        {
            byte[] data = new byte[6];

            int offset = 0;
            data.Add(ref offset, year);
            data.Add(ref offset, (byte)(month-1));
            data.Add(ref offset, day);
            data.Add(ref offset, hour);
            data.Add(ref offset, minute);
            data.Add(ref offset, 0);

            V45Packet cmdPack = new V45Packet(V45Packet.V45Commands.cmdSetRTC, data, sender, receiver);

            var ret = SendMessage(cmdPack);
            ChangeRtcResult changertcresult = new ChangeRtcResult(ret.Payload);

            return changertcresult;
        }


        public GetRtcResult getRTC()
        {
            byte[] data = new byte[0];
            V45Packet cmdPack = new V45Packet(V45Packet.V45Commands.cmdGetRTC, data, sender, receiver);

            var ret = SendMessage(cmdPack);
            GetRtcResult changertcresult = new GetRtcResult(ret.Payload);

            return changertcresult;
        }

        public int DiskCheck(byte target)
        {
            byte[] data = new byte[1];

            int offset = 0;
            data.Add(ref offset, target);


            V45Packet cmdPack = new V45Packet(V45Packet.V45Commands.cmdDiskTest, data, sender, receiver);
            var packet = SendMessage(cmdPack);

            return packet.Payload[0];
        }

        public int GetRTC(byte target)
        {
            byte[] data = new byte[0];

            int offset = 0;
            data.Add(ref offset, target);


            V45Packet cmdPack = new V45Packet(V45Packet.V45Commands.cmdGetRTC, data, sender, receiver);
            var packet = SendMessage(cmdPack);

            return packet.Payload[0];
        }

        public bool CopyProfile(byte sourceProfile, byte destinationProfile)
        {
            byte[] data = new byte[2];

            int offset = 0;
            data.Add(ref offset, sourceProfile);
            data.Add(ref offset, destinationProfile);

            V45Packet cmdPack = new V45Packet(V45Packet.V45Commands.cmdCopyProfile, data, sender, receiver);

            var ret = SendMessage(cmdPack);


            return Convert.ToBoolean(ret.Payload[0]);
        }

        public List<UInt16> GetAvailableDeviceInfo()
        {
             List<UInt16> deviceinfolist = new List<UInt16>();

            V45Packet cmdPack = new V45Packet(V45Packet.V45Commands.cmdGetAvailableDeviceInfo, new byte[0], sender, receiver);
            var packet = SendMessage(cmdPack);
            UInt16 count = BitConverter.ToUInt16(packet.Payload, 0);
            int j = 0;
            for (int i = 0; i < count; i++)
            {
                UInt16 v45i = BitConverter.ToUInt16(packet.Payload, i * 2 + 2);
                deviceinfolist.Add(v45i);
            }

            return deviceinfolist;
        }

        public Vivo45DeviceInfo GetDeviceInfoInformation(UInt16 id)
        {
            byte[] data = new byte[2];

            int offset = 0;
            data.Add(ref offset, BitConverter.GetBytes(id));

            V45Packet cmdPack = new V45Packet(V45Packet.V45Commands.cmdGetDeviceInfo, data, sender, receiver);
            var packet = SendMessage(cmdPack);

            Vivo45DeviceInfo v45i = new Vivo45DeviceInfo(packet.Payload);

            return v45i;
        }

        public string GetDeviceInfoValue(UInt16 id)
        {
            byte[] data = new byte[2];

            int offset = 0;
            data.Add(ref offset, BitConverter.GetBytes(id));

            V45Packet cmdPack = new V45Packet(V45Packet.V45Commands.cmdGetDeviceValue, data, sender, receiver);
            var packet = SendMessage(cmdPack);
            var rtnId = BitConverter.ToUInt16(packet.Payload, 0);
            var str = System.Text.Encoding.UTF8.GetString(packet.Payload, 2, packet.Payload.Skip(2).TakeWhile(s => s != 0).ToArray().Length);

            return str;
        }

        public Boolean SetDeviceInfoValue(UInt16 id, string str)
        {
            byte[] data = new byte[2 + System.Text.Encoding.UTF8.GetBytes(str).Length + 1]; //Add one for null termination

            for (int i = 0; i < data.Length; i++)
            {
                data[i] = 0;
            }

            int offset = 0;
            data.Add(ref offset, BitConverter.GetBytes(id));
            data.Add(ref offset, System.Text.Encoding.UTF8.GetBytes(str));

            V45Packet cmdPack = new V45Packet(V45Packet.V45Commands.cmdSetDeviceValue, data, sender, receiver);
            var packet = SendMessage(cmdPack);
            return Convert.ToBoolean(packet.Payload[0]);
        }

        public Boolean Screenshot(string str)
        {
            if (str.Length == 55)
            {
                str = str+"\n";
            }
            byte[] data = new byte[System.Text.Encoding.UTF8.GetBytes(str).Length + 1]; //Add one for null termination

            for (int i = 0; i < data.Length; i++)
            {
                data[i] = 0;
            }

            int offset = 0;
            data.Add(ref offset, System.Text.Encoding.UTF8.GetBytes(str));

            V45Packet cmdPack = new V45Packet(V45Packet.V45Commands.cmdScreenshoot, data, sender, receiver);
            var packet = SendMessage(cmdPack);
            return Convert.ToBoolean(packet.Payload[0]);
        }

        public Boolean BottonPress(UInt16 buttonNumber)
        {
            byte[] data = new byte[3]; 
            int offset = 0;
            data.Add(ref offset, BitConverter.GetBytes(buttonNumber));
            data.Add(ref offset, 1);//Pressed

            V45Packet cmdPack = new V45Packet(V45Packet.V45Commands.cmdButtonPress, data, sender, receiver);
            var packet = SendMessage(cmdPack);
            return Convert.ToBoolean(packet.Payload[0]);
        }

        public Boolean SetAlarm(byte alarm,byte active)
        {
            byte[] data = new byte[3];
            int offset = 0;
            data.Add(ref offset, alarm);
            data.Add(ref offset, active);//Pressed

            V45Packet cmdPack = new V45Packet(V45Packet.V45Commands.cmdSetAlarm, data, sender, receiver);
            var packet = SendMessage(cmdPack);
            return Convert.ToBoolean(packet.Payload[0]);
        }
        //
        //ChangeCalibration
        //EndCalibration
        //ReadCalibration
        //SetPassword


        public Boolean EnterTestState()
        {
            byte[] data = new byte[1];

            int offset = 0;
            data.Add(ref offset, Convert.ToByte(V45Packet.V45SystemStateRequests.pwrTEST_ON));


            V45Packet cmdPack = new V45Packet(V45Packet.V45Commands.cmdSetSystemState, data, sender, receiver);
            var packet = SendMessage(cmdPack);

            return Convert.ToBoolean(packet.Payload[0]);
        }

        public Boolean LeaveTestState()
        {
            byte[] data = new byte[1];

            int offset = 0;
            data.Add(ref offset, Convert.ToByte(V45Packet.V45SystemStateRequests.pwrTEST_OFF));


            V45Packet cmdPack = new V45Packet(V45Packet.V45Commands.cmdSetSystemState, data, sender, receiver);
            var packet = SendMessage(cmdPack);

            return Convert.ToBoolean(packet.Payload[0]);
        }

        public Boolean EndCalibration()
        {
            byte[] data = new byte[1];

            int offset = 0;
            data.Add(ref offset, Convert.ToByte(V45Packet.V45SystemStateRequests.pwrCALIB_OFF));


            V45Packet cmdPack = new V45Packet(V45Packet.V45Commands.cmdSetSystemState, data, sender, receiver);
            var packet = SendMessage(cmdPack);

            return Convert.ToBoolean(packet.Payload[0]);
        }

        public Boolean StartTempCompensation()
        {
            byte[] data = new byte[1];

            int offset = 0;
            data.Add(ref offset, Convert.ToByte(2));


            V45Packet cmdPack = new V45Packet(V45Packet.V45Commands.cmdHandleTempCompensation, data, sender, receiver);
            var packet = SendMessage(cmdPack);

            return Convert.ToBoolean(packet.Payload[0]);
        }

        public Boolean StopTempCompensation()
        {
            byte[] data = new byte[1];

            int offset = 0;
            data.Add(ref offset, Convert.ToByte(3));


            V45Packet cmdPack = new V45Packet(V45Packet.V45Commands.cmdHandleTempCompensation, data, sender, receiver);
            var packet = SendMessage(cmdPack);

            return Convert.ToBoolean(packet.Payload[0]);
        }

        public Boolean StartCalibration()
        {
            byte[] data = new byte[1];

            int offset = 0;
            data.Add(ref offset, Convert.ToByte( V45Packet.V45SystemStateRequests.pwrCALIB_ON));


            V45Packet cmdPack = new V45Packet(V45Packet.V45Commands.cmdSetSystemState, data, sender, receiver);
            var packet = SendMessage(cmdPack);

            return Convert.ToBoolean(packet.Payload[0]);
        }

        public Boolean StartTreatment()
        {
            byte[] data = new byte[1];

            int offset = 0;
            data.Add(ref offset, Convert.ToByte(V45Packet.V45SystemStateRequests.pwrSTART));


            V45Packet cmdPack = new V45Packet(V45Packet.V45Commands.cmdSetSystemState, data, sender, receiver);
            var packet = SendMessage(cmdPack);

            return Convert.ToBoolean(packet.Payload[0]);
        }

        public Boolean EraseLog()
        {
            byte[] data = new byte[1];

            V45Packet cmdPack = new V45Packet(V45Packet.V45Commands.cmdEraseLogs, data, sender, receiver);
            var packet = SendMessage(cmdPack);

            return Convert.ToBoolean(packet.Payload[0]);
        }

        public Boolean StopTreatment()
        {
            byte[] data = new byte[1];

            int offset = 0;
            data.Add(ref offset, Convert.ToByte(V45Packet.V45SystemStateRequests.pwrSTOP));


            V45Packet cmdPack = new V45Packet(V45Packet.V45Commands.cmdSetSystemState, data, sender, receiver);
            var packet = SendMessage(cmdPack);

            return Convert.ToBoolean(packet.Payload[0]);
        }


        public Boolean StartBuzzer()
        {
            byte[] data = new byte[6];

            int offset = 0;
            data.Add(ref offset, 0);
            data.Add(ref offset, 0);
            UInt32 time = 1000;
            data.Add(ref offset, BitConverter.GetBytes(time)); //Time
            //data.Add(ref offset, 5); //Volume

            V45Packet cmdPack = new V45Packet(V45Packet.V45Commands.cmdTestBuzzer, data, sender, receiver);
            var packet = SendMessage(cmdPack);

            return Convert.ToBoolean(packet.Payload[0]);
        }

        public Boolean StartSpeaker()
        {
            byte[] data = new byte[7];

            int offset = 0;
            data.Add(ref offset, 1); //Speaker
            data.Add(ref offset, 0); //Action 
            UInt32 time = 1000;
            data.Add(ref offset, BitConverter.GetBytes(time)); //Time
            data.Add(ref offset, 5); //Volume
            


            V45Packet cmdPack = new V45Packet(V45Packet.V45Commands.cmdTestBuzzer, data, sender, receiver);
            var packet = SendMessage(cmdPack);

            return Convert.ToBoolean(packet.Payload[0]);
        }

        public Boolean MuteSpeaker()
        {
            byte[] data = new byte[7];

            int offset = 0;
            data.Add(ref offset, 1); //Speaker
            data.Add(ref offset, 1); //Action 
            UInt32 time = 30000;
            data.Add(ref offset, BitConverter.GetBytes(time)); //Time
            data.Add(ref offset, 1); //Volume



            V45Packet cmdPack = new V45Packet(V45Packet.V45Commands.cmdTestBuzzer, data, sender, receiver);
            var packet = SendMessage(cmdPack);

            return Convert.ToBoolean(packet.Payload[0]);
        }

        public List<Vivo45UsageData> GetUsageData()
        {
            List<Vivo45UsageData> usageDataList = new List<Vivo45UsageData>();
            byte[] data = new byte[1];
            int offset = 0;
            data.Add(ref offset, 10); //Max Event

            V45Packet cmdPack = new V45Packet(V45Packet.V45Commands.cmdGetUsageData, data, sender, receiver);
            var packet = SendMessage(cmdPack);
            UInt16 count = BitConverter.ToUInt16(packet.Payload, 0);
            int j = 2;

            for (int i = 0; i < count; i++)
            {
                usageDataList.Add(new Vivo45UsageData(packet.Payload.Skip(j).ToArray()));

                j += usageDataList[i].size;
            }

            return usageDataList;
        }


        public List<Vivo45Alarm> GetActiveAlarms(AlarmType ErrorType)
        {
            List<Vivo45Alarm> ActiveAlarms = new List<Vivo45Alarm>();
            byte[] data = new byte[1];
            data[0] = Convert.ToByte(ErrorType);

            V45Packet cmdPack = new V45Packet(V45Packet.V45Commands.cmdGetActiveAlarms, data, sender, receiver);
            var packet = SendMessage(cmdPack);
            int j = 0;
            AlarmType type = (AlarmType)packet.Payload[j++];
            byte count = packet.Payload[j++];

            for (int i = 0; i < count; i++)
            {
                ActiveAlarms.Add(new Vivo45Alarm(packet.Payload.Skip(j).ToArray()));
                j += ActiveAlarms[i].size;
            }

            return ActiveAlarms;
        }

     

        private UInt32 GetEpochSecondsFromDateTime(DateTime timestamp)
        {
            TimeSpan epochtime = timestamp - Device.Epoch;
            return Convert.ToUInt32(epochtime.TotalSeconds);
        }

        private V45Packet HandleLogData(Vivo45LogPacket.Vivo45LogAction action, Vivo45LogPacket.Vivo45LogLevel level, DateTime startTimestamp, DateTime endTimestamp, bool storeSync)
        {
            byte[] data = new byte[11];

            int offset = 0;
            data.Add(ref offset, Convert.ToByte(action));
            data.Add(ref offset, Convert.ToByte(level));
            data.Add(ref offset, BitConverter.GetBytes(GetEpochSecondsFromDateTime(startTimestamp)));
            data.Add(ref offset, BitConverter.GetBytes(GetEpochSecondsFromDateTime(endTimestamp)));
            data.Add(ref offset, Convert.ToByte(storeSync));

            V45Packet cmdPack = new V45Packet(V45Packet.V45Commands.cmdHandleLogData, data, sender, receiver);
            var packet = SendMessage(cmdPack);

            return packet;
        }

        private V45Packet HandleFileTransfer(String fileName,Byte option, byte[] tempdata)
        {
            byte[] data;
            if (option < 4)
                data = new byte[1];
            else
                data = new byte[1+ tempdata.Length];
            //byte[] data = new byte[fileName.Length];

            int offset = 0;
            data.Add(ref offset, option);
            if(option >= 4)
            data.Add(ref offset, tempdata);
            //data.Add(ref offset, Convert.ToByte(fileName));


            V45Packet cmdPack = new V45Packet(V45Packet.V45Commands.cmdSendFile, data, sender, receiver);
            var packet = SendMessage(cmdPack);

            return packet;
        }

        public bool StartLogDownload(Vivo45LogPacket.Vivo45LogLevel level, DateTime startTimestamp, DateTime endTimestamp, bool storeSync)
        {
            bool rtn = false;

            V45Packet cmdResponse = HandleLogData(Vivo45LogPacket.Vivo45LogAction.sendLog, level, startTimestamp, endTimestamp, storeSync);
            if ((Vivo45LogPacket.Vivo45LogAction)cmdResponse.Payload[0] == Vivo45LogPacket.Vivo45LogAction.sendLog &&
                (Vivo45LogPacket.Vivo45LogLevel)cmdResponse.Payload[1] == level)
            {
                rtn = true;
            }

            return rtn;
        }

        public bool StartUpload(String fileName, byte option)
        {
            bool rtn = false;

            V45Packet cmdResponse = HandleFileTransfer(fileName, option, null);
            if (cmdResponse.Status == 0x80)
            {
                rtn = true;
            }

            return rtn;
        }

        public byte ContinueUpload(String fileName, byte option, byte[] tempdata)
        {
            V45Packet cmdResponse = HandleFileTransfer(fileName, option, tempdata);

            return cmdResponse.Status;
        }

        public Vivo45LogPacket GetNextLogPacket(Vivo45LogPacket.Vivo45LogLevel level)
        {
            V45Packet cmdResponse = HandleLogData(Vivo45LogPacket.Vivo45LogAction.continueLog, level, Device.Epoch, Device.Epoch, false);
            return new Vivo45LogPacket(cmdResponse.Payload, cmdResponse.Status);
        }

        public V45Packet StopLogDownload(Vivo45LogPacket.Vivo45LogLevel level)
        {
            V45Packet cmdResponse = HandleLogData(Vivo45LogPacket.Vivo45LogAction.stopLog, level, Device.Epoch, Device.Epoch, false);
            return cmdResponse;
        }

        public Boolean UpgradeTreatmentFW()
        {
            byte[] data = new byte[1];



            V45Packet cmdPack = new V45Packet(V45Packet.V45Commands.cmdUpdateTreatmentFW, data, sender, receiver);
            var packet = SendMessage(cmdPack);

            return true;// Convert.ToBoolean(packet.Payload[0]);
        }

        public Boolean deleteCalibrationDB ()
        {
            byte[] data = new byte[1];

            V45Packet cmdPack = new V45Packet(V45Packet.V45Commands.cmdDeleteCalDatabase, data, sender, receiver);
            var packet = SendMessage(cmdPack);

            return true;// Convert.ToBoolean(packet.Payload[0]);
        }

        public Boolean deleteOtherDBs()
        {
            byte[] data = new byte[1];



            V45Packet cmdPack = new V45Packet(V45Packet.V45Commands.cmdDeleteDatabase, data, sender, receiver);
            var packet = SendMessage(cmdPack);

            return true;// Convert.ToBoolean(packet.Payload[0]);
        }

        public byte GetDeviceFWMode()
        {
            byte[] data = new byte[1];

            int offset = 0;
            //data.Add(ref offset, Convert.ToByte(V45Packet.V45SystemStateRequests.pwrSTART));


            V45Packet cmdPack = new V45Packet(V45Packet.V45Commands.cmdGetDeviceFWMode, data, sender, receiver);
            var packet = SendMessage(cmdPack);

            return packet.Payload[0];// Convert.ToBoolean(packet.Payload[0]);
        }

        public Boolean UpgradeTreatmentBL()
        {
            byte[] data = new byte[1];



            V45Packet cmdPack = new V45Packet(V45Packet.V45Commands.cmdUpdateTreatmentBL, data, sender, receiver);
            var packet = SendMessage(cmdPack);

            return true;// Convert.ToBoolean(packet.Payload[0]);
        }

        public Boolean PrepareEMMC()
        {
            byte[] data = new byte[1];



            V45Packet cmdPack = new V45Packet(V45Packet.V45Commands.cmdPrepareEMMC, data, sender, receiver);
            var packet = SendMessage(cmdPack);

            return true;// Convert.ToBoolean(packet.Payload[0]);
        }

        public Boolean UpgradeUboot()
        {
            byte[] data = new byte[1];

            V45Packet cmdPack = new V45Packet(V45Packet.V45Commands.cmdUpgradeUboot, data, sender, receiver);
            var packet = SendMessage(cmdPack);

            return true;// Convert.ToBoolean(packet.Payload[0]);
        }

        public Boolean UpgradeFirmware()
        {
            byte[] data = new byte[1];

            V45Packet cmdPack = new V45Packet(V45Packet.V45Commands.cmdUpgradeFirmware, data, sender, receiver);
            var packet = SendMessage(cmdPack);

            return true;// Convert.ToBoolean(packet.Payload[0]);
        }

        public Boolean GotoRecoverMode()
        {
            byte[] data = new byte[1];


            V45Packet cmdPack = new V45Packet(V45Packet.V45Commands.cmdGotoRecoverMode, data, sender, receiver);
            var packet = SendMessage(cmdPack);

            return true;// Convert.ToBoolean(packet.Payload[0]);
        }

        public Boolean RestartDevice()
        {
            byte[] data = new byte[1];


            V45Packet cmdPack = new V45Packet(V45Packet.V45Commands.cmdGotoNormalMode, data, sender, receiver);
            var packet = SendMessage(cmdPack);

            return true;// Convert.ToBoolean(packet.Payload[0]);
        }

        public bool SetEncryptionKey(byte[] key)
        {
            byte[] data = new byte[key.Length + sizeof(byte)];

            int offset = 0;
            data.Add(ref offset, Convert.ToByte(key.Length));
            data.Add(ref offset, key);

            V45Packet cmdPack = new V45Packet(V45Packet.V45Commands.cmdSetEncryptionKey, data, sender, receiver);
            var packet = SendMessage(cmdPack);

            return packet.Payload.Length == data.Length;
        }

        public byte[] GetChallengeString()
        {
            byte[] data = new byte[0];

            V45Packet cmdPack = new V45Packet(V45Packet.V45Commands.cmdGetChallengeString, data, sender, receiver);
            var packet = SendMessage(cmdPack);

            UInt16 count = BitConverter.ToUInt16(packet.Payload, 0);

            byte[] challengeString = new byte[count];
            Array.Copy(packet.Payload, sizeof(UInt16), challengeString, 0, challengeString.Length);

            return challengeString;
        }

        public bool VerifyChallengeString(byte[] challengeString)
        {
            byte[] data = new byte[challengeString.Length + sizeof(UInt16)];

            int offset = 0;
            data.Add(ref offset, BitConverter.GetBytes((UInt16)challengeString.Length));
            data.Add(ref offset, challengeString);

            V45Packet cmdPack = new V45Packet(V45Packet.V45Commands.cmdVerifyChallengeString, data, sender, receiver);
            var packet = SendMessage(cmdPack);

            return Convert.ToBoolean(packet.Payload[0]);
        }

        public void EndSession()
        {
            V45Packet cmdPack = new V45Packet(V45Packet.V45Commands.cmdEndSession, new byte[0], sender, receiver);
            SendMessage(cmdPack);
        }

        private void CheckStatusByte(V45Packet Status)
        {
            if (Status.AlarmAvailable)
            {
                RaiseAlarmAvailable();
            }

            //CheckEvents

            //CheckNack?
        }

        private void RaiseAlarmAvailable()
        {
            if (NewAlarmsAvailableNow != null)
                NewAlarmsAvailableNow();
        }


        public string ScanWifi(int networkId)
        {
            return "";
        }


        public string SendWifiCommand(string cmd)
        {
            byte[] data = new byte[System.Text.Encoding.UTF8.GetByteCount(cmd)+1]; //+1 for null termination

            int offset = 0;     
            data.Add(ref offset, System.Text.Encoding.UTF8.GetBytes(cmd)); 
            data.Add(ref offset, 0); //null termination



            V45Packet cmdPack = new V45Packet(V45Packet.V45Commands.cmdHandleWifi, data, sender, receiver);
            var packet = SendMessage(cmdPack);

            return System.Text.Encoding.UTF8.GetString(packet.Payload, 0, packet.PayloadLength); 
        }

        public bool StartWifiScan()
        {
            string result = SendWifiCommand("SCAN");

            if (result.StartsWith("OK"))
            {
                return true;
            }
            return false;
        }

        public scan_results GetWifiScanResult()
        {
            string result = SendWifiCommand("SCAN_RESULTS");
            scan_results scanResults = new scan_results(result);
            return scanResults;
        }

        public int AddWifiNetwork()
        {
            string result = SendWifiCommand("ADD_NETWORK");
            return Convert.ToInt32( result);
        }

        public networkList ListWifiNetworks()
        {
            string result = SendWifiCommand("LIST_NETWORKS");
            networkList networks = new networkList(result);
            return networks;
        }

        public bool enableWifiNetwork(int network)
        {
            string result = SendWifiCommand("ENABLE_NETWORK " + network.ToString());
            if (result.StartsWith("OK"))
            {
                return true;
            }
            return false;
        }

        public bool disableWifiNetwork(int network)
        {
            string result = SendWifiCommand("DISABLE_NETWORK " + network.ToString());
            if (result.StartsWith("OK"))
            {
                return true;
            }
            return false;
        }

        public bool selectWifiNetwork(int network)
        {
            string result = SendWifiCommand("SELECT_NETWORK " + network.ToString());
            if (result.StartsWith("OK"))
            {
                return true;
            }
            return false;
        }


        public bool removeWifiNetwork(int network)
        {
            string result = SendWifiCommand("REMOVE_NETWORK " + network.ToString());
            if (result.StartsWith("OK"))
            {
                return true;
            }
            return false;
        }

        public bool setWifiSSID(int network, string ssidName)
        {
            string result = SendWifiCommand("SET_NETWORK " + network.ToString() + " ssid \"" + ssidName + "\"");
            if (result.StartsWith("OK"))
            {
                return true;
            }
            return false;
        }

        public bool setWifiPassword(int network, string password)
        {
            string result = SendWifiCommand("SET_NETWORK " + network.ToString() + " psk \"" + password + "\"");
            if (result.StartsWith("OK"))
            {
                return true;
            }
            return false;
        }     

        public bool saveWifiConfig()
        {
            string result = SendWifiCommand("SAVE_CONFIG");
            if (result.StartsWith("OK"))
            {
                return true;
            }
            return false;
        }        

        public class wifiNetwork
        {
            public string bssid { get; set; }
            public Int32 frequency { get; set; }
            public Int32 signalLevel { get; set; }
            public string flags { get; set; }
            public string ssid { get; set; }

            public wifiNetwork()
            {
                bssid = "";
                frequency = 0;
                signalLevel = 0;
                flags = "";
                ssid = "";
            }

            public wifiNetwork(string inBssid, Int32 inFrequency, Int32 inSignalLevel, string inFlags, string inSsid)
            {
                bssid = inBssid;
                frequency = inFrequency;
                signalLevel = inSignalLevel;
                flags = inFlags;
                ssid = inSsid;
            }
        }

        public class scan_results
        {
            public List<wifiNetwork> wifiNetworks { get; set; }

            public scan_results()
            {
                wifiNetworks = new List<wifiNetwork>();
            }

            public scan_results(string inResult)
            {
                wifiNetworks = new List<wifiNetwork>();

                var strnetworks = inResult.Split('\n'); //allways starts with bssid / frequency / signal level / flags / ssid
                foreach (string str in strnetworks)
                {
                    if(!str.StartsWith("bssid"))
                    {
                        var strlist = str.Split('\t');
                        if (strlist.Length > 4)
                        {
                            wifiNetworks.Add(new wifiNetwork(strlist[0], Convert.ToInt32(strlist[1]), Convert.ToInt32(strlist[2]), strlist[3], strlist[4]));
                        }
                    }                    
                }
            }
        }
        
        public class internalNetwork
        {
            public Int32 networkId { get; set; }
            public string bssid { get; set; }
            public string flags { get; set; }
            public string ssid { get; set; }

            public internalNetwork()
            {
                networkId = 0;
                bssid = "";   
                flags = "";
                ssid = "";
            }

            public internalNetwork(Int32 inNetworkId, string inSsid, string inBssid, string inFlags)
            {
                networkId = inNetworkId;
                ssid = inSsid;
                bssid = inBssid;   
                flags = inFlags;                
            }
        }

        public class networkList
        {
            public List<internalNetwork> internalNetworks { get; set; }

            public networkList()
            {
                internalNetworks = new List<internalNetwork>();
            }

            public networkList(string inResult)
            {
                internalNetworks = new List<internalNetwork>();

                var strnetworks = inResult.Split('\n');  //Always starts with network id / ssid / bssid / flags
                foreach (string str in strnetworks)
                {
                    if (!str.StartsWith("network id"))
                    {
                        var strlist = str.Split('\t');
                        if (strlist.Length > 3)
                        {
                            internalNetworks.Add(new internalNetwork(Convert.ToInt32(strlist[0]), strlist[1], strlist[2], strlist[3]));
                        }
                    }
                }
            }


           
        }
    }
}