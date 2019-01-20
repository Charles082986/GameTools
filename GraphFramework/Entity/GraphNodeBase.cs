using System;
using System.Collections.Generic;
using System.Text;

namespace GraphFramework.Entity
{
    public abstract class GraphNodeBase : GraphEntityBase
    {
        public GraphNodeBase(string label) : base(label) { }
        public GraphNodeBase(string label, string tokenSuffix = "", int index = 0) : base(label, tokenSuffix, index) { }

        public Guid NodeId { get; set; }
        
        public virtual string GetMergeStatement(Dictionary<string, object> parameters)
        {
            StringBuilder cyp = new StringBuilder("MERGE ");
            cyp.Append("(" + Label + Suffix + ":" + Label + "{ nodeid: $" + Token + "_nodeid })");
            cyp.AppendLine("ON CREATE SET " + Token + ".nodeid = $" + Token + "_nodeid");
            cyp.AppendLine("SET ");
            bool useSeparator = false;
            foreach (var property in GetType().GetProperties())
            {
                if (property.Name != "NodeId")
                {
                    if (useSeparator) { cyp.Append(","); } else { useSeparator = true; }
                    cyp.Append(Token + "." + property.Name.ToLower() + " = " + Token + "_" + property.Name.ToLower());
                }
            }
            return cyp.ToString();
        }
    }
}
