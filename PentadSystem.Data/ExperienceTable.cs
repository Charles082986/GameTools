using System;
using System.Collections.Generic;
using System.Text;

namespace PentadSystem.Data
{
    public class ExperienceTable
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int[] Values { get; set; }
    }
}
