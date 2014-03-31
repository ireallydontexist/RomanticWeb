﻿using System;
using System.Diagnostics;
using NullGuard;
using RomanticWeb.Converters;

namespace RomanticWeb.Mapping.Model
{
    [NullGuard(ValidationFlags.All)]
    [DebuggerDisplay("Property {Name}")]
    [DebuggerTypeProxy(typeof(DebuggerViewProxy))]
    internal class PropertyMapping:IPropertyMapping
    {
        public PropertyMapping(Type returnType,string name,Uri predicateUri)
        {
            ReturnType=returnType;
            Name=name;
            Uri=predicateUri;
        }

        public IEntityMapping EntityMapping { get; internal set; }

        public Uri Uri { get; private set; }

        public string Name { get; private set; }

        public Type ReturnType { get; private set; }

        public INodeConverter Converter { get; internal set; }

#pragma warning disable 1591
        public override string ToString()
        {
            return Name;
        }
#pragma warning restore

        private class DebuggerViewProxy
        {
            private readonly PropertyMapping _mapping;

            public DebuggerViewProxy(PropertyMapping mapping)
            {
                _mapping=mapping;
            }

            public Uri Predicate
            {
                get
                {
                    return _mapping.Uri;
                }
            }

            public string Name
            {
                get
                {
                    return _mapping.Name;
                }
            }

            public Type ReturnType
            {
                get
                {
                    return _mapping.ReturnType;
                }
            }

            public IEntityMapping EntityMapping
            {
                get
                {
                    return _mapping.EntityMapping;
                }
            }
        }
    }
}
