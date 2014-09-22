using System;
using RomanticWeb.Entities;

namespace RomanticWeb.Updates
{
    /// <summary>
    /// Represents a change, which removes triples, which reference <see cref="RemoveReferences.Entity"/>
    /// </summary>
    public class RemoveReferences : DatasetChange
    {
        internal RemoveReferences(EntityId entity)
            : base(entity)
        {
            if (entity is BlankId)
            {
                throw new ArgumentOutOfRangeException("entity", "Reference cannot be a blank id");
            }
        }

        public override string ToString()
        {
            return string.Format("Removing references to {0}", Entity);
        }

        public override bool CanMergeWith(DatasetChange other)
        {
            return false;
        }

        public override DatasetChange MergeWith(DatasetChange other)
        {
            throw new InvalidOperationException("Cannot merge RemoveReferences with any other change");
        }
    }
}