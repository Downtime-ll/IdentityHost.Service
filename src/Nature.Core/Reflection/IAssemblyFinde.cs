using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Nature.Core.Reflection
{
    public interface IAssemblyFinder
    {
        /// <summary>
        /// This method should return all assemblies used by application.
        /// </summary>
        /// <returns>List of assemblies</returns>
        List<Assembly> GetAllAssemblies();
    }
}
