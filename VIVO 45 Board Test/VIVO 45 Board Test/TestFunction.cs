using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VIVO_45_Board_Test
{
    class TestFunction
    {
        Func<TestResult> function;
        Version minHwRev;
        Version maxHwRev;
        Version minTreatmentFw;
        Version maxTreatmentFw;
        Version minCommFw;
        Version maxCommFw;
        string description;
        bool enabled;

        public TestFunction()
        {
            function = NoTestDefined;
            minHwRev = new Version(0, 0);
            maxHwRev = null;
            minTreatmentFw = new Version(0, 0);
            maxTreatmentFw = null;
            minCommFw = new Version(0, 0);
            maxCommFw = null;
            description = "No defined test";
            enabled = true;
        }

        public TestFunction(string testname)
        {
            function = NoTestDefined;
            minHwRev = new Version(0, 0);
            maxHwRev = null;
            minTreatmentFw = new Version(0, 0);
            maxTreatmentFw = null;
            minCommFw = new Version(0, 0);
            maxCommFw = null;
            description = "No defined test: " + testname;
            enabled = true;
        }

        public TestFunction(Func<TestResult> func, string descr)
        {
            function = func;
            minHwRev = new Version(0, 0);
            maxHwRev = null;
            minTreatmentFw = new Version(0, 0);
            maxTreatmentFw = null;
            minCommFw = new Version(0, 0);
            maxCommFw = null;
            description = descr;
            enabled = true;
        }

        public Version MinHwRev
        {
            get { return minHwRev; }
            set { minHwRev = value; }
        }

        public Version MaxHwRev
        {
            get { return maxHwRev; }
            set { maxHwRev = value; }
        }

        public Version MinTreatmentFwRev
        {
            get { return minTreatmentFw; }
            set { minTreatmentFw = value; }
        }

        public Version MaxTreatmentFwRev
        {
            get { return maxTreatmentFw; }
            set { maxTreatmentFw = value; }
        }

        public Version MinCommFwRev
        {
            get { return minCommFw; }
            set { minCommFw = value; }
        }

        public Version MaxCommFwRev
        {
            get { return maxCommFw; }
            set { maxCommFw = value; }
        }

        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        public string Description
        {
            get
            {
                return description;
            }
        }

        public TestResult RunTest()
        {
            return function();
        }

        private TestResult NoTestDefined()
        {
            //No default test defined yet, return a NA test result
            TestResult outputResult = new TestResult(-1, TestType.NA, description);
            return outputResult;
        }
    }
}
