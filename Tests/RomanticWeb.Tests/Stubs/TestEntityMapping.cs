﻿using System;
using System.Collections.Generic;
using System.Linq;
using ImpromptuInterface;
using RomanticWeb.Converters;
using RomanticWeb.Entities;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Tests.Stubs
{
    public class TestEntityMapping<TEntity>:IEntityMapping
    {
        private readonly IList<IClassMapping> _classes = new List<IClassMapping>();
        private readonly IList<IPropertyMapping> _properties = new List<IPropertyMapping>(); 

        public Type EntityType
        {
            get
            {
                return typeof(TEntity);
            }
        }

        public IEnumerable<IClassMapping> Classes
        {
            get
            {
                return _classes;
            }
        }

        public IEnumerable<IPropertyMapping> Properties
        {
            get
            {
                return _properties;
            }
        }

        public IPropertyMapping PropertyFor(string propertyName)
        {
            return _properties.FirstOrDefault(p => p.Name==propertyName);
        }

        protected void Class(Uri clazz)
        {
            _classes.Add(new { Uri=Vocabularies.Foaf.Person }.ActLike<IClassMapping>());
        }

        protected void Property(string name,Uri predicate,Type returnType,INodeConverter converter)
        {
            _properties.Add(new
                                {
                                    Name=name,
                                    Uri=predicate,
                                    ReturnType=returnType,
                                    Converter=converter,
                                    EntityMapping=this
                                }.ActLike<IPropertyMapping>());
        }

        protected void Collection(string name,Uri predicate,Type returnType,INodeConverter converter)
        {
            _properties.Add(new
            {
                Name = name,
                Uri = predicate,
                ReturnType = returnType,
                Converter = converter,
                EntityMapping = this,
                StoreAs = StoreAs.SimpleCollection
            }.ActLike<ICollectionMapping>());
        }

        protected void RdfList(string name,Uri predicate,Type returnType)
        {
            _properties.Add(new
            {
                Name = name,
                Uri = predicate,
                ReturnType = returnType,
                Converter = new AsEntityConverter<IEntity>(),
                EntityMapping = this,
                StoreAs = StoreAs.RdfList
            }.ActLike<ICollectionMapping>());
        }
    }
}