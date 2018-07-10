using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Collections.Specialized;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Management;
using System.Windows.Forms;
using System.Text.RegularExpressions;


namespace VIVO_45_Board_Test
{
    public partial class Form1 : Form
    {
        TestSuite testSuite;        
        BackgroundWorker runTestWorker = new BackgroundWorker();
        BackgroundWorker initFixtureWorker = new BackgroundWorker();
        string reportPath;
        string jsonReportPath;
        string xmlReportPath;
        string sn;
        FixtureConfigForm fcForm = new FixtureConfigForm();
        int mainsDcIdx, extDcIdx, extBattIdx, intBattIdx;
        int mainScopeIdx, speakerScope1Idx, speakerScope2Idx;
        int supercapHiScopeIdx, supercapMidScopeIdx, supercapReturnScopeIdx;
        int primaryOutIdx, secondaryOutIdx;

        public Form1()
        {
            InitializeComponent();
            testSuite = new TestSuite();

            initFixtureWorker.DoWork += new DoWorkEventHandler(initFixtureWorker_DoWork);
            initFixtureWorker.ProgressChanged += new ProgressChangedEventHandler(initFixtureWorker_ProgressChanged);
            initFixtureWorker.WorkerReportsProgress = true;
            initFixtureWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(initFixtureWorker_RunWorkerCompleted);
            initFixtureWorker.WorkerSupportsCancellation = true;

            runTestWorker.DoWork += new DoWorkEventHandler(runTestWorker_DoWork);
            runTestWorker.ProgressChanged += new ProgressChangedEventHandler(runTestWorker_ProgressChanged);
            runTestWorker.WorkerReportsProgress = true;
            runTestWorker.WorkerSupportsCancellation = true;
            runTestWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(runTestWorker_RunWorkerCompleted);

            reportOptionsToolStripMenuItem.DropDown.Closing += toolStripDropDown_Closing;
        }

        private void InitializeTestList()
        {
            int numTests = Enum.GetNames(typeof(TestNames)).Length;
            string[] testList = testSuite.GetTestDescriptions();

            checkedListBoxTests.Items.Clear();

            //Add the test listings to list box, or add "description missing" if string is null
            int i = 0;
            foreach (string s in testList)
            {
                if (String.IsNullOrEmpty(s))
                {
                    checkedListBoxTests.Items.Add("Test description missing");
                }
                else
                {
                    checkedListBoxTests.Items.Add(s);
                }
                checkedListBoxTests.SetItemChecked(i, true);
                i++;
            }
        }

        private void InitializeFixtureChannels()
        {
            //Read in power source channel setup from config file
            bool readConfig = true;
            readConfig &= Int32.TryParse(ConfigurationManager.AppSettings.Get("MainsDcChannel"), out mainsDcIdx);
            readConfig &= Int32.TryParse(ConfigurationManager.AppSettings.Get("ExtDcChannel"), out extDcIdx);
            readConfig &= Int32.TryParse(ConfigurationManager.AppSettings.Get("ExtBattChannel"), out extBattIdx);
            readConfig &= Int32.TryParse(ConfigurationManager.AppSettings.Get("IntBattChannel"), out intBattIdx);

            //If an error occurs during reading the config file, use default values instead\
            //TODO confirm default values
            if (!readConfig)
            {
                mainsDcIdx = 0;
                extBattIdx = 1;
                intBattIdx = 2;
                extDcIdx = 3;
            }

            //Read in scope channel setup from config file
            readConfig = true;
            readConfig &= Int32.TryParse(ConfigurationManager.AppSettings.Get("MainScopeChannel"), out mainScopeIdx);
            readConfig &= Int32.TryParse(ConfigurationManager.AppSettings.Get("SpeakerScopeChannel1"), out speakerScope1Idx);
            readConfig &= Int32.TryParse(ConfigurationManager.AppSettings.Get("SpeakerScopeChannel2"), out speakerScope2Idx);
            readConfig &= Int32.TryParse(ConfigurationManager.AppSettings.Get("SupercapHiScopeChannel"), out supercapHiScopeIdx);
            readConfig &= Int32.TryParse(ConfigurationManager.AppSettings.Get("SupercapMidScopeChannel"), out supercapMidScopeIdx);
            readConfig &= Int32.TryParse(ConfigurationManager.AppSettings.Get("SupercapReturnScopeChannel"), out supercapReturnScopeIdx);

            //If an error occurs during reading the config file, use default values instead
            //TODO confirm default values
            if (!readConfig)
            {
                mainScopeIdx = 0;
                supercapHiScopeIdx = 1;
                supercapMidScopeIdx = 2;
                supercapReturnScopeIdx = 3;
                speakerScope1Idx = 4;
                speakerScope2Idx = 5;
            }

            //Read in digital output channel setup from config file
            readConfig = true;
            readConfig &= Int32.TryParse(ConfigurationManager.AppSettings.Get("PrimaryDigitalDevice"), out primaryOutIdx);
            readConfig &= Int32.TryParse(ConfigurationManager.AppSettings.Get("SecondaryDigitalDevice"), out secondaryOutIdx);

            //If an error occurs during reading the config file, use default values instead
            //TODO confirm default values
            if(!readConfig)
            {
                primaryOutIdx = 0;
                secondaryOutIdx = 1;
            }
        }

