using System;
using System.Collections.Generic;
using System.Text;

namespace PentadSystem.Data
{
    public class Effect
    {
        public string AttributeName { get; set; }
        public string Magnitude { get; set; }
        public string Duration { get; set; }
        public Target Target { get; set; }
    }
}
