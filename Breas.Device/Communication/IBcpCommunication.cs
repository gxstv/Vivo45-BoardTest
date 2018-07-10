using Breas.Device.Monitoring.Measurements;

namespace Breas.Device.Communication
{
    public interface IBcpCommunication : ICommunication
    {
        void FillMeasurementPointValues(int numPoints, int[] ids, int[] sizes, int[] results, out int resultsLen);

        byte[] GetCaptureData();

        int GetMeasurePointValue(ushort index);

        string GetStringValue(ushort index);

        void SetMeasurePointValue(ushort index, int value);

        void SetStringValue(ushort index, string value);

        bool StartFlowLog();

        bool StopFlowLog();

        string Version { get; }

        short GetNumMeasurePoints();

        MeasurePointDefinitionStringKey GetMeasurePointDefinition(ushort nativeId);

        string GetStringPointInfo(ushort nativeId);

        short GetNumStringPoints();

        void ClearBuffer();

        string GetVersion();
    }
}