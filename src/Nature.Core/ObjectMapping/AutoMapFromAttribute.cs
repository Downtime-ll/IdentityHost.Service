using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nature.Core.ObjectMapping
{
    public class AutoMapFromAttribute : AutoMapAttribute
    {
        internal override AutoMapDirection Direction => AutoMapDirection.From;

        public AutoMapFromAttribute(params Type[] targetTypes)
            : base(targetTypes)
        {

        }
    }
}
