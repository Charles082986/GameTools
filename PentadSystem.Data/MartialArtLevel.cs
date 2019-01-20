using System;
using System.Collections.Generic;
using System.Text;

namespace PentadSystem.Data
{
    public class MartialArtLevel
    {
        public MartialArt MartialArt { get; set; }
        public int Level { get; set; }
        public int ActionsPerRound { get; set; }
        public string Strike { get; set; }
        public string Parry { get; set; }
        public string Disarm { get; set; }
        public string Damage { get; set; }
        public string Dodge { get; set; }
        public string AC { get; set; }
        public string Leap { get; set; }
        public List<Ability> Abilities { get; set; }
    }
}
