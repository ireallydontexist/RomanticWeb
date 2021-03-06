﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NullGuard;
using RomanticWeb.Entities;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Model;

namespace RomanticWeb.Converters
{
    /// <summary>Default converter for <see cref="Node"/>s to value objects or entities.</summary>
    public sealed class NodeConverter:INodeConverter
    {
        private readonly IEntityContext _entityContext;
        private readonly IConverterCatalog _converters;

        /// <summary>Constructor with entity context passed.</summary>
        /// <param name="entityContext">Entity context to be used.</param>
        /// <param name="converters">Converter catalog</param>
        public NodeConverter(IEntityContext entityContext,IConverterCatalog converters)
        {
            _entityContext=entityContext;
            _converters=converters;
        }

        /// <summary>Converts <see cref="Node"/>s and checks for validity against destination property mapping.</summary>
        /// <remarks>
        ///     <ul>
        ///         <li>Returns typed instances of <see cref="Entity"/> based on property's return value</li>
        ///         <li>Doesn't check the type of literals against the property's return type</li>
        ///     </ul>
        /// </remarks>
        public IEnumerable<object> ConvertNodes(IEnumerable<Node> objects,[AllowNull] IPropertyMapping propertyMapping)
        {
            foreach (var objectNode in objects.ToList())
            {
                Type type;
                if (ShouldConvertNodeToLiteral(objectNode,propertyMapping,out type))
                {
                    yield return ConvertLiteral(objectNode,type);
                }
                else
                {
                    yield return ConvertUri(objectNode,propertyMapping);
                }
            }
        }

        /// <summary>Converts <see cref="Node"/>s to most appropriate type based on raw RDF data.</summary>
        /// <remarks>This will always return untyped instanes of <see cref="Entity"/> for URI nodes.</remarks>
        public IEnumerable<object> ConvertNodes(IEnumerable<Node> objects)
        {
            return ConvertNodes(objects,null);
        }

        /// <summary>Converts a value to nodes.</summary>
        public IEnumerable<Node> ConvertBack(object value,IPropertyMapping property)
        {
            var convertedNodes=new List<Node>();

            if (typeof(IEntity).IsAssignableFrom(property.ReturnType))
            {
                convertedNodes.Add(ConvertOneBack(value));
            }

            if (typeof(IEnumerable<IEntity>).IsAssignableFrom(property.ReturnType))
            {
                var convertedEntities=from entity in ((IEnumerable)value).Cast<IEntity>()
                                      select ConvertOneBack(entity);
                convertedNodes.AddRange(convertedEntities);
            }

            if ((value is IEnumerable)&&!(value is string))
            {
                Type targetType=property.ReturnType.GetGenericArguments().First();
                foreach (object item in (IEnumerable)value)
                {
                    bool canAddItem=true;
                    Type[] constraints=new Type[0];
                    if ((targetType.IsGenericParameter)&&((constraints=targetType.GetGenericParameterConstraints()).Length>0))
                    {
                        foreach (Type constraint in constraints)
                        {
                            if (!constraint.IsAssignableFrom(item.GetType()))
                            {
                                canAddItem=false;
                                break;
                            }
                        }
                    }
                    else
                    {
                        canAddItem=targetType.IsAssignableFrom(item.GetType());
                    }

                    if (canAddItem)
                    {
                        var converter=_converters.UriNodeConverters.FirstOrDefault(c => c.CanConvertBack(item,property));
                        if (converter!=null)
                        {
                            convertedNodes.AddRange(converter.ConvertBack(item));
                        }
                        else
                        {
                            convertedNodes.Add(ConvertOneBack(item));
                        }
                    }
                }
            }
            else
            {
                convertedNodes.Add(ConvertOneBack(value));
            }

            return convertedNodes;
        }

        private static bool ShouldConvertNodeToLiteral(Node objectNode,IPropertyMapping propertyMapping,out Type type)
        {
            type=null;
            bool shouldConvert=false;

            // convert literal node
            shouldConvert|=objectNode.IsLiteral;

            if ((!shouldConvert)&&(propertyMapping!=null))
            {
                type=propertyMapping.ReturnType.FindItemType();

                // or convert primitive/string values
                shouldConvert|=(type.IsPrimitive)||(type==typeof(string));

                // and don't convert rdf lists or dictionary nodes
                var collectionMapping=propertyMapping as ICollectionMapping;
                if (collectionMapping!=null)
                {
                    shouldConvert&=(collectionMapping.StoreAs!=StoreAs.RdfList);
                }

                shouldConvert&=!(propertyMapping is IDictionaryMapping);
            }

            return shouldConvert;
        }

        private Node ConvertOneBack(object element)
        {
            if (element is IEntity)
            {
                return Node.FromEntityId(((IEntity)element).Id);
            }

            // todo: this is a hack, and should be in a complex type converter
            if (element is Uri)
            {
                return Node.ForUri((Uri)element);
            }

            return Node.ForLiteral(element.ToString());
        }

        private object ConvertLiteral(Node objectNode,Type resultType)
        {
            object result=objectNode.ToString();
            if (resultType==typeof(string))
            {
                result=(objectNode.IsUri?objectNode.Uri.ToString():objectNode.Literal);
            }
            else if (resultType==typeof(Uri))
            {
                result=(objectNode.IsUri?objectNode.Uri:new Uri(objectNode.Literal,UriKind.RelativeOrAbsolute));
            }
            else
            {
                var converter=_converters.GetBestConverter(objectNode);
                if (converter!=null)
                {
                    result=converter.Convert(objectNode);
                    if ((resultType!=null)&&(resultType.IsGenericType)&&(resultType.GetGenericTypeDefinition()==typeof(Nullable<>)))
                    {
                        result=resultType.GetConstructors(System.Reflection.BindingFlags.Public|System.Reflection.BindingFlags.Instance).First().Invoke(new[] { result });
                    }
                }
            }

            return result;
        }

        private object ConvertUri(Node uriNode,IPropertyMapping predicate)
        {
            IEntity entity;
            if ((predicate==null)||(!typeof(EntityId).IsAssignableFrom(predicate.ReturnType.FindItemType())))
            {
                entity=_entityContext.Load<IEntity>(uriNode.ToEntityId(),false);
            }
            else
            {
                entity=new Entity(uriNode.ToEntityId());
            }

            var converter=_converters.UriNodeConverters.FirstOrDefault(c => c.CanConvert(entity,predicate));
            if (converter!=null)
            {
                return converter.Convert(entity,predicate);
            }

            return entity;
        }
    }
}