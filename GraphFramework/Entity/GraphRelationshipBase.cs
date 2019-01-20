using System;
using System.Collections.Generic;
using System.Text;

namespace GraphFramework.Entity
{
    public class GraphRelationshipBase : GraphEntityBase
    {
        public GraphRelationshipBase(string type) : base(type) { }
        public GraphRelationshipBase(string type, string tokenSuffix = "", int index = 0) : base(type, tokenSuffix, index) { }

        public string GetRelationshipCypher(GraphNodeBase source, GraphNodeBase destination)
        {
            return "(" + source.Token + ")-[" + Token + ":" + Label.ToUpper() + "]->(" + destination.Token + ")";
        }
    }
}
