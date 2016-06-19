using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nature.Core.ObjectMapping
{
    public class AutoMapToAttribute : AutoMapAttribute
    {
        internal override AutoMapDirection Direction => AutoMapDirection.To;

        public AutoMapToAttribute(params Type[] targetTypes)
            : base(targetTypes)
        {

        }
    }
}
