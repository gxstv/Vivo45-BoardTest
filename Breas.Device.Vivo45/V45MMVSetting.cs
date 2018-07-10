using Breas.Device.Monitoring.Measurements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breas.Device.Vivo45
{
    /// <summary>
    /// Creates and contains V45 Min Max Value settings
    /// </summary>
    public class V45MMVSetting : V45Setting
    {
        public V45Setting ValueSetting { get; set; }

        public V45Setting MinSetting { get; set; }

        public V45Setting MaxSetting { get; set; }

        public V45MMVSetting(TSettingsId settingsId, string nameKey, Unit? unit, Func<double, string> formatter)
            : base(settingsId, nameKey, unit)
        {
            this.ValueSetting = new V45Setting(TreatmentSettingType.Value, nameKey, settingsId, unit, formatter);
            this.MinSetting = new V45Setting(TreatmentSettingType.MinLimit, nameKey, settingsId, unit, formatter);
            this.MaxSetting = new V45Setting(TreatmentSettingType.MaxLimit, nameKey, settingsId, unit, formatter);
        }

        public V45MMVSetting(TSettingsId settingsId, string nameKey, Unit? unit)
            : this(settingsId, nameKey, unit, null)
        {
        }
    }
}