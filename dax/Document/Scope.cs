using System;
using System.Collections.Generic;
using System.Linq;

namespace dax.Document
{
    public class Scope
    {
        public Scope(String version, IEnumerable<Block> blocks)
        {
            Version = version;
            Blocks = blocks.ToList();
            Groups = Blocks.SelectMany(p => p.Groups).Distinct().ToList();
        }

        public String Version
        {
            get;
            private set;
        }

        public IEnumerable<Block> Blocks
        {
            get;
            private set;
        }

        public IEnumerable<Group> Groups
        {
            get;
            private set;
        }

        public override String ToString()
        {
            return String.Format("Version={0}", Version);
        }
    }
}
