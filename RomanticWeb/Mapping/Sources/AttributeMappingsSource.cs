using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Anotar.NLog;
using RomanticWeb.Entities;
using RomanticWeb.Mapping.Attributes;
using RomanticWeb.Mapping.Providers;

namespace RomanticWeb.Mapping.Sources
{
    /// <summary>
    /// Mappings repository, which reads mapping attributes from an assembly
    /// </summary>
    public sealed class AttributeMappingsSource : AssemblyMappingsSource
    {
        /// <summary>
        /// Creates a new instance of <see cref="AttributeMappingsSource"/>
        /// </summary>
        public AttributeMappingsSource(Assembly assembly)
            : base(assembly)
        {
            LogTo.Trace("Created attribute mappings repository for assembly {0}", assembly);
        }

        /// <inheritdoc />
        public override string Description
        {
            get
            {
                return string.Format("Attribute mappings from assembly {0}", Assembly);
            }
        }

        /// <summary>
        /// Create mapping propviders from mapping attributes
        /// </summary>
        public override IEnumerable<IEntityMappingProvider> GetMappingProviders()
        {
            var builder = new AttributeMappingProviderBuilder();
            return from type in Assembly.GetTypesWhere(type => typeof(IEntity).IsAssignableFrom(type))
                   select builder.Visit(type);
        }
    }
}