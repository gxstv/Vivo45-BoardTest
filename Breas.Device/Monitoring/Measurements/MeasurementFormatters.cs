namespace Breas.Device.Monitoring.Measurements
{
    public class MeasurementFormatters
    {
        public static string FormatOffableInteger(double input)
        {
            if (input == 0)
                return "Off";
            else
                return ((int)input).ToString();
        }

        public static string FormatOffableDecimal3_1(double input)
        {
            if (input == 0)
                return "Off";
            else
                return input.ToString("N1");
        }

        public static string FormatOnOff(double input)
        {
            if (input == 0)
                return "Off";
            else
                return "On";
        }

        public static string FormatInteger4(double input)
        {
            return ((int)input).ToString();
        }

        public static string FormatDoubleDecimal3_2(double input)
        {
            return input.ToString("N2");
        }

        public static string FormatDecimal3_1(double input)
        {
            return input.ToString("N1");
        }

        public static string FormatIe(double input)
        {
            ushort val = (ushort)input;
            int[] bcd = new int[] { val & 0x0F, (val >> 4) & 0x0F, (val >> 8) & 0x0F, (val >> 12) & 0x0F };
            double inhale = bcd[3] + bcd[2] / 10.0;
            double exhale = bcd[1] + bcd[0] / 10.0;
            if (inhale == 1 && exhale == 0)
            {
                inhale = 1;
                exhale = 0;
            }
            if (exhale <= 0)
                exhale = 1;
            return inhale + ":" + exhale;
        }

        public static string FormatFlowPattern(double input)
        {
            return input == 0 ? "Sqrd" : "Declr";
        }

        public static string FormatVentilatorMode(double input)
        {
            VentilationMode mode = (VentilationMode)(int)input;
            switch (mode)
            {
                case VentilationMode.Cpap:
                    return "CPAP";

                case VentilationMode.Volume:
                    return "Volume";

                case VentilationMode.Pressure:
                    return "Pressure";

                default:
                    return "Error";
            }
        }

        public static string FormatPatientMode(double input)
        {
            int intVal = (int)input;
            switch (intVal)
            {
                case 1:
                    return "Paediatric";

                case 0:
                    return "Adult";

                default:
                    return "Error";
            }
        }

        public static string FormatBreathMode(double input)
        {
            int intVal = (int)input;
            switch (intVal)
            {
                case 0:
                    return "Support";

                case 1:
                    return "Assist Control";

                case 2:
                    return "SIMV";

                default:
                    return "Error";
            }
        }

        public static string FormatCircuitMode(double input)
        {
            int intVal = (int)input;
            switch (intVal)
            {
                case 0:
                    return "Leakage";

                case 1:
                    return "Single";

                case 2:
                    return "Dual";

                default:
                    return "Error";
            }
        }
    }
}