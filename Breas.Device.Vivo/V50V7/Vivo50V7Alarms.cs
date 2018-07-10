﻿using Breas.Device.Monitoring.Alarms;

namespace Breas.Device.Vivo.V50V7
{
    public class Vivo50V7Alarms
    {
        public static readonly Alarm[] Alarms;

        static Vivo50V7Alarms()
        {
            Alarms = new Alarm[160];

            Alarms.Add(new Alarm(0, "ALARM_TXT_NO_ALARM", AlarmType.None));
            Alarms.Add(new Alarm(1, "ALARM_RAM", AlarmType.FunctionFail));
            Alarms.Add(new Alarm(2, "ALARM_MAIN_PRESSURE_SENSOR", AlarmType.FunctionFail));
            Alarms.Add(new Alarm(3, "ALARM_BACKUP_PRESSURE_SENSOR", AlarmType.FunctionFail));
            Alarms.Add(new Alarm(4, "ALARM_PILOT_PRESSURE_SENSOR", AlarmType.FunctionFail));
            Alarms.Add(new Alarm(5, "ALARM_EXP_PRESSURE_SENSOR", AlarmType.FunctionFail));
            Alarms.Add(new Alarm(6, "ALARM_OXY_PRESSURE_SENSOR", AlarmType.FunctionFail));
            Alarms.Add(new Alarm(7, "ALARM_FLOW_SENSOR", AlarmType.FunctionFail));
            Alarms.Add(new Alarm(8, "ALARM_EXP_FLOW_SENSOR", AlarmType.FunctionFail));
            Alarms.Add(new Alarm(9, "ALARM_SER_COMM_TO_UI", AlarmType.FunctionFail));
            Alarms.Add(new Alarm(10, "ALARM_FAN_HIGH_CURRENT", AlarmType.FunctionFail));
            Alarms.Add(new Alarm(11, "ALARM_FAN_SHUTDOWN", AlarmType.FunctionFail));
            Alarms.Add(new Alarm(12, "ALARM_UI_FAN_SHUTDOWN", AlarmType.FunctionFail));
            Alarms.Add(new Alarm(13, "ALARM_RTC_FAIL", AlarmType.FunctionFail));
            Alarms.Add(new Alarm(14, "ALARM_BEEPER_FAIL", AlarmType.FunctionFail));
            Alarms.Add(new Alarm(15, "ALARM_UI_BEEPER_FAIL", AlarmType.FunctionFail));
            Alarms.Add(new Alarm(16, "ALARM_BEEPER_VOLUME_FAIL", AlarmType.FunctionFail));
            Alarms.Add(new Alarm(17, "ALARM_CPU_VOLT_FAIL", AlarmType.FunctionFail));
            Alarms.Add(new Alarm(18, "ALARM_PSU_VOLT_FAIL", AlarmType.FunctionFail));
            Alarms.Add(new Alarm(19, "ALARM_SETTINGS_CORRUPT_IN_RAM", AlarmType.FunctionFail));
            Alarms.Add(new Alarm(20, "ALARM_SETTINGS_CORRUPT_IN_FLASH", AlarmType.FunctionFail));
            Alarms.Add(new Alarm(21, "ALARM_ADC_SPI_COMM", AlarmType.FunctionFail));

            Alarms.Add(new Alarm(30, "ALARM_PRESS_SENSOR_PG1_PG2_EQUAL", AlarmType.FunctionFail));
            Alarms.Add(new Alarm(31, "ALARM_PRESSURE_SENSOR_TEMP_HIGH", AlarmType.FunctionFail));
            Alarms.Add(new Alarm(32, "ALARM_PRESSURE_SENSOR_TEMP_LOW", AlarmType.FunctionFail));
            Alarms.Add(new Alarm(33, "ALARM_BLDC_HIGH_TEMP", AlarmType.FunctionFail));
            Alarms.Add(new Alarm(34, "ALARM_BLDC_LOW_TEMP", AlarmType.FunctionFail));
            Alarms.Add(new Alarm(35, "ALARM_MAINS_POWER_SUPPLY_TEMP_HIGH", AlarmType.FunctionFail));
            Alarms.Add(new Alarm(36, "ALARM_MAINS_POWER_SUPPLY_TEMP_LOW", AlarmType.FunctionFail));
            Alarms.Add(new Alarm(37, "ALARM_SENSOR_CALIBRATION", AlarmType.FunctionFail));
            Alarms.Add(new Alarm(38, "ALARM_TEMP_COMPENSATION", AlarmType.FunctionFail));
            Alarms.Add(new Alarm(39, "ALARM_FAN_MOTOR_ERROR", AlarmType.FunctionFail));
            Alarms.Add(new Alarm(40, "ALARM_PATIENT_TEMP_LOW_V7", AlarmType.FunctionFail));

            Alarms.Add(new Alarm(50, "ALARM_UI_RAM", AlarmType.FunctionFail));
            Alarms.Add(new Alarm(51, "ALARM_UI_COOLING_FAN_V7", AlarmType.FunctionFail));
            Alarms.Add(new Alarm(52, "ALARM_UI_KEYS", AlarmType.FunctionFail));
            Alarms.Add(new Alarm(53, "ALARM_UI_DISPLAY", AlarmType.FunctionFail));
            Alarms.Add(new Alarm(54, "ALARM_UI_HIGH_PRESSURE", AlarmType.FunctionFail));
            Alarms.Add(new Alarm(55, "ALARM_NO_ALARM_ACK_V7", AlarmType.FunctionFail));

            Alarms.Add(new Alarm(64, "ALARM_TXT_POWER_FAIL", AlarmType.High));
            Alarms.Add(new Alarm(65, "ALARM_TXT_LOW_PRESSURE", AlarmType.High));
            Alarms.Add(new Alarm(66, "ALARM_TXT_HIGH_PRESSURE", AlarmType.High));
            Alarms.Add(new Alarm(67, "ALARM_TXT_LOW_MINUTE_VOLUME", AlarmType.High));
            Alarms.Add(new Alarm(68, "ALARM_TXT_LOW_TIDAL_VOLUME", AlarmType.High));
            Alarms.Add(new Alarm(69, "ALARM_TXT_LOW_BREATH_RATE", AlarmType.High));
            Alarms.Add(new Alarm(70, "ALARM_TXT_APNEA", AlarmType.High));
            Alarms.Add(new Alarm(71, "ALARM_TXT_LOW_FIO2", AlarmType.High));
            Alarms.Add(new Alarm(72, "ALARM_TXT_LOW_PULSE", AlarmType.High));
            Alarms.Add(new Alarm(73, "ALARM_TXT_HIGH_LEAKAGE", AlarmType.High));
            Alarms.Add(new Alarm(74, "ALARM_TXT_LOW_SPO2", AlarmType.High));
            Alarms.Add(new Alarm(75, "ALARM_TXT_HIGH_EtCO2", AlarmType.High));
            Alarms.Add(new Alarm(76, "ALARM_TXT_HIGH_InspCO2", AlarmType.High));
            Alarms.Add(new Alarm(77, "ALARM_TXT_DISCONNECT_FiO2", AlarmType.High));
            Alarms.Add(new Alarm(78, "ALARM_TXT_DISCONNECT_CO2", AlarmType.High));
            Alarms.Add(new Alarm(79, "ALARM_TXT_DISCONNECT_SpO2", AlarmType.High));
            Alarms.Add(new Alarm(80, "ALARM_TXT_PERFUSION_ARTIFACT_SpO2", AlarmType.High));
            Alarms.Add(new Alarm(81, "ALARM_TXT_CO2_CHECK_ADAPTER", AlarmType.High));
            Alarms.Add(new Alarm(82, "ALARM_TXT_CO2_UNSPEC_ACC", AlarmType.High));
            Alarms.Add(new Alarm(83, "ALARM_CO2_SENSOR_ERROR", AlarmType.High));

            Alarms.Add(new Alarm(96, "ALARM_TXT_HIGH_MINUTE_VOLUME", AlarmType.Medium));
            Alarms.Add(new Alarm(97, "ALARM_TXT_HIGH_TIDAL_VOLUME", AlarmType.Medium));
            Alarms.Add(new Alarm(98, "ALARM_TXT_PAT_HIGH_TEMP", AlarmType.Medium));
            Alarms.Add(new Alarm(99, "ALARM_TXT_LOW_PEEP", AlarmType.Medium));
            Alarms.Add(new Alarm(100, "ALARM_TXT_HIGH_PEEP", AlarmType.Medium));
            Alarms.Add(new Alarm(101, "ALARM_TXT_HIGH_PULSE", AlarmType.Medium));
            Alarms.Add(new Alarm(102, "ALARM_TXT_HIGH_FIO2", AlarmType.Medium));
            Alarms.Add(new Alarm(103, "ALARM_TXT_5VISO_FAIL", AlarmType.Medium));
            Alarms.Add(new Alarm(104, "ALARM_TXT_HIGH_SPO2", AlarmType.Medium));
            Alarms.Add(new Alarm(105, "ALARM_TXT_LOW_INT_BAT", AlarmType.Medium));
            Alarms.Add(new Alarm(106, "ALARM_TXT_LOW_ALARM_TXT_ACK", AlarmType.Medium));
            Alarms.Add(new Alarm(107, "ALARM_TXT_HIGH_BREATH_RATE", AlarmType.Medium));
            Alarms.Add(new Alarm(108, "ALARM_TXT_LOW_LEAKAGE", AlarmType.Medium));
            Alarms.Add(new Alarm(109, "ALARM_TXT_LEDS_TEST", AlarmType.Medium));
            Alarms.Add(new Alarm(110, "ALARM_TXT_AMBIENT_PRESSURE", AlarmType.Medium));
            Alarms.Add(new Alarm(111, "ALARM_TXT_LOW_EtCO2", AlarmType.Medium));
        }
    }
}