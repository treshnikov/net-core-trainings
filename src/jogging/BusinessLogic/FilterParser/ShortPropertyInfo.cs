using System;

namespace BusinessLogic.FilterParser
{
    public class ShortPropertyInfo
    {
        public string Name { get; }
        public Type Type { get; }

        public ShortPropertyInfo(string name, Type type)
        {
            Name = name;
            Type = type;
        }
    }
}