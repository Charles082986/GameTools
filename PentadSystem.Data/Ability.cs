using System;
using System.Collections.Generic;
using System.Text;

namespace PentadSystem.Data
{
    public class Ability
    {
        public string Name { get; set; }
        public bool IsAttack { get; set; }
        public int ActionCost { get; set; }
        public Target Target { get; set; }
        public string Duration { get; set; }
        public Effect Effects { get; set; }
    }
}
