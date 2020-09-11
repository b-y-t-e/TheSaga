using System.Collections.Generic;
using System.Linq;
using TheSaga.Exceptions;

namespace TheSaga.Utils
{
    internal class UniqueNameGenerator
    {
        private readonly HashSet<string> usedNames = new HashSet<string>();

        internal string Generate(params string[] names)
        {
            names = names.Select(n => !string.IsNullOrEmpty(n) ? n : "_").ToArray();
            string baseName = string.Join(" | ", names);
            int index = 0;
            string name = $"{baseName} | #{index}";
            while (usedNames.Contains(name))
            {
                index++;
                name = $"{baseName} | #{index}";
            }

            usedNames.Add(name);
            return name;
        }

        internal void ThrowIfNotUnique(string name)
        {
            if (usedNames.Contains(name))
                throw new NotUniqueStepNameException();

            usedNames.Add(name);
        }
    }
}