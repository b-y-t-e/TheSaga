using System;
using System.Collections.Generic;

namespace TheSaga.Utils
{
    internal class UniqueNameGenerator
    {
        private HashSet<string> usedNames = new HashSet<string>();

        internal string Generate(params string[] names)
        {
            string baseName = String.Join("_", names);
            string name = baseName;
            int index = 0;
            while (usedNames.Contains(name))
            {
                index++;
                name = $"{name}_{index}";
            }
            usedNames.Add(name);
            return name;
        }
    }
}