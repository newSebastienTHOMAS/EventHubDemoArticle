using System;
using System.Collections.Generic;
using System.Text;

namespace DemoEventHubIngestion.Model
{
    public class MessageEvent
    {
        public DateTime DateMessage { get; set; }
        public string OtherInfo { get; set; }
        public int SleepTime { get; set; } = 500;
    }
}
