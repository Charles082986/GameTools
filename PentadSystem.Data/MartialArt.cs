using System;
using System.Collections.Generic;
using System.Text;

namespace PentadSystem.Data
{
    public class MartialArt
    {
        public int ProficiencyCost { get; set; }
        public string Name { get; set; }
        public int ExternalRating { get; set; }
        public int InternalRating { get; set; }
        public int HardRating { get; set; }
        public int SoftRating { get; set; }
        public string Description { get; set; }
        public List<MartialArtLevel> Levels { get; set; }
    }
}
