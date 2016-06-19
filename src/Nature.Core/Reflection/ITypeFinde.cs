using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nature.Core.Reflection
{
    public interface ITypeFinder
    {
        Type[] Find(Func<Type, bool> predicate);

        Type[] FindAll();
    }
}
