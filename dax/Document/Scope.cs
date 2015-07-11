using System;
using System.Collections.Generic;
using System.Linq;

namespace dax.Document
{
    public class Scope
    {
        public Scope(String version, List<Block> blocks)
        {
            Version = version;
            Blocks = blocks.ToList();
        }        

        public String Version
        {
            get;
            private set;
        }
        
        public List<Block> Blocks
        {
            get;
            private set;
        }

        public override String ToString()
        {
            return String.Format("Version={0}; Blocks Count={1}", Version, Blocks.Count);
        }
    }
}