        private void InitializeReportOptions()
        {
            string enable = ConfigurationManager.AppSettings.Get("GenerateXML");
            if (enable == "0")
            {
                outputXMLToolStripMenuItem.Checked = false;
            }
            else
            {
                outputXMLToolStripMenuItem.Checked = true;
            }

            enable = ConfigurationManager.AppSettings.Get("GenerateJSON");
            if (enable == "0")
            {
                outputJSONToolStripMenuItem.Checked = false;
            }
            else
            {
                outputJSONToolStripMenuItem.Checked = true;
            }

            enable = ConfigurationManager.AppSettings.Get("GeneratePlaintext");
            if (enable == "0")
            {
                outputPlaintextToolStripMenuItem.Checked = false;
            }
            else
            {
                outputPlaintextToolStripMenuItem.Checked = true;
            }

            enable = ConfigurationManager.AppSettings.Get("GenerateSubfolder");
            if (enable == "0")
            {
                generateSubfolderToolStripMenuItem.Checked = false;
            }
            else
            {
                generateSubfolderToolStripMenuItem.Checked = true;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitializeTestList();

            InitializeFixtureChannels();

            InitializeReportOptions();

            string saveDir = ConfigurationManager.AppSettings.Get("LastSaveDir");
            if(Directory.Exists(saveDir))
            {
                textBoxSaveDirectory.Text = saveDir;
            }
        }

        private void linkLabelClearTests_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            int numTests = checkedListBoxTests.Items.Count;
            for (int i = 0; i < numTests; i++)
            {
                checkedListBoxTests.SetItemChecked(i, false);
            }
        }

        private void linkLabelSelectAll_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            int numTests = checkedListBoxTests.Items.Count;
            for (int i = 0; i < numTests; i++)
            {
                checkedListBoxTests.SetItemChecked(i, true);
            }
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            if(runTestWorker.IsBusy)
            {
                runTestWorker.CancelAsync();
            }
        }

        private void buttonStartAll_Click(object sender, EventArgs e)
        {
            if (!CreateReportID())
            {
                return;
            }

            testSuite.ResetTestSuite(sn);
            testSuite.EnableAllTests();

            BeginTesting();
        }

        private void buttonStartSelected_Click(object sender, EventArgs e)
        {
            if (!CreateReportID())
            {
                return;
            }

            testSuite.ResetTestSuite(sn);

            int numTests = checkedListBoxTests.Items.Count;
            for(int i = 0; i < numTests; i++)
            {
                if(checkedListBoxTests.GetItemChecked(i))
                {
                    testSuite.EnableTest((TestNames)i);
                }
            }

            BeginTesting();
        }

        private void SaveReport(string reportTime, string extension)
        {

        }

