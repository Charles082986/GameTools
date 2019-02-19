using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorldBuilder.Data;

namespace BlackFolderGames.Web.Areas.WorldBuilder.Models
{
    public class WorldIndexViewModel
    {
        public List<RegionSet> OwnedRegionSets { get; set; }
        public List<RegionSetCollectionModel> EditableRegionSets { get; set; }
        public List<RegionSetCollectionModel> ViewableRegionSets { get; set; }
    }
}
