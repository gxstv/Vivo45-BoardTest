using Breas.Device.Communication;
using Breas.Device.DeviceTypes;
using Breas.Device.Monitoring;
using Breas.Device.Monitoring.Alarms;
using Breas.Device.Monitoring.Measurements;
using Breas.Device.Vivo.Messenger;
using System;
using System.Collections.Generic;

namespace Breas.Device.Vivo
{
    /// <summary>
    /// The Vivo50/Vivo60/Vivo50US share a lot of functionality so they all use this class for their device
    /// </summary>
    public class VivoDevice : Breas.Device.Device, IMonitorDevice
    {
        public const string CaptureActiveKey = "CaptureActive";
        public const string SerialNumberKey = "SerialNo";
        public const int SystemStateActiveId = 3;

        private static readonly SystemState[] systemStates = new SystemState[]
        {
            new SystemState(0, false, "SYSTEM_STATE_STANDBY"),
            new SystemState(1, false, "SYSTEM_STATE_OFF_CHARGING"),
            new SystemState(2, false, "SYSTEM_STATE_STANDBY"),
            new SystemState(SystemStateActiveId, false, "SYSTEM_STATE_ACTIVE"),
            new SystemState(4, true, "SYSTEM_STATE_POWER_FAIL"),
            new SystemState(5, true, "SYSTEM_STATE_FUNCTION_FAIL_OFF"),
            new SystemState(6, false, "SYSTEM_STATE_FUNCTION_FAIL_STANDBY"),
            new SystemState(7, false, "SYSTEM_STATE_FUNCTION_FAIL_ACTIVE"),
            new SystemState(8, true, "SYSTEM_STATE_SHUTDOWN"),
            new SystemState(9, false, "SYSTEM_STATE_RESTART"),
            new SystemState(10, false, "SYSTEM_STATE_CALIB"),
            new SystemState(11, false, "SYSTEM_STATE_TEMP_COMPENSATION"),
        };

        private VivoAlarmMonitor _alarmMonitor;
        private VivoCaptureHandler _captureHandler;
        private VivoMeasurements _measurements;

        public SystemState[] SystemStates
        {
            get
            {
                return systemStates;
            }
        }

        public Dictionary<object, MeasurePointDefinition> MeasurePointDefinitions
        {
            get
            {
                return Messenger.MeasurePointDefinitionMap;
            }
        }

        public ICaptureHandler CaptureHandler
        {
            get { return _captureHandler; }
        }

        public IDeviceMeasurements Measurements
        {
            get { return _measurements; }
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
                return Monitoring ? Connected : Messenger.GetVersion() != string.Empty;
            }
        }

        public VivoMessenger Messenger { get; private set; }

        public VivoDevice(Product product, ICommunication communication)
            : base(product, communication)
        {
            _captureHandler = new VivoCaptureHandler(this);
            this.Messenger = new VivoMessenger(communication);
            this.Product = product;
            this._measurements = VivoMeasurements.Measurements[Product];
            this._alarmMonitor = new VivoAlarmMonitor(Product,
                                                 _measurements.AlarmMask0_31,
                                                 _measurements.AlarmMask32_63,
                                                 _measurements.AlarmMask64_95,
                                                 _measurements.AlarmMask96_127,
                                                 _measurements.AlarmMask128_159);
        }

        public override bool Initialize()
        {
            base.Initialize();
            if (Communication.ResolverInfo.DeviceName == null || Communication.ResolverInfo.SerialNumber == null || Product == null)
            {
                if (!Connected)
                {
                    if (!Connect())
                    {
                        return false;
                    }
                }
                var resolverInfo = Communication.ResolverInfo;
                string version = Messenger.GetVersion();
                int spaceIndex = version.LastIndexOf(' ');
                if (spaceIndex == -1)
                {
                    spaceIndex = version.Length;
                }
                version = version.Substring(0, spaceIndex);
                int interfaceVersion = Messenger.GetMeasurePointValue(0);
                resolverInfo.DeviceName = version;
                resolverInfo.SerialNumber = Messenger.GetStringValue(0);
                Product = VivoProducts.GetRealVivoProduct(interfaceVersion, version);
                if (Product == null)
                {
                    return false;
                }
                _measurements = VivoMeasurements.Measurements[Product];
                _alarmMonitor = new VivoAlarmMonitor(Product,
                                                     _measurements.AlarmMask0_31,
                                                     _measurements.AlarmMask32_63,
                                                     _measurements.AlarmMask64_95,
                                                     _measurements.AlarmMask96_127,
                                                     _measurements.AlarmMask128_159);
            }
            return true;
        }

        public bool StartMonitoring()
        {
            StopMonitoring();
            if (Messenger.StartFlowLog())
            {
                //this controls the flow log capture rate on the device as well
                //as tells the device to start the flow log
                Messenger.SetMeasurePointValue(CaptureActiveKey, 1);
                Monitoring = true;
                return true;
            }
            return false;
        }
        public int getshift()
        {
            
            return Messenger.getshift(); 
        }

        public bool StopMonitoring()
        {
            Messenger.SetMeasurePointValue(CaptureActiveKey, 0);
            Messenger.StopFlowLog();
            Monitoring = false;
            return true;
        }

        public Dictionary<object, MeasurePointDefinition> MapMeasurepointDefinitions(Dictionary<object, MeasurePointDefinition> definitions = null)
        {
            return Messenger.MapMeasurePointDefinitions(definitions);
        }

        public bool GetMeasurementPointValues(Dictionary<DeviceMeasurement, int> measurementValues)
        {
            return Messenger.GetMeasurementPointValues(measurementValues);
        }

        public String GetString(ushort index)
        {
            return Messenger.GetStringValue(index);
        }

        public byte[] GetCaptureData()
        {
            try
            {
                return Messenger.GetCaptureData();
            }
            catch
            {
                Connected = false;
                return null;
            }
        }

        public ICaptureData CreateEmptyCaptureData()
        {
            return new VivoCaptureData(0, 0, 0, 0, 0, 0, 0, 0);
        }

        //public override string ToString()
        //{
        //    return this.Product.DisplayName;
        //}
    }
}