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
            int index = 0;
            string name = $"{baseName}_{index}";
            while (usedNames.Contains(name))
            {
                index++;
                name = $"{baseName}_{index}";
            }
            usedNames.Add(name);
            return name;
        }
    }
}