﻿using System;
using System.Reflection;
using RomanticWeb.Entities.ResultAggregations;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Mapping.Visitors;

namespace RomanticWeb.Mapping.Providers
{
    /// <summary>
    /// Mapping provider, which returns a mapping for collection property predicate
    /// </summary>
    public class CollectionMappingProvider:PropertyMappingProvider,ICollectionMappingProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionMappingProvider"/> class.
        /// </summary>
        /// <param name="termUri">The term URI.</param>
        /// <param name="storeAs">The storage strategy.</param>
        /// <param name="property">The property.</param>
        public CollectionMappingProvider(Uri termUri,StoreAs storeAs,PropertyInfo property)
            :base(termUri,property)
        {
            ((ICollectionMappingProvider)this).StoreAs = storeAs;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionMappingProvider"/> class.
        /// </summary>
        /// <param name="namespacePrefix">The namespace prefix.</param>
        /// <param name="term">The term.</param>
        /// <param name="storeAs">The storate strategy.</param>
        /// <param name="property">The property.</param>
        public CollectionMappingProvider(string namespacePrefix,string term,StoreAs storeAs,PropertyInfo property)
            :base(namespacePrefix,term,property)
        {
            ((ICollectionMappingProvider)this).StoreAs=storeAs;
        }

        /// <summary>
        /// Gets or sets the storage strategy
        /// </summary>
        /// <remarks>Setting this updated the <see cref="Aggregation"/> property</remarks>
        StoreAs ICollectionMappingProvider.StoreAs { get; set; }

        /// <inheritdoc/>
        public override void Accept(IMappingProviderVisitor mappingProviderVisitor)
        {
            mappingProviderVisitor.Visit(this);
        }
    }
}