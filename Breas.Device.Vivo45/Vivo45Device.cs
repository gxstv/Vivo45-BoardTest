using Breas.Device.Communication;
using Breas.Device.DeviceTypes;
using Breas.Device.Finder;
using Breas.Device.Monitoring;
using Breas.Device.Monitoring.Alarms;
using Breas.Device.Monitoring.Measurements;
using Breas.Device.Vivo45.Messenger;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Breas.Device.Vivo45
{
    public class Vivo45Device : Breas.Device.Device, IMonitorDevice, ILogDevice
    {
        //private static readonly ILog Log = LogManager.GetLogger(typeof(Vivo45Device));

        private Vivo45CaptureHandler _captureHandler;
        private Vivo45Messenger messenger;
        private Vivo45Measurements _measurements;
        private Vivo45AlarmMonitor _alarmMonitor;


        public Breas.Device.Logs.ILogReader LogReader
        {
            get;
            private set;
        }

        public Dictionary<object, MeasurePointDefinition> MeasurePointDefinitions
        {
            get { return messenger.MeasurepointDefinitions; }
        }

        public ICaptureHandler CaptureHandler
        {
            get
            {
                return _captureHandler;
            }
        }

        public IDeviceMeasurements Measurements
        {
            get
            {
                return _measurements;
            }
        }

        public Vivo45Messenger Messenger
        {
            get { return messenger; }
        }

        public IAlarmMonitor AlarmMonitor
        {
            get { return _alarmMonitor; }
        }

        public bool Monitoring
        {
            get;
            set;
        }

        public override bool Heartbeat
        {
            get
            {
                if (Monitoring)
                    return Connected;
                return Communication.Heartbeat;
            }
        }

        public int getshift()
        {
            return 0;
        }

        public Vivo45Device(Product product, ICommunication communication)
            : base(product, communication)
        {
            this._captureHandler = new Vivo45CaptureHandler(this);
            this.messenger = new Vivo45Messenger(communication);
            this.LogReader = new Vivo45LogReader(this);
            this._measurements = new Vivo45Measurements();
            this._alarmMonitor = new Vivo45AlarmMonitor(this.messenger);
        }

        public override bool Initialize()
        {
            if (Communication.ResolverInfo is UsbResolverInfo)
            {
                (Communication.ResolverInfo as UsbResolverInfo).V45Endpoints = true;
            }
            if (string.IsNullOrEmpty(Communication.ResolverInfo.SerialNumber))
            {
                var connected = Connect();
                if (!connected)
                {
                    return false;
                }
                Communication.ResolverInfo.DeviceName = GetModel();
                Communication.ResolverInfo.SerialNumber = "Recover";// GetSerialNumber();
            }
            return true;
        }

        private ushort flowKey = 17;
        private ushort pressureKey = 14;
        private ushort volumeKey = 19;
        private ushort inhalingKey = 165;

        public bool StartMonitoring()
        {
            readSettings = false;
            Monitoring = true;
            //_alarmMonitor.Update();
            return messenger.StartCapturing(new List<MeasurePointDefinition>()
            {
                messenger.MeasurepointDefinitions[flowKey],
                messenger.MeasurepointDefinitions[pressureKey],
                messenger.MeasurepointDefinitions[volumeKey],
                messenger.MeasurepointDefinitions[inhalingKey]
            });
        }

        public bool StopMonitoring()
        {
            Monitoring = false;
            return true;
        }

        public Dictionary<object, MeasurePointDefinition> MapMeasurepointDefinitions(Dictionary<object, MeasurePointDefinition> definitions = null)
        {
            return messenger.MapMeasurepointDefinitions(definitions);
        }

        bool readSettings = false;

        public bool GetMeasurementPointValues(Dictionary<DeviceMeasurement, int> measurementValues)
        {
            try
            {
                foreach(var m in measurementValues.Keys.ToList())
                {
                    if (!(m is V45Setting))
                        measurementValues[m] = messenger.GetMeasurePointValue((ushort)m.Key);
                    else if (!readSettings)
                    {
                        var v45Setting = (V45Setting)m;
                        measurementValues[m] = messenger.GetSettingValue((byte)TProfileNumber.P1, (byte)v45Setting.TreatmentSettingType, v45Setting.DeviceSettingId);
                    }
                }
                readSettings = true;
                return true;
            }
            catch (Exception e)
            {
                Connected = false;
               // Log.Error("Error reading measurepoints", e);
            }
            return false;
        }

        public byte[] GetCaptureData()
        {
            try
            {
                return messenger.GetStream();
            }
            catch (Exception e)
            {
                Connected = false;
               // Log.Error("Error reading capture data", e);
            }
            return null;
        }

        public ICaptureData CreateEmptyCaptureData()
        {
            return new Vivo45CaptureData();
        }

        private string serialNumber;
        private string firmware;
        private string model;

        public string GetSerialNumber() 
        {
            return serialNumber ?? (serialNumber = Messenger.GetDeviceInfoValue(0));
        }

        public string GetFirmware() 
        {
            return firmware ?? (firmware = /*Messenger.GetDeviceInfoValue(1)*/"1.0.9");
        }

        public string GetModel() 
        {
            
            return model ?? (model = /*Messenger.GetDeviceInfoValue(1)*/"VIVO_45");
        }

        public override string ToString()
        {
            return this.Product.DisplayName;
        }

    }

}