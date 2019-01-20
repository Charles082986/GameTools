using System;
using System.Collections.Generic;
using System.Text;

namespace PentadSystem.Data
{
    public class RegionalAttributes
    {
        public Guid RegionId { get; set; }
        public List<Creature> Creatures { get; set; }
        public List<Attribute> Attributes { get; set; }
    }
}
