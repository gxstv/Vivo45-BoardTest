using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VIVO_45_Board_Test
{
    public partial class FixtureConfigForm : Form
    {
        int mainsDcIdx;
        int extDcIdx;
        int extBattIdx;
        int intBattIdx;

        int mainScopeIdx;
        int speakerScope1Idx;
        int speakerScope2Idx;
        int supercapHiScopeIdx;
        int supercapMidScopeIdx;
        int supercapReturnScopeIdx;

        int primaryOutIdx;
        int secondaryOutIdx;

        public FixtureConfigForm()
        {
            InitializeComponent();
        }

        public int MainsDcIdx { get { return mainsDcIdx; } }
        public int ExtDcIdx { get { return extDcIdx; } }
        public int ExtBattIdx { get { return extBattIdx; } }
        public int IntBattIdx { get { return intBattIdx; } }

        public int MainScopeIdx { get { return mainScopeIdx; } }
        public int SpeakerScope1Idx { get { return speakerScope1Idx; } }
        public int SpeakerScope2Idx { get { return speakerScope2Idx; } }
        public int SupercapHiScopeIdx { get { return supercapHiScopeIdx; } }
        public int SupercapMidScopeIdx { get { return supercapMidScopeIdx; } }
        public int SupercapReturnScopeIdx { get { return supercapReturnScopeIdx; } }

        public int PrimaryOutIdx { get { return primaryOutIdx; } }
        public int SecondaryOutIdx { get { return secondaryOutIdx; } }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            List<int> psChannelList = new List<int>();
            psChannelList.Add(comboBoxMainsDC.SelectedIndex);
            psChannelList.Add(comboBoxExtDC.SelectedIndex);
            psChannelList.Add(comboBoxExtBatt.SelectedIndex);
            psChannelList.Add(comboBoxIntBatt.SelectedIndex);

            List<int> scopeChannelList = new List<int>();
            scopeChannelList.Add(comboBoxMainScope.SelectedIndex);
            scopeChannelList.Add(comboBoxSpeakerScope1.SelectedIndex);
            scopeChannelList.Add(comboBoxSpeakerScope2.SelectedIndex);
            scopeChannelList.Add(comboBoxSupercapHiScope.SelectedIndex);
            scopeChannelList.Add(comboBoxSupercapMidScope.SelectedIndex);
            scopeChannelList.Add(comboBoxSupercapReturnScope.SelectedIndex);

            List<int> outputChannelList = new List<int>();
            outputChannelList.Add(comboBoxPrimaryOut.SelectedIndex);
            outputChannelList.Add(comboBoxSecondaryOut.SelectedIndex);

            //Check to make sure all combo boxes are selected and unique
            if (psChannelList.Contains(-1))
            {
                MessageBox.Show("Please select an entry for all power source channels");
            }
            else if(psChannelList.Count != psChannelList.Distinct().Count())
            {
                MessageBox.Show("Please ensure all power source channels are unique");
            }
            else if(scopeChannelList.Contains(-1))
            {
                MessageBox.Show("Please select an entry for all scope channels");
            }
            else if(scopeChannelList.Count != scopeChannelList.Distinct().Count())
            {
                MessageBox.Show("Please ensure all scope channels are unique");
            }
            else if(outputChannelList.Contains(-1))
            {
                MessageBox.Show("Please select an entry for all output channels");
            }
            else if(outputChannelList.Count != outputChannelList.Distinct().Count())
            {
                MessageBox.Show("Please ensure all output channels are unique");
            }
            else
            {
                //Save power source channel indexing
                mainsDcIdx = comboBoxMainsDC.SelectedIndex;
                extDcIdx = comboBoxExtDC.SelectedIndex;
                extBattIdx = comboBoxExtBatt.SelectedIndex;
                intBattIdx = comboBoxIntBatt.SelectedIndex;

                //Save scope channel indexing
                mainScopeIdx = comboBoxMainScope.SelectedIndex;
                speakerScope1Idx = comboBoxSpeakerScope1.SelectedIndex;
                speakerScope2Idx = comboBoxSpeakerScope2.SelectedIndex;
                supercapHiScopeIdx = comboBoxSupercapHiScope.SelectedIndex;
                supercapMidScopeIdx = comboBoxSupercapMidScope.SelectedIndex;
                supercapReturnScopeIdx = comboBoxSupercapReturnScope.SelectedIndex;

                //Save output channel indexing
                primaryOutIdx = comboBoxPrimaryOut.SelectedIndex;
                secondaryOutIdx = comboBoxSecondaryOut.SelectedIndex;

                this.DialogResult = DialogResult.OK;
            }
        }

        public void SetPsChannels(int mainsDC, int extDC, int extBatt, int intBatt)
        {
            if(mainsDC < comboBoxMainsDC.Items.Count && mainsDC >= 0)
            {
                comboBoxMainsDC.SelectedIndex = mainsDC;
            }
            if(extDC < comboBoxExtDC.Items.Count && extDC >= 0)
            {
                comboBoxExtDC.SelectedIndex = extDC;
            }
            if (extBatt < comboBoxExtBatt.Items.Count && extBatt >= 0)
            {
                comboBoxExtBatt.SelectedIndex = extBatt;
            }
            if (intBatt < comboBoxIntBatt.Items.Count && intBatt >= 0)
            {
                comboBoxIntBatt.SelectedIndex = intBatt;
            }
        }

        public void SetScopeChannels(int mainScope, int speakerScope1, int speakerScope2, int supercapHiScope, int supercapMidScope, int supercapReturnScope)
        {
            if(mainScope < comboBoxMainScope.Items.Count && mainScope >= 0)
            {
                comboBoxMainScope.SelectedIndex = mainScope;
            }
            if (speakerScope1 < comboBoxSpeakerScope1.Items.Count && speakerScope1 >= 0)
            {
                comboBoxSpeakerScope1.SelectedIndex = speakerScope1;
            }
            if (speakerScope2 < comboBoxSpeakerScope2.Items.Count && speakerScope2 >= 0)
            {
                comboBoxSpeakerScope2.SelectedIndex = speakerScope2;
            }
            if (supercapHiScope < comboBoxSupercapHiScope.Items.Count && supercapHiScope >= 0)
            {
                comboBoxSupercapHiScope.SelectedIndex = supercapHiScope;
            }
            if (supercapMidScope < comboBoxSupercapMidScope.Items.Count && supercapMidScope >= 0)
            {
                comboBoxSupercapMidScope.SelectedIndex = supercapMidScope;
            }
            if (supercapReturnScope < comboBoxSupercapReturnScope.Items.Count && supercapReturnScope >= 0)
            {
                comboBoxSupercapReturnScope.SelectedIndex = supercapReturnScope;
            }
        }

        public void SetOutputChannels(int primary, int secondary)
        {
            if(primary < comboBoxPrimaryOut.Items.Count && primary >= 0)
            {
                comboBoxPrimaryOut.SelectedIndex = primary;
            }
            if (secondary < comboBoxSecondaryOut.Items.Count && secondary >= 0)
            {
                comboBoxSecondaryOut.SelectedIndex = secondary;
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
