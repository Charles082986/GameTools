using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorldBuilder.Data;

namespace BlackFolderGames.Web.Areas.WorldBuilder.Models
{
    public class RegionSetCollectionModel : List<RegionSet>
    {
        public string UserName { get; set; }
        public IEnumerable<RegionSet> RegionSets { get; set; }

        public RegionSetCollectionModel() { }
        public RegionSetCollectionModel(IGrouping<string,RegionSet> source)
        {
            UserName = source.Key;
            RegionSets = source;
        }
    }
}