        private bool CreateReportID()
        {
            //Check to see if a save directory is selected, create one if it does not exist
            if(textBoxSaveDirectory.Text == "")
            {
                MessageBox.Show("Please select a report directory");
                return false;
            }
            try
            {
                Directory.CreateDirectory(textBoxSaveDirectory.Text);
            }
            catch
            {
                MessageBox.Show("Report directory selection error");
                return false;
            }
            

            //Check for PCB serial number, remove any non-alphanumeric characters
            sn = textBoxSerialNumber.Text;
            Regex rgx = new Regex("[^a-zA-Z0-9]");
            sn = rgx.Replace(sn, "");

            if (sn == "")
            {
                MessageBox.Show("Please enter a valid PCB serial number");
                return false;
            }

            bool reportFormatSelected = outputPlaintextToolStripMenuItem.Checked;
            reportFormatSelected |= outputXMLToolStripMenuItem.Checked;
            reportFormatSelected |= outputJSONToolStripMenuItem.Checked;

            string reportTime = DateTime.Now.ToString("yyMMdd-HHmmss");

            if (!reportFormatSelected)
            {
                MessageBox.Show("Please select at least one report output format");
                return false;
            }

            if (generateSubfolderToolStripMenuItem.Checked)
            {
                string newDir = textBoxSaveDirectory.Text + Path.DirectorySeparatorChar + sn;
                Directory.CreateDirectory(newDir);
            }

            //Form report path from serial number, selected directory and current date-time
            string reportName = sn + "-" + reportTime;

            //Create text path, add subfolder directory if requested
            reportPath = textBoxSaveDirectory.Text + Path.DirectorySeparatorChar;
            if (generateSubfolderToolStripMenuItem.Checked)
            {
                reportPath += sn + Path.DirectorySeparatorChar;
            }
            reportPath += reportName + ".txt";

            //Create JSON path, add subfolder directory if requested
            jsonReportPath = textBoxSaveDirectory.Text + Path.DirectorySeparatorChar;
            if (generateSubfolderToolStripMenuItem.Checked)
            {
                jsonReportPath += sn + Path.DirectorySeparatorChar;
            }
            jsonReportPath += reportName + ".json";


            xmlReportPath = textBoxSaveDirectory.Text + Path.DirectorySeparatorChar;
            if (generateSubfolderToolStripMenuItem.Checked)
            {
                xmlReportPath += sn + Path.DirectorySeparatorChar;
            }
            xmlReportPath += reportName + ".xml";

            return true;
        }

        private void BeginTesting()
        {
            if (!initFixtureWorker.IsBusy)
            {
                initFixtureWorker.RunWorkerAsync();
                buttonStartSelected.Enabled = false;
                buttonStartAll.Enabled = false;
                buttonBrowse.Enabled = false;
                toolStripProgressBar1.Value = 0;
                toolStripStatusLabel1.Text = "Connecting";
            }
        }

