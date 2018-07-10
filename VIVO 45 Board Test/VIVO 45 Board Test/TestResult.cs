using System;
using System.Collections.Generic;

namespace VIVO_45_Board_Test
{
    public enum Result { NA, Pass, Fail, Conditional }
    public enum TestType { NA, Battery, BoardPeripheral, CommunicationInterface, CurrentLimiter, Heater, InternalVoltage, Motor, PowerControl, PowerSource, PowerSupply, Supercap, UserInterface }

    public enum Operation { Equal = 0, NotEqual, GreaterThan, LessThan, Between, Outside }

    public static class MyReportWriter
    {
        public static string PrintXmlElement(string tag, string value, int tabCount)
        {
            string element = "";
            element += new string('\t', tabCount);
            element += "<" + tag + ">" + value + "</" + tag + ">\r\n";
            return element;
        }
        
        public static string PrintJsonElement(string tag, string value, int tabCount, bool useComma)
        {
            string element = "";
            string comma = "";
            if(useComma)
            {
                comma = ",";
            }
            element += new string('\t', tabCount);
            element += "\"" + tag + "\":\"" + value + "\"" + comma + "\r\n";
            return element;
        }

        public static string PrintJsonNumElement(string tag, double value, int tabCount, bool useComma)
        {
            string element = "";
            string comma = "";
            if (useComma)
            {
                comma = ",";
            }
            element += new string('\t', tabCount);
            element += "\"" + tag + "\":" + value.ToString() + comma + "\r\n";
            return element;
        }
    }

    public class TestCondition
    {
        string id;
        Operation operation;
        double param1;
        double param2;
        bool useTwoParams;
        double outcome;
        Result status;

        public TestCondition(string conditionID, Operation op, double value1, double value2)
        {
            id = conditionID;
            operation = op;
            param1 = value1;
            param2 = value2;
            if (operation < Operation.Between)
            {
                useTwoParams = false;
            }
            else
            {
                useTwoParams = true;
            }
            status = Result.NA;
        }

        public string ID
        {
            get { return id; }
        }

        public Result Status
        {
            get
            {
                return status;
            }
        }

        public double Outcome
        {
            set
            {
                outcome = value;
                UpdateStatus();
                /*Note that the status is immediately updated here, instead of when retrieving the status property
                This is useful in case we want to print the condition field before something else wants
                to retrieve the status property*/
            }
        }

        private void UpdateStatus()
        {
            status = Result.Fail;
            switch (operation)
            {
                case Operation.Equal:
                    if (outcome == param1)
                    {
                        status = Result.Pass;
                    }
                    break;
                case Operation.NotEqual:
                    if (outcome != param1)
                    {
                        status = Result.Pass;
                    }
                    break;
                case Operation.GreaterThan:
                    if (outcome > param1)
                    {
                        status = Result.Pass;
                    }
                    break;
                case Operation.LessThan:
                    if (outcome < param1)
                    {
                        status = Result.Pass;
                    }
                    break;
                case Operation.Between:
                    if (outcome >= param1 && outcome <= param2)
                    {
                        status = Result.Pass;
                    }
                    break;
                case Operation.Outside:
                    if (outcome < param1 || outcome > param2)
                    {
                        status = Result.Pass;
                    }
                    break;
                default:
                    status = Result.NA;
                    break;
            }
        }

        public string PrintToString()
        {
            string printout = "";

            return printout;
        }

        public string PrintToJSON(int tabCount, bool useComma)
        {
            string printout = "";
            string comma = "";
            if (useComma)
            {
                comma = ",";
            }

            printout += new string('\t', tabCount) + "{\r\n";

            printout += MyReportWriter.PrintJsonElement("ConditionId", id, tabCount + 1, true);
            printout += MyReportWriter.PrintJsonElement("ConditionOperation", operation.ToString(), tabCount + 1, true);
            printout += MyReportWriter.PrintJsonNumElement("ConditionParam1", param1, tabCount + 1, true);
            if (useTwoParams)
            {
                printout += MyReportWriter.PrintJsonNumElement("ConditionParam2", param2, tabCount + 1, true);
            }
            printout += MyReportWriter.PrintJsonNumElement("ConditionValue", outcome, tabCount + 1, true);
            printout += MyReportWriter.PrintJsonElement("ConditionStatus", status.ToString(), tabCount + 1, false);

            printout += new string('\t', tabCount) + "}" + comma + "\r\n";

            return printout;
        }

