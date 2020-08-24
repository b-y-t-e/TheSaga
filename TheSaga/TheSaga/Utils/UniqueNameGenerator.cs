using System;
using System.Collections.Generic;
using System.Linq;

namespace TheSaga.Builders
{
    internal class UniqueNameGenerator
    {
        HashSet<string> usedNames = new HashSet<string>();

        internal string Generate(params string [] names)
        {
            string baseName = String.Join("_", names);
            string name = baseName;
            int index = 0;
            while(usedNames.Contains(name))
            {
                index++;
                name = $"{name}_{index}";
            }
            usedNames.Add(name);
            return name;
        }
    }
}