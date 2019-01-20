using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace GraphFramework.Entity
{
    public abstract class GraphEntityBase
    { 
        public string Label { get; private set; }
        protected string TokenSuffix { get; set; }
        protected int Index { get; set; }
        public string Token
        {
            get
            {
                return Label.ToLower() + Suffix;
            }
        }
        protected string Suffix
        {
            get
            {
                return string.Format("_{0}{1}", TokenSuffix, Index);
            }
        }

        public GraphEntityBase(string label, string tokenSuffix = "", int index = 0)
        {
            TokenSuffix = tokenSuffix;
            Index = index;
            Label = label;
        }

        public Dictionary<string,object> GetParametersDictionary()
        {
            var properties = GetType().GetProperties().ToList();
            return properties.ToDictionary(prop => Token + "_" + prop.Name.ToLower(), prop => prop.GetValue(this));
        }

        
    }
}