        private void initFixtureWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            int[] pwrChannels = { mainsDcIdx, extDcIdx, extBattIdx, intBattIdx };
            int[] scopeChannels = { mainScopeIdx, speakerScope1Idx, speakerScope2Idx, supercapHiScopeIdx, supercapMidScopeIdx, supercapReturnScopeIdx };
            int[] outputChannels = { primaryOutIdx, secondaryOutIdx };
            if(!testSuite.InitializeHardware(initFixtureWorker, pwrChannels, scopeChannels, outputChannels))
            {
                testSuite.DisconnectHardware();
                e.Cancel = true;
            }
        }

        private void initFixtureWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            toolStripStatusLabel1.Text = e.UserState as String;
        }

        private void initFixtureWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //If MP or NI connection disrupted during initial connection
            //Reset to initial state and warn user
            if (e.Cancelled)
            {
                buttonStartSelected.Enabled = true;
                buttonStartSelected.Visible = true;
                buttonStartAll.Enabled = true;
                buttonStartAll.Visible = true;
                buttonStop.Enabled = false;
                buttonStop.Visible = false;

                buttonBrowse.Enabled = true;
                toolStripStatusLabel1.Text = "Fixture hardware setup error";
                toolStripProgressBar1.Value = 0;
                return;
            }

            //Otherwise proceed to run tests
            buttonStartSelected.Enabled = false;
            buttonStartSelected.Visible = false;
            buttonStartAll.Enabled = false;
            buttonStartAll.Visible = false;
            buttonStop.Enabled = true;
            buttonStop.Visible = true;
            if (!runTestWorker.IsBusy)
            {
                runTestWorker.RunWorkerAsync();
            }
        }

        private void runTestWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            //Run through all tests (disabled tests will immediately return)
            int numTests = Enum.GetNames(typeof(TestNames)).Length;
            for(int i = 0; i < numTests; i++)
            {

                runTestWorker.ReportProgress(i);
                testSuite.RunTest(i);
                if(testSuite.getResultList().Count>0)
                    TestRes.Invoke(new MethodInvoker(delegate
                    {
                        TestName.Text = testSuite.getResultList().Last().getName();
                        TestRes.Text = testSuite.getResultList().Last().getStatus();
                    }));
                
                //Stop running tests if user cancels
                if(runTestWorker.CancellationPending)
                {
                    e.Cancel = true;
                    runTestWorker.ReportProgress(-1);
                    return;
                }
            }


        }

        private void runTestWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //Update status bar progress and display the current test being run
            string[] testStringName = Enum.GetNames(typeof(TestNames));
            int currentTestIdx = e.ProgressPercentage;
            if(currentTestIdx < testStringName.Length && currentTestIdx >= 0)
            {
                toolStripStatusLabel1.Text = "Current Test: " + testStringName[currentTestIdx];
                toolStripProgressBar1.Value = ((currentTestIdx + 1) * 100) / testStringName.Length;
            }
            else if (currentTestIdx < 0)
            {
                toolStripStatusLabel1.Text = "Testing stopped";
            }            
        }

        private void runTestWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                toolStripStatusLabel1.Text = "Testing stopped";
            }
            else
            {
                toolStripStatusLabel1.Text = "Testing completed";
            }

            testSuite.DisconnectHardware();

            //Reset UI state and generate test report
            buttonStartSelected.Enabled = true;
            buttonStartSelected.Visible = true;
            buttonStartAll.Enabled = true;
            buttonStartAll.Visible = true;
            buttonStop.Enabled = false;
            buttonStop.Visible = false;
            buttonBrowse.Enabled = true;

            try
            {
                File.WriteAllText(reportPath, testSuite.PrintTestResults());
                File.WriteAllText(jsonReportPath, testSuite.PrintTestResultsJSON());
                File.WriteAllText(xmlReportPath, testSuite.PrintTestResultsXML());
            }
            catch
            {
                MessageBox.Show("Error writing report file");
                return;
            }
            bool pass = true;
            foreach(TestResult r in testSuite.getResultList())
            {
                if(r.getStatus()=="Fail")
                {
                    pass = false;
                    break;
                }
            }
            if(pass)
                MessageBox.Show("Test Pass.\nReport saved to " + reportPath + " and " + jsonReportPath + " and " + xmlReportPath);
            else
                MessageBox.Show("Test Fail.\nReport saved to " + reportPath + " and " + jsonReportPath + " and " + xmlReportPath);
            reportPath = "";
            jsonReportPath = "";
            xmlReportPath = "";
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            if (folderBrowserDialog1.SelectedPath == "") return;
            textBoxSaveDirectory.Text = folderBrowserDialog1.SelectedPath;
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["LastSaveDir"].Value = textBoxSaveDirectory.Text;
            config.Save(ConfigurationSaveMode.Modified);
        }

        private void toolStripDropDown_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            if(e.CloseReason == ToolStripDropDownCloseReason.ItemClicked)
            {
                e.Cancel = true;
            }
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void startWiFiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process startWifi = new Process();
            ProcessStartInfo wifiInfo = new ProcessStartInfo();
            wifiInfo.WindowStyle = ProcessWindowStyle.Hidden;
            wifiInfo.FileName = "cmd.exe";
            wifiInfo.Arguments = "/c netsh wlan start hostednetwork";
            wifiInfo.Verb = "runas";
            startWifi.StartInfo = wifiInfo;
            try
            {
                startWifi.Start();
                bool wifiComplete = startWifi.WaitForExit(3000);
                if (!wifiComplete)
                {
                    MessageBox.Show("Error starting wifi");
                }
                else
                {
                    MessageBox.Show("Wifi started");
                }
            }
            catch
            {
                MessageBox.Show("Error starting wifi");
            }
        }

        private void SaveReportOptions()
        {
            string trueVal = "1";
            string falseVal = "0";
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if(outputXMLToolStripMenuItem.Checked)
            {
                config.AppSettings.Settings["GenerateXML"].Value = trueVal;
            }
            else
            {
                config.AppSettings.Settings["GenerateXML"].Value = falseVal;
            }

            if (outputJSONToolStripMenuItem.Checked)
            {
                config.AppSettings.Settings["GenerateJSON"].Value = trueVal;
            }
            else
            {
                config.AppSettings.Settings["GenerateJSON"].Value = falseVal;
            }

            if (outputPlaintextToolStripMenuItem.Checked)
            {
                config.AppSettings.Settings["GeneratePlaintext"].Value = trueVal;
            }
            else
            {
                config.AppSettings.Settings["GeneratePlaintext"].Value = falseVal;
            }

            if (generateSubfolderToolStripMenuItem.Checked)
            {
                config.AppSettings.Settings["GenerateSubfolder"].Value = trueVal;
            }
            else
            {
                config.AppSettings.Settings["GenerateSubfolder"].Value = falseVal;
            }

            config.Save(ConfigurationSaveMode.Modified);
        }

        private void outputXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveReportOptions();
        }

        private void outputJSONToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveReportOptions();
        }

        private void outputPlaintextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveReportOptions();
        }

        private void generateSubfolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveReportOptions();
        }

        private void editFixtureSetupToolStripMenuItem_Click(object sender, EventArgs e)
        {
#if DEBUG
            //Set up current channel settings
            fcForm.SetPsChannels(mainsDcIdx, extDcIdx, extBattIdx, intBattIdx);
            fcForm.SetScopeChannels(mainScopeIdx, speakerScope1Idx, speakerScope2Idx,
                supercapHiScopeIdx, supercapMidScopeIdx, supercapReturnScopeIdx);
            fcForm.SetOutputChannels(primaryOutIdx, secondaryOutIdx);

            //Show settings and check if user wants to save new settings
            if (fcForm.ShowDialog() == DialogResult.OK)
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                //Get and save power source channels
                mainsDcIdx = fcForm.MainsDcIdx;
                extDcIdx = fcForm.ExtDcIdx;
                extBattIdx = fcForm.ExtBattIdx;
                intBattIdx = fcForm.IntBattIdx;
                config.AppSettings.Settings["MainsDcChannel"].Value = mainsDcIdx.ToString();
                config.AppSettings.Settings["ExtDcChannel"].Value = extDcIdx.ToString();
                config.AppSettings.Settings["ExtBattChannel"].Value = extBattIdx.ToString();
                config.AppSettings.Settings["IntBattChannel"].Value = intBattIdx.ToString();

                //Get and save scope channels
                mainScopeIdx = fcForm.MainScopeIdx;
                speakerScope1Idx = fcForm.SpeakerScope1Idx;
                speakerScope2Idx = fcForm.SpeakerScope2Idx;
                supercapHiScopeIdx = fcForm.SupercapHiScopeIdx;
                supercapMidScopeIdx = fcForm.SupercapMidScopeIdx;
                supercapReturnScopeIdx = fcForm.SupercapReturnScopeIdx;
                config.AppSettings.Settings["MainScopeChannel"].Value = mainScopeIdx.ToString();
                config.AppSettings.Settings["SpeakerScopeChannel1"].Value = speakerScope1Idx.ToString();
                config.AppSettings.Settings["SpeakerScopeChannel2"].Value = speakerScope2Idx.ToString();
                config.AppSettings.Settings["SupercapHiScopeChannel"].Value = supercapHiScopeIdx.ToString();
                config.AppSettings.Settings["SupercapMidScopeChannel"].Value = supercapMidScopeIdx.ToString();
                config.AppSettings.Settings["SupercapReturnScopeChannel"].Value = supercapReturnScopeIdx.ToString();

                //Get and save digital output channels
                primaryOutIdx = fcForm.PrimaryOutIdx;
                secondaryOutIdx = fcForm.SecondaryOutIdx;
                config.AppSettings.Settings["PrimaryDigitalDevice"].Value = primaryOutIdx.ToString();
                config.AppSettings.Settings["SecondaryDigitalDevice"].Value = secondaryOutIdx.ToString();

                config.Save(ConfigurationSaveMode.Modified);
            }
