using System;
using System.Collections.Generic;

namespace PentadSystem.Data
{
    public class Creature
    {
        public string Name { get; set; }
        public List<Attribute> Attributes { get; set; }
        public List<RegionalAttributes> RegionalAttributes { get; set; }
    }
}