        public string PrintToXML(int tabCount)
        {
            string printout = "";
            printout += new string('\t', tabCount);
            printout += "<TestCondition>\r\n";

            printout += MyReportWriter.PrintXmlElement("ConditionId", id, tabCount + 1);
            printout += MyReportWriter.PrintXmlElement("ConditionOperation", operation.ToString(), tabCount + 1);
            printout += MyReportWriter.PrintXmlElement("ConditionParam1", param1.ToString(), tabCount + 1);
            if(useTwoParams)
            {
                printout += MyReportWriter.PrintXmlElement("ConditionParam2", param2.ToString(), tabCount + 1);
            }
            printout += MyReportWriter.PrintXmlElement("ConditionValue", outcome.ToString(), tabCount + 1);
            printout += MyReportWriter.PrintXmlElement("ConditionStatus", status.ToString(), tabCount + 1);

            printout += new string('\t', tabCount);
            printout += "</TestCondition>\r\n";
            return printout;
        }
    }

    public class TestResult
    {
        int idx;
        TestType type;
        string description;
        List<TestCondition> conditions;
        Result status;
        string comments;
        public string getName()
        {
            return description.ToString();
        }
        public string getStatus()
        {
            return status.ToString();
        }
        public TestResult(int testNum, TestType testType, string testDescription)
        {
            conditions = new List<TestCondition>();
            idx = testNum;
            type = testType;
            description = testDescription;
            status = Result.NA;
            comments = "";
        }

        public TestType Type
        {
            get { return type; }
        }

        public void AddCondition(string id, Operation operation, double value)
        {
            AddCondition(id, operation, value, 0);
        }

        public void AddCondition(string id, Operation operation, double value1, double value2)
        {
            //Check to make sure condition list does not already contain given ID
            if (!conditions.Exists(x => x.ID == id))
            {
                TestCondition condition = new TestCondition(id, operation, value1, value2);
                conditions.Add(condition);
            }
        }

        public void SetOutcome(string id, double outcome)
        {
            //Check to make sure condition list contains given ID
            int idx = conditions.FindIndex(x => x.ID == id);
            if(idx < 0)
            {
                return;
            }
            conditions[idx].Outcome = outcome;

            //Update the overall test result status after the condition is updated
            status = Result.Pass;
            foreach (TestCondition condition in conditions)
            {
                if (condition.Status != Result.Pass)
                {
                    status = condition.Status;
                    break;
                }
            }
        }

        public string PrintToString()
        {
            string printout = "";

            return printout;
        }
        
        public string PrintToJSON(int tabCount, bool useComma)
        {
            string printout = "";
            string comma = "";
            if(useComma)
            {
                comma = ",";
            }

            printout += new string('\t', tabCount) + "{\r\n";

            printout += MyReportWriter.PrintJsonNumElement("TestIdx", idx, tabCount + 1, true);
            printout += MyReportWriter.PrintJsonElement("TestType", type.ToString(), tabCount + 1, true);
            printout += MyReportWriter.PrintJsonElement("TestDescription", description, tabCount + 1, true);

            printout += new string('\t', tabCount + 1) +  "\"Conditions\": [\r\n";
            for(int i = 0; i < conditions.Count; i++)
            {
                if(i < (conditions.Count-1))
                {
                    printout += conditions[i].PrintToJSON(tabCount + 2, true);
                }
                else
                {
                    printout += conditions[i].PrintToJSON(tabCount + 2, false);
                }
            }
            printout += new string('\t', tabCount + 1) + "],\r\n";

            if(comments != "")
            {
                printout += MyReportWriter.PrintJsonElement("ResultStatus", status.ToString(), tabCount + 1, true);
                printout += MyReportWriter.PrintJsonElement("Comments", comments, tabCount+1, false);
            }
            else
            {
                printout += MyReportWriter.PrintJsonElement("ResultStatus", status.ToString(), tabCount + 1, false);
            }

            printout += new string('\t', tabCount) + "}" + comma + "\r\n";

            return printout;
        }

        public string PrintToXML(int tabCount)
        {
            string printout = "";
            printout += new string('\t', tabCount);
            printout += "<TestResult>\r\n";

            printout += MyReportWriter.PrintXmlElement("TestIdx", idx.ToString(), tabCount+1);
            printout += MyReportWriter.PrintXmlElement("TestType", type.ToString(), tabCount + 1);
            printout += MyReportWriter.PrintXmlElement("TestDescription", description, tabCount + 1);

            foreach(TestCondition condition in conditions)
            {
                printout += condition.PrintToXML(tabCount + 1);
            }

            printout += MyReportWriter.PrintXmlElement("ResultStatus", status.ToString(), tabCount+1);

            if(comments != "")
            {
                printout += MyReportWriter.PrintXmlElement("Comments", comments, tabCount + 1);
            }

            printout += new string('\t', tabCount);
            printout += "</TestResult>\r\n";
            return printout;
        }
    }
}


