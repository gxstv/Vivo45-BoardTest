using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Breas.Device;
using Breas.Device.Finder;
using Breas.Device.Vivo;
using Breas.Device.Vivo45;
using Breas.Device.Vivo45.Messenger;
using Breas.Device.Finder.Windows.Usb;
using Breas.Device.Monitoring.Measurements;

namespace VIVO_45_Board_Test
{
    class DeviceUnderTest
    {
        DeviceHandler handler;
        Vivo45Device v45;
        Dictionary<string, int> measurePointDescriptions;
        Dictionary<string, int> mpDeviceInfo;
        Dictionary<string, int> mpSettings;

        public DeviceUnderTest()
        {
            measurePointDescriptions = new Dictionary<string, int>();
            mpDeviceInfo = new Dictionary<string, int>();
            v45 = null;
        }

        public bool IsVivo45Connected()
        {
            IDeviceFinder[] finders = new IDeviceFinder[]
            {
                new LibUsbDeviceFinder()
            };
            handler = new DeviceHandler(Vivo45Product.All, finders);
            handler.DeviceFound += V45DeviceFound;
            handler.DeviceLost += V45DeviceLost;

            List<Device> deviceList = handler.FindDevices();
            if (deviceList.Count < 1)
            {
                return false;
            }
            return true;
        }

        private void V45DeviceFound(Device device)
        {
            if (device is Vivo45Device)
            {
                if (device is Vivo45Device)
                {
                    v45 = (Vivo45Device)device;
                    if (!v45.Connected)
                    {
                        v45.Connect();
                    }
                    else
                    {
                        //TBD BRE feedback
                    }
                }
            }
        }

        public void Disconnect()
        {
            if(v45 != null)
            {
                v45.Disconnect();
            }
        }

        private void V45DeviceLost(Device device)
        {
            //MessageBox.Show("device lost");
        }

        public bool ResetMPList()
        {
            //Reset the MP description list
            measurePointDescriptions = new Dictionary<string, int>();

            //Check to see that given device exists and is connected
            if (v45 == null)
            {
                return false;
            }

            if (!v45.Connected)
            {
                return false;
            }

            //Try to loop through and get all text descriptions
            try
            {
                v45.Messenger.EndCalibration();
                UInt16[] availablemplist = v45.Messenger.GetAvailableMeasurePoints();
                foreach (UInt16 i in availablemplist)
                {
                    MeasurePointDefinition measurepointinfo =
                        (MeasurePointDefinition)v45.Messenger.GetMeasurePointInformation(i);
                    measurePointDescriptions.Add(measurepointinfo.ToString(), i);
                }
            }
            //If at any point connection is interrupted, return false
            catch
            {
                return false;
            }
            return true;
        }

        public bool ResetDeviceInfoList()
        {
            //Reset the device information list
            mpDeviceInfo = new Dictionary<string, int>();

            //Check to see that given device exists and is connected
            if (v45 == null)
            {
                return false;
            }

            if (!v45.Connected)
            {
                return false;
            }

            //Try to loop through and get all text descriptions
            try
            {
                v45.Messenger.EndCalibration();
                List<UInt16> availableDevInfo = v45.Messenger.GetAvailableDeviceInfo();
                foreach (UInt16 i in availableDevInfo)
                {
                    Vivo45DeviceInfo devInfo =
                        (Vivo45DeviceInfo)v45.Messenger.GetDeviceInfoInformation(i);
                    mpDeviceInfo.Add(devInfo.ExplanatoryText, devInfo.Id);
                }
            }
            //If at any point connection is interrupted, return false
            catch
            {
                return false;
            }
            return true;
        }

        public bool ResetSettingsList()
        {
            //Reset the MP settings list
            mpSettings = new Dictionary<string, int>();

            //Check to see that given device exists and is connected
            if (v45 == null)
            {
                return false;
            }

            if (!v45.Connected)
            {
                return false;
            }

            //Try to loop through and get all text descriptions
            try
            {
                ushort[] availableSettingsList = v45.Messenger.GetAvailableSettings().Settings;
                foreach (ushort i in availableSettingsList)
                {
                    MeasurePointDefinition measurepointinfo =
                        (MeasurePointDefinition)v45.Messenger.GetSettingInformation(i);
                    mpSettings.Add(measurepointinfo.ToString(), i);
                }
            }
            //If at any point connection is interrupted, return false
            catch
            {
                return false;
            }
            return true;
        }

        public bool SetMP(string mpDescription, int value)
        {
            int mpIdx;
            if (!measurePointDescriptions.TryGetValue(mpDescription, out mpIdx))
            {
                //System.Windows.Forms.MessageBox.Show("Mp not found: " + mpDescription);
                return false;
            }

            try
            {
                v45.Messenger.SetMeasurePointValue((ushort)mpIdx, (uint)value);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public int GetMpValue(string mpDescription)
        {
            int mpIdx;
            int mpValue = -1;
            if (measurePointDescriptions.TryGetValue(mpDescription, out mpIdx))
            {
                try
                {
                    mpValue = v45.Messenger.GetMeasurePointValue((ushort)mpIdx);
                }
                catch
                {
                    mpValue = -1;
                }
            }
            
            return mpValue;
        }

        public bool SetMpSetting(string mpDescription, int value)
        {
            int mpIdx;
            if (!mpSettings.TryGetValue(mpDescription, out mpIdx))
            {
                return false;
            }

            try
            {
                v45.Messenger.SetSetting(0, 0, (ushort)mpIdx, (ushort)value);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public string GetMpDeviceInfo(string mpDescription)
        {
            int mpIdx;
            string mpValue = "";

            if(mpDeviceInfo.TryGetValue(mpDescription, out mpIdx))
            {
                try
                {
                    mpValue = v45.Messenger.GetDeviceInfoValue((ushort)mpIdx);
                }
                catch
                {
                    mpValue = "";
                }
            }

            return mpValue;
        }

        public bool StartCalibration()
        {
            try
            {
                return v45.Messenger.StartCalibration();
            }
            catch
            {
                return false;
            }
        }

        public bool EndCalibration()
        {
            try
            {
                return v45.Messenger.EndCalibration();
            }
            catch
            {
                return false;
            }
        }

        public void StartBuzzer()
        {
            v45.Messenger.StartBuzzer();
        }

        public void StartSpeaker()
        {
            v45.Messenger.StartSpeaker();
        }

        public List<Vivo45Alarm> GetActiveAlarm(Breas.Device.Monitoring.Alarms.AlarmType type)
        {

            List<Vivo45Alarm> result;
            result = v45.Messenger.GetActiveAlarms(type);


            return result;
        }

        public int MemoryTest(byte target)
        {
            int result;
            try
            {
                result = v45.Messenger.DiskCheck(target);
            }
            catch
            {
                result = -1 ;
            }

            return result;
        }

        public bool WifiTest()
        {
            bool result;
            try
            {
                result = v45.Messenger.StartWifiScan();
            }
            catch
            {
                result = false;
            }

            return result;
        }


        public DateTime RTCTest()
        {
            DateTime result;
            try
            {
                return v45.Messenger.getRTC().time;
            }
            catch
            {
                result = new DateTime();
            }

            return result;
        }
    }
}