#else
            MessageBox.Show("Fixture configuration currently locked.");
#endif
        }

        private void scanForHWToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!File.Exists("C:/Program Files (x86)/Windows Kits/10/Tools/x86/devcon.exe"))
            {
                MessageBox.Show("Cannot find devcon. Please ensure WDK 10 is installed");
                return;
            }
            Process hwRescan = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/c cd \"C:/Program Files (x86)/Windows Kits/10/Tools/x86\" && devcon rescan";
            startInfo.Verb = "runas";
            hwRescan.StartInfo = startInfo;
            try
            {
                hwRescan.Start();
                bool scanComplete = hwRescan.WaitForExit(1000);
                if (!scanComplete)
                {
                    MessageBox.Show("Hardware scan failed, please restart machine");
                }
                else
                {
                    MessageBox.Show("Hardware scan completed");
                }
            }
            catch
            {
                MessageBox.Show("Error performing hardware scan");
            }
        }

        private void buttonTest_Click(object sender, EventArgs e)
        {
            /*PressureSensor pSensor = new PressureSensor();
            pSensor.Connect();
            MessageBox.Show(pSensor.GetTemperature().ToString());
            pSensor.Disconnect();*/
            /*PressureSensor pSensor = new PressureSensor();
            ProgrammablePowerSupply pSupply = new ProgrammablePowerSupply();
            if (pSensor.Connect() && pSupply.Connect())
            {
                pSupply.SetVoltage(1, 5);
                pSupply.EnableChannel(1);
                pSupply.EnableOutput();
                MessageBox.Show("Connection successful");
                pSupply.DisableOutput();
                pSupply.Disconnect();
                pSensor.Disconnect();
            }
            else MessageBox.Show("fail");*/
        }
        
    }
}
