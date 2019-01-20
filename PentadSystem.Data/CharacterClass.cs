using System;
using System.Collections.Generic;
using System.Text;

namespace PentadSystem.Data
{
    public class CharacterClass
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ExperienceTable XPTable { get; set; }
        public List<ClassLevel> Levels { get; set; }
        public int Proficiencies { get; set; }
        public int MartialArtsProgression { get; set; }
        
    }
}
