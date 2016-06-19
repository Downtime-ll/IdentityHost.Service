using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nature.Core.ObjectMapping
{
    public class AutoMapAttribute : Attribute
    {
        public Type[] TargetTypes { get; private set; }

        internal virtual AutoMapDirection Direction => AutoMapDirection.From | AutoMapDirection.To;

        public AutoMapAttribute(params Type[] targetTypes)
        {
            TargetTypes = targetTypes;
        }
    }
}
