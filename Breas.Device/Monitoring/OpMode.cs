using System;

namespace Breas.Device.Monitoring
{
    public enum OpMode
    {
        PSV,
        PSV_T,
        PCV,
        PCV_T,
        PCV_A,
        PCV_A_T,
        VCV,
        VCV_A,
        CPAP,
        PCV_SIMV,
        VCV_SIMV
    }

    public static class OpModeHelper
    {
        public static readonly OpMode[] CpapModes = new OpMode[] { OpMode.CPAP };
        public static readonly OpMode[] VolumeAndCpapModes = new OpMode[] { OpMode.CPAP, OpMode.VCV, OpMode.VCV_A, OpMode.VCV_SIMV };
        public static readonly OpMode[] TargetModes = new OpMode[] { OpMode.PCV_A_T, OpMode.PSV_T, OpMode.PCV_T };
        public static readonly OpMode[] NonTargetModes = new OpMode[] { OpMode.CPAP, OpMode.VCV, OpMode.VCV_A, OpMode.VCV_SIMV, OpMode.PCV_A, OpMode.PCV_SIMV, OpMode.PSV };
        public static readonly OpMode[] NonCpapModes = new OpMode[] { OpMode.PCV_A_T, OpMode.PSV_T, OpMode.PCV_T, OpMode.VCV, OpMode.VCV_A, OpMode.VCV_SIMV, OpMode.PCV_A, OpMode.PCV_SIMV, OpMode.PSV };

        public static OpMode? FindOpMode(VentilationMode? ventMode, BreathMode? breathMode, int inspTrig, int targetVol)
        {
            if (ventMode == null)
                return null;
            if (inspTrig == int.MaxValue || targetVol == int.MaxValue)
                return null;
            bool inspTriggerUnknown = inspTrig == -1;
            bool inspTriggerOff = inspTrig == 0;
            bool targetVolUnknown = targetVol == -1;
            bool targetVolOff = targetVol == 0;
            switch (ventMode)
            {
                case VentilationMode.Cpap:
                    return OpMode.CPAP;

                case VentilationMode.Volume:
                    return FindVolumeOpMode(breathMode, inspTriggerUnknown, inspTriggerOff);

                case VentilationMode.Pressure:
                    return FindPressureOpMode(breathMode, inspTriggerUnknown, inspTriggerOff, targetVolUnknown, targetVolOff);

                default:
                    throw new ArgumentException("Unknown ventilation mode: " + ventMode);
            }
        }

        private static OpMode? FindVolumeOpMode(BreathMode? breathMode, bool inspTriggerUnknown, bool inspTriggerOff)
        {
            if (breathMode == BreathMode.Simv) return OpMode.VCV_SIMV;

            if (inspTriggerUnknown) return null;

            return (inspTriggerOff ? OpMode.VCV : OpMode.VCV_A);
        }

        private static OpMode? FindPressureOpMode(BreathMode? breathMode, bool inspTriggerUnknown, bool inspTriggerOff, bool targetVolumeUnknown, bool targetVolumeOff)
        {
            if (breathMode == null)
            {
                return null;
            }

            switch (breathMode)
            {
                case BreathMode.Support:
                    if (targetVolumeUnknown) return null;

                    return (targetVolumeOff ? OpMode.PSV : OpMode.PSV_T);

                case BreathMode.AssistControl:
                    if (inspTriggerUnknown || targetVolumeUnknown)
                    {
                        return null;
                    }
                    if (inspTriggerOff)
                    {
                        return (targetVolumeOff ? OpMode.PCV : OpMode.PCV_T);
                    }
                    return (targetVolumeOff ? OpMode.PCV_A : OpMode.PCV_A_T);

                case BreathMode.Simv:
                    return OpMode.PCV_SIMV;

                default:
                    throw new ArgumentException("Unknown breath mode: " + breathMode);
            }
        }
    }
}