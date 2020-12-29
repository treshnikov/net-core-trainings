using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Domain.Attributes;

namespace BusinessLogic.FilterParser
{
    public static class PropertiesList
    {
        private static readonly ConcurrentDictionary<Type, IReadOnlyDictionary<string, ShortPropertyInfo>> Properties = 
            new ConcurrentDictionary<Type, IReadOnlyDictionary<string, ShortPropertyInfo>>();

        public static void Add(Type type)
        {
            if (!Properties.ContainsKey(type))
            {
                Properties.TryAdd(
                    type,
                    type.GetProperties()
                        .Where(p => !p.GetCustomAttributes(typeof(NonFilterableAttribute), true).Any())
                        .ToDictionary(
                            p => p.Name,
                            p => new ShortPropertyInfo(p.Name, p.PropertyType),
                            StringComparer.OrdinalIgnoreCase));
            }
        }

        public static ShortPropertyInfo Get(Type type, string propertyName)
        {
            var d = Properties[type];
            if (!d.TryGetValue(propertyName, out var result))
            {
                throw new FilterException($"Unknown identifier: {propertyName}");
            }

            return result;
        }
    }
}