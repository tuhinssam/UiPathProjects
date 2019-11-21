using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecLogDiag
{
    public class LogData
    {
        public int LogNum { get; set; }
        public string TimeStamp { get; set; }
        public string LogLevel { get; set; }
        public string LogMessage { get; set; }
        public string ProcessName { get; set; }
        public string FileName { get; set; }
        public string RobotName { get; set; }
    }
}
