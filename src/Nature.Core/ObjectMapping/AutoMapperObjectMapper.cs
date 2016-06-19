using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nature.Core.ObjectMapping
{
    public class AutoMapperObjectMapper : IObjectMapper
    {
        public TDestination Map<TDestination>(object source)
        {
            return source.MapTo<TDestination>();
        }

        public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            return source.MapTo(destination);
        }
    }
}
