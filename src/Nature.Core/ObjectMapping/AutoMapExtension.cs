using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nature.Core.Reflection;

namespace Nature.Core.ObjectMapping
{
    public static class AutoMapExtensions
    {
        private static readonly object _syncObj = new object();
        private static bool _createdMappingsBefore;

        /// <summary>
        /// Converts an object to another using AutoMapper library. Creates a new object of <see cref="TDestination"/>.
        /// There must be a mapping between objects before calling this method.
        /// </summary>
        /// <typeparam name="TDestination">Type of the destination object</typeparam>
        /// <param name="source">Source object</param>
        public static TDestination MapTo<TDestination>(this object source)
        {
            return Mapper.Map<TDestination>(source);
        }

        /// <summary>
        /// Execute a mapping from the source object to the existing destination object
        /// There must be a mapping between objects before calling this method.
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object</param>
        /// <param name="destination">Destination object</param>
        /// <returns></returns>
        public static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination)
        {
            return Mapper.Map(source, destination);
        }

        public static void UseAtuoMapper(this IApplicationBuilder app)
        {
            CreateMappings(app);
        }

        private static void CreateMappings(IApplicationBuilder app)
        {
            lock (_syncObj)
            {
                //We should prevent duplicate mapping in an application, since AutoMapper is static.
                if (_createdMappingsBefore)
                {
                    return;
                }

                FindAndAutoMapTypes(app);
                _createdMappingsBefore = true;
            }
        }

        private static void FindAndAutoMapTypes(IApplicationBuilder app)
        {
            var typeFinder = app.ApplicationServices.GetService<ITypeFinder>();
            ILogger logger = app.ApplicationServices.GetRequiredService<ILoggerFactory>()
                .CreateLogger(typeof(AutoMapExtensions));
            var types = typeFinder.Find(type =>
                type.IsDefined(typeof(AutoMapAttribute),true) ||
                type.IsDefined(typeof(AutoMapFromAttribute),true) ||
                type.IsDefined(typeof(AutoMapToAttribute),true)
                );

            logger.LogDebug("Found {0} classes defines auto mapping attributes", types.Length);
            foreach (var type in types)
            {
                logger.LogDebug(type.FullName);
                AutoMapperHelper.CreateMap(type);
            }
        }
    }
}
