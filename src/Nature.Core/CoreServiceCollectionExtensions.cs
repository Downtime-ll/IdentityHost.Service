using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nature.Core.ObjectMapping;
using Nature.Core.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CoreServiceCollectionExtensions
    {
        public static void AddCoreServices(this IServiceCollection services)
        {
            //Reflection
            services.AddSingleton<IAssemblyFinder, CurrentDomainAssemblyFinder>();
            services.AddSingleton<ITypeFinder, TypeFinder>();

            //Mapping
            services.AddSingleton<IObjectMapper, AutoMapperObjectMapper>();
        }
    }
}
